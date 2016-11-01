using QuranAuthor.Models;
using System.Drawing;

namespace QuranAuthor.Helps
{
    public static class BitmapHelper
    {
        private static Color YellowColor = Color.FromArgb(255, 246, 129);

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
    }
}