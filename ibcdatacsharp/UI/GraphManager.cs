using ibcdatacsharp.UI.DeviceList;
using ibcdatacsharp.UI.Graphs;
using ibcdatacsharp.UI.Graphs.AngleGraph;
using ibcdatacsharp.UI.Graphs.GraphWindow;
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

namespace ibcdatacsharp.UI
{
    // Se encarga de manejar los grafos
    public class GraphManager
    {
        private CaptureManager captureManager;
        private ReplayManager replayManager;
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
            graphs.Add(mainWindow.angle);
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
        private const int CAPTURE_MS = 10;
        private const int RENDER_MS = 100;
        private System.Timers.Timer timerCapture;
        private System.Timers.Timer timerRender;
        private List<Frame> graphs;
        private VirtualToolBar virtualToolBar;
        private Device.Device device;

        public Graphs.GraphWindow.GraphAccelerometer accelerometer;
        public Graphs.GraphWindow.GraphGyroscope gyroscope;
        public Graphs.GraphWindow.GraphMagnetometer magnetometer;

        MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;


        //Begin Wise
        public Dictionary<string, WisewalkSDK.Device> devices_list = new Dictionary<string, WisewalkSDK.Device>();
        public List<int> counter = new List<int>();
        
        public string frame2;
        public int sr;
        int timespan;
        string ts;
        int frame = 0;

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


        //End Wise
        public CaptureManager(List<Frame> graphs, VirtualToolBar virtualToolBar, Device.Device device)
        {
            active = false;
            this.graphs = graphs;
            this.virtualToolBar = virtualToolBar;
            this.device = device;

            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;

        }

       
        public void activate()
        {

            if (!active)
            {
                active = true;
                timerCapture = new System.Timers.Timer(CAPTURE_MS);
                timerCapture.AutoReset = true;
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
                            //timerRender.Elapsed += graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.initCapture();
                        
                       
                        //timerCapture.Elapsed += graph.onTick;
                        //timerRender.Elapsed += graph.onRender;
                    }
                }


