using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using Windows.Graphics.Imaging;
using System.IO;
using BitmapEncoder = Windows.Graphics.Imaging.BitmapEncoder;
using System;
using System.Drawing;
using System.Windows.Interop;

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
        public static Bitmap UWP2WPF(ImageStream imageStream)
        {
            /*
            using (var stream = new Windows.Storage.Streams.InMemoryRandomAccessStream())
            {
                BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, stream);
                SoftwareBitmap softwareBitmap = new SoftwareBitmap()
                encoder.SetSoftwareBitmap(imageStream);
                await encoder.FlushAsync();
                return new Bitmap(stream.AsStream());
            }
            */
            using(Stream stream = imageStream.AsStream())
            {
                Bitmap bitmap = new Bitmap(stream);
                return bitmap;
            }
        }

        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        public static extern bool DeleteObject(IntPtr hObject);

        public static BitmapSource Bitmap2BitmapImage(Bitmap bitmap)
        {
            IntPtr hBitmap = bitmap.GetHbitmap();
            BitmapSource retval;

            try
            {
                retval = Imaging.CreateBitmapSourceFromHBitmap(
                             hBitmap,
                             IntPtr.Zero,
                             Int32Rect.Empty,
                             BitmapSizeOptions.FromEmptyOptions());
            }
            finally
            {
                DeleteObject(hBitmap);
            }

            return retval;
        }
    }
}
