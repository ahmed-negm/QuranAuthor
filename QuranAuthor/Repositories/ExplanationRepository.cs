using QuranAuthor.Models;
using System;
using System.Collections.Generic;
using System.Data.SQLite;

namespace QuranAuthor.Repositories
{
    public class ExplanationRepository : Repository
    {
        public List<Explanation> GetExplanations(int snippetid)
        {
            var explanations = new List<Explanation>();

            string sql = "SELECT * FROM explanations WHERE snippetid = @snippetid Order By id";
            SQLiteCommand command = new SQLiteCommand(sql, base.Connection);

            command.Parameters.AddWithValue("@snippetid", snippetid);

            SQLiteDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                explanations.Add(new Explanation(reader));
            }

            return explanations;
        }

        public Explanation AddExplanation(Explanation explanation)
        {
            string sql = "INSERT INTO explanations(SnippetsId, Type, Top, Text) VALUES (@SnippetsId, @Type, @Top, @Text);";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@SnippetsId", explanation.SnippetId);
            command.Parameters.AddWithValue("@Type", (int)explanation.Type);
            command.Parameters.AddWithValue("@Top", explanation.Top);
            command.Parameters.AddWithValue("@Text", explanation.Text);

            command.ExecuteNonQuery();

            explanation.Id = Convert.ToInt32(Connection.LastInsertRowId);
            transaction.Commit();

            return explanation;
        }

        public void Delete(int id)
        {
            string sql = "DELETE FROM explanations WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void Swap(Explanation explanation1, Explanation explanation2)
        {
            this.Update(explanation1.Id, explanation2);
            this.Update(explanation2.Id, explanation1);
        }

        private void Update(int id, Explanation explanation)
        {
            string sql = "UPDATE explanations SET SnippetsId=@SnippetsId, Type=@Type, Top=@Top, Text=@Text WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@SnippetsId", explanation.SnippetId);
            command.Parameters.AddWithValue("@Type", (int)explanation.Type);
            command.Parameters.AddWithValue("@Top", explanation.Top);
            command.Parameters.AddWithValue("@Text", explanation.Text);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}
