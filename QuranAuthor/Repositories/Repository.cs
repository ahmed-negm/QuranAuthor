using System.Configuration;
using System.Data.SQLite;

namespace QuranAuthor.Repositories
{
    public abstract class Repository
    {
        private static SQLiteConnection connection;

        protected SQLiteConnection Connection
        {
            get
            {
                if(connection == null)
                {
                    connection = new SQLiteConnection(ConfigurationManager.AppSettings["DbConnection"]);
                    connection.Open();
                }

                return connection;
            }
        }
    }
}

