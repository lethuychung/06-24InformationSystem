using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;

namespace _06_24InformationSystem.Model.MySQL.UserHandler
{
    class userController : LogMaster
    {

        static string conString = XMLReader.getMySqlConnectionString();

        public static bool checkIfPasswordExists(string user)
        {
            var password = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Pwd FROM users WHERE Username = @username", connection))
                    {
                        cmd.Parameters.AddWithValue("@username", user);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                password = reader.GetString("Pwd");
                            }
                        }
                        if(password == "")
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return false;
            }
        }

        public static bool checkIfUserExists(string user)
        {
            var userExist = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT UserName FROM users WHERE Username = @username", connection))
                    {
                        cmd.Parameters.AddWithValue("@username", user);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userExist = reader.GetString("UserName");
                            }
                        }
                        if (userExist == "")
                        {
                            return false;
                        }
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return false;
            }
        }

        public static bool updatePassword(string Username, string Password)
        {
            try
            {
                var hash = PasswordEncryption.GeneratePasswordHash(Password);

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "UPDATE users SET Pwd=@PWD WHERE Username = @UN";

                    comm.Parameters.AddWithValue("@UN", Username);
                    comm.Parameters.AddWithValue("@PWD", hash);

                    comm.ExecuteNonQuery();
                    return true;
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return false;
            }
        }

    }
}
