using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranAuthor.Repositories
{
    public class SnippetRepository : Repository
    {
        public Snippet AddSnippet(Snippet snippet)
        {
            var verses = new List<Verse>();

            string sql = "INSERT INTO Snippet(ChapterId, Page, StartVerse, EndVerse, StartLine, EndLine, StartPoint, EndPoint, Text) VALUES (@ChapterId, @Page, @StartVerse, @EndVerse, @StartLine, @EndLine, @StartPoint, @EndPoint, @Text);";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);
            
            command.Parameters.AddWithValue("@chapterId", snippet.ChapterId);
            command.Parameters.AddWithValue("@Page", snippet.Page);
            command.Parameters.AddWithValue("@StartVerse", snippet.StartVerse);
            command.Parameters.AddWithValue("@EndVerse", snippet.EndVerse);
            command.Parameters.AddWithValue("@StartLine", snippet.StartLine);
            command.Parameters.AddWithValue("@EndLine", snippet.EndLine);
            command.Parameters.AddWithValue("@StartPoint", snippet.StartPoint);
            command.Parameters.AddWithValue("@EndPoint", snippet.EndPoint);
            command.Parameters.AddWithValue("@Text", snippet.Text);

            command.ExecuteNonQuery();

            snippet.Id = Convert.ToInt32(Connection.LastInsertRowId);
            transaction.Commit();

            return snippet;
        }
    }
}
