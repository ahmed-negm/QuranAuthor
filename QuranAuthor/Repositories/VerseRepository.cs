using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranAuthor.Repositories
{
    public class VerseRepository : Repository
    {
        public List<Verse> GetVerses(int chapterId, int startNumber, int endNumber)
        {
            var verses = new List<Verse>();

            string sql = "SELECT * FROM verses WHERE chapterid = @chapterId AND number >= @startNumber AND number <= @endNumber";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            SQLiteParameter param = command.CreateParameter();
            param.ParameterName = "@chapterId";
            param.Value = chapterId;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@startNumber";
            param.Value = startNumber;
            command.Parameters.Add(param);

            param = command.CreateParameter();
            param.ParameterName = "@endNumber";
            param.Value = endNumber;
            command.Parameters.Add(param);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                verses.Add(new Verse(reader));
            }

            return verses;
        }
    }
}
