using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranAuthor.Repositories
{
    public class ChapterRepository : Repository
    {
        public List<Chapter> GetChapters()
        {
            var chapters = new List<Chapter>();
            string sql = "SELECT * FROM Chapters";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                chapters.Add(new Chapter(reader));
            }

            return chapters;
        }
    }
}
