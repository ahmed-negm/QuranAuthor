using System;
using System.Data.Common;

namespace QuranAuthor.Models
{
    public class Snippet
    {
        public string Id { get; set; }
        public int Type { get; set; }
        public int Order { get; set; }
        public int ChapterId { get; set; }
        public int Page { get; set; }
        public int StartVerse { get; set; }
        public int EndVerse { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public string Text { get; set; }
        public string Rtf { get; set; }
        public string ParentId { get; set; }

        public Snippet()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public Snippet(DbDataReader reader)
        {
            this.Id = reader.GetString(0);
            this.Type = reader.GetInt32(1);
            this.Order = reader.GetInt32(2);
            this.ChapterId = reader.GetInt32(3);
            this.Page = reader.GetInt32(4);
            this.StartVerse = reader.GetInt32(5);
            this.EndVerse = reader.GetInt32(6);
            this.StartLine = reader.GetInt32(7);
            this.EndLine = reader.GetInt32(8);
            this.StartPoint = reader.GetInt32(9);
            this.EndPoint = reader.GetInt32(10);
            this.Text = reader.GetString(11);
            this.Rtf = reader.GetString(12);
            try
            {
                this.ParentId = reader.GetString(13);
            }
            catch { }
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", StartVerse, Text);
        }
    }
}