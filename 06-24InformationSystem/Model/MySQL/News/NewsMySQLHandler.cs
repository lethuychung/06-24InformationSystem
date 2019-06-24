using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_24InformationSystem.Model.MySQL.News
{
    class NewsMySQLHandler : LogMaster
    {

        static string conString = XMLReader.getMySqlConnectionString();

        public static bool checkIfDisplayNews(string user)
        {
            var temp = getReadNews(user);
            if(temp == 1) { return true; }
            return false;
        }


        private static int getReadNews(string user)
        {
            string NewsRead = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    //   var custID = getCustomerID(customer);
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT ReadNews FROM users WHERE Username = @userName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@userName", user);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NewsRead = reader.GetString(0); // Customer ID
                                var temp = Convert.ToInt32(NewsRead);
                                return temp;
                            }
                            return 0;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return 0;
            }
        }


        public static string getNews()
        {
            string NewsBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT NewsBlob FROM News;", connection))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                NewsBlob = reader.GetString(0); // Customer ID
                                return NewsBlob;
                            }
                            return NewsBlob;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }

        public static void updateReadNews(string username)
        {
            try
            {
                string qaid = string.Empty;
                
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "UPDATE users SET ReadNews = '1' WHERE Username = @username;";

                    comm.Parameters.AddWithValue("@username", username);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void updateNews(string news)
        {
            var Date = DateTime.Now.ToString();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "UPDATE News SET NewsBlob = @news, Date = @date WHERE NewsID = 1;";

                    comm.Parameters.AddWithValue("@news", news);
                    comm.Parameters.AddWithValue("@date", Date);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void resetNewsRead()
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "SET SQL_SAFE_UPDATES = 0; UPDATE users SET ReadNews = '0'; SET SQL_SAFE_UPDATES = 1;";
                    
                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

    }
}
