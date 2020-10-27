using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace IconExtract
{
    public class ExtractIcon
    {
        public static Icon AddIconOverlay(Icon originalIcon, Icon overlay)
        {
            Image a = IconToAlphaBitmap(originalIcon);
            Image b = IconToAlphaBitmap(overlay);
            Bitmap bitmap = new Bitmap(16, 16);
            Graphics canvas = Graphics.FromImage(bitmap);
            canvas.DrawImage(a, new Point(0, 0));
            canvas.DrawImage(b, new Point(0, 0));
            canvas.Save();
            return Icon.FromHandle(bitmap.GetHicon());
        }
        public static Icon Extract(string file, int number, bool largeIcon)
        {
            IntPtr large;
            IntPtr small;
            ExtractIconEx(file, number, out large, out small, 1);
            try
            {
                return Icon.FromHandle(largeIcon ? large : small);
            }
            catch
            {
                return null;
            }

        }
        [DllImport("Shell32.dll", EntryPoint = "ExtractIconExW", CharSet = CharSet.Unicode, ExactSpelling = true, CallingConvention = CallingConvention.StdCall)]
        private static extern int ExtractIconEx(string sFile, int iIndex, out IntPtr piLargeVersion, out IntPtr piSmallVersion, int amountIcons);

        [DllImport("gdi32.dll", SetLastError = true)]
        static extern bool DeleteObject(IntPtr hObject);

        [DllImport("user32.dll")]
        static extern bool GetIconInfo(IntPtr hIcon, out ICONINFO piconinfo);

        public struct ICONINFO
        {
            public bool fIcon;
            public int xHotspot;
            public int yHotspot;
            public IntPtr hbmMask;
            public IntPtr hbmColor;
        }


        public static Bitmap IconToAlphaBitmap(Icon ico)
        {
            ICONINFO ii = new ICONINFO();
            GetIconInfo(ico.Handle, out ii);
            Bitmap bmp = Bitmap.FromHbitmap(ii.hbmColor);
            DeleteObject(ii.hbmColor);
            DeleteObject(ii.hbmMask);

            if (Bitmap.GetPixelFormatSize(bmp.PixelFormat) < 32)
                return ico.ToBitmap();

            BitmapData bmData;
            Rectangle bmBounds = new Rectangle(0, 0, bmp.Width, bmp.Height);

            bmData = bmp.LockBits(bmBounds, ImageLockMode.ReadOnly, bmp.PixelFormat);

            Bitmap dstBitmap = new Bitmap(bmData.Width, bmData.Height, bmData.Stride, PixelFormat.Format32bppArgb, bmData.Scan0);

            bool IsAlphaBitmap = false;

            for (int y = 0; y <= bmData.Height - 1; y++)
            {
                for (int x = 0; x <= bmData.Width - 1; x++)
                {
                    Color PixelColor = Color.FromArgb(Marshal.ReadInt32(bmData.Scan0, (bmData.Stride * y) + (4 * x)));
                    if (PixelColor.A > 0 & PixelColor.A < 255)
                    {
                        IsAlphaBitmap = true;
                        break;
                    }
                }
                if (IsAlphaBitmap) break;
            }

            bmp.UnlockBits(bmData);

            if (IsAlphaBitmap == true)
                return new Bitmap(dstBitmap);
            else
                return new Bitmap(ico.ToBitmap());

        }
    }
    }

