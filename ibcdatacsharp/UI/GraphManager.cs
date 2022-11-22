using ibcdatacsharp.UI.Graphs;
using ibcdatacsharp.UI.ToolBar;
using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI
{
    // Se encarga de manejar los grafos
    public class GraphManager
    {
        private CaptureManager captureManager;
        private ReplayManager replayManager;
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
            List<Frame> graphs = new List<Frame>();
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
        public CaptureManager(List<Frame> graphs, VirtualToolBar virtualToolBar, Device.Device device)
        {
            active = false;
            this.graphs = graphs;
            this.virtualToolBar = virtualToolBar;
            this.device = device;
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
                foreach (Frame frame in graphs)
                {
                    if (frame.Content == null)
                    {
                        frame.Navigated += delegate (object sender, NavigationEventArgs e)
                        {
                            // Todos los grafos deberian implementar esta interface
                            GraphInterface graph = frame.Content as GraphInterface;
                            graph.initCapture();
                            timerCapture.Elapsed += graph.onTick;
                            timerRender.Elapsed += graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.initCapture();
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
                            timerCapture.Elapsed -= graph.onTick;
                            timerRender.Elapsed -= graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
                        graph.initCapture();
                        timerCapture.Elapsed -= graph.onTick;
                        timerRender.Elapsed -= graph.onRender;
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
