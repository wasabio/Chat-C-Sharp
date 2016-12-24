using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.IO;

namespace chat
{

    class Auth
    {
        static private SQLiteConnection co;

        public Auth()
        {

            if(File.Exists("Users.db"))
            {
                co = new SQLiteConnection("Data Source=Users.db;Version=3;");
            }
            else
            {
                SQLiteConnection.CreateFile("Users.db");
                co = new SQLiteConnection("Data Source=Users.db;Version=3;");
                string sql = "create table users (id INTEGER PRIMARY KEY, username TEXT, password TEXT)";
                executeQuery(sql);
            }
        }

        //Register a user, returns true if registration was successful
        bool register(int id, string username, string password)
        {
            string sql = "insert into users (id, username, password) values ( " + id + ", '" + username + "', '" + password + "')";
            int count = executeQuery(sql);

            if (count == 1)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        //Verify in DB if a user exists with this password
        bool login(string username, string password)
        {
            string sql = "select id from users where username = '" + username + "' and password='" + password + "'";
            int count = searchQuery(sql);

            if (count != 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //Used to make INSERT, UPDATE and DELETE query in DB
        private int executeQuery(string sql)
        {
            try
            {
                Auth.co.Open();
                SQLiteCommand command = new SQLiteCommand(sql, co);
                int count = command.ExecuteNonQuery();
                return count;
            }
            catch
            {
                Console.WriteLine("Error accessing database.");
                return 0;
                throw;
            }
            finally
            {
                Auth.co.Close();
            }
        }

        //Make a search query in DB
        private int searchQuery(string sql)
        {
            try
            {
                Auth.co.Open();
                SQLiteCommand command = new SQLiteCommand(sql, co);
                var result = command.ExecuteScalar();
                if (result != null)
                {
                    return (int)result;         //Returns corresponding ID for the given username
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                Console.WriteLine("Error accessing database.");
                return 0;
                throw;
            }
            finally
            {
                Auth.co.Close();
            }
        }
    }

}
