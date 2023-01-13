using ibcdatacsharp.UI.ToolBar.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.ToolBar
{
    public class VirtualToolBarProperties: INotifyPropertyChanged
    {
        private VirtualToolBar virtualToolBar;

        public event PropertyChangedEventHandler PropertyChanged;
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
        public VirtualToolBarProperties(VirtualToolBar virtualToolBar)
        {
            this.virtualToolBar = virtualToolBar;
            virtualToolBar.buttonsEnabledChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(captureEnabled)));
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(recordEnabled)));
            };
            virtualToolBar.recordChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(captureEnabled)));
            };
            virtualToolBar.captureChanged += (s, e) =>
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(recordEnabled)));
            };
        }
    }
}
