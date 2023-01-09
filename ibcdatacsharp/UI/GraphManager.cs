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
using ibcdatacsharp.DeviceList.TreeClasses;

namespace ibcdatacsharp.UI
{
    // Se encarga de manejar los grafos
    public class GraphManager
    {
        public CaptureManager captureManager;
        public ReplayManager replayManager;
        public List<Frame> graphs1IMU;
        public List<Frame> graphs2IMU;



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
            graphs1IMU = new List<Frame>();
            graphs2IMU = new List<Frame>();
            graphs1IMU.Add(mainWindow.accelerometer);
            graphs1IMU.Add(mainWindow.gyroscope);
            graphs1IMU.Add(mainWindow.magnetometer);
            graphs1IMU.Add(mainWindow.linAcc);
            graphs2IMU.Add(mainWindow.angleX);
            graphs2IMU.Add(mainWindow.angleY);
            graphs2IMU.Add(mainWindow.angleZ);
            graphs2IMU.Add(mainWindow.angularVelocity);
            graphs2IMU.Add(mainWindow.angularAcceleration);
            graphs1IMU.Add(mainWindow.quaternions);
            if (mainWindow.timeLine.Content == null)
            {
                mainWindow.timeLine.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    TimeLine.TimeLine timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                    replayManager = new ReplayManager(timeLine, graphs1IMU, graphs2IMU);
                };
            }
            else
            {
                TimeLine.TimeLine timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                replayManager = new ReplayManager(timeLine, graphs1IMU, graphs2IMU);
            }

