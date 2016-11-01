using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace QuranAuthor.Helps
{
    public static class WindowCapturer
    {
        public static Bitmap Capture()
        {
            Rectangle bounds;

            var foregroundWindowsHandle = Win32.GetForegroundWindow();
            var rect = new Rect();
            Win32.GetWindowRect(foregroundWindowsHandle, ref rect);
            bounds = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);

            var result = new Bitmap(bounds.Width, bounds.Height);

            using (var g = Graphics.FromImage(result))
            {
                g.CopyFromScreen(new Point(bounds.Left, bounds.Top), Point.Empty, bounds.Size);
            }

            return result;
        }
    }
}
