using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace QuranAuthor.Helps
{
    public static class BitmapHelper
    {
        private static string pagesPath = ConfigurationManager.AppSettings["PagesPath"];
        private static string iconsPath = ConfigurationManager.AppSettings["IconsPath"];
        private static Color yellowColor = Color.FromArgb(255, 246, 129);
        private static Pen explainBorderPen = new Pen(Color.FromArgb(255, 112, 173, 71), 2);
        private static Pen noteBorderPen = new Pen(Color.FromArgb(255, 191, 191, 191), 2);
        private static Pen guideBorderPen = new Pen(Color.FromArgb(255, 255, 165, 0), 2);
        private static Brush explainBrush = new SolidBrush(Color.FromArgb(255, 0, 112, 192));
        private static Brush noteBrush = new SolidBrush(Color.Black);
        private static Brush guideBrush = new SolidBrush(Color.FromArgb(255, 255, 165, 0));
        private static Brush focusBrush = new SolidBrush(Color.FromArgb(255, 132, 60, 12));
        private static Font font36 = new Font("GE SS Text Light", 36);
        private static Font font30 = new Font("GE SS Text Light", 30);
        private static StringFormat rightToLeftStringFormat = new StringFormat(StringFormatFlags.DirectionRightToLeft);
        private static Dictionary<string, Bitmap> icons = new Dictionary<string, Bitmap>();

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
                        if (firstX)
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
                    if (y < startY || y > endY)
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

            guideBorderPen.DashCap = System.Drawing.Drawing2D.DashCap.Round;
            guideBorderPen.DashPattern = new float[] { 4.0F, 2.0F, 1.0F, 3.0F };

            foreach (var explanation in explanations)
            {
                var font = explanation.Type == ExplanationType.Explain ? font36 : font30;
                var pen = explanation.Type == ExplanationType.Explain ? explainBorderPen : explanation.Type == ExplanationType.Note ? noteBorderPen : guideBorderPen;
                var brush = explanation.Type == ExplanationType.Explain ? explainBrush : explanation.Type == ExplanationType.Note ? noteBrush : guideBrush;
                var width = 916;
                if (explanation.Icon > 0 && explanation.Type != ExplanationType.Explain)
                {
                    width -= 50;
                }

                var height = (int)g.MeasureString(explanation.Text, font, new SizeF(width, 1000), rightToLeftStringFormat).Height;

                if (explanation.Type == ExplanationType.Explain)
                {
                    g.FillRoundedRectangle(Brushes.White, new Rectangle(51, explanation.Top + 2, 956, height + 20), 40);
                    g.DrawRoundedRectangle(pen, new Rectangle(49, explanation.Top, 960, height + 24), 40);
                }
                else
                {
                    g.FillRectangle(Brushes.White, new Rectangle(51, explanation.Top + 2, 956, height + 20));
                    g.DrawRectangle(pen, new Rectangle(49, explanation.Top, 960, height + 24));
                    if (explanation.Icon > 0)
                    {
                        g.DrawImage(GetIcon(explanation.Type, explanation.Icon), new Point(940, explanation.Top + 8));
                    }
                }

                DrawString(g, explanation, font, brush, height, width);
            }

            g.Flush();
            return bitmap;
        }

        public static Bitmap DrawSimilarSnippets(Bitmap bitmap, IList<Snippet> similarSnippets)
        {
            var explanations = new List<Explanation>();
            foreach (var snippet in similarSnippets)
            {
                var exp = new Explanation();
                exp.Text = snippet.Text;
                exp.Top = snippet.Top;
                explanations.Add(exp);
            }
            return DrawExplanation(bitmap, explanations);
        }

        private static Bitmap GetIcon(ExplanationType explanationType, int icon)
        {
            var file = string.Empty;
            switch (explanationType)
            {
                case ExplanationType.Note:
                    file = ((NoteIcons)icon).ToString();
                    break;
                case ExplanationType.Guide:
                    file = ((GuideIcons)icon).ToString();
                    break;
                default:
                    break;
            }
            if (string.IsNullOrEmpty(file))
            {
                throw new Exception(string.Format("Icon {0} has no file", icon));
            }
            if (!icons.ContainsKey(file))
            {
                icons.Add(file, new Bitmap(Path.Combine(iconsPath, file + ".png")));
            }
            return icons[file];
        }

        private static void Blur(Bitmap bitmap, int y, int x)
        {
            var color = bitmap.GetPixel(x, y);
            if (color.R == 255 & color.G == 255 && color.B == 255)
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
                    if (line == snippet.StartLine && line == snippet.EndLine)
                    {
                        for (int x = snippet.EndPoint; x <= snippet.StartPoint; x++)
                        {
                            points.Add(new SnippetPoint(x, y));
                        }
                    }
                    else if (line == snippet.StartLine)
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

        private static GraphicsPath RoundedRect(Rectangle bounds, int radius)
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

        private static void DrawRoundedRectangle(this Graphics graphics, Pen pen, Rectangle bounds, int cornerRadius)
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

        private static void FillRoundedRectangle(this Graphics graphics, Brush brush, Rectangle bounds, int cornerRadius)
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

        private static List<CharacterRange> Sansitize(ref string text)
        {
            var ranges = new List<CharacterRange>();
            var start = text.IndexOf("**");
            var end = text.IndexOf("**", start + 1);
            while (start > -1 && end > -1)
            {
                text = text.Substring(0, start) + text.Substring(start + 2, end - start - 2) + text.Substring(end + 2);

                var wordStart = start;
                for (int i = start; i < end - 2; i++)
                {
                    if (text[i] == ' ')
                    {
                        if ((i - wordStart) > 0)
                        {
                            ranges.Add(new CharacterRange(wordStart, i - wordStart));
                        }
                        wordStart = i + 1;
                    }
                }

                ranges.Add(new CharacterRange(wordStart, end - wordStart - 2));

                if (end + 1 >= text.Length)
                {
                    break;
                }
                start = text.IndexOf("**", end + 1);
                end = text.IndexOf("**", start + 1);
            }
            return ranges;
        }

        private static string Range(string text, CharacterRange charRange)
        {
            return text.Substring(charRange.First, charRange.Length);
        }

        private static void DrawString(Graphics g, Explanation explanation, Font font, Brush brush, int height, int width)
        {
            var text = explanation.Text;
            var ranges = Sansitize(ref text);
            var textRect = new RectangleF(71, explanation.Top + 12, width, height);

            g.DrawString(text, font, brush, textRect, rightToLeftStringFormat);


            CharacterRange[][] chunks = ranges
                    .Select((s, i) => new { Value = s, Index = i })
                    .GroupBy(x => x.Index / 30)
                    .Select(grp => grp.Select(x => x.Value).ToArray())
                    .ToArray();

            foreach (var chunk in chunks)
            {
                textRect = DrawBold(g, font, text, chunk, textRect);
            }
        }

        private static RectangleF DrawBold(Graphics g, Font font, string text, CharacterRange[] chunk, RectangleF textRect)
        {
            var stringFormat = new StringFormat();
            stringFormat.FormatFlags = StringFormatFlags.DirectionRightToLeft;
            stringFormat.SetMeasurableCharacterRanges(chunk);

            var regions = g.MeasureCharacterRanges(text, font, textRect, stringFormat);

            for (int i = 0; i < regions.Length; i++)
            {
                var rect = regions[i].GetBounds(g);
                g.DrawRectangle(new Pen(Color.White, 2), Rectangle.Round(rect));
                g.FillRectangle(Brushes.White, Rectangle.Round(rect));
                g.DrawString(Range(text, chunk[i]), font, focusBrush, AdjustRegionRect(rect), stringFormat);
            }
            return textRect;
        }

        private static RectangleF AdjustRegionRect(RectangleF rect)
        {
            return new RectangleF(rect.Left - 15, rect.Top - 1, rect.Width + 24, rect.Height);
        }
    }
}