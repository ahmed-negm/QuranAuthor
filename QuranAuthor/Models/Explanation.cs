using System;
using System.Data.Common;

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
        public string Id { get; set; }
        public string SnippetId { get; set; }
        public int Order { get; set; }
        public ExplanationType Type { get; set; }
        public int Top { get; set; }
        public int Icon { get; set; }
        public string Text { get; set; }

        public Explanation()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public Explanation(DbDataReader reader)
        {
            this.Id = reader.GetString(0);
            this.SnippetId = reader.GetString(1);
            this.Order = reader.GetInt32(2);
            this.Type = (ExplanationType)reader.GetInt32(3);
            this.Top = reader.GetInt32(4);
            this.Icon = reader.GetInt32(5);
            this.Text = reader.GetString(6);
        }

        public override string ToString()
        {
            return this.Text;
        }
    }
}