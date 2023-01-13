using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ibcdatacsharp.UI.ToolBar
{
    public class VirtualToolBarProperties: INotifyPropertyChanged
    {
        private VirtualToolBar virtualToolBar;
        private CamaraViewport.CamaraViewport camaraViewport;

        public event PropertyChangedEventHandler PropertyChanged;
        public bool scanEnabled
        {
            get
            {
                return !virtualToolBar.capturing 
                    && virtualToolBar.recordState == RecordState.RecordStopped;
            }
            set
            {

            }
        }
        public bool connectEnabled
        {
            get
            {
                return virtualToolBar.buttonsEnabled && !virtualToolBar.capturing
                    && virtualToolBar.recordState == RecordState.RecordStopped;
            }
            set
            {

            }
        }
        public bool disconnectEnabled
        {
            get
            {
                return virtualToolBar.buttonsEnabled && !virtualToolBar.capturing
                    && virtualToolBar.recordState == RecordState.RecordStopped;
            }
            set
            {

            }
        }
        public bool captureEnabled
        {
            get
            {
                return virtualToolBar.buttonsEnabled && virtualToolBar.recordState == RecordState.RecordStopped;
            }
            set
            {

            }
        }
        public bool recordEnabled
        {
            get
            {
                return virtualToolBar.buttonsEnabled && virtualToolBar.capturing;
            }
            set
            {

            }
        }
        public bool videoCheckboxEnabled
        {
            get
            {
                return virtualToolBar.recordState == RecordState.RecordStopped &&
                    camaraViewport.someCameraOpened();
            }
            set
            {

            }
        }
        public VirtualToolBarProperties(VirtualToolBar virtualToolBar)
        {
            void initCameraViewport()
            {
                camaraViewport.cameraChanged += (s, e) =>
                {
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(videoCheckboxEnabled)));
                };
            }
            this.virtualToolBar = virtualToolBar;
            virtualToolBar.buttonsEnabledChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(captureEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(recordEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(connectEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(disconnectEnabled)));
            };
            virtualToolBar.recordChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(captureEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(scanEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(connectEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(disconnectEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(videoCheckboxEnabled)));
            };
            virtualToolBar.captureChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(recordEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(scanEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(connectEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(disconnectEnabled)));
            };
            if(((MainWindow)Application.Current.MainWindow).camaraViewport.Content != null)
            {
                camaraViewport = ((MainWindow)Application.Current.MainWindow).camaraViewport.Content as CamaraViewport.CamaraViewport;
                initCameraViewport();
            }
            else
            {
                ((MainWindow)Application.Current.MainWindow).camaraViewport.Navigated += (s, e) =>
                {
                    camaraViewport = ((MainWindow)Application.Current.MainWindow).camaraViewport.Content as CamaraViewport.CamaraViewport;
                    initCameraViewport();
                };
            }
        }
    }
}
