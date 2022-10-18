using ibcdatacsharp.UI.FileSaver;
using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Windows;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI.ToolBar
{
    // Mantiene el estado para la ToolBar y la MenuBar
    public class VirtualToolBar
    {
        public PauseState pauseState { get; set; } 
        public RecordState recordState { get; set; }

        private ToolBar toolBar;
        private MenuBar.MenuBar menuBar;

        private SavingMenu menu;

        public delegate void PauseEventHandler(object sender, PauseState args);
        public delegate void RecordEventHandler(object sender, RecordState args);
        public delegate void StopEventHandler(object sender);
        public event PauseEventHandler pauseEvent;
        public event RecordEventHandler recordEvent;
        public event StopEventHandler stopEvent;
        public event EventHandler<SaveArgs> saveEvent;

        public VirtualToolBar()
        {
            pauseState = PauseState.Play;
            recordState = RecordState.RecordStopped;
            MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
            if (mainWindow.toolBar.Content == null)
            {
                mainWindow.toolBar.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    toolBar = mainWindow.toolBar.Content as ToolBar;
                };
            }
            else
            {
                toolBar = mainWindow.toolBar.Content as ToolBar;
            }
            if(mainWindow.menuBar.Content == null)
            {
                mainWindow.menuBar.Navigated += delegate (object sender, NavigationEventArgs e)
                {
                    menuBar = mainWindow.menuBar.Content as MenuBar.MenuBar;
                };
            }
            else
            {
                menuBar = mainWindow.menuBar.Content as MenuBar.MenuBar;
            }
        }
        // Pasa referencia
        public void setToolBar(ToolBar toolBar)
        {
            this.toolBar = toolBar;
        }
        // Pasa referencia
        public void setMenuBar(MenuBar.MenuBar menuBar)
        {
            this.menuBar = menuBar;
        }
        // Se ejecuta al clicar el boton pause
        public void pauseClick()
        {
            if(pauseState == PauseState.Play)
            {
                pauseState = PauseState.Pause;
                toolBar.changePauseState(PauseState.Play);
                menuBar.changePauseState(PauseState.Play);
                if(pauseEvent != null)
                {
                    pauseEvent?.Invoke(this, PauseState.Pause);
                }
            }
            else if(pauseState == PauseState.Pause)
            {
                pauseState = PauseState.Play;
                toolBar.changePauseState(PauseState.Pause);
                menuBar.changePauseState(PauseState.Pause);
                if (pauseEvent != null)
                {
                    pauseEvent?.Invoke(this, PauseState.Play);
                }
            }
        }
        // Se ejecuta al clicar record
        public void recordClick()
        {
            if(recordState == RecordState.RecordStopped)
            {
                menu = new SavingMenu();
                MainWindow mainWindow = (MainWindow)Application.Current.MainWindow;
                menu.Owner = mainWindow;
                menu.ok.Click += continueRecord;
                menu.Show();
            }
        }
        private void continueRecord(object sender, RoutedEventArgs e)
        {
            if(saveEvent != null)
            {
                saveEvent?.Invoke(this, new SaveArgs { directory = menu.route.Text, csv = (bool)menu.csv.IsChecked, video = (bool)menu.video.IsChecked });
            }
            menu.Close();
            ((MainWindow)Application.Current.MainWindow).Focus();
            recordState = RecordState.Recording;
            toolBar.changeRecordState(RecordState.Recording);
            menuBar.changeRecordState(RecordState.Recording);
            if (recordEvent != null)
            {
                recordEvent?.Invoke(this, RecordState.Recording);
            }
        }
        // Se ejecuta al clicar stop
        public void stopClick()
        {
            if(pauseState == PauseState.Pause)
            {
                pauseState = PauseState.Play;
                toolBar.changePauseState(PauseState.Pause);
                menuBar.changePauseState(PauseState.Pause);
            }
            if(recordState == RecordState.Recording)
            {
                recordState = RecordState.RecordStopped;
                toolBar.changeRecordState(RecordState.RecordStopped);
                menuBar.changeRecordState(RecordState.RecordStopped);
                if(recordEvent != null)
                {
                    recordEvent?.Invoke(this, RecordState.RecordStopped);
                }
            }
            if(stopEvent != null)
            {
                stopEvent?.Invoke(this);
            }
        }
    }
}
