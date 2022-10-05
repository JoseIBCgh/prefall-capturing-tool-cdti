using ibcdatacsharp.UI.AngleGraph;
using ibcdatacsharp.UI.GraphWindow;
using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Device
{
    // Clase que simula un IMU para hacer pruebas
    internal class Device
    {
        private DispatcherTimer timer;
        private bool paused;
        public delegate void RawDataEventHandler(object sender, RawArgs args);
        public delegate void AngleDataEventHandler(object sender, AngleArgs args);
        public delegate void ClearDataEventHandler(object sender);
        public event RawDataEventHandler rawData;
        public event AngleDataEventHandler angleData;
        public event ClearDataEventHandler clearData;
        public Device()
        {
            paused = false;
        }
        // Empieza a emitir datos
        public void play()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(emitData);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                if (!paused)
                {
                    timer.Start();
                }
            }
        }
        public void onPause(object sender, PauseState pauseState)
        {
            if(pauseState == PauseState.Pause)
            {
                paused = true;
                if(timer != null)
                {
                    timer.Stop();
                }
            }
            else if(pauseState == PauseState.Play)
            {
                paused = false;
                if(timer != null)
                {
                    timer.Start();
                }
            }
        }
        public void onStop(object sender)
        {
            if(timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                clearData?.Invoke(this); //Borra los datos de los graficos. Se tiene que llamar.
                timer = null;
            }
        }
        // Emite datos inventados
        public void emitData(object sender, EventArgs e)
        {
            if(rawData != null)
            {
                rawData?.Invoke(this, generateRandomRawData());
            }
            if(angleData != null)
            {
                angleData?.Invoke(this, generateRandomAngleData());
            }
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
