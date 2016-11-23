using System.Data.SQLite;
using System.Drawing;

namespace QuranAuthor.Models
{
    public enum ExplanationType
    {
        Explain = 0,
        Note = 1,
        Guide = 2
    }

    public enum NoteIcons
    {
        None = 0,
        Note = 1
    }

    public enum GuideIcons
    {
        None = 0,
        Idea = 1,
        Arrow = 2
    }

    public class Explanation : ModelBase
    {
        public int Id { get; set; }
        public int SnippetId { get; set; }
        public ExplanationType Type { get; set; }
        public int Top { get; set; }
        public int Icon { get; set; }
        public string Text { get; set; }

        public Explanation()
        {
        }

        public Explanation(SQLiteDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.SnippetId = reader.GetInt32(1);
            this.Type = (ExplanationType)reader.GetInt32(2);
            this.Top = reader.GetInt32(3);
            this.Icon = reader.GetInt32(4);
            this.Text = reader.GetString(5);
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}