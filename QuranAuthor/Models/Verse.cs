using System.Data.Common;

namespace QuranAuthor.Models
{
    public class Verse
    {
        public int Id { get; set; }
        public int ChapterId { get; set; }
        public int Number { get; set; }
        public int Page { get; set; }
        public string Text { get; set; }
        public string PureText { get; set; }

        public Verse()
        {
        }

        public Verse(DbDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.ChapterId = reader.GetInt32(1);
            this.Number = reader.GetInt32(2);
            this.Page = reader.GetInt32(3);
            this.Text = reader.GetString(4);
            this.PureText = reader.GetString(5);
        }
    }
}
