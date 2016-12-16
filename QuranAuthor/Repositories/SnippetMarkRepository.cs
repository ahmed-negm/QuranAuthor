using QuranAuthor.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace QuranAuthor.Repositories
{
    public class SnippetMarkRepository : Repository
    {
        public List<SnippetMark> GetMarks(string snippetid)
        {
            var marks = new List<SnippetMark>();

            string sql = "SELECT * FROM snippetmarks WHERE snippetid = @snippetid";
            SQLiteCommand command = new SQLiteCommand(sql, base.Connection);

            command.Parameters.AddWithValue("@snippetid", snippetid);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                marks.Add(new SnippetMark(reader));
            }

            return marks;
        }

        public SnippetMark AddMark(SnippetMark mark)
        {
            string sql = "INSERT INTO snippetmarks(Id, SnippetId, Line, StartPoint, EndPoint) VALUES (@Id, @SnippetId, @Line, @StartPoint, @EndPoint);";

            var transaction = Connection.BeginTransaction();
            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", mark.Id);
            command.Parameters.AddWithValue("@SnippetId", mark.SnippetId);
            command.Parameters.AddWithValue("@Line", mark.Line);
            command.Parameters.AddWithValue("@StartPoint", mark.StartPoint);
            command.Parameters.AddWithValue("@EndPoint", mark.EndPoint);

            command.ExecuteNonQuery();
            transaction.Commit();

            return mark;
        }

        public void Delete(string id)
        {
            string sql = "DELETE FROM snippetmarks WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void Swap(SnippetMark mark1, SnippetMark mark2)
        {
            this.Update(mark1.Id, mark2);
            this.Update(mark2.Id, mark1);
        }

        public void Update(SnippetMark mark)
        {
            this.Update(mark.Id, mark);
        }

        private void Update(string id, SnippetMark mark)
        {
            string sql = "UPDATE snippetmarks SET SnippetId=@SnippetId, Line=@Line, StartPoint=@StartPoint, EndPoint=@EndPoint WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@SnippetId", mark.SnippetId);
            command.Parameters.AddWithValue("@Line", mark.Line);
            command.Parameters.AddWithValue("@StartPoint", mark.StartPoint);
            command.Parameters.AddWithValue("@EndPoint", mark.EndPoint);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}