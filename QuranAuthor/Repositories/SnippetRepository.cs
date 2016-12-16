using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

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

            string sql = "SELECT * FROM snippets WHERE chapterid = @chapterId AND page = @page AND type = @type AND parentId IS NULL Order By [order]";
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

        public List<Snippet> GetSnippetsByParentId(string parentId)
        {
            var snippets = new List<Snippet>();

            string sql = "SELECT * FROM snippets WHERE parentId = @parentId Order By [order]";
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@parentId", parentId);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                snippets.Add(new Snippet(reader));
            }

            return snippets;
        }

        public Snippet AddSnippet(Snippet snippet)
        {
            string sql = "INSERT INTO snippets(Id, Type, [Order], ChapterId, Page, StartVerse, EndVerse, StartLine, EndLine, StartPoint, EndPoint, Text, Rtf, Top, ParentId) VALUES (@Id, @Type, @Order, @ChapterId, @Page, @StartVerse, @EndVerse, @StartLine, @EndLine, @StartPoint, @EndPoint, @Text, @Rtf, @Top, @ParentId);";

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
            command.Parameters.AddWithValue("@Top", snippet.Top);
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
            this.UpdateOrder(snippet1.Id, snippet2.Order);
            this.UpdateOrder(snippet2.Id, snippet1.Order);
        }

        public void Delete(string id)
        {
            var transaction = Connection.BeginTransaction();

            string sql = "DELETE FROM snippets WHERE Id=@Id";
            var command = new SQLiteCommand(sql, Connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            sql = "DELETE FROM snippets WHERE ParentId=@Id";
            command = new SQLiteCommand(sql, Connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            sql = "DELETE FROM explanations WHERE snippetid = @Id";
            command = new SQLiteCommand(sql, Connection);
            command.Parameters.AddWithValue("@Id", id);
            command.ExecuteNonQuery();

            transaction.Commit();
        }

        public void UpdateTop(string id, int top)
        {
            string sql = "UPDATE snippets SET [top]=@top WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@top", top);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private void UpdateOrder(string id, int order)
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