using System.Data.SQLite;
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

        public Snippet()
        {
        }

        public Snippet(SQLiteDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.ChapterId = reader.GetInt32(1);
            this.Page = reader.GetInt32(2);
            this.StartVerse = reader.GetInt32(3);
            this.EndVerse = reader.GetInt32(4);
            this.StartLine = reader.GetInt32(5);
            this.EndLine = reader.GetInt32(6);
            this.StartPoint = reader.GetInt32(7);
            this.EndPoint = reader.GetInt32(8);
            this.Text = reader.GetString(9);
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", StartVerse, Text);
        }
    }
}
