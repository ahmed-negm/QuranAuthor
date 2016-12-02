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
            base.Type = 0;
        }
    }

    public class SimilarSnippetsRepository : SnippetRepository
    {
        public SimilarSnippetsRepository()
        {
            base.Type = 1;
        }
    }

    public abstract class SnippetRepository : Repository
    {
        protected int Type { get; set; }

        public List<Snippet> GetSnippets(int chapterId, int page)
        {
            var snippets = new List<Snippet>();

            string sql = "SELECT * FROM snippets WHERE chapterid = @chapterId AND page = @page AND type = @type Order By [order]";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@chapterId", chapterId);
            command.Parameters.AddWithValue("@page", page);
            command.Parameters.AddWithValue("@type", this.Type);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                snippets.Add(new Snippet(reader));
            }

            return snippets;
        }

        public Snippet AddSnippet(Snippet snippet)
        {
            string sql = "INSERT INTO snippets(Id, Type, [Order], ChapterId, Page, StartVerse, EndVerse, StartLine, EndLine, StartPoint, EndPoint, Text, Rtf, ParentId) VALUES (@Id, @Type, @Order, @ChapterId, @Page, @StartVerse, @EndVerse, @StartLine, @EndLine, @StartPoint, @EndPoint, @Text, @Rtf, @ParentId);";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", snippet.Id);
            command.Parameters.AddWithValue("@Type", this.Type);
            command.Parameters.AddWithValue("@Order", snippet.Order);
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
            if (string.IsNullOrEmpty(snippet.ParentId))
            {
                command.Parameters.AddWithValue("@ParentId", DBNull.Value);
            }
            else
            {
                command.Parameters.AddWithValue("@ParentId", snippet.ParentId);
            }
            
            command.ExecuteNonQuery();
            transaction.Commit();
            return snippet;
        }

        public void Swap(Snippet snippet1, Snippet snippet2)
        {
            this.Update(snippet1.Id, snippet2.Order);
            this.Update(snippet2.Id, snippet1.Order);
        }

        public void Delete(string id)
        {
            string sql = "DELETE FROM snippets WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private void Update(string id, int order)
        {
            string sql = "UPDATE snippets SET [order]=@order WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@order", order);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}