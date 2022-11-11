using ibcdatacsharp.UI.Graphs;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI
{
    // Se encarga de manejar los grafos
    public class GraphManager
    {
        private const int CAPTURE_MS = 10;
        private const int RENDER_MS = 100;

        private System.Timers.Timer timerCapture;
        private System.Timers.Timer timerRender;

        private Device.Device device;
        private VirtualToolBar virtualToolBar;
        private TimeLine.TimeLine timeLine;
        // Añadir todos los grafos en esta lista
        private List<Frame> graphs;

        public GraphManager()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            virtualToolBar = mainWindow.virtualToolBar;
            device = mainWindow.device;
            if (mainWindow.timeLine.Content == null) {
                mainWindow.timeLine.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
                };
            }
            else
            {
                timeLine = mainWindow.timeLine.Content as TimeLine.TimeLine;
            }
            graphs = new List<Frame>();
            graphs.Add(mainWindow.accelerometer);
            graphs.Add(mainWindow.gyroscope);
            graphs.Add(mainWindow.magnetometer);
            graphs.Add(mainWindow.angle);
        }
        // Configura el timer capture
        public void initCapture()
        {
            // Se ejecuta al clicar pause
            void onPause(object sender, PauseState pauseState)
            {
                if (pauseState == PauseState.Pause)
                {
                    timerCapture.Stop();
                    timerRender.Stop();
                    // Pausa el timer de la linea de tiempo hay que llamarlo
                    timeLine.Pause();
                }
                else if (pauseState == PauseState.Play)
                {
                    timerCapture.Start();
                    timerRender.Start();
                    // Inicia el timer de la linea de tiempo hay que llamarlo
                    timeLine.Start();
                }
            }
            // Se ejecuta al clicar stop
            void onStop(object sender)
            {
                virtualToolBar.pauseEvent -= onPause;
                virtualToolBar.stopEvent -= onStop;
                timerCapture.Dispose();
                timerCapture = null;
                timerRender.Dispose();
                timerRender = null;
                timeLine.Stop();
            }
            if (timerCapture == null)
            {
                timerCapture = new System.Timers.Timer(CAPTURE_MS);
                timerCapture.AutoReset = true;
                timerRender = new System.Timers.Timer(RENDER_MS);
                timerRender.AutoReset = true;
                foreach(Frame frame in graphs)
                {
                    if(frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.clearData();
                            timerCapture.Elapsed += graph.onTick;
                            timerRender.Elapsed += graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        timerCapture.Elapsed += graph.onTick;
                        timerRender.Elapsed += graph.onRender;
                    }
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
            timeLine.startCapture();
        }
    }
}
