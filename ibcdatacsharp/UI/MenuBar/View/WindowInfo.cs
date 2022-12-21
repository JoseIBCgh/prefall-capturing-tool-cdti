using AvalonDock.Layout;
using ibcdatacsharp.Common;
using System.Windows.Input;

namespace ibcdatacsharp.UI.MenuBar.View
{
    public class WindowInfo : PropertyNotifier
    {
        protected LayoutAnchorable _layout;
        public LayoutAnchorable layout
        {
            get
            {
                return _layout;
            }
            set
            {
                _layout = value;
                OnPropertyChanged("layout");
            }
        }
        public bool hidden
        {
            get { return !layout.IsVisible; }
        }
        public string title
        {
            get { return layout.Title; }
            set { 
                layout.Title = value;
                OnPropertyChanged("title");
            }
        }
        public void hide()
        {
            layout.Hide();
        }
        public void show()
        {
            layout.Show();
        }

        public WindowInfo() { }
        public WindowInfo(LayoutAnchorable layout)
        {
            this.layout = layout;
            layout.IsVisibleChanged += (s, e) => {
                OnPropertyChanged("hidden"); 
            };
        }
    }
}
