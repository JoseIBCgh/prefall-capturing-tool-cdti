using AvalonDock.Layout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.MenuBar.View
{
    public class WindowInfoTitle: WindowInfo
    {
        private string _title;
        public new string title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged("title");
            }
        }
        public WindowInfoTitle(LayoutAnchorable layout, string title): base(layout)
        {
            this.title = title;
        }
    }
}
