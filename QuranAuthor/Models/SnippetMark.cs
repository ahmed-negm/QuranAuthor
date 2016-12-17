using System;
using System.Data.Common;

namespace QuranAuthor.Models
{
    public class SnippetMark
    {
        public string Id { get; set; }
        public string SnippetId { get; set; }
        public int Line { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }

        public SnippetMark()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Line = 1;
            this.StartPoint = 100;
            this.EndPoint = 400;
        }

        public SnippetMark(DbDataReader reader)
        {
            this.Id = reader.GetString(0);
            this.SnippetId = reader.GetString(1);
            this.Line = reader.GetInt32(2);
            this.StartPoint = reader.GetInt32(3);
            this.EndPoint = reader.GetInt32(4);
        }

        public override string ToString()
        {
            return this.Id;
        }
    }
}