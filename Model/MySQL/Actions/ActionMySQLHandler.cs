using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;

namespace _06_24InformationSystem.Model.MySQL.Actions
{
    class ActionMySQLHandler : LogMaster
    {

        static string conString = XMLReader.getMySqlConnectionString();


        public static void addAction(string newAction)
        {
            try
            {
                string AID = string.Empty;
                
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MAX(ActionID) FROM LogActions;", connection))
                    {
                        connection.Open();

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                AID = reader.GetString(0); // Customer ID
                            }
                        }
                    }
                    AID += 1;
                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO LogActions (ActionID, Action) VALUES (@aid, @action);";

                    comm.Parameters.AddWithValue("@aid", AID);
                    comm.Parameters.AddWithValue("@action", newAction);

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
