using System.Drawing;

namespace QuranAuthor.Models
{
    public class Snippet
    {
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", StartLine, EndLine);
        }
    }
}
