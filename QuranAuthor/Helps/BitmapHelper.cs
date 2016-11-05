using QuranAuthor.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Drawing;

namespace QuranAuthor.Helps
{
    public static class BitmapHelper
    {
        private static Color YellowColor = Color.FromArgb(255, 246, 129);
        private static Color FocusColor = Color.FromArgb(125, 255, 246, 129);

        public static SnippetSelection GetSnippetSelection(Bitmap bitmap)
        {
            var selection = new SnippetSelection();
            for (int y = 0; y < bitmap.Size.Height; y++)
            {
                bool firstX = true;
                for (int x = 0; x < bitmap.Size.Width; x++)
                {
                    Color pixel = bitmap.GetPixel(x, y);
                    if (pixel.Equals(YellowColor))
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
    }
}