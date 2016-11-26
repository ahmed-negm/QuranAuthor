using System.Data.Common;

namespace QuranAuthor.Models
{
    public class Chapter
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int StartPage { get; set; }

        public Chapter()
        {
        }

        public Chapter(DbDataReader reader)
        {
            this.Id = reader.GetInt32(0);
            this.Name = reader.GetString(1);
            this.StartPage = reader.GetInt32(2);
        }

        public override string ToString()
        {
            return string.Format("{0}:{1}", this.Id, this.Name);
        }
    }
}
