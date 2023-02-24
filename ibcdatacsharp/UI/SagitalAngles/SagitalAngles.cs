using ibcdatacsharp.DeviceList.TreeClasses;
using ibcdatacsharp.UI.Common;
using ibcdatacsharp.UI.DeviceList;
using ibcdatacsharp.UI.Graphs.Models;
using ibcdatacsharp.UI.Graphs.Sagital;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using MS.WindowsAPICodePack.Internal;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace ibcdatacsharp.UI.SagitalAngles
{
    public class SagitalAngles
    {
        private MainWindow mainWindow;
        private DeviceList.DeviceList deviceList;
        private VirtualToolBar virtualToolBar;

        private int TOTAL_SENSORS = 4;
        private int TOTAL_JOINTS = 3;
        private Quaternion[] mQ_sensors_raw;
        private Quaternion[] mQ_segments;
        private Quaternion[] mQ_sensors_ref;
        private Quaternion[] mQ_compensate;
        private Quaternion idenQuat = Quaternion.Identity;

        private Quaternion mQ_virtual;
        private Quaternion mQ_left;
        private Quaternion mQ_right;

        private Quaternion[] mQ_joints;

        private double[] eulerAnglesZ;

        private Quaternion[,] mQ_sensors_raw_list;
        private bool[] updated_quats;

        //Guarda un indice para cada deviceHandler.
        //Podrias usar el handler directamente como indice pero como no esta muy claro que la
        //api te garantize que sean 0, 1, ..., (n - 1) lo he dejado asi por si acaso
        private Dictionary<byte, int> indices;

        private int NUM_PACK = 4;

        private GraphAnkle ankle;
        private GraphHip hip;
        private GraphKnee knee;
        private int ankleIndex = 2;  // anteriormente a ankleIndex = 0
        private int hipIndex = 0; // anteriormente a hipIndex = 1
        private int kneeIndex = 1; // kneeIndex = 2

        private float fakets = 0f;
        private int frame = 0;

        private int IMUsReceived = 0;

        private List<Quaternion[,]> quat_history = new List<Quaternion[,]>();

        private bool recalculating = false;

        System.Timers.Timer fakeTimer; // Datos falsos

        public SagitalAngles()
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
            mainWindow.initialized += (sender, args) =>
            {
                virtualToolBar = mainWindow.virtualToolBar;
            };
            if (mainWindow.deviceList.Content == null)
            {
                mainWindow.deviceList.Navigated += (sender, args) =>
                {
                    deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                };
            }
            else
            {
                deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
            }
            /*
            Helpers.callWhenNavigated(mainWindow.deviceList, delegate 
            {
                deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
            });
            Helpers.callWhenNavigated(mainWindow.ankle, delegate
            {
                ankle = mainWindow.ankle.Content as GraphAnkle;
            });
            Helpers.callWhenNavigated(mainWindow.ankle, delegate
            {
                hip = mainWindow.hip.Content as GraphHip;
            });
            Helpers.callWhenNavigated(mainWindow.ankle, delegate
            {
                knee = mainWindow.knee.Content as GraphKnee;
            });
            */
            if (mainWindow.ankle.Content == null)
            {
                mainWindow.ankle.Navigated += (s, e) =>
                {
                    ankle = mainWindow.ankle.Content as GraphAnkle;
                };
            }
            else
            {
                ankle = mainWindow.ankle.Content as GraphAnkle;
            }
            if (mainWindow.hip.Content == null)
            {
                mainWindow.hip.Navigated += (s, e) =>
                {
                    hip = mainWindow.hip.Content as GraphHip;
                };
            }
            else
            {
                hip = mainWindow.hip.Content as GraphHip;
            }
            if (mainWindow.knee.Content == null)
            {
                mainWindow.knee.Navigated += (s, e) =>
                {
                    knee = mainWindow.knee.Content as GraphKnee;
                };
            }
            else
            {
                knee = mainWindow.knee.Content as GraphKnee;
            }
            quaternionCalcsConnect();
        }
        public void initRecord()
        {
            frame = 0;
            fakets = 0;
        }
        // Inicializa el indice que correspone a cada handler
        public void initIMUs()
        {
            
            byte handlerFromMAC(string mac)
            {
                string handler = mainWindow.devices_list.Where(z => z.Value.Id == mac).FirstOrDefault().Key;
                return byte.Parse(handler);
            }

            indices = new Dictionary<byte, int>();
            List<IMUInfo> imus = deviceList.IMUsUsed;
            Trace.WriteLine("Lista UI");
            foreach (IMUInfo imu in imus)
            {
                Trace.WriteLine("A = " + imu.A + ", MAC = " + imu.address);
            }
            Trace.WriteLine("Funcion de sagital angles");
            for (int i = 0; i < 4; i++)
            {
                string i_str = i.ToString();
                IMUInfo? imu = imus.Where(imu => imu.A == i_str).FirstOrDefault();
                if (imu != null)
                {
                    byte handler = handlerFromMAC(imu.address);
                    indices[handler] = i;
                    Trace.WriteLine("A = " + i + " MAC = " + imu.address + " , handler = " + handler);
                }
                else
                {
                    Trace.WriteLine("A = " + i + " no encontrado");
                }
            }
            ankle.initCapture();
            hip.initCapture();
            knee.initCapture();
            quaternionCalcsConnect();
        }
        // Convierte deviceHandler en indice. Es una funcion por si hay que cambiarlo mas adelante
        private int handlerToIndex(byte deviceHandler) 
        {
            return indices[deviceHandler];
        }
        private void changeQuaternionsRandom()
        {
            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                mQ_sensors_raw[i] = Helpers.random_quaternion();
            }
        }
        private void changeQuaternionsListRandom()
        {
            for (int i = 0; i < NUM_PACK; i++)
            {
                for (int s = 0; s < TOTAL_SENSORS; s++)
                {
                    mQ_sensors_raw_list[i, s] = Helpers.random_quaternion();
                }
            }
        }
        private void changeQuaternionsListDeterministic()
        {
            Vector3 axis = new Vector3(2, 3, 1);
            for (int i = 0; i < NUM_PACK; i++)
            {
                for (int s = 0; s < TOTAL_SENSORS; s++)
                {
                    mQ_sensors_raw_list[i, s] = Quaternion.CreateFromAxisAngle(axis, i * s);
                }
            }
        }
        /*
        public void test()
        {
            ankle.initCapture();
            hip.initCapture();
            knee.initCapture();
            quaternionCalcsConnect();
            changeQuaternionsRandom();
            calculateMounting();
            changeQuaternionsRandom();
            calculateVirtualOrientation();
            changeQuaternionsRandom();
            updateLeftAndRightQuats();
            updateSegmentsAndJoints();
            Trace.WriteLine("result");
            ankle.drawData(new float[] { 0, 1, 90, 50 });
            hip.drawData(new float[] { 0, 1, 90, 30 });
            knee.drawData(new float[] { 0, 1, 90, 60 });
        }
        */
        // Datos falsos
        public void fakeData()
        {
            ankle.initCapture();
            hip.initCapture();
            knee.initCapture();
            quaternionCalcsConnect();
            fakeTimer = new System.Timers.Timer();
            fakeTimer.Interval = 40;
            fakeTimer.Elapsed += (sender, eventArgs) =>
            {
                changeQuaternionsListRandom();
                Quaternion[,] mQ_sensors_raw_list_clone = mQ_sensors_raw_list.Clone() as Quaternion[,];
                quat_history.Add(mQ_sensors_raw_list_clone);
                //updated_quats = new bool[TOTAL_SENSORS]; // Reinicializa a false
                float[] ankleData = new float[NUM_PACK];
                float[] hipData = new float[NUM_PACK];
                float[] kneeData = new float[NUM_PACK];
                for (int i = 0; i < NUM_PACK; i++)
                {
                    for (int s = 0; s < TOTAL_SENSORS; s++)
                    {
                        mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                    }
                    Quaternion mQ_left = new Quaternion();
                    Quaternion mQ_right = new Quaternion();
                    updateLeftAndRightQuats(mQ_virtual, ref mQ_left, ref mQ_right);
                    Quaternion[] mQ_joints = new Quaternion[TOTAL_JOINTS];
                    Quaternion[] mQ_segments = new Quaternion[TOTAL_SENSORS];
                    double[] eulerAnglesZ = new double[3];
                    updateSegmentsAndJoints(mQ_sensors_raw, mQ_left, mQ_right, ref mQ_joints, ref mQ_segments, ref eulerAnglesZ);
                    ankleData[i] = (float)eulerAnglesZ[ankleIndex];
                    hipData[i] = (float)eulerAnglesZ[hipIndex];
                    kneeData[i] = (float)eulerAnglesZ[kneeIndex];
                }
                ankle.drawData(ankleData);
                hip.drawData(hipData);
                knee.drawData(kneeData);
            };
            fakeTimer.Start();
        }
        // Datos falsos
        public async void fakeRecalculate()
        {
            changeQuaternionsListRandom();
            calculateMounting();
            calculateVirtualOrientation();
            recalculate();
        }

        public void test2()
        {
            ankle.initCapture();
            hip.initCapture();
            knee.initCapture();
            quaternionCalcsConnect();
            changeQuaternionsListDeterministic();
            float[] ankleData = new float[NUM_PACK];
            float[] hipData = new float[NUM_PACK];
            float[] kneeData = new float[NUM_PACK];
            for (int i = 0; i < NUM_PACK; i++)
            {
                for (int s = 0; s < TOTAL_SENSORS; s++)
                {
                    mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                }
                Quaternion mQ_left = new Quaternion();
                Quaternion mQ_right = new Quaternion();
                updateLeftAndRightQuats(mQ_virtual, ref mQ_left, ref mQ_right);
                Quaternion[] mQ_joints = new Quaternion[TOTAL_JOINTS];
                Quaternion[] mQ_segments = new Quaternion[TOTAL_SENSORS];
                double[] eulerAnglesZ = new double[3];
                updateSegmentsAndJoints(mQ_sensors_raw, mQ_left, mQ_right, ref mQ_joints, ref mQ_segments, ref eulerAnglesZ);
                ankleData[i] = (float)eulerAnglesZ[ankleIndex];
                hipData[i] = (float)eulerAnglesZ[hipIndex];
                kneeData[i] = (float)eulerAnglesZ[kneeIndex];
            }
            Trace.WriteLine("ankleData");
            Trace.WriteLine("[");
            foreach(double d in ankleData)
            {
                Trace.WriteLine(d);
            }
            Trace.WriteLine("]");
            Trace.WriteLine("hipData");
            Trace.WriteLine("[");
            foreach (double d in hipData)
            {
                Trace.WriteLine(d);
            }
            Trace.WriteLine("]");
            Trace.WriteLine("kneeData");
            Trace.WriteLine("[");
            foreach (double d in kneeData)
            {
                Trace.WriteLine(d);
            }
            Trace.WriteLine("]");
            ankle.drawData(ankleData);
            hip.drawData(hipData);
            knee.drawData(kneeData);
        }
        
        public void processSerialData(byte deviceHandler, WisewalkSDK.WisewalkData data)
        {
            if(indices == null)
            {
                return;
            }
            int index = handlerToIndex(deviceHandler);
            for(int i = 0; i < NUM_PACK; i++)
            {
                mQ_sensors_raw_list[i, index] = new Quaternion((float)data.Quat[i].X, (float)data.Quat[i].Y, (float)data.Quat[i].Z, (float)data.Quat[i].W);
            }
            IMUsReceived++;
            //updated_quats[index] = true;
            //if (updated_quats.All(x => x)) // si todos son true
            if(IMUsReceived % 4 == 0)
            {
                // No se si hace falta esto
                Quaternion[,] mQ_sensors_raw_list_clone = mQ_sensors_raw_list.Clone() as Quaternion[,];
                quat_history.Add(mQ_sensors_raw_list_clone);
                //updated_quats = new bool[TOTAL_SENSORS]; // Reinicializa a false
                float[] ankleData = new float[NUM_PACK];
                float[] hipData = new float[NUM_PACK];
                float[] kneeData = new float[NUM_PACK];
                for (int i = 0; i < NUM_PACK; i++)
                {
                    for (int s = 0; s < TOTAL_SENSORS; s++)
                    {
                        mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                    }
                    Quaternion mQ_left = new Quaternion();
                    Quaternion mQ_right = new Quaternion();
                    updateLeftAndRightQuats(mQ_virtual, ref mQ_left, ref mQ_right);
                    Quaternion[] mQ_joints = new Quaternion[TOTAL_JOINTS];
                    Quaternion[] mQ_segments = new Quaternion[TOTAL_SENSORS];
                    double[] eulerAnglesZ = new double[3];
                    updateSegmentsAndJoints(mQ_sensors_raw, mQ_left, mQ_right, ref mQ_joints, ref mQ_segments, ref eulerAnglesZ);
                    ankleData[i] = (float)eulerAnglesZ[ankleIndex];
                    hipData[i] = (float)eulerAnglesZ[hipIndex];
                    kneeData[i] = (float)eulerAnglesZ[kneeIndex];
                }
                ankle.drawData(ankleData);
                hip.drawData(hipData);
                knee.drawData(kneeData);
                if (virtualToolBar.recordState == RecordState.Recording)
                {
                    float offsetAnkle = (float)ankle.model.offset;
                    float offsetHip = (float)hip.model.offset;
                    float offsetKnee = (float)knee.model.offset;
                    string dataline = "";
                    for (int i = 0; i < NUM_PACK; i++)
                    {
                        dataline += "1 " + (fakets + 0.01 * i).ToString("F2", CultureInfo.InvariantCulture) + " " + (frame + i).ToString() + " " +
                            (ankleData[i] + offsetAnkle).ToString("F2", CultureInfo.InvariantCulture) + " " + (hipData[i] + offsetHip).ToString("F2", CultureInfo.InvariantCulture) + " " +
                            (kneeData[i] + offsetKnee).ToString("F2", CultureInfo.InvariantCulture) + "\n";
                    }
                    frame += NUM_PACK;
                    fakets += NUM_PACK * 0.01f;
                    mainWindow.fileSaver.appendCSVManual(dataline);
                }
            }
        }
        private void recalculate()
        {
            int size = Math.Min(NUM_PACK * quat_history.Count, ModelSagital.captureCapacity);
            Trace.WriteLine("size = " + size);
            float[] ankleData = new float[size];
            float[] hipData = new float[size];
            float[] kneeData = new float[size];
            int last_index = quat_history.Count - 1; // quat_history.Count puede cambiar en medio del bucle
            int start_index = Math.Max(quat_history.Count - (ModelSagital.captureCapacity / NUM_PACK), 0);
            for (int h = start_index; h < last_index; h++)
            {
                Quaternion[,] mQ_sensors_raw_list = quat_history[h];
                for (int i = 0; i < NUM_PACK; i++)
                {
                    for (int s = 0; s < TOTAL_SENSORS; s++)
                    {
                        mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                    }
                    Quaternion mQ_left = new Quaternion();
                    Quaternion mQ_right = new Quaternion();
                    updateLeftAndRightQuats(mQ_virtual, ref mQ_left, ref mQ_right);
                    Quaternion[] mQ_joints = new Quaternion[TOTAL_JOINTS];
                    Quaternion[] mQ_segments = new Quaternion[TOTAL_SENSORS];
                    double[] eulerAnglesZ = new double[3];
                    updateSegmentsAndJoints(mQ_sensors_raw, mQ_left, mQ_right, ref mQ_joints, ref mQ_segments, ref eulerAnglesZ);
                    int index = (h - start_index) * NUM_PACK + i;
                    ankleData[index] = (float)eulerAnglesZ[ankleIndex];
                    hipData[index] = (float)eulerAnglesZ[hipIndex];
                    kneeData[index] = (float)eulerAnglesZ[kneeIndex];
                }
            }
            ankle.redrawData(ankleData);
            hip.redrawData(hipData);
            knee.redrawData(kneeData);
        }
        public void quaternionCalcsConnect()
        {
            mQ_sensors_raw_list = new Quaternion[NUM_PACK,TOTAL_SENSORS];
            updated_quats = new bool[TOTAL_SENSORS]; // default false

            mQ_sensors_raw = new Quaternion[TOTAL_SENSORS];
            mQ_segments = new Quaternion[TOTAL_SENSORS];
            mQ_sensors_ref = new Quaternion[TOTAL_SENSORS];
            mQ_compensate = new Quaternion[TOTAL_SENSORS];

            mQ_joints = new Quaternion[TOTAL_JOINTS];

            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                for(int j = 0; j < NUM_PACK; j++)
                {
                    mQ_sensors_raw_list[j, i] = idenQuat;
                }
                mQ_sensors_raw[i] = idenQuat;
                mQ_segments[i] = idenQuat;
                mQ_sensors_ref[i] = idenQuat;
                mQ_compensate[i] = idenQuat;
            }

            mQ_virtual = Quaternion.Identity;
        }
        public void calculateMounting()
        {
            Array.Copy(mQ_sensors_raw, mQ_sensors_ref, TOTAL_SENSORS);
            for (int iSen = 0; iSen <= 3; ++iSen)
            {
                mQ_sensors_ref[iSen] = Quaternion.Normalize(mQ_sensors_ref[iSen]);
            }
            Task.Run(() =>
            {
                MessageBox.Show("Sensor Mounting Done", "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
            });
            recalculate();
        }
        public void calculateVirtualOrientation()
        {
            //Virtual axis

            Quaternion Q_virtual_creator = mQ_sensors_raw[0];

            Vector3 world_X = new Vector3(1, 0, 0);
            Vector3 world_Z = new Vector3(0, 0, 1);


            Vector3 vc_Z = Vector3.Transform(world_Z, Q_virtual_creator);       // Apply rotation to Z axis of the sensor
            Vector3 vc_Z_proy_XY = vc_Z - (Vector3.Dot(vc_Z, world_Z) * world_Z); // Projection: rotated Y axis on the XY plane
            Vector3 forward_line = Vector3.Normalize(vc_Z_proy_XY);              // Normalize projection to create a "forward line"
            if (float.IsNaN(forward_line.X)) // solo hay que checkear un (se dividirian todos por 0)
            {
                forward_line = vc_Z_proy_XY;
            }
            double dotx = Vector3.Dot(world_X, forward_line);
            double detx = Vector3.Dot(world_Z, Vector3.Cross(world_X, forward_line));
            double angle = Math.Atan2(detx, dotx);                           // Determine angle between w_X and the forward line

            Quaternion Qvirtual = new Quaternion((float)angle, 0, 0, 1);   // Construct a quat with the needed rotation
                                                                           // result quaternion = (angle, 0, 0, 1)
            mQ_virtual = Quaternion.Normalize(Qvirtual);
            string message = "Frontal Reference Done\n" + "Q_virtual_creator: " +
                mQ_virtual.X.ToString("0.#", CultureInfo.InvariantCulture) + ", " + mQ_virtual.Y.ToString("0.#", CultureInfo.InvariantCulture) + ", " +
                mQ_virtual.Z.ToString("0.#", CultureInfo.InvariantCulture) + ", " + mQ_virtual.W.ToString("0.#", CultureInfo.InvariantCulture) + "\n";
            Task.Run(() =>
            {
                MessageBox.Show(message, "Info", MessageBoxButton.OK, MessageBoxImage.Information, MessageBoxResult.Yes);
            });
            recalculate();
        }
        public void updateLeftAndRightQuats(Quaternion mQ_virtual, ref Quaternion mQ_left, ref Quaternion mQ_right)
        {

            Quaternion Q_isb = new Quaternion(new Vector3(1, 0, 0), 90 * (float)Utils.M_PI / 180);
            Q_isb = Quaternion.Normalize(Q_isb);

            mQ_left = Quaternion.Conjugate(Q_isb) * Quaternion.Conjugate(mQ_virtual);
            mQ_left = Quaternion.Normalize(mQ_left);

            mQ_right = mQ_virtual * Q_isb;
            mQ_right = Quaternion.Normalize(mQ_right);
        }
        public void updateSegmentsAndJoints(Quaternion[] mQ_sensors_raw, Quaternion mQ_left, Quaternion mQ_right,
            ref Quaternion[] mQ_joints, ref Quaternion[] mQ_segments, ref double[] eulerAnglesZ)
        {

            //this->compensateRawQuats(mQ_sensors_raw);

            Quaternion refLiveDifference, operateLeft;

            // Store segments (ISB)

            for (int iSensor = 0; iSensor <= (TOTAL_SENSORS - 1); ++iSensor)
            {

                // Raw quats were not normalized. Ref quats were (in ModelCalibrationPanel).

                refLiveDifference = Quaternion.Normalize(mQ_sensors_raw[iSensor]) * Quaternion.Conjugate(mQ_sensors_ref[iSensor]);
                refLiveDifference = Quaternion.Normalize(refLiveDifference);

                operateLeft = mQ_left * refLiveDifference;
                operateLeft = Quaternion.Normalize(operateLeft);

                mQ_segments[iSensor] = operateLeft * mQ_right;
                mQ_segments[iSensor] = Quaternion.Normalize(mQ_segments[iSensor]);

            }

            // Store joints (ISB)

            Quaternion Q_proximal, Q_distal;

            for (int iJoint = 0; iJoint <= (TOTAL_JOINTS - 1); ++iJoint)
            {

                Q_proximal = mQ_segments[iJoint];
                Q_distal = mQ_segments[iJoint + 1];

                // 0 and 1 : right hip   [0]
                // 1 and 2 : right knee  [1]
                // 2 and 3 : right ankle [2]
                // 0 and 4 : left hip    [3]  <--- 0 and 4!

                mQ_joints[iJoint] = Quaternion.Conjugate(Q_proximal) * Q_distal;
                mQ_joints[iJoint] = Quaternion.Normalize(mQ_joints[iJoint]);

                //Tenemos los joints
            }

            /// Quaternion to euler angles conversion.

            eulerAnglesZ = new double[] { 0, 0, 0 }; // 3 sagittal angles, init them with 0.0
                                                         // esto sería un array con 6 ángulos correspondientes a las articulaciones e incializados a 0.0

            for (int iAngle = 0; iAngle <= (eulerAnglesZ.Length - 1); ++iAngle)
            {
                double[] res = new double[3];
                Utils.quaternion2euler(mQ_joints[iAngle], ref res, Utils.RotSeq.yxz); // rotation sequence conventions are backwards

                // Change the angle signs according to clinical conventions

                int sign = 1;

                // change signs for right and left knees
                if (iAngle == 1 || iAngle == 4)
                {
                    sign = -1;
                }

                // LE QUITO EL SIGNO POR AHORA

                eulerAnglesZ[iAngle] = sign * res[0] * 180 / Utils.M_PI; // otherlib
            }
            //Esto es lo que obtenemos: eulerAnglesZ , mQ_sensors_raw, mQ_segments, mQ_joints 
        }
    }
}
