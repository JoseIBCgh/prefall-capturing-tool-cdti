using DirectShowLib.SBE;
using ibcdatacsharp.UI.DeviceList;
using ibcdatacsharp.UI.FileSaver;
using ibcdatacsharp.UI.Graphs;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using MS.WindowsAPICodePack.Internal;
using OpenCvSharp;
using ScottPlot.Statistics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Threading;
using Quaternion = System.Numerics.Quaternion;
using ibcdatacsharp.UI.Common;

namespace ibcdatacsharp.UI
{
    // Se encarga de manejar los grafos
    public class GraphManager
    {
        public CaptureManager captureManager;
        public ReplayManager replayManager;
        public List<Frame> graphs;

       

        public GraphManager()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.initialized += (sender, args) => finishInit();
        }
        // Para solucionar problemas de dependencias
        private void finishInit()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            VirtualToolBar virtualToolBar = mainWindow.virtualToolBar;
            Device.Device device = mainWindow.device;
            graphs = new List<Frame>();
            graphs.Add(mainWindow.accelerometer);
            graphs.Add(mainWindow.gyroscope);
            graphs.Add(mainWindow.magnetometer);
            graphs.Add(mainWindow.linAcc);
            graphs.Add(mainWindow.angleX);
            graphs.Add(mainWindow.angleY);
            graphs.Add(mainWindow.angleZ);
            graphs.Add(mainWindow.angularVelocity);
            graphs.Add(mainWindow.angularAcceleration);
            if (mainWindow.timeLine.Content == null)
            {
                mainWindow.timeLine.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    TimeLine.TimeLine timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                    replayManager = new ReplayManager(timeLine, graphs);
                };
            }
            else
            {
                TimeLine.TimeLine timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                replayManager = new ReplayManager(timeLine, graphs);
            }


            captureManager = new CaptureManager(graphs, virtualToolBar, device);
        }
        public void initReplay(GraphData data)
        {
            if (captureManager.active)
            {
                captureManager.deactivate();
            }
            if (!replayManager.active)
            {
                replayManager.activate(data);
            }
            else
            {
                replayManager.reset(data);
            }
        }
        // Configura el modo capture
        public void initCapture()
        {
            if (replayManager.active)
            {
                replayManager.deactivate();
            }
            if (!captureManager.active)
            {
                captureManager.activate();
            }
            else
            {
                captureManager.reset();
            }
        }
    }
    public class CaptureManager
    {
        public bool active { get; private set; }
        private const int RENDER_MS = 100;
        private System.Timers.Timer timerRender;
        private List<Frame> graphs;
        private VirtualToolBar virtualToolBar;
        private Device.Device device;

        public GraphAccelerometer accelerometer;
        public GraphGyroscope gyroscope;
        public GraphMagnetometer magnetometer;
        public GraphLinAcc linAcc;

        public AngleGraphX angleX;
        public AngleGraphY angleY;
        public AngleGraphZ angleZ;
        public GraphAngularVelocity angleVel;
        public GraphAngularAcceleration angleAcc;

        private int numIMUs;

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;


        //Begin Wise
        public Dictionary<string, WisewalkSDK.Device> devices_list = new Dictionary<string, WisewalkSDK.Device>();
        public List<int> counter = new List<int>();
        
        public string frame2;
        public int sr;
        int timespan;
        string ts;
        int frame = 0;
        private bool resetFrame = false;

        Quaternion q1 = new Quaternion();
        Quaternion refq = new Quaternion();
        Quaternion q_lower = new Quaternion();
        Quaternion q_upper = new Quaternion();

        int anglequat = 0;
        int mac1 = 0;
        int mac2 = 0;

        double alpha = 0.0d;
        double delta = 0.0d;
        double phi = 0.0d;

        float a1 = 0.0f;
        float a2 = 0.0f;
        float a3 = 0.0f;

        string dataline;

        float fakets = 0.01f;

        string error = "";

        GraphAccelerometer acc;
        GraphGyroscope gyr;
        GraphMagnetometer mag;


        Vector3 v0, v1, v2, v3;

        public delegate void QuaternionEventHandler(object sender, byte id, Quaternion q);
        public event QuaternionEventHandler quaternionEvent;
        //End Wise
        public CaptureManager(List<Frame> graphs, VirtualToolBar virtualToolBar, Device.Device device)
        { 
            active = false;
            this.graphs = graphs;
            this.virtualToolBar = virtualToolBar;
            this.device = device;
            saveGraphs();

            mainWindow.virtualToolBar.saveEvent += onInitRecord;

        }
        private void saveGraphs()
        {
            if (mainWindow.accelerometer.Content == null)
            {
                mainWindow.accelerometer.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    accelerometer = mainWindow.accelerometer.Content as GraphAccelerometer;
                };
            }
            else
            {
                accelerometer = mainWindow.accelerometer.Content as GraphAccelerometer;
            }
            if (mainWindow.gyroscope.Content == null)
            {
                mainWindow.gyroscope.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    gyroscope = mainWindow.gyroscope.Content as GraphGyroscope;
                };
            }
            else
            {
                gyroscope = mainWindow.gyroscope.Content as GraphGyroscope;
            }
            if (mainWindow.magnetometer.Content == null)
            {
                mainWindow.magnetometer.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    magnetometer = mainWindow.magnetometer.Content as GraphMagnetometer;
                };
            }
            else
            {
                magnetometer = mainWindow.magnetometer.Content as GraphMagnetometer;
            }
            if (mainWindow.linAcc.Content == null)
            {
                mainWindow.linAcc.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    linAcc = mainWindow.linAcc.Content as GraphLinAcc;
                };
            }
            else
            {
                linAcc = mainWindow.linAcc.Content as GraphLinAcc;
            }

            if (mainWindow.angleX.Content == null)
            {
                mainWindow.angleX.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    angleX = mainWindow.angleX.Content as AngleGraphX;
                };
            }
            else
            {
                angleX = mainWindow.angleX.Content as AngleGraphX;
            }
            if (mainWindow.angleY.Content == null)
            {
                mainWindow.angleY.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    angleY = mainWindow.angleY.Content as AngleGraphY;
                };
            }
            else
            {
                angleY = mainWindow.angleY.Content as AngleGraphY;
            }
            if (mainWindow.angleZ.Content == null)
            {
                mainWindow.angleZ.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    angleZ = mainWindow.angleZ.Content as AngleGraphZ;
                };
            }
            else
            {
                angleZ = mainWindow.angleZ.Content as AngleGraphZ;
            }
            if (mainWindow.angularVelocity.Content == null)
            {
                mainWindow.angularVelocity.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    angleVel = mainWindow.angularVelocity.Content as GraphAngularVelocity;
                };
            }
            else
            {
                angleVel = mainWindow.angularVelocity.Content as GraphAngularVelocity;
            }
            if (mainWindow.angularAcceleration.Content == null)
            {
                mainWindow.angularAcceleration.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    angleAcc = mainWindow.angularAcceleration.Content as GraphAngularAcceleration;
                };
            }
            else
            {
                angleAcc = mainWindow.angularAcceleration.Content as GraphAngularAcceleration;
            }
        }


        public void activate()
        {

            if (!active)
            {
                active = true;
                timerRender = new System.Timers.Timer(RENDER_MS);
                timerRender.AutoReset = true;

                mainWindow.api.dataReceived += Api_dataReceived;

                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;

                            graph.initCapture();



                            //timerCapture.Elapsed += graph.onTick;
                            timerRender.Elapsed += graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.initCapture();


                        //timerCapture.Elapsed += graph.onTick;
                        timerRender.Elapsed += graph.onRender;
                    }
                }


                virtualToolBar.pauseEvent += onPause; //funcion local
                virtualToolBar.stopEvent += onStop; //funcion local
                if (virtualToolBar.pauseState == PauseState.Play)
                {
                    timerRender.Start();
                }
                device.initTimer();


            }


            Trace.WriteLine("Imu seleccionado en el graphmanager: ", mainWindow.imuInfo.id.ToString());

            mainWindow.api.StopStream(out error);
            if (virtualToolBar.pauseState == PauseState.Play)
            {
                mainWindow.startActiveDevices();
            }

            // Puede que haya que cambiar esto
            Application.Current.Dispatcher.InvokeAsync(() =>
            {
                numIMUs = (mainWindow.deviceList.Content as DeviceList.DeviceList).numIMUsUsed;
            });
        }
        public void deactivate()
        {
            if (active)
            {
                active = false;
                timerRender.Stop();
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            graph.initCapture();
                            //timerCapture.Elapsed -= graph.onTick;
                            //timerRender.Elapsed -= graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.initCapture();
                        //timerCapture.Elapsed -= graph.onTick;
                        //timerRender.Elapsed -= graph.onRender;
                    }
                }
                virtualToolBar.pauseEvent -= onPause; //funcion local
                virtualToolBar.stopEvent -= onStop; //funcion local
                timerRender.Dispose();

                mainWindow.api.StopStream(out error);
            }
        }
        public void reset()
        {
            if (active)
            {
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            graph.initCapture();
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.initCapture();
                    }
                }
                mainWindow.api.StopStream(out error);
                if (virtualToolBar.pauseState == PauseState.Play)
                {
                    mainWindow.startActiveDevices();
                }

                // Puede que haya que cambiar esto
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    numIMUs = (mainWindow.deviceList.Content as DeviceList.DeviceList).numIMUsUsed;
                });
            }
        }
        // Se ejecuta al clicar pause
        void onPause(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                timerRender.Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                timerRender.Start();
            }
        }
        // Se ejecuta al clicar stop
        void onStop(object sender)
        {
            deactivate();
        }

        public void onInitRecord(object sender, EventArgs args)
        {
            fakets = 0;
            frame = 0;
        }

        //Begin Wise
        //Callback para recoger datas del IMU
        public void Api_dataReceived(byte deviceHandler, WisewalkSDK.WisewalkData data)
        {

            

            // refq = 0.823125, 0.000423, 0.009129, -0.567773


            /**
             * 
             *  
                X1 = Q1*X*conj(Q1);
                Y1 = Q1*Y*conj(Q1);
                Z1 = Q1*Z*conj(Q1);
                X2 = Q2*X*conj(Q2);
                Y2 = Q2*Y*conj(Q2);
                Z2 = Q2*Z*conj(Q2);
                DiffAngleX = acos(dot(X1,X2));
                DiffAngleY = acos(dot(Y1,Y2));
                DiffAngleZ = acos(dot(Z1,Z2));
             */

            /*
            
            Q1 = C0:97:3C:F2:DA:40
            Q2 = D8:D3:A5:0A:4F:BC
             */


            //ref_quaternion:
            //0.823125, 0.000423, 0.009129, -0.567773 Z para arriba
            // 0.176144, -0.189621, 0.693031, -0.672846 Y para arriba
            // 0.516224, 0.4542, 0.55528, -0.467841 X para arriba

            refq.W = 0.176144f;
            refq.X = -0.189621f;
            refq.Y = 0.693031f;
            refq.Z = -0.672846f;


            Matrix4x4 refmat = Matrix4x4.CreateFromQuaternion(refq);


            //if (data.Imu.Count > 0 ) {
            ////    Trace.WriteLine("Data: " + " " + devices_list[deviceHandler.ToString()].Id.ToString() + " " + tsA.ToString("F3") + " " + devices_list[deviceHandler.ToString()].NPackets.ToString() + " "
            ////+ data.Quat[0].W.ToString() + ", " + data.Quat[0].X.ToString() + ", " + data.Quat[0].Y.ToString() + ", " + data.Quat[0].Z.ToString());

            //    if (devices_list[deviceHandler.ToString()].Id.ToString() == "C0:97:3C:F2:DA:40"  )  
            //    {
            //        q_lower.W = (float)data.Quat[0].W;
            //        q_lower.X = (float)data.Quat[0].X;
            //        q_lower.Y = (float)data.Quat[0].Y;
            //        q_lower.Z = (float)data.Quat[0].Z;
            //        anglequat++;

            //    }

            //    else if ( devices_list[deviceHandler.ToString()].Id.ToString() == "D8:D3:A5:0A:4F:BC" )
            //    {
            //        q_upper.W = (float)data.Quat[0].W;
            //        q_upper.X = (float)data.Quat[0].X;
            //        q_upper.Y = (float)data.Quat[0].Y;
            //        q_upper.Z = (float)data.Quat[0].Z;
            //        anglequat++;
            //    }



            //   if (anglequat % 2 == 0)
            //    {
            //        vector3 angle_low = new();
            //        vector3 angle_up = new();
            //        vector3 angle_ref = new();
            //        angle_low = toeulerangles(q_lower);
            //        angle_up = toeulerangles(q_upper);
            //        angle_ref = toeulerangles(refq);
            //        a1 = angle_low.x - angle_up.x + angle_ref.x;
            //        a2 = angle_low.y - angle_up.y + angle_ref.y;
            //        a3 = angle_low.z - angle_up.z + angle_ref.z;
            //        a1 = todegrees(a1);
            //        a2 = todegrees(a2);
            //        a3 = todegrees(a3);

            //       // trace.writeline(":::::: angle joint: " + a1.tostring() + " " + a2.tostring() + " " + a3.tostring());

            //        matrix4x4 m_lower = matrix4x4.createfromquaternion(q_lower);
            //        matrix4x4 m_upper = matrix4x4.createfromquaternion(q_upper);

            //        matrix4x4 r = matrix4x4.multiply(m_lower, m_upper);

            //        double beta = math.atan2(r.m32 , math.sqrt( math.pow(r.m12,2) * math.pow(r.m22, 2) ) );
            //        double delta = math.atan2(-(r.m12 / math.cos(beta)), r.m22 / math.cos(beta));
            //        double phi = math.atan2(-(r.m31 / math.cos(beta)), r.m33 / math.cos(beta));

            //        if (beta >= 90.0 && beta < 91.0)
            //        {
            //            beta = 90.0d;
            //            delta = 0.0d;
            //            phi = math.atan2(r.m13, r.m23);

            //        }

            //        //trace.writeline("beta: " + todegrees((float) beta).tostring() + " delta:" + todegrees((float) delta).tostring() + 
            //        //    " phi: " + todegrees((float) phi).tostring());                   
            //    }
            //}

            

            
           
            // Only a IMU
            if (numIMUs == 1)
            {
                // Cálculo de la aceleración lineal
                v0 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[0].W, (float)data.Quat[0].X, (float)data.Quat[0].Y, (float)data.Quat[0].Z), new Vector3(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z));
                v1 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[1].W, (float)data.Quat[1].X, (float)data.Quat[1].Y, (float)data.Quat[1].Z), new Vector3(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z));
                v2 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[2].W, (float)data.Quat[2].X, (float)data.Quat[2].Y, (float)data.Quat[2].Z), new Vector3(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z));
                v3 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[3].W, (float)data.Quat[3].X, (float)data.Quat[3].Y, (float)data.Quat[3].Z), new Vector3(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z));

                int index = 3;
                Quaternion q = new Quaternion((float)data.Quat[index].W, (float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z);
                quaternionEvent?.Invoke(this, deviceHandler, q);

                if (virtualToolBar.recordState == RecordState.Recording)
                {
                    dataline = "1 " + (fakets).ToString("F2") + " " + frame.ToString() + " " + data.Imu[0].acc_x.ToString("F3") + " " + data.Imu[0].acc_y.ToString("F3") + " " + data.Imu[0].acc_z.ToString("F3") + " " + data.Imu[0].gyro_x.ToString("F3") + " " + data.Imu[0].gyro_y.ToString("F3") + " " + data.Imu[0].gyro_z.ToString("F3") + " " + data.Imu[0].mag_x.ToString("F3") + " " + data.Imu[0].mag_y.ToString("F3") + " " + data.Imu[0].mag_z.ToString("F3") + " " + v0.X.ToString("F3") + " " + v0.Y.ToString("F3") + " " + v0.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.01).ToString("F2") + " " + (frame + 1).ToString() + " " + data.Imu[1].acc_y.ToString("F3") + " " + data.Imu[1].acc_y.ToString("F3") + " " + data.Imu[1].acc_z.ToString("F3") + " " + data.Imu[1].gyro_x.ToString("F3") + " " + data.Imu[1].gyro_y.ToString("F3") + " " + data.Imu[1].gyro_z.ToString("F3") + " " + data.Imu[1].mag_x.ToString("F3") + " " + data.Imu[1].mag_y.ToString("F3") + " " + data.Imu[1].mag_z.ToString("F3") + " " + v1.X.ToString("F3") + " " + v1.Y.ToString("F3") + " " + v1.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.02).ToString("F2") + " " + (frame + 2).ToString() + " " + data.Imu[2].acc_y.ToString("F3") + " " + data.Imu[2].acc_y.ToString("F3") + " " + data.Imu[2].acc_z.ToString("F3") + " " + data.Imu[2].gyro_x.ToString("F3") + " " + data.Imu[2].gyro_y.ToString("F3") + " " + data.Imu[2].gyro_z.ToString("F3") + " " + data.Imu[2].mag_x.ToString("F3") + " " + data.Imu[2].mag_y.ToString("F3") + " " + data.Imu[2].mag_z.ToString("F3") + " " + v2.X.ToString("F3") + " " + v2.Y.ToString("F3") + " " + v2.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.03).ToString("F2") + " " + (frame + 3).ToString() + " " + data.Imu[3].acc_y.ToString("F3") + " " + data.Imu[3].acc_y.ToString("F3") + " " + data.Imu[3].acc_z.ToString("F3") + " " + data.Imu[3].gyro_x.ToString("F3") + " " + data.Imu[3].gyro_y.ToString("F3") + " " + data.Imu[3].gyro_z.ToString("F3") + " " + data.Imu[3].mag_x.ToString("F3") + " " + data.Imu[3].mag_y.ToString("F3") + " " + data.Imu[3].mag_z.ToString("F3") + " " + v3.X.ToString("F3") + " " + v3.Y.ToString("F3") + " " + v3.Z.ToString("F3") + "\n";

                    mainWindow.fileSaver.appendCSVManual(dataline);
                }

                //Trace.WriteLine(dataline);

                //accelerometer.drawRealTimeData(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z);

                //Forma Async de pintar gráficas

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    GraphAccelerometer acc = (GraphAccelerometer)graphs[0].Content;

                    acc.drawRealTimeData(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z);
                    acc.drawRealTimeData(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z);
                    acc.drawRealTimeData(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z);
                    acc.drawRealTimeData(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z);

                    GraphGyroscope gyr = (GraphGyroscope)graphs[1].Content;

                    gyr.drawRealTimeData(data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z);
                    gyr.drawRealTimeData(data.Imu[1].gyro_x, data.Imu[1].gyro_y, data.Imu[1].gyro_z);
                    gyr.drawRealTimeData(data.Imu[2].gyro_x, data.Imu[2].gyro_y, data.Imu[2].gyro_z);
                    gyr.drawRealTimeData(data.Imu[3].gyro_x, data.Imu[3].gyro_y, data.Imu[3].gyro_z);

                    GraphMagnetometer mag = (GraphMagnetometer)graphs[2].Content;

                    mag.drawRealTimeData(data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z);
                    mag.drawRealTimeData(data.Imu[1].mag_x, data.Imu[1].mag_y, data.Imu[1].mag_z);
                    mag.drawRealTimeData(data.Imu[2].mag_x, data.Imu[2].mag_y, data.Imu[2].mag_z);
                    mag.drawRealTimeData(data.Imu[3].mag_x, data.Imu[3].mag_y, data.Imu[3].mag_z);

                    GraphLinAcc linAcc = (GraphLinAcc)graphs[3].Content;
                    linAcc.drawRealTimeData(v0);
                    linAcc.drawRealTimeData(v1);
                    linAcc.drawRealTimeData(v2);
                    linAcc.drawRealTimeData(v3);

                });


                //Application.Current.Dispatcher.InvokeAsync(() =>
                //{
                //    gyr = (GraphGyroscope)graphs[1].Content;

                //    gyr.drawRealTimeData(data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z);
                //    gyr.render();

                //});

                //Application.Current.Dispatcher.InvokeAsync(() =>
                //{
                //    mag = (GraphMagnetometer)graphs[2].Content;

                //    mag.drawRealTimeData(data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z);
                //    mag.render();

                //});





                //using (StreamWriter sw = File.AppendText("C:\\Temp\\output.txt"))
                //{
                //    sw.WriteLine(dataline);

                //}
                frame += 4;
                fakets += 0.04f;

                //    await Task.WhenAll(new Task[] {
                //        updateAccelerometer(frame, data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z),
                //        updateMagnetometer(frame, data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z),
                //        updateGyroscope(frame, data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z),
                //        renderAcceletometer(),
                //        renderGyroscope(),
                //        renderMagnetometer(),



                //    //v.appendCSV( data.timespans[0] , frame,data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z, data.Imu[0].gyro_x, 
                //    //data.Imu[0].gyro_y, data.Imu[0].gyro_z, data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z),

                //    //v.appendCSV( data.timespans[1] , frame + 1,data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z, data.Imu[1].gyro_x,
                //    //data.Imu[1].gyro_y, data.Imu[1].gyro_z, data.Imu[1].mag_x, data.Imu[1].mag_y, data.Imu[1].mag_z),

                //    //v.appendCSV( data.timespans[2] , frame + 2,data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z, data.Imu[2].gyro_x,
                //    //data.Imu[2].gyro_y, data.Imu[2].gyro_z, data.Imu[2].mag_x, data.Imu[2].mag_y, data.Imu[2].mag_z),

                //    // v.appendCSV( data.timespans[3] , frame + 3, data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z, data.Imu[3].gyro_x,
                //    //data.Imu[3].gyro_y, data.Imu[3].gyro_z, data.Imu[3].mag_x, data.Imu[3].mag_y, data.Imu[3].mag_z),


                //});

            }
            else if(numIMUs == 2)
            {
                float[] angleX = new float[4] { Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90) };
                float[] angleY = new float[4] { Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90) };
                float[] angleZ = new float[4] { Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90), Helpers.NextFloat(-90, 90) };
                Vector3[] angularVelocity = new Vector3[4] { Vector3.One, Vector3.One, Vector3.One, Vector3.One };
                Vector3[] angularAcceleration = new Vector3[4] { Vector3.One, Vector3.One, Vector3.One, Vector3.One };
                if (virtualToolBar.recordState == RecordState.Recording)
                {
                    dataline = "1 " + (fakets).ToString("F2") + " " + frame.ToString() + " " + angleX[0].ToString("F3") + " " + angleY[0].ToString("F3") + " " + angleZ[0].ToString("F3") + " " + angularVelocity[0].X.ToString("F3") + " " + angularVelocity[0].Y.ToString("F3") + " " + angularVelocity[0].Z.ToString("F3") + " " + angularAcceleration[0].X.ToString("F3") + " " + angularAcceleration[0].Y.ToString("F3") + " " + angularAcceleration[0].Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.01).ToString("F2") + " " + (frame + 1).ToString() + " " + angleX[1].ToString("F3") + " " + angleY[1].ToString("F3") + " " + angleZ[1].ToString("F3") + " " + angularVelocity[1].X.ToString("F3") + " " + angularVelocity[1].Y.ToString("F3") + " " + angularVelocity[1].Z.ToString("F3") + " " + angularAcceleration[1].X.ToString("F3") + " " + angularAcceleration[1].Y.ToString("F3") + " " + angularAcceleration[1].Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.02).ToString("F2") + " " + (frame + 2).ToString() + " " + angleX[2].ToString("F3") + " " + angleY[2].ToString("F3") + " " + angleZ[2].ToString("F3") + " " + angularVelocity[2].X.ToString("F3") + " " + angularVelocity[2].Y.ToString("F3") + " " + angularVelocity[2].Z.ToString("F3") + " " + angularAcceleration[2].X.ToString("F3") + " " + angularAcceleration[2].Y.ToString("F3") + " " + angularAcceleration[2].Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.03).ToString("F2") + " " + (frame + 3).ToString() + " " + angleX[3].ToString("F3") + " " + angleY[3].ToString("F3") + " " + angleZ[3].ToString("F3") + " " + angularVelocity[3].X.ToString("F3") + " " + angularVelocity[3].Y.ToString("F3") + " " + angularVelocity[3].Z.ToString("F3") + " " + angularAcceleration[3].X.ToString("F3") + " " + angularAcceleration[3].Y.ToString("F3") + " " + angularAcceleration[3].Z.ToString("F3") + "\n";

                    mainWindow.fileSaver.appendCSVManual(dataline);
                }

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    AngleGraphX angleXGraph = (AngleGraphX)graphs[4].Content;

                    angleXGraph.drawRealTimeData(angleX[0]);
                    angleXGraph.drawRealTimeData(angleX[1]);
                    angleXGraph.drawRealTimeData(angleX[2]);
                    angleXGraph.drawRealTimeData(angleX[3]);

                    AngleGraphY angleYGraph = (AngleGraphY)graphs[5].Content;

                    angleYGraph.drawRealTimeData(angleY[0]);
                    angleYGraph.drawRealTimeData(angleY[1]);
                    angleYGraph.drawRealTimeData(angleY[2]);
                    angleYGraph.drawRealTimeData(angleY[3]);

                    AngleGraphZ angleZGraph = (AngleGraphZ)graphs[6].Content;

                    angleZGraph.drawRealTimeData(angleZ[0]);
                    angleZGraph.drawRealTimeData(angleZ[1]);
                    angleZGraph.drawRealTimeData(angleZ[2]);
                    angleZGraph.drawRealTimeData(angleZ[3]);

                    GraphAngularVelocity graphAngularVelocity = (GraphAngularVelocity)graphs[7].Content;

                    graphAngularVelocity.drawRealTimeData(angularVelocity[0]);
                    graphAngularVelocity.drawRealTimeData(angularVelocity[1]);
                    graphAngularVelocity.drawRealTimeData(angularVelocity[2]);
                    graphAngularVelocity.drawRealTimeData(angularVelocity[3]);

                    GraphAngularAcceleration graphAngularAcceleration = (GraphAngularAcceleration)graphs[8].Content;

                    graphAngularAcceleration.drawRealTimeData(angularAcceleration[0]);
                    graphAngularAcceleration.drawRealTimeData(angularAcceleration[1]);
                    graphAngularAcceleration.drawRealTimeData(angularAcceleration[2]);
                    graphAngularAcceleration.drawRealTimeData(angularAcceleration[3]);
                });

                frame += 4;
                fakets += 0.04f;
            }

        }
        //End Wise
    }
    public class ReplayManager
    {
        public bool active { get; private set; }

        public delegate void FrameEventHandler(object sender, int frame);
        public event FrameEventHandler frameEvent;

        private GraphData graphData;
        private TimeLine.TimeLine timeLine;
        private List<Frame> graphs;
        public ReplayManager(TimeLine.TimeLine timeLine, List<Frame> graphs)
        {
            active = false;
            this.timeLine = timeLine;
            this.graphs = graphs;
        }
        public void activate(GraphData graphData)
        {
            if (!active)
            {
                active = true;
                this.graphData = graphData;
                timeLine.model.timeEvent += onUpdateTimeLine;
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.drawData(graphData);
                            frameEvent += graph.onUpdateTimeLine;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.drawData(graphData);
                        frameEvent += graph.onUpdateTimeLine;
                    }
                }
                timeLine.startReplay();
            }
        }
        public void deactivate()
        {
            if (active)
            {
                active = false;
                graphData = null;
                timeLine.model.timeEvent -= onUpdateTimeLine;
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            
                            graph.clearData();
                            frameEvent -= graph.onUpdateTimeLine;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        frameEvent -= graph.onUpdateTimeLine;
                    }
                }
            }
        }
        public void reset(GraphData graphData)
        {
            if (active)
            {
                this.graphData = graphData;
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            graph.drawData(graphData);
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.drawData(graphData);
                    }
                }
                timeLine.startReplay();
            }
        }
        public void onUpdateTimeLine(object sender, double time)
        {
            int initialEstimation(double time)
            {
                double timePerFrame = graphData.maxTime / graphData.maxFrame;
                int expectedFrame = (int)Math.Round(time / timePerFrame);
                return expectedFrame;
            }
            int searchFrameLineal(double time, int currentFrame, int previousFrame, double previousDiference)
            {
                double currentTime = graphData.time(currentFrame);
                double currentDiference = Math.Abs(time - currentTime);
                if (currentDiference >= previousDiference)
                {
                    return previousFrame;
                }
                else if (currentTime < time)
                {
                    if (currentFrame == graphData.maxFrame) //Si es el ultimo frame devolverlo
                    {
                        return graphData.maxFrame;
                    }
                    else
                    {
                        return searchFrameLineal(time, currentFrame + 1, currentFrame, currentDiference);
                    }
                }
                else if (currentTime > time)
                {
                    if (currentFrame == graphData.minFrame) //Si es el primer frame devolverlo
                    {
                        return graphData.minFrame;
                    }
                    else
                    {
                        return searchFrameLineal(time, currentFrame - 1, currentFrame, currentDiference);
                    }
                }
                else //currentTime == time muy poco probable (decimales) pero puede pasar
                {
                    return currentFrame;
                }
            }
            int estimatedFrame = initialEstimation(time);
            estimatedFrame = Math.Max(estimatedFrame, graphData.minFrame); // No salirse del rango
            estimatedFrame = Math.Min(estimatedFrame, graphData.maxFrame); // No salirse del rango
            frameEvent?.Invoke(this, searchFrameLineal(time, estimatedFrame, -1, double.MaxValue));
        }
    }
}
