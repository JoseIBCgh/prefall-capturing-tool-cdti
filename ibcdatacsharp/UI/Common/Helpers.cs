using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;

namespace ibcdatacsharp.UI.Common
{
    public static class Helpers
    {
        // Devuelve el primer descendiente de tipo T de un objeto en el arbol xaml
        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
        public static BitmapImage imageStream2BitmapImage(ImageStream imageStream)
        {
            return new BitmapImage();
        }
    }
}
