using QuranAuthor.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.IO;
using System.Drawing.Imaging;
using System.Configuration;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace QuranAuthor.Helps
{
    public static class BitmapHelper
    {
        private static string pagesPath = ConfigurationManager.AppSettings["PagesPath"];
        private static Color yellowColor = Color.FromArgb(255, 246, 129);
        private static Brush explainBorderBrush = new SolidBrush(Color.FromArgb(255, 112, 173, 71));
        private static Brush explainBrush = new SolidBrush(Color.FromArgb(255, 0, 112, 192));

        public static SnippetSelection GetSnippetSelection(Bitmap bitmap)
        {
            var selection = new SnippetSelection();
            for (int y = 0; y < bitmap.Size.Height; y++)
            {
                bool firstX = true;
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.Equals(yellowColor))
                    {
                        if (selection.Start.Y == -1)
                        {
                            selection.Start.Y = y;
                        }
                        if (selection.Start.Y == y && x > selection.Start.X)
                        {
                            selection.Start.X = x;
                        }
                        if(firstX)
                        {
                            firstX = false;
                            selection.End.X = x;
                        }
                        selection.End.Y = y;
                    }
                }
            }
            return selection;
        }

        public static Snippet CalculatePageSelection(Snippet snippet, SnippetSelection selection)
        {
            snippet.StartLine = selection.Start.Y < 80 ? 1 : (int)Math.Round(((decimal)selection.Start.Y - 66) / 40, 0) + 1;
            snippet.EndLine = selection.End.Y < 120 ? 1 : (int)Math.Round(((decimal)selection.End.Y - 66) / 40, 0);

            snippet.StartPoint = 49 + ((selection.Start.X - 450) * 950 / 338) + 5;
            snippet.EndPoint = 49 + ((selection.End.X - 450) * 950 / 338);

            return snippet;
        }

        public static Bitmap FocusSelection(Bitmap bitmap, Snippet snippet)
        {
            var startY = 100 + ((snippet.StartLine - 1) * 104);
            var endY = 100 + ((snippet.EndLine) * 104);
            for (int y = 80; y < 1675; y++)
            {
                for (int x = 49; x <= 1008; x++)
                {
                    if(y < startY || y > endY)
                    {
                        Blur(bitmap, y, x);
                    }
                }
            }

            for (int y = startY; y <= startY + 104; y++)
            {
                for (int x = snippet.StartPoint; x <= 1008; x++)
                {
                    Blur(bitmap, y, x);
                }
            }

            for (int y = endY; y >= endY - 104; y--)
            {
                for (int x = 49; x <= snippet.EndPoint; x++)
                {
                    Blur(bitmap, y, x);
                }
            }

            return bitmap;
        }

        public static BitmapImage BitmapToImageSource(Bitmap page)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                page.Save(memory, ImageFormat.Png);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }

        public static Bitmap LoadPage(int pageNumber)
        {
            var bmp = new Bitmap(Path.Combine(pagesPath, pageNumber + ".png"));
            var rect = new System.Drawing.Rectangle(0, 0, bmp.Width, bmp.Height);

            var result = bmp.Clone(rect, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
            bmp.Dispose();

            return result;
        }

        public static Bitmap DrawExplanation(Bitmap bitmap, IList<Explanation> explanations)
        {
            var g = Graphics.FromImage(bitmap);

            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.PixelOffsetMode = PixelOffsetMode.HighQuality;
            g.TextRenderingHint = TextRenderingHint.AntiAlias;
            
            foreach (var explanation in explanations)
            {
                g.FillRoundedRectangle(explainBorderBrush, new Rectangle(49, explanation.Top, 960, 150), 20);
                g.FillRoundedRectangle(Brushes.White, new Rectangle(51, explanation.Top + 2, 956, 146), 20);
                g.DrawString(explanation.Text, new Font("GE SS Text Light", 36), explainBrush, new RectangleF(71, explanation.Top + 12, 916, 126), new StringFormat(StringFormatFlags.DirectionRightToLeft));
            }
            
            g.Flush();

            return bitmap;
        }

        private static void Blur(Bitmap bitmap, int y, int x)
        {
            var color = bitmap.GetPixel(x, y);
            if(color.R == 255 & color.G == 255 && color.B == 255)
            {
                return;
            }
            
            bitmap.SetPixel(x, y, Color.FromArgb(50, color));
        }

        private static List<SnippetPoint> GetSnippetPoints(Snippet snippet)
        {
            var points = new List<SnippetPoint>();

            for (int line = snippet.StartLine; line <= snippet.EndLine; line++)
            {
                var startY = 100 + ((line - 1) * 104);
                for (int y = startY; y <= startY + 104; y++)
                {
                    if(line == snippet.StartLine && line == snippet.EndLine)
                    {
                        for (int x = snippet.EndPoint; x <= snippet.StartPoint; x++)
                        {
                            points.Add(new SnippetPoint(x, y));
                        }
                    }
                    else if(line == snippet.StartLine)
                    {
                        for (int x = 49; x <= snippet.StartPoint; x++)
                        {
                            points.Add(new SnippetPoint(x, y));
                        }
                    }
                    else if (line == snippet.EndLine)
                    {
                        for (int x = snippet.EndPoint; x <= 1008; x++)
                        {
                            points.Add(new SnippetPoint(x, y));
                        }
                    }
                    else
                    {
                        for (int x = 49; x <= 1008; x++)
                        {
                            points.Add(new SnippetPoint(x, y));
                        }
                    }
                }
            }

            return points;
        }

        public static GraphicsPath RoundedRect(Rectangle bounds, int radius)
        {
            int diameter = radius * 2;
            Size size = new Size(diameter, diameter);
            Rectangle arc = new Rectangle(bounds.Location, size);
            GraphicsPath path = new GraphicsPath();

            if (radius == 0)
            {
                path.AddRectangle(bounds);
                return path;
            }

            // top left arc  
            path.AddArc(arc, 180, 90);

            // top right arc  
            arc.X = bounds.Right - diameter;
            path.AddArc(arc, 270, 90);

            // bottom right arc  
            arc.Y = bounds.Bottom - diameter;
            path.AddArc(arc, 0, 90);

            // bottom left arc 
            arc.X = bounds.Left;
            path.AddArc(arc, 90, 90);

            path.CloseFigure();
            return path;
        }

        public static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (pen == null)
                throw new ArgumentNullException("pen");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.DrawPath(pen, path);
            }
        }


        public static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
        {
            if (graphics == null)
                throw new ArgumentNullException("graphics");
            if (brush == null)
                throw new ArgumentNullException("brush");

            using (GraphicsPath path = RoundedRect(bounds, cornerRadius))
            {
                graphics.FillPath(brush, path);
            }
        }
    }
}