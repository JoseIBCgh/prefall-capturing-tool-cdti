using ibcdatacsharp.Common;
using System.Collections.ObjectModel;

namespace ibcdatacsharp.UI.MenuBar.View
{
    public class ViewInfo : BaseObject
    {
        public ObservableCollection<WindowInfo> windows
        {
            get { return GetValue<ObservableCollection<WindowInfo>>("windows"); }
            set { SetValue("windows", value); }
        }
        public void Add(WindowInfo windowInfo)
        {
            windows.Add(windowInfo);
            OnPropertyChanged("windows");
        }
        public ViewInfo()
        {
            windows = new ObservableCollection<WindowInfo>();
        }
    }
}
