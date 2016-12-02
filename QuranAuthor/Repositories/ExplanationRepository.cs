using QuranAuthor.Models;
using System.Collections.Generic;
using System.Data.SQLite;

namespace QuranAuthor.Repositories
{
    public class ExplanationRepository : Repository
    {
        public List<Explanation> GetExplanations(string snippetid)
        {
            var explanations = new List<Explanation>();

            string sql = "SELECT * FROM explanations WHERE snippetid = @snippetid Order By [order]";
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
            string sql = "INSERT INTO explanations(Id, SnippetId, [Order], Type, Top, Icon, Text) VALUES (@Id, @SnippetId, @Order, @Type, @Top, @Icon, @Text);";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", explanation.Id);
            command.Parameters.AddWithValue("@SnippetId", explanation.SnippetId);
            command.Parameters.AddWithValue("@Order", explanation.Order);
            command.Parameters.AddWithValue("@Type", (int)explanation.Type);
            command.Parameters.AddWithValue("@Top", explanation.Top);
            command.Parameters.AddWithValue("@Icon", explanation.Icon);
            command.Parameters.AddWithValue("@Text", explanation.Text);

            command.ExecuteNonQuery();
            transaction.Commit();
            return explanation;
        }

        public void Delete(string id)
        {
            string sql = "DELETE FROM explanations WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        public void Update(Explanation explanation)
        {
            this.Update(explanation.Id, explanation);
        }

        public void Swap(Explanation explanation1, Explanation explanation2)
        {
            this.Update(explanation1.Id, explanation2.Order);
            this.Update(explanation2.Id, explanation1.Order);
        }

        private void Update(string id, Explanation explanation)
        {
            string sql = "UPDATE explanations SET SnippetId=@SnippetId, [Order]=@Order, Type=@Type, Top=@Top, Icon=@Icon, Text=@Text WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@SnippetId", explanation.SnippetId);
            command.Parameters.AddWithValue("@Order", explanation.Order);
            command.Parameters.AddWithValue("@Type", (int)explanation.Type);
            command.Parameters.AddWithValue("@Top", explanation.Top);
            command.Parameters.AddWithValue("@Icon", explanation.Icon);
            command.Parameters.AddWithValue("@Text", explanation.Text);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }

        private void Update(string id, int order)
        {
            string sql = "UPDATE explanations SET [Order]=@Order WHERE Id=@Id";

            var transaction = Connection.BeginTransaction();

            SQLiteCommand command = new SQLiteCommand(sql, Connection);

            command.Parameters.AddWithValue("@Order", order);
            command.Parameters.AddWithValue("@Id", id);

            command.ExecuteNonQuery();
            transaction.Commit();
        }
    }
}