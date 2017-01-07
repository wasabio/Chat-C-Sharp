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
                Console.WriteLine("Connecting to the existing database.");
            }
            else
            {
                SQLiteConnection.CreateFile("Users.db");
                co = new SQLiteConnection("Data Source=Users.db;Version=3;");
                string sql = "CREATE TABLE users (id INT PRIMARY KEY NOT NULL, username VARCHAR(30), password VARCHAR(30))";
                executeQuery(sql);
                Console.WriteLine("Database not found, creating a new one.");
            }
        }

        //Register a user, returns true if registration was successful
        public static bool register(int id, string username, string password)
        {
            string sql = "INSERT INTO users (id, username, password) VALUES ( " + id + ", '" + username + "', '" + password + "')";
            
            //if (userNameExists(username) == true) return false;

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
        public static bool login(string username, string password)
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
        private static int executeQuery(string sql)
        {
            try
            {
                Auth.co.Open();
                SQLiteCommand command = new SQLiteCommand(sql, Auth.co);
                int count = command.ExecuteNonQuery();
                Console.WriteLine("Success : " + count);
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
        private static int searchQuery(string sql)
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

        public static int userNameToId(string userName)
        {
            return searchQuery("select id from users where username = '" + userName + "'");
        }

        public static bool userNameExists(string userName)
        {
            if (searchQuery("select id from users where username = '" + userName + "'") == 0) return true;
            else return false;
        }

        public static int generateId()  //Id max +1
        {
            return (searchQuery("select max(id) from users")+1);
        }
    }
}
