using ibcdatacsharp.UI.ToolBar.Enums;

namespace ibcdatacsharp.UI.ToolBar
{
    // Mantiene el estado para la ToolBar y la MenuBar
    internal class VirtualToolBar
    {
        public PauseState pauseState { get; set; } 
        public RecordState recordState { get; set; }

        private ToolBar toolBar;
        private MenuBar.MenuBar menuBar;

        public delegate void PauseEventHandler(object sender, PauseState args);
        public delegate void RecordEventHandler(object sender, RecordState args);
        public delegate void StopEventHandler(object sender);
        public event PauseEventHandler pauseEvent;
        public event RecordEventHandler recordEvent;
        public event StopEventHandler stopEvent;

        public VirtualToolBar()
        {
            pauseState = PauseState.Play;
            recordState = RecordState.RecordStopped;
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
            if (recordState == RecordState.RecordStopped)
            {
                recordState = RecordState.Recording;
                toolBar.changeRecordState(RecordState.Recording);
                menuBar.changeRecordState(RecordState.Recording);
                if(recordEvent != null)
                {
                    recordEvent?.Invoke(this, RecordState.Recording);
                }
            }
        }
        // Se ejecuta al clicar stop
        public void stopClick()
        {
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