            if (mainWindow.deviceList.Content == null)
            {
                mainWindow.deviceList.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    DeviceList.DeviceList deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                    captureManager = new CaptureManager(graphs1IMU, graphs2IMU, virtualToolBar, device, deviceList);
                };
            }
            else
            {
                DeviceList.DeviceList deviceList = mainWindow.deviceList.Content as DeviceList.DeviceList;
                captureManager = new CaptureManager(graphs1IMU, graphs2IMU, virtualToolBar, device, deviceList);
            }
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
        private const int RENDER_MS = Config.RENDER_MS_CAPTUE;
        //private System.Timers.Timer timerRender;
        private List<Frame> graphs1IMU;
        private List<Frame> graphs2IMU;
        private VirtualToolBar virtualToolBar;
        private Device.Device device;
        private DeviceList.DeviceList deviceList;
        private TimeLine.TimeLine timeLine;

        public GraphAccelerometer accelerometer;
        public GraphGyroscope gyroscope;
        public GraphMagnetometer magnetometer;
        public GraphLinAcc linAcc;
        public GraphQuaternion quaternions;

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
        Quaternion[] q_lower = new Quaternion[4];
        Quaternion[] q_upper = new Quaternion[4];

        Vector3 prev_angle = new Vector3(0, 0, 0);
        Vector3 prev_angle_vel = new Vector3(0, 0, 0);

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
        float dt = 0.01f;

        string error = "";

        GraphAccelerometer acc;
        GraphGyroscope gyr;
        GraphMagnetometer mag;


        Vector3 v0, v1, v2, v3;

        const float G = 9.8f;

        public delegate void QuaternionEventHandler(object sender, byte handler, Quaternion q);
        public event QuaternionEventHandler quaternionEvent;
        //End Wise
        public CaptureManager(List<Frame> graphs1IMU, List<Frame> graphs2IMU, VirtualToolBar virtualToolBar, Device.Device device, DeviceList.DeviceList deviceList)
        { 
            active = false;
            this.graphs1IMU = graphs1IMU;
            this.graphs2IMU = graphs2IMU;
            this.virtualToolBar = virtualToolBar;
            this.device = device;
            this.deviceList = deviceList;
            saveGraphs();
            saveTimeLine();

            mainWindow.virtualToolBar.saveEvent += onInitRecord;

            refq.W = 0.176144f;
            refq.X = -0.189621f;
            refq.Y = 0.693031f;
            refq.Z = -0.672846f;
        }
        public void setReference(Quaternion q)
        {
            refq = q;
        }
        /*
        private void onNumIMUsFunction(int n)
        {
            if (n == 0)
            {
                //if(numIMUs == 1)
                //{
                accelerometer.hasToRender = false;
                gyroscope.hasToRender = false;
                magnetometer.hasToRender = false;
                linAcc.hasToRender = false;
                quaternions.hasToRender = false;
                //}
                //else if(numIMUs == 2)
                //{
                angleX.hasToRender = false;
                angleY.hasToRender = false;
                angleZ.hasToRender = false;
                angleVel.hasToRender = false;
                angleAcc.hasToRender = false;
                //}
                numIMUs = n;
            }
            else if (n == 1)
            {
                accelerometer.hasToRender = true;
                gyroscope.hasToRender = true;
                magnetometer.hasToRender = true;
                linAcc.hasToRender = true;
                quaternions.hasToRender = true;
                //if(numIMUs == 2)
                //{
                angleX.hasToRender = false;
                angleY.hasToRender = false;
                angleZ.hasToRender = false;
                angleVel.hasToRender = false;
                angleAcc.hasToRender = false;
                //}
                numIMUs = n;
            }
            else if (n == 2)
            {
                angleX.hasToRender = true;
                angleY.hasToRender = true;
                angleZ.hasToRender = true;
                angleVel.hasToRender = true;
                angleAcc.hasToRender = true;
                //if(numIMUs == 1)
                //{
                accelerometer.hasToRender = false;
                gyroscope.hasToRender = false;
                magnetometer.hasToRender = false;
                linAcc.hasToRender = false;
                quaternions.hasToRender = false;
                //}
                numIMUs = n;
            }
        }
        */
        private void saveTimeLine()
        {
            if(mainWindow.timeLine.Content == null)
            {
                mainWindow.timeLine.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                };
            }
            else
            {
                timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
            }
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
            if (mainWindow.quaternions.Content == null)
            {
                mainWindow.quaternions.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    quaternions = mainWindow.quaternions.Content as GraphQuaternion;
                };
            }
            else
            {
                quaternions = mainWindow.quaternions.Content as GraphQuaternion;
            }
        }


        public void activate()
        {

            if (!active)
            {
                active = true;
                timeLine.endReplay(); // De momento cuando empieza a stremear apaga el replay
                //timerRender = new System.Timers.Timer(RENDER_MS);
                //timerRender.AutoReset = true;

                // Se puede poner esta linea para quitar el evento si havia 
                // alguno en caso que haya problemas de que el csv tenga el doble de
                // duracion si no se encuentra otra solucion
                //mainWindow.api.dataReceived -= Api_dataReceived;
                mainWindow.api.dataReceived += Api_dataReceived;

                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    numIMUs = (mainWindow.deviceList.Content as DeviceList.DeviceList).numIMUsUsed;
                    Trace.WriteLine(numIMUs);
                    if (numIMUs == 1)
                    {
                        foreach (Frame frame in graphs1IMU)
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
                    }

                    else if (numIMUs == 2)
                    {
                        foreach (Frame frame in graphs2IMU)
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
                    }


                    //virtualToolBar.pauseEvent += onPause; //funcion local
                    virtualToolBar.stopEvent += onStop; //funcion local
                    //if (virtualToolBar.pauseState == PauseState.Play)
                    //{
                        //timerRender.Start();
                    //}

                });
            }


            Trace.WriteLine("Imu seleccionado en el graphmanager: ", mainWindow.imuInfo.id.ToString());

            mainWindow.api.StopStream(out error);
            if (virtualToolBar.pauseState == PauseState.Play)
            {
                mainWindow.startActiveDevices();
            }
        }
        public void deactivate()
        {
            if (active)
            {
                active = false;
                //timerRender.Stop();
                foreach (Frame frame in graphs1IMU)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            graph.initCapture();
                            //timerRender.Elapsed -= graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.initCapture();
                        //timerRender.Elapsed -= graph.onRender;
                    }
                }
                foreach (Frame frame in graphs2IMU)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            graph.initCapture();
                            //timerRender.Elapsed -= graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.initCapture();
                        //timerRender.Elapsed -= graph.onRender;
                    }
                }
                //virtualToolBar.pauseEvent -= onPause; //funcion local
                virtualToolBar.stopEvent -= onStop; //funcion local
                //timerRender.Dispose();

                mainWindow.api.StopStream(out error);
            }
        }
        public void reset()
        {
            if (active)
            {
                // Puede que haya que cambiar esto
                Application.Current.Dispatcher.InvokeAsync(() =>
                {
                    numIMUs = (mainWindow.deviceList.Content as DeviceList.DeviceList).numIMUsUsed;
                    if (numIMUs == 1)
                    {
                        foreach (Frame frame in graphs1IMU)
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
                    else if (numIMUs == 2)
                    {
                        foreach (Frame frame in graphs2IMU)
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
                    mainWindow.api.StopStream(out error);
                    if (virtualToolBar.pauseState == PauseState.Play)
                    {
                        mainWindow.startActiveDevices();
                    }
                });
            }
        }
        // Se ejecuta al clicar pause
        void onPause(object sender, PauseState pauseState)
        {
            if (pauseState == PauseState.Pause)
            {
                //timerRender.Stop();
            }
            else if (pauseState == PauseState.Play)
            {
                //timerRender.Start();
            }
        }
        // Se ejecuta al clicar stop
        void onStop(object sender)
        {
            deactivate();
            mainWindow.api.dataReceived -= Api_dataReceived;
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
            int index = 3;
            Quaternion q = new Quaternion((float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z, (float)data.Quat[index].W);
            quaternionEvent?.Invoke(this, deviceHandler, q);
            // Only a IMU
            if (numIMUs == 1)
            {
                // Cálculo de la aceleración lineal
                //v0 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[0].W, (float)data.Quat[0].X, (float)data.Quat[0].Y, (float)data.Quat[0].Z), new Vector3(data.Imu[0].acc_x, data.Imu[0].acc_y, data.Imu[0].acc_z));
                //v1 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[1].W, (float)data.Quat[1].X, (float)data.Quat[1].Y, (float)data.Quat[1].Z), new Vector3(data.Imu[1].acc_x, data.Imu[1].acc_y, data.Imu[1].acc_z));
                //v2 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[2].W, (float)data.Quat[2].X, (float)data.Quat[2].Y, (float)data.Quat[2].Z), new Vector3(data.Imu[2].acc_x, data.Imu[2].acc_y, data.Imu[2].acc_z));
                //v3 = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[3].W, (float)data.Quat[3].X, (float)data.Quat[3].Y, (float)data.Quat[3].Z), new Vector3(data.Imu[3].acc_x, data.Imu[3].acc_y, data.Imu[3].acc_z));
                Vector3[] v = new Vector3[4];
                for(int i = 0; i < 4; i++)
                {
                    v[i] = LinearAcceleration.calcLinAcc(new Quaternion((float)data.Quat[i].X, (float)data.Quat[i].Y, (float)data.Quat[i].Z, (float)data.Quat[i].W), new Vector3(data.Imu[i].acc_x, data.Imu[i].acc_y, data.Imu[i].acc_z));
                    //Trace.WriteLine(v[i]);
                }
                v0 = v[0];
                v1 = v[1];
                v2 = v[2];
                v3 = v[3];

                if (virtualToolBar.recordState == RecordState.Recording)
                {
                    /*
                    dataline = "1 " + (fakets).ToString("F2") + " " + frame.ToString() + " " + data.Imu[0].acc_x.ToString("F3") + " " + data.Imu[0].acc_y.ToString("F3") + " " + data.Imu[0].acc_z.ToString("F3") + " " + data.Imu[0].gyro_x.ToString("F3") + " " + data.Imu[0].gyro_y.ToString("F3") + " " + data.Imu[0].gyro_z.ToString("F3") + " " + data.Imu[0].mag_x.ToString("F3") + " " + data.Imu[0].mag_y.ToString("F3") + " " + data.Imu[0].mag_z.ToString("F3") + " " + v0.X.ToString("F3") + " " + v0.Y.ToString("F3") + " " + v0.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.01).ToString("F2") + " " + (frame + 1).ToString() + " " + data.Imu[1].acc_x.ToString("F3") + " " + data.Imu[1].acc_y.ToString("F3") + " " + data.Imu[1].acc_z.ToString("F3") + " " + data.Imu[1].gyro_x.ToString("F3") + " " + data.Imu[1].gyro_y.ToString("F3") + " " + data.Imu[1].gyro_z.ToString("F3") + " " + data.Imu[1].mag_x.ToString("F3") + " " + data.Imu[1].mag_y.ToString("F3") + " " + data.Imu[1].mag_z.ToString("F3") + " " + v1.X.ToString("F3") + " " + v1.Y.ToString("F3") + " " + v1.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.02).ToString("F2") + " " + (frame + 2).ToString() + " " + data.Imu[2].acc_x.ToString("F3") + " " + data.Imu[2].acc_y.ToString("F3") + " " + data.Imu[2].acc_z.ToString("F3") + " " + data.Imu[2].gyro_x.ToString("F3") + " " + data.Imu[2].gyro_y.ToString("F3") + " " + data.Imu[2].gyro_z.ToString("F3") + " " + data.Imu[2].mag_x.ToString("F3") + " " + data.Imu[2].mag_y.ToString("F3") + " " + data.Imu[2].mag_z.ToString("F3") + " " + v2.X.ToString("F3") + " " + v2.Y.ToString("F3") + " " + v2.Z.ToString("F3") + "\n" +
                    "1 " + (fakets + 0.03).ToString("F2") + " " + (frame + 3).ToString() + " " + data.Imu[3].acc_x.ToString("F3") + " " + data.Imu[3].acc_y.ToString("F3") + " " + data.Imu[3].acc_z.ToString("F3") + " " + data.Imu[3].gyro_x.ToString("F3") + " " + data.Imu[3].gyro_y.ToString("F3") + " " + data.Imu[3].gyro_z.ToString("F3") + " " + data.Imu[3].mag_x.ToString("F3") + " " + data.Imu[3].mag_y.ToString("F3") + " " + data.Imu[3].mag_z.ToString("F3") + " " + v3.X.ToString("F3") + " " + v3.Y.ToString("F3") + " " + v3.Z.ToString("F3") + "\n";
                    */
                    string dataline = "";
                    for(int i = 0; i < 4; i++)
                    {
                        dataline += "1 " + (fakets + 0.01 * i).ToString("F2") + " " + (frame + i).ToString() + " " +
                            data.Imu[i].acc_x.ToString("F3") + " " + data.Imu[i].acc_y.ToString("F3") + " " +
                            data.Imu[i].acc_z.ToString("F3") + " " + data.Imu[i].gyro_x.ToString("F3") + " " +
                            data.Imu[i].gyro_y.ToString("F3") + " " + data.Imu[i].gyro_z.ToString("F3") + " " +
                            data.Imu[i].mag_x.ToString("F3") + " " + data.Imu[i].mag_y.ToString("F3") + " " +
                            data.Imu[i].mag_z.ToString("F3") + " " + v[i].X.ToString("F3") + " " + 
                            v[i].Y.ToString("F3") + " " + v[i].Z.ToString("F3") + " " +
                            data.Quat[i].X.ToString("0.##") + " " + data.Quat[i].Y.ToString("0.##") + " " +
                            data.Quat[i].Z.ToString("0.##") + " " + data.Quat[i].W.ToString("0.##") + "\n";
                    }

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
                    GraphAccelerometer acc = accelerometer;

                    Vector3[] acc_data = new Vector3[4];
                    for(int i = 0; i < 4; i++)
                    {
                        acc_data[i] = new Vector3(data.Imu[i].acc_x, data.Imu[i].acc_y, data.Imu[i].acc_z);
                    }
                    acc.drawData(acc_data);

                    GraphGyroscope gyr = gyroscope;

                    Vector3[] gyr_data = new Vector3[4];
                    for (int i = 0; i < 4; i++)
                    {
                        gyr_data[i] = new Vector3(data.Imu[i].gyro_x, data.Imu[i].gyro_y, data.Imu[i].gyro_z);
                    }
                    gyr.drawData(gyr_data);

                    GraphMagnetometer mag = magnetometer;

                    Vector3[] mag_data = new Vector3[4];
                    for (int i = 0; i < 4; i++)
                    {
                        mag_data[i] = new Vector3(data.Imu[i].mag_x, data.Imu[i].mag_y, data.Imu[i].mag_z);
                    }
                    mag.drawData(mag_data);

                    linAcc.drawData(v);

                    Quaternion[] quat_data = new Quaternion[4];
                    for(int i= 0; i < 4; i++)
                    {
                        Quaternion q = new Quaternion((float)data.Quat[i].X, (float)data.Quat[i].Y, (float)data.Quat[i].Z, (float)data.Quat[i].W);
                        q = Quaternion.Normalize(q);
                        quat_data[i] = q;
                    }
                    quaternions.drawData(quat_data);
                });

                frame += 4;
                fakets += 0.04f;

            }
            else if(numIMUs == 2)
            {
                List<IMUInfo> imus = deviceList.IMUsUsed;
                int id_lower = imus[0].id;
                int id_upper = imus[1].id;
                if (deviceHandler == id_lower)
                {
                    for(int i = 0; i < 4; i++)
                    {
                        float W = (float)data.Quat[i].W;
                        float X = (float)data.Quat[i].X;
                        float Y = (float)data.Quat[i].Y;
                        float Z = (float)data.Quat[i].Z;
                        q_lower[i] = new Quaternion(X, Y, Z, W);
                    }
                    anglequat++;

                }
                else if (deviceHandler == id_upper)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        float W = (float)data.Quat[i].W;
                        float X = (float)data.Quat[i].X;
                        float Y = (float)data.Quat[i].Y;
                        float Z = (float)data.Quat[i].Z;
                        q_upper[i] = new Quaternion(X, Y, Z, W);
                    }
                    anglequat++;
                }
                if (anglequat % 2 == 0)
                {
                    float[] angleX = new float[4];
                    float[] angleY = new float[4];
                    float[] angleZ = new float[4];
                    Matrix4x4 refmat = Matrix4x4.CreateFromQuaternion(refq);
                    Vector3 angle_ref = new();
                    angle_ref = Helpers.ToEulerAngles(refq);
                    for (int i = 0; i < 4; i++)
                    {
                        Vector3 angle_low = new();
                        Vector3 angle_up = new();
                        angle_low = Helpers.ToEulerAngles(q_lower[i]);
                        angle_up = Helpers.ToEulerAngles(q_upper[i]);
                        a1 = angle_low.X - angle_up.X + angle_ref.X;
                        a2 = angle_low.Y - angle_up.Y + angle_ref.Y;
                        a3 = angle_low.Z - angle_up.Z + angle_ref.Z;
                        a1 = Helpers.ToDegrees(a1);
                        a2 = Helpers.ToDegrees(a2);
                        a3 = Helpers.ToDegrees(a3);
                        a1 = Helpers.NormalizeAngle(a1);
                        a2 = Helpers.NormalizeAngle(a2);
                        a3 = Helpers.NormalizeAngle(a3);
                        angleX[i] = a1;
                        angleY[i] = a2;
                        angleZ[i] = a3;

                        // trace.writeline(":::::: angle joint: " + a1.tostring() + " " + a2.tostring() + " " + a3.tostring());

                        Matrix4x4 m_lower = Matrix4x4.CreateFromQuaternion(q_lower[i]);
                        Matrix4x4 m_upper = Matrix4x4.CreateFromQuaternion(q_upper[i]);

                        Matrix4x4 r = Matrix4x4.Multiply(m_lower, m_upper);

                        double beta = Math.Atan2(r.M32, Math.Sqrt(Math.Pow(r.M12, 2) * Math.Pow(r.M22, 2)));
                        double delta = Math.Atan2(-(r.M12 / Math.Cos(beta)), r.M22 / Math.Cos(beta));
                        double phi = Math.Atan2(-(r.M31 / Math.Cos(beta)), r.M33 / Math.Cos(beta));

                        if (beta >= 90.0 && beta < 91.0)
                        {
                            beta = 90.0d;
                            delta = 0.0d;
                            phi = Math.Atan2(r.M13, r.M23);

                        }
                    }
                    Vector3[] angularVelocity = new Vector3[4];
                    angularVelocity[0].X = Helpers.AngularVelocityFromDegrees(angleX[0], prev_angle.X, dt);
                    angularVelocity[0].Y = Helpers.AngularVelocityFromDegrees(angleY[0], prev_angle.Y, dt);
                    angularVelocity[0].Z = Helpers.AngularVelocityFromDegrees(angleZ[0], prev_angle.Z, dt);
                    for (int i = 1; i < 4; i++)
                    {
                        angularVelocity[i].X = Helpers.AngularVelocityFromDegrees(angleX[i], angleX[i - 1], dt);
                        angularVelocity[i].Y = Helpers.AngularVelocityFromDegrees(angleY[i], angleY[i - 1], dt);
                        angularVelocity[i].Z = Helpers.AngularVelocityFromDegrees(angleZ[i], angleZ[i - 1], dt);
                    }
                    prev_angle = new Vector3(angleX[3], angleY[3], angleZ[3]);

                    Vector3[] angularAcceleration = new Vector3[4];
                    angularAcceleration[0] = Helpers.AngularAcceleration(angularVelocity[0], prev_angle_vel, dt);
                    for(int i = 1; i < 4; i++)
                    {
                        angularAcceleration[i] = Helpers.AngularAcceleration(angularVelocity[i], angularVelocity[i - 1], dt);
                    }
                    prev_angle_vel = angularVelocity[3];

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
                        AngleGraphX angleXGraph = this.angleX;

                        angleXGraph.drawData(angleX);

                        AngleGraphY angleYGraph = this.angleY;

                        angleYGraph.drawData(angleY);

                        AngleGraphZ angleZGraph = this.angleZ;

                        angleZGraph.drawData(angleZ);

                        GraphAngularVelocity graphAngularVelocity = this.angleVel;

                        graphAngularVelocity.drawData(angularVelocity);

                        GraphAngularAcceleration graphAngularAcceleration = this.angleAcc;

                        graphAngularAcceleration.drawData(angularAcceleration);
                    });
                    frame += 4;
                    fakets += 0.04f;
                }
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
        private List<Frame> graphs1IMU;
        private List<Frame> graphs2IMU;
        public ReplayManager(TimeLine.TimeLine timeLine, List<Frame> graphs1IMU, List<Frame> graphs2IMU)
        {
            active = false;
            this.timeLine = timeLine;
            this.graphs1IMU = graphs1IMU;
            this.graphs2IMU = graphs2IMU;
        }
        public void activate(GraphData graphData)
        {
            if (!active)
            {
                active = true;
                this.graphData = graphData;
                timeLine.model.timeEvent += onUpdateTimeLine;
                if (graphData.numIMUs == 1)
                {
                    Trace.WriteLine("1 IMUs");
                    foreach (Frame frame in graphs1IMU)
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
                }
                else if(graphData.numIMUs == 2)
                {
                    Trace.WriteLine("2 IMUs");
                    foreach (Frame frame in graphs2IMU)
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
                }
                timeLine.startReplay();
            }
        }
        public void deactivate()
        {
            if (active)
            {
                active = false;
                timeLine.endReplay();
                timeLine.model.timeEvent -= onUpdateTimeLine;
                if (graphData.numIMUs == 1)
                {
                    foreach (Frame frame in graphs1IMU)
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
                else if(graphData.numIMUs == 2)
                {
                    foreach (Frame frame in graphs2IMU)
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
                graphData = null;
            }
        }
        public void reset(GraphData graphData)
        {
            if (active)
            {
                this.graphData = graphData;
                if (graphData.numIMUs == 1)
                {
                    foreach (Frame frame in graphs1IMU)
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
                }
                else if (graphData.numIMUs == 2)
                {
                    foreach (Frame frame in graphs2IMU)
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
