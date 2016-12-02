using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuranAuthor.Repositories
{
    public class ExplanationSnippetsRepository : SnippetRepository
    {
        public ExplanationSnippetsRepository()
        {
            base.TableName = "ExplanationSnippets";
        }
    }

    public class SimilarSnippetsRepository : SnippetRepository
    {
        public SimilarSnippetsRepository()
        {
            base.TableName = "SimilarSnippets";
        }
    }

    public abstract class SnippetRepository : Repository
    {
        protected string TableName { get; set; }

        public List<Snippet> GetSnippets(int chapterId, int page)
        {
            var snippets = new List<Snippet>();

            string sql = "SELECT * FROM " + this.TableName + " WHERE chapterid = @chapterId AND page = @page Order By id";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@chapterId", chapterId);
            command.Parameters.AddWithValue("@page", page);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                snippets.Add(new Snippet(reader));
            }

            return snippets;
        }

        public Snippet AddSnippet(Snippet snippet)
        {
            string sql = "INSERT INTO " + this.TableName + "(ChapterId, Page, StartVerse, EndVerse, StartLine, EndLine, StartPoint, EndPoint, Text, Rtf) VALUES (@ChapterId, @Page, @StartVerse, @EndVerse, @StartLine, @EndLine, @StartPoint, @EndPoint, @Text, @Rtf);";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@ChapterId", snippet.ChapterId);
            command.Parameters.AddWithValue("@Page", snippet.Page);
            command.Parameters.AddWithValue("@StartVerse", snippet.StartVerse);
            command.Parameters.AddWithValue("@EndVerse", snippet.EndVerse);
            command.Parameters.AddWithValue("@StartLine", snippet.StartLine);
            command.Parameters.AddWithValue("@EndLine", snippet.EndLine);
            command.Parameters.AddWithValue("@StartPoint", snippet.StartPoint);
            command.Parameters.AddWithValue("@EndPoint", snippet.EndPoint);
            command.Parameters.AddWithValue("@Text", snippet.Text);
            command.Parameters.AddWithValue("@Rtf", snippet.Rtf);
            
            command.ExecuteNonQuery();

            snippet.Id = Convert.ToInt32(Connection.LastInsertRowId);
            transaction.Commit();

            return snippet;
        }

        public void Swap(Snippet snippet1, Snippet snippet2)
        {
            this.Update(snippet1.Id, snippet2);
            this.Update(snippet2.Id, snippet1);
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM " + this.TableName + " WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private void Update(int id, Snippet snippet)
        {
            string sql = "UPDATE " + this.TableName + " SET ChapterId=@ChapterId, Page=@Page, StartVerse=@StartVerse, EndVerse=@EndVerse, StartLine=@StartLine, EndLine=@EndLine, StartPoint=@StartPoint, EndPoint=@EndPoint, Text=@Text, Rtf=@Rtf WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@ChapterId", snippet.ChapterId);
            command.Parameters.AddWithValue("@Page", snippet.Page);
            command.Parameters.AddWithValue("@StartVerse", snippet.StartVerse);
            command.Parameters.AddWithValue("@EndVerse", snippet.EndVerse);
            command.Parameters.AddWithValue("@StartLine", snippet.StartLine);
            command.Parameters.AddWithValue("@EndLine", snippet.EndLine);
            command.Parameters.AddWithValue("@StartPoint", snippet.StartPoint);
            command.Parameters.AddWithValue("@EndPoint", snippet.EndPoint);
            command.Parameters.AddWithValue("@Text", snippet.Text);
            command.Parameters.AddWithValue("@Rtf", snippet.Rtf);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}