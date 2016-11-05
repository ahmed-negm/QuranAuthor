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
        private static List<Chapter> cachedChapters;

        public List<Chapter> GetChapters()
        {
            if (cachedChapters == null)
            {
                cachedChapters = new List<Chapter>();
                string sql = "SELECT * FROM Chapters";
                SQLiteCommand command = new SQLiteCommand(sql, Connection);
                SQLiteDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    cachedChapters.Add(new Chapter(reader));
                }
            }
            return cachedChapters;
        }
    }
}
