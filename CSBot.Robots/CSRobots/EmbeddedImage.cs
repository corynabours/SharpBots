using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Media.Imaging;

namespace CSRobots
{
    public class EmbeddedImage
    {
        public static BitmapSource GetEmbeddedImageResource(string fileName)
        {
            var stream = GetDataStreamFromEmbeddResource(fileName);
            var bitmap = new Bitmap(stream);
            bitmap.MakeTransparent();
            var bmSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
            return bmSource;
        }

        public static BitmapSource ModifyImageResource(string filename, int c1, int c2, int c3)
        {
            var stream = GetDataStreamFromEmbeddResource(filename);

            var buffer = new byte[10000];
            var read = stream.Read(buffer, 0, 10000);
            var ncolors = Math.Pow(2, 1 + Convert.ToInt32(buffer[10] & 7));
            for (var j = 0; j < ncolors; j++)
            {
                var off = buffer[13 + j * 3];
                var on = buffer[14 + j * 3];
                buffer[13 + j * 3] = (c1 == 1) ? on : off;
                buffer[14 + j * 3] = (c2 == 1) ? on : off;
                buffer[15 + j * 3] = (c3 == 1) ? on : off;
            }
            var ms = new MemoryStream();
            ms.Write(buffer, 0, read);
            
            var bitmap = new Bitmap(ms);
            bitmap.MakeTransparent();
            var bmSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(bitmap.GetHbitmap(),IntPtr.Zero,Int32Rect.Empty,BitmapSizeOptions.FromEmptyOptions());
            return bmSource;
        }

        private static Stream GetDataStreamFromEmbeddResource(string fileName)
        {
            var assembly = Assembly.GetExecutingAssembly();
            if (assembly == null) throw new Exception("Cannot get existing assembly.");
            var stream = assembly.GetManifestResourceStream(fileName);
            if (stream == null) throw new Exception("Cannot find requested resource: " + fileName);
            return stream;
        }

    }
}
