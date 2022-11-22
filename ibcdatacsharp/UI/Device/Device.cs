using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Diagnostics;
using System.Timers;
using System.Windows;

namespace ibcdatacsharp.UI.Device
{
    // Clase que simula un IMU para hacer pruebas
    public class Device
    {
        private const int DEVICE_MS = 10;
        private System.Timers.Timer timer;
        public RawArgs rawData { get; set; }
        public AngleArgs angleData { get; set; }
        public Device()
        {
            rawData = generateRandomRawData();
            angleData = generateRandomAngleData();
        }
        // Inicializa el timer
        public void initTimer()
        {
            void onStop(object sender)
            {
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent -= onPause;
                timer.Dispose();
                timer = null;
                mainWindow.virtualToolBar.stopEvent -= onStop;
            }
            void onPause(object sender, PauseState pauseState)
            {
                if(pauseState == PauseState.Pause)
                {
                    timer.Stop();
                }
                else if(pauseState == PauseState.Play)
                {
                    timer.Start();
                }
            }
            if (timer == null)
            {
                timer = new System.Timers.Timer();
                timer.AutoReset = true;
                timer.Interval = DEVICE_MS;
                timer.Elapsed += generateData;
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                mainWindow.virtualToolBar.pauseEvent += onPause;
                mainWindow.virtualToolBar.stopEvent += onStop;
                if (mainWindow.virtualToolBar.pauseState == PauseState.Play)
                {
                    timer.Start();
                }
            }
        }
        // Genera datos inventados
        public void generateData(object sender, ElapsedEventArgs args)
        {
            rawData = generateRandomRawData();
            angleData = generateRandomAngleData();
        }
        // Genera datos inventados para el Graph Window
        private RawArgs generateRandomRawData()
        {
            RawArgs rawArgs = new RawArgs();
            Random random = new Random();
            double x = 50.0 + (random.NextDouble() - 0.5) * 20;
            double y = 0.0 + (random.NextDouble() - 0.5) * 20;
            double z = -50.0 + (random.NextDouble() - 0.5) * 20;
            rawArgs.accelerometer = new double[] {x,y,z };

            x = 300.0 + (random.NextDouble() - 0.5) * 100;
            y = 0.0 + (random.NextDouble() - 0.5) * 100;
            z = -300.0 + (random.NextDouble() - 0.5) * 100;
            rawArgs.gyroscope = new double[] { x, y, z };

            x = 2.5 + (random.NextDouble() - 0.5);
            y = 0.0 + (random.NextDouble() - 0.5);
            z = -2.5 + (random.NextDouble() - 0.5);
            rawArgs.magnetometer = new double[] { x, y, z };

            return rawArgs;
        }
        // Genera datos inventados para el Angle Graph
        private AngleArgs generateRandomAngleData()
        {
            AngleArgs angleArgs = new AngleArgs();
            Random random = new Random();
            double x = (random.NextDouble() - 0.5) * 400;
            double y = (random.NextDouble() - 0.5) * 400;
            double z = (random.NextDouble() - 0.5) * 400;

            angleArgs.angle = new double[] { x, y, z };
            return angleArgs;
        }
    }
}
