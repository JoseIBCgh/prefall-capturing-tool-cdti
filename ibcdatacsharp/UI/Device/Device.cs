using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows.Threading;

namespace ibcdatacsharp.UI.Device
{
    // Clase que simula un IMU para hacer pruebas
    internal class Device
    {
        private DispatcherTimer timer;
        private bool recording;
        public delegate void RawDataEventHandler(object sender, RawArgs args);
        public delegate void AngleDataEventHandler(object sender, AngleArgs args);
        public event RawDataEventHandler rawData;
        public event AngleDataEventHandler angleData;
        // Empieza a emitir datos
        public void play()
        {
            if (timer == null)
            {
                timer = new DispatcherTimer();
                timer.Tick += new EventHandler(emitData);
                timer.Interval = new TimeSpan(0, 0, 0, 0, 10);
                timer.Start();
            }
        }
        // Pausa la emision de datos
        public void pause(ToolBar.ToolBar toolBar, MenuBar.MenuBar menuBar)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                    toolBar.changePauseState(PauseState.Play); //Cambia la ToolBar a play. Se tiene que llamar.
                    menuBar.changePauseState(PauseState.Play); //Cambia el Menu a play. Se tiene que llamar.
                }
                else
                {
                    timer.Start();
                    toolBar.changePauseState(PauseState.Pause); //Cambia la ToolBar a pause. Se tiene que llamar.
                    menuBar.changePauseState(PauseState.Pause); //Cambia el Menu a pause. Se tiene que llamar.
                }
            }
        }
        // Detiene la emision de datos y borra los graficos.
        public void stop(ToolBar.ToolBar toolBar, MenuBar.MenuBar menuBar, GraphWindow.GraphWindow graphWindow, AngleGraph.AngleGraph angleGraph)
        {
            if (timer != null)
            {
                if (timer.IsEnabled)
                {
                    timer.Stop();
                }
                toolBar.changePauseState(PauseState.Pause); // Cambia la ToolBar a pause. Se tiene que llamar.
                menuBar.changePauseState(PauseState.Pause); // Cambia el Menu a pause. Se tiene que llamar.
                graphWindow.clearModels(); // Borra los datos del Graph Window. Se tiene que llamar.
                angleGraph.clearModels(); // Borra los datos del Angle Graph. Se tiene que llamar.
                timer = null;
            }
        }
        // Empieza o para el modo Record
        public void record(ToolBar.ToolBar toolBar, MenuBar.MenuBar menuBar)
        {
            if (recording)
            {
                recording = false;
                toolBar.changeRecordState(RecordState.RecordStopped); //Cambia la ToolBar a record stopped. Se tiene que llamar.
                menuBar.changeRecordState(RecordState.RecordStopped); //Cambia el Menu a record stopped. Se tiene que llamar.
            }
            else
            {
                recording = true;
                toolBar.changeRecordState(RecordState.Recording); //Cambia la ToolBar a recording. Se tiene que llamar.
                menuBar.changeRecordState(RecordState.Recording); //Cambia el Menu a recording. Se tiene que llamar.
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