                if (mainWindow.accelerometer.Content == null)
                {
                    mainWindow.accelerometer.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        accelerometer = mainWindow.accelerometer.Content as Graphs.GraphWindow.GraphAccelerometer;
                    };
                }
                else
                {
                    accelerometer = mainWindow.accelerometer.Content as Graphs.GraphWindow.GraphAccelerometer;
                }
                if (mainWindow.gyroscope.Content == null)
                {
                    mainWindow.gyroscope.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        gyroscope = mainWindow.gyroscope.Content as Graphs.GraphWindow.GraphGyroscope;
                    };
                }
                else
                {
                    gyroscope = mainWindow.gyroscope.Content as Graphs.GraphWindow.GraphGyroscope;
                }
                if (mainWindow.magnetometer.Content == null)
                {
                    mainWindow.magnetometer.Navigated += delegate (object sender, NavigationEventArgs e)
                    {
                        magnetometer = mainWindow.magnetometer.Content as Graphs.GraphWindow.GraphMagnetometer;
                    };
                }
                else
                {
                    magnetometer = mainWindow.magnetometer.Content as Graphs.GraphWindow.GraphMagnetometer;
                }

                virtualToolBar.pauseEvent += onPause; //funcion local
                virtualToolBar.stopEvent += onStop; //funcion local
                if (virtualToolBar.pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                    timerRender.Start();
                }
                device.initTimer();
            }
            mainWindow.api.SetDevicesConfigurations(100, 3, out error);
            Thread.Sleep(1000);
            mainWindow.api.SetRTCDevices(mainWindow.GetDateTime(), out error);
            Thread.Sleep(1000);

            mainWindow.api.StartStream(out error);

        }
        public void deactivate()
        {
            if (active)
            {
                active = false;
                timerCapture.Stop();
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
                timerCapture.Dispose();
                timerRender.Dispose();
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
            }
        }
        // Se ejecuta al clicar pause
        void onPause(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                timerCapture.Stop();
                timerRender.Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                timerCapture.Start();
                timerRender.Start();
            }
        }
        // Se ejecuta al clicar stop
        void onStop(object sender)
        {
            deactivate();
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
            //        Vector3 angle_low = new();
            //        Vector3 angle_up = new();
            //        Vector3 angle_ref = new();
            //        angle_low = ToEulerAngles(q_lower);
            //        angle_up = ToEulerAngles(q_upper);
            //        angle_ref = ToEulerAngles(refq);
            //        a1 = angle_low.X - angle_up.X + angle_ref.X;
            //        a2 = angle_low.Y - angle_up.Y + angle_ref.Y;
            //        a3 = angle_low.Z - angle_up.Z + angle_ref.Z;
            //        a1 = ToDegrees(a1);
            //        a2 = ToDegrees(a2);
            //        a3 = ToDegrees(a3);

            //       // Trace.WriteLine(":::::: ANGLE JOINT: " + a1.ToString() + " " + a2.ToString() + " " + a3.ToString());

            //        Matrix4x4 m_lower = Matrix4x4.CreateFromQuaternion(q_lower);
            //        Matrix4x4 m_upper = Matrix4x4.CreateFromQuaternion(q_upper);

            //        Matrix4x4 R = Matrix4x4.Multiply(m_lower, m_upper);

            //        double beta = Math.Atan2(R.M32 , Math.Sqrt( Math.Pow(R.M12,2) * Math.Pow(R.M22, 2) ) );
            //        double delta = Math.Atan2(-(R.M12 / Math.Cos(beta)), R.M22 / Math.Cos(beta));
            //        double phi = Math.Atan2(-(R.M31 / Math.Cos(beta)), R.M33 / Math.Cos(beta));

            //        if (beta >= 90.0 && beta < 91.0)
            //        {
            //            beta = 90.0d;
            //            delta = 0.0d;
            //            phi = Math.Atan2(R.M13, R.M23);

            //        }

            //        //Trace.WriteLine("Beta: " + ToDegrees((float) beta).ToString() + " Delta:" + ToDegrees((float) delta).ToString() + 
            //        //    " Phi: " + ToDegrees((float) phi).ToString());                   
            //    }
            //}

            // Only a IMU



            if (true)


            {
                v0 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[0].W, (float)data.Quat[0].X, (float)data.Quat[0].Y, (float)data.Quat[0].Z), new Vector3(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z));
                v1 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[1].W, (float)data.Quat[1].X, (float)data.Quat[1].Y, (float)data.Quat[1].Z), new Vector3(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z));
                v2 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[2].W, (float)data.Quat[2].X, (float)data.Quat[2].Y, (float)data.Quat[2].Z), new Vector3(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z));
                v3 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[3].W, (float)data.Quat[3].X, (float)data.Quat[3].Y, (float)data.Quat[3].Z), new Vector3(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z));


                dataline = "1 " + (fakets).ToString("F2") + " " + frame.ToString() + " " + data.Imu[0].acc_x.ToString("F3") + " " + data.Imu[0].acc_y.ToString("F3") + " " + data.Imu[0].acc_z.ToString("F3") + " " + data.Imu[0].gyro_x.ToString("F3") + " " + data.Imu[0].gyro_y.ToString("F3") + " " + data.Imu[0].gyro_z.ToString("F3") + " " + data.Imu[0].mag_x.ToString("F3") + " " + data.Imu[0].mag_y.ToString("F3") +" " + data.Imu[0].mag_z.ToString("F3") + " " + v0.X.ToString("F3")+ " "+ v0.Y.ToString("F3")+" "+ v0.Z.ToString("F3")+"\n" +
                "1 " + (fakets + 0.01).ToString("F2") + " " + (frame + 1).ToString() + " " + data.Imu[1].acc_y.ToString("F3") + " " + data.Imu[1].acc_y.ToString("F3") + " " + data.Imu[1].acc_z.ToString("F3") + " " + data.Imu[1].gyro_x.ToString("F3") + " " + data.Imu[1].gyro_y.ToString("F3") + " " + data.Imu[1].gyro_z.ToString("F3") + " " + data.Imu[1].mag_x.ToString("F3") + " " + data.Imu[1].mag_y.ToString("F3") + " " + data.Imu[1].mag_z.ToString("F3") + " " + v1.X.ToString("F3") + " " + v1.Y.ToString("F3") + " " + v1.Z.ToString("F3") + "\n" +
                "1 " + (fakets + 0.02).ToString("F2") + " " + (frame + 2).ToString() + " " + data.Imu[2].acc_y.ToString("F3") + " " + data.Imu[2].acc_y.ToString("F3") + " " + data.Imu[2].acc_z.ToString("F3") + " " + data.Imu[2].gyro_x.ToString("F3") + " " + data.Imu[2].gyro_y.ToString("F3") + " " + data.Imu[2].gyro_z.ToString("F3") + " " + data.Imu[2].mag_x.ToString("F3") + " " + data.Imu[2].mag_y.ToString("F3") + " " + data.Imu[2].mag_z.ToString("F3") + " " + v2.X.ToString("F3") + " " + v2.Y.ToString("F3") + " " + v2.Z.ToString("F3") + "\n" +
                "1 "+ (fakets + 0.03).ToString("F2") + " " + (frame + 3).ToString() + " " + data.Imu[3].acc_y.ToString("F3") + " " + data.Imu[3].acc_y.ToString("F3") + " " + data.Imu[3].acc_z.ToString("F3") + " " + data.Imu[3].gyro_x.ToString("F3") + " " + data.Imu[3].gyro_y.ToString("F3") + " " + data.Imu[3].gyro_z.ToString("F3") + " " + data.Imu[3].mag_x.ToString("F3") + " " + data.Imu[3].mag_y.ToString("F3") + " " + data.Imu[3].mag_z.ToString("F3") + " " + v3.X.ToString("F3") + " " + v3.Y.ToString("F3") + " " + v3.Z.ToString("F3") + "\n";

                v0 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[0].W, (float)data.Quat[0].X, (float)data.Quat[0].Y, (float)data.Quat[0].Z), new Vector3(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z));
                

                fakets += 0.04f;
                frame += 4;

                mainWindow.fileSaver.appendCSVManual(dataline);

                Trace.WriteLine(dataline);

                //accelerometer.drawRealTimeData(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z);
                //accelerometer.drawRealTimeData(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z);

                //Forma Async de pintar gráficas

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    acc = (GraphAccelerometer)graphs[0].Content;

                    acc.drawRealTimeData(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z);
                    acc.drawRealTimeData(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z);
                    acc.drawRealTimeData(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z);
                    acc.drawRealTimeData(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z);

                    gyr = (GraphGyroscope)graphs[1].Content;

                    gyr.drawRealTimeData(data.Imu[0].gyro_x, data.Imu[0].gyro_y, data.Imu[0].gyro_z);
                    gyr.drawRealTimeData(data.Imu[1].gyro_x, data.Imu[1].gyro_y, data.Imu[1].gyro_z);
                    gyr.drawRealTimeData(data.Imu[2].gyro_x, data.Imu[2].gyro_y, data.Imu[2].gyro_z);
                    gyr.drawRealTimeData(data.Imu[3].gyro_x, data.Imu[3].gyro_y, data.Imu[3].gyro_z);

                    mag = (GraphMagnetometer)graphs[2].Content;

                    mag.drawRealTimeData(data.Imu[0].mag_x, data.Imu[0].mag_y, data.Imu[0].mag_z);
                    mag.drawRealTimeData(data.Imu[1].mag_x, data.Imu[1].mag_y, data.Imu[1].mag_z);
                    mag.drawRealTimeData(data.Imu[2].mag_x, data.Imu[2].mag_y, data.Imu[2].mag_z);
                    mag.drawRealTimeData(data.Imu[3].mag_x, data.Imu[3].mag_y, data.Imu[3].mag_z);


                    acc.render();
                    gyr.render();
                    mag.render();
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
            //else if (devices_list.Count == 2)
            //{
            //await Task.WhenAll(new Task[] {

            //angleGraph.updateX(frame, a1),
            //angleGraph.updateY(frame, a2),
            //angleGraph.updateZ(frame, a3),
            //angleGraph.renderX(),
            //angleGraph.renderY(),
            //angleGraph.renderZ()


            //});


            //}

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
