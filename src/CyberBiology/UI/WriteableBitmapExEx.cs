using System.Runtime.CompilerServices;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CyberBiology.UI
{
    public static class WriteableBitmapExEx
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FillRectangleWH(this WriteableBitmap bmp, int x1, int y1, int width, int height, Color color)
        {
            bmp.FillRectangle(x1, y1, x1 + width, y1 + height, color);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void DrawRectangleWH(this WriteableBitmap bmp, int x1, int y1, int width, int height, Color color)
        {
            bmp.DrawRectangle(x1, y1, x1 + width, y1 + height, color);
        }
    }
}