﻿using ibcdatacsharp.DeviceList.TreeClasses;
using ibcdatacsharp.UI.Common;
using ibcdatacsharp.UI.DeviceList;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Windows;

namespace ibcdatacsharp.UI.SagitalAngles
{
    public class SagitalAngles
    {
        private MainWindow mainWindow;
        private DeviceList.DeviceList deviceList;

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
        private int ankleIndex = 0;
        private int hipIndex = 1;
        private int kneeIndex = 2;

        private bool mounted = false;
        private bool reference_saved = false;

        public SagitalAngles()
        {
            mainWindow = Application.Current.MainWindow as MainWindow;
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
            if (imus.Count == TOTAL_SENSORS) //??
            {
                for(int i = 0; i < imus.Count; i++)
                {
                    byte handler = handlerFromMAC(imus[i].address);
                    indices[handler] = imus[i].id; // Supone que los ids son (0, 1, 2, 3)
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
        public void test2()
        {
            ankle.initCapture();
            hip.initCapture();
            knee.initCapture();
            quaternionCalcsConnect();
            changeQuaternionsListRandom();
            float[] ankleData = new float[NUM_PACK];
            float[] hipData = new float[NUM_PACK];
            float[] kneeData = new float[NUM_PACK];
            for (int i = 0; i < NUM_PACK; i++)
            {
                for (int s = 0; s < TOTAL_SENSORS; s++)
                {
                    mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                }
                updateLeftAndRightQuats();
                updateSegmentsAndJoints();
                /*
                Trace.WriteLine("eulerAngles");
                foreach(float a in eulerAnglesZ)
                {
                    Trace.WriteLine(a);
                }
                */
                ankleData[i] = (float)eulerAnglesZ[ankleIndex];
                hipData[i] = (float)eulerAnglesZ[hipIndex];
                kneeData[i] = (float)eulerAnglesZ[kneeIndex];
            }
            ankle.drawData(ankleData);
            hip.drawData(hipData);
            knee.drawData(kneeData);
        }
        public void processSerialData(byte deviceHandler, WisewalkSDK.WisewalkData data)
        {
            int index = handlerToIndex(deviceHandler);
            for(int i = 0; i < NUM_PACK; i++)
            {
                mQ_sensors_raw_list[i, index] = new Quaternion((float)data.Quat[i].X, (float)data.Quat[i].Y, (float)data.Quat[i].Z, (float)data.Quat[i].W);
            }
            if (mounted && reference_saved)
            {
                updated_quats[index] = true;
                if (updated_quats.All(x => x)) // si todos son true
                {
                    updated_quats = new bool[TOTAL_SENSORS]; // Reinicializa a false
                    float[] ankleData = new float[NUM_PACK];
                    float[] hipData = new float[NUM_PACK];
                    float[] kneeData = new float[NUM_PACK];
                    for (int i = 0; i < NUM_PACK; i++)
                    {
                        for (int s = 0; s < TOTAL_SENSORS; s++)
                        {
                            mQ_sensors_raw[s] = mQ_sensors_raw_list[i, s];
                        }
                        updateLeftAndRightQuats();
                        updateSegmentsAndJoints();
                        ankleData[i] = (float)eulerAnglesZ[ankleIndex];
                        hipData[i] = (float)eulerAnglesZ[hipIndex];
                        kneeData[i] = (float)eulerAnglesZ[kneeIndex];
                    }
                    ankle.drawData(ankleData);
                    hip.drawData(hipData);
                    knee.drawData(kneeData);
                }
            }
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
                mQ_sensors_raw[i] = idenQuat;
                mQ_segments[i] = idenQuat;
                mQ_sensors_ref[i] = idenQuat;
                mQ_compensate[i] = idenQuat;
            }
        }
        public void calculateMounting()
        {
            for(int i = 0; i < TOTAL_SENSORS; i++)
            {
                mQ_sensors_raw[i] = mQ_sensors_raw_list[0, i];
            }
            Array.Copy(mQ_sensors_raw, mQ_sensors_ref, TOTAL_SENSORS);
            for (int iSen = 0; iSen <= 3; ++iSen)
            {
                mQ_sensors_ref[iSen] = Quaternion.Normalize(mQ_sensors_ref[iSen]);
            }
            mounted = true;
        }
        public void calculateVirtualOrientation()
        {
            if (mounted)
            {
                for (int i = 0; i < TOTAL_SENSORS; i++)
                {
                    mQ_sensors_raw[i] = mQ_sensors_raw_list[0, i];
                }
                //Virtual axis

                Quaternion Q_virtual_creator = mQ_sensors_raw[0];

                Vector3 world_X = new Vector3(1, 0, 0);
                Vector3 world_Z = new Vector3(0, 0, 1);


                Vector3 vc_Z = Vector3.Transform(world_Z, Q_virtual_creator);       // Apply rotation to Z axis of the sensor
                Vector3 vc_Z_proy_XY = vc_Z - (Vector3.Dot(vc_Z, world_Z) * world_Z); // Projection: rotated Y axis on the XY plane
                Vector3 forward_line = Vector3.Normalize(vc_Z_proy_XY);              // Normalize projection to create a "forward line"

                double dotx = Vector3.Dot(world_X, forward_line);
                double detx = Vector3.Dot(world_Z, Vector3.Cross(world_X, forward_line));
                double angle = Math.Atan2(detx, dotx);                           // Determine angle between w_X and the forward line

                Quaternion Qvirtual = new Quaternion((float)angle, 0, 0, 1);   // Construct a quat with the needed rotation
                                                                               // result quaternion = (angle, 0, 0, 1)

                mQ_virtual = Quaternion.Normalize(Qvirtual);
                //Trace.WriteLine("mQ_virtual");
                //Trace.WriteLine(mQ_virtual.ToString());
                reference_saved = true;
            }
        }
        public void updateLeftAndRightQuats()
        {

            Quaternion Q_isb = new Quaternion(new Vector3(1, 0, 0), 90 * (float)Utils.M_PI / 180);
            Q_isb = Quaternion.Normalize(Q_isb);

            mQ_left = Quaternion.Conjugate(Q_isb) * Quaternion.Conjugate(mQ_virtual);
            mQ_left = Quaternion.Normalize(mQ_left);

            mQ_right = mQ_virtual * Q_isb;
            mQ_right = Quaternion.Normalize(mQ_right);
            //Trace.WriteLine("mQ_right");
            //Trace.WriteLine(mQ_right.ToString());
        }
        public void updateSegmentsAndJoints()
        {

            //this->compensateRawQuats(mQ_sensors_raw);

            Quaternion refLiveDifference, operateLeft;
            /*
            Trace.WriteLine("mQ_sensors_raw");
            foreach (var sensor in mQ_sensors_raw)
            {
                Trace.WriteLine(sensor.ToString());
            }

            Trace.WriteLine("mQ_sensors_ref");
            foreach (var sensor in mQ_sensors_ref)
            {
                Trace.WriteLine(sensor.ToString());
            }
            */

            // Store segments (ISB)

            for (int iSensor = 0; iSensor <= (TOTAL_SENSORS - 1); ++iSensor)
            {

                // Raw quats were not normalized. Ref quats were (in ModelCalibrationPanel).

                refLiveDifference = Quaternion.Normalize(mQ_sensors_raw[iSensor]) * Quaternion.Conjugate(mQ_sensors_ref[iSensor]);
                refLiveDifference = Quaternion.Normalize(refLiveDifference);

                //Trace.WriteLine("mQ_left");
                //Trace.WriteLine(mQ_left.ToString());

                operateLeft = mQ_left * refLiveDifference;
                operateLeft = Quaternion.Normalize(operateLeft);

                mQ_segments[iSensor] = operateLeft * mQ_right;
                mQ_segments[iSensor] = Quaternion.Normalize(mQ_segments[iSensor]);

            }
            /*
            Trace.WriteLine("mQ_segments");
            foreach(var sensor in mQ_segments)
            {
                Trace.WriteLine(sensor.ToString());
            }
            */

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
                //Trace.WriteLine("eulerAnglesZ before change sign " + res[0]);

                // LE QUITO EL SIGNO POR AHORA

                eulerAnglesZ[iAngle] = sign * res[0] * 180 / Utils.M_PI; // otherlib
            }
            //Esto es lo que obtenemos: eulerAnglesZ , mQ_sensors_raw, mQ_segments, mQ_joints 
        }
    }
}
