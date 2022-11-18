﻿using ibcdatacsharp.UI.Graphs;
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
        private const int CAPTURE_MS = 10;
        private const int RENDER_MS = 100;

        private System.Timers.Timer timerCapture;
        private System.Timers.Timer timerRender;

        private Device.Device device;
        private VirtualToolBar virtualToolBar;
        private TimeLine.TimeLine timeLine;
        // Añadir todos los grafos en esta lista
        private List<Frame> graphs;
        private GraphData graphData;

        public delegate void FrameEventHandler(object sender, int frame);
        public event FrameEventHandler frameEvent;
        public GraphManager()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            mainWindow.initialized += (sender, args) => finishInit();
        }
        // Para solucionar problemas de dependencias
        private void finishInit()
        {
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            virtualToolBar = mainWindow.virtualToolBar;
            device = mainWindow.device;
            if (mainWindow.timeLine.Content == null)
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
            graphs = new List<Frame>();
            graphs.Add(mainWindow.accelerometer);
            graphs.Add(mainWindow.gyroscope);
            graphs.Add(mainWindow.magnetometer);
            graphs.Add(mainWindow.angle);
        }
        public void initReplay(GraphData data)
        {
            if(timerCapture != null)
            {
                clearCapture();
            }
            graphData = data;
            timeLine.model.timeEvent -= onUpdateTimeLine;
            timeLine.model.timeEvent += onUpdateTimeLine;
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
                        frameEvent -= graph.onUpdateTimeLine;
                        frameEvent += graph.onUpdateTimeLine;
                    };
                }
                else
                {
                    GraphInterface graph = frame.Content as GraphInterface;
                    graph.clearData();
                    graph.drawData(graphData);
                    frameEvent -= graph.onUpdateTimeLine;
                    frameEvent += graph.onUpdateTimeLine;
                }
            }
        }
        // Deshace el replay
        private void clearReplay()
        {
            if (graphData != null)
            {
                graphData = null;
                timeLine.model.timeEvent -= onUpdateTimeLine;
                timeLine.Stop();
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
                if(currentDiference >= previousDiference)
                {
                    return previousFrame;
                }
                else if(currentTime < time)
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
                else if(currentTime > time) 
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
        // Configura el timer capture
        public void initCapture()
        {
            if (timerCapture == null)
            {
                clearReplay();
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
                            graph.initCapture();
                            timerCapture.Elapsed += graph.onTick;
                            timerRender.Elapsed += graph.onRender;
                        };
                    }
                    else
                    {
                        GraphInterface graph = frame.Content as GraphInterface;
                        graph.clearData();
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
            clearCapture();
        }
        // Deshace la captura
        private void clearCapture()
        {
            virtualToolBar.pauseEvent -= onPause;
            virtualToolBar.stopEvent -= onStop;
            timerCapture.Dispose();
            timerCapture = null;
            timerRender.Dispose();
            timerRender = null;
            timeLine.Stop();
        }
    }
}
