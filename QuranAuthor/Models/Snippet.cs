using System.Drawing;

namespace QuranAuthor.Models
{
    public class Snippet
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int Page { get; set; }
        public int StartVerse { get; set; }
        public int EndVerse { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public string Text { get; set; }

        public override string ToString()
        {
            return string.Format("{0} -> {1}", StartLine, EndLine);
        }
    }
}
