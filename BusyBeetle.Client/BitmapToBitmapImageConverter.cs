using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using BusyBeetle.Core;

namespace BusyBeetle.Client
{
    public class BitmapToBitmapImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                using (MemoryStream memory = new MemoryStream())
                {
                    Bitmap world = value as Bitmap;
                    if (world == null)
                        return null;
                    lock (World.LockObject)
                    {
                        world.Save(memory, ImageFormat.Bmp);
                        memory.Position = 0;
                        BitmapImage bmpImage = new BitmapImage();
                        bmpImage.BeginInit();
                        bmpImage.StreamSource = memory;
                        bmpImage.CacheOption = BitmapCacheOption.OnLoad;
                        bmpImage.EndInit();
                        return bmpImage;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception("Failed to convert bitmap to bitmapImage", e);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}