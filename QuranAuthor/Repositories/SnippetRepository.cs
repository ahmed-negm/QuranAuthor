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
        public List<Snippet> GetSnippets(int chapterId)
        {
            var snippets = new List<Snippet>();

            string sql = "SELECT * FROM snippets WHERE chapterid = @chapterId Order By id";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@chapterId", chapterId);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                snippets.Add(new Snippet(reader));
            }

            return snippets;
        }

        public Snippet AddSnippet(Snippet snippet)
        {
            var verses = new List<Verse>();

            string sql = "INSERT INTO snippets(ChapterId, Page, StartVerse, EndVerse, StartLine, EndLine, StartPoint, EndPoint, Text) VALUES (@ChapterId, @Page, @StartVerse, @EndVerse, @StartLine, @EndLine, @StartPoint, @EndPoint, @Text);";

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
