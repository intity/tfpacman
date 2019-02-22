using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TFlex.PackageManager.Common
{
    public class CustomImage : Image
    {
        protected override void OnRender(DrawingContext dc)
        {
            if (!(Source is BitmapSource bitmapSource))
            {
                return;
            }

            if (!IsEnabled)
            {
                Opacity = 0.5;
                OpacityMask = new ImageBrush(bitmapSource);
                bitmapSource = new FormatConvertedBitmap(bitmapSource, PixelFormats.Gray8, null, 0);
            }
            else
                Opacity = 1;

            dc.DrawImage(bitmapSource, new Rect(new Point(), RenderSize));
        }
    }
}