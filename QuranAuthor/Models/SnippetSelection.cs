using System.Drawing;

namespace QuranAuthor.Models
{
    public class SnippetSelection
    {
        public SnippetPoint Start { get; set; }
        public SnippetPoint End { get; set; }

        public SnippetSelection()
        {
            this.Start = new SnippetPoint(-1, -1);
            this.End = new SnippetPoint(-1, -1);
        }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", Start, End);
        }
    }

    public class SnippetPoint
    {
        public int X { get; set; }
        public int Y { get; set; }

        public SnippetPoint(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public override string ToString()
        {
            return string.Format("[{0},{1}]", X, Y);
        }
    }
}
