using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace _06_24InformationSystem.Model.MySQL.Logbook
{
    class LOGMySQLHandler : LogMaster
    {

        static string conString = XMLReader.getMySqlConnectionString();

        public static void logThis(string Action, string User, string Error, string Comment, bool Swap, string RegNr, string Customer, string Equipment, string IncDate, string GUID)
        {
            var corrTime = DateTime.ParseExact(IncDate, "dd-MM-yyyy HH:mm:ss", CultureInfo.InvariantCulture).ToString("yyyy-MM-dd HH:mm:ss");
            // INSERT INTO LogBook (ActionTaken, User, Error, Comment, SwapID, Customer, Equipment, LoggedDate, IncDate, GUID) VALUES(@AT, @USER, @ERROR, @COMMENT, @SWAPID, @REGNR, @CUSTOMERID, @EQPID, @LOGDATE, @LOGINC, @GUID) ON DUPLICATE KEY UPDATE Error=@ERROR, Comment="ab";"
            try
            {
                if (Swap == true)
                {
                    using (MySqlConnection connection = new MySqlConnection(conString))
                    {
                        MySqlCommand comm = connection.CreateCommand();
                        connection.Open();
                        comm.CommandText = "INSERT INTO LogBook (ActionTaken, User, Error, Comment, SwapID, RegNr, Customer, Equipment, IncDate, GUID) VALUES(@AT, @USER, @ERROR, @COMMENT, @SWAPID, @REGNR, @CUSTOMERID, @EQPID, @LOGINC, @GUID) ON DUPLICATE KEY UPDATE Error = @ERROR, Comment = @COMMENT, ActionTaken = @AT, SwapID = @SWAPID, RegNr = @REGNR";

                        comm.Parameters.AddWithValue("@AT", Action);
                        comm.Parameters.AddWithValue("@USER", getUserID(User));
                        comm.Parameters.AddWithValue("@ERROR", Error);
                        comm.Parameters.AddWithValue("@COMMENT", Comment);
                        comm.Parameters.AddWithValue("@SWAPID", "JA");
                        comm.Parameters.AddWithValue("@REGNR", RegNr);
                        comm.Parameters.AddWithValue("@CUSTOMERID", getCustomerID(Customer));
                        comm.Parameters.AddWithValue("@EQPID", getEquipmentID(Equipment, Customer));
                        comm.Parameters.AddWithValue("@LOGINC", corrTime);
                        comm.Parameters.AddWithValue("@GUID", GUID);

                        comm.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    using (MySqlConnection connection = new MySqlConnection(conString))
                    {
                        MySqlCommand comm = connection.CreateCommand();
                        connection.Open();
                        comm.CommandText = "INSERT INTO LogBook (ActionTaken, User, Error, Comment, SwapID, Customer, Equipment, IncDate, GUID) VALUES(@AT, @USER, @ERROR, @COMMENT, @SWAPID, @CUSTOMERID, @EQPID, @LOGINC, @GUID) ON DUPLICATE KEY UPDATE Error = @ERROR, Comment = @COMMENT, ActionTaken = @AT, SwapID = '', RegNr = ''";

                        comm.Parameters.AddWithValue("@AT", Action);
                        comm.Parameters.AddWithValue("@USER", getUserID(User));
                        comm.Parameters.AddWithValue("@ERROR", Error);
                        comm.Parameters.AddWithValue("@COMMENT", Comment);
                        comm.Parameters.AddWithValue("@SWAPID", "");
                        comm.Parameters.AddWithValue("@CUSTOMERID", getCustomerID(Customer));
                        comm.Parameters.AddWithValue("@EQPID", getEquipmentID(Equipment, Customer));
                        comm.Parameters.AddWithValue("@LOGINC", corrTime);
                        comm.Parameters.AddWithValue("@GUID", GUID);

                        comm.ExecuteNonQuery();

                        connection.Close();
                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        private static int getCustomerID(string customer)
        {
            string CustID = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    //   var custID = getCustomerID(customer);
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        cmd.Parameters.AddWithValue("@cname", customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                CustID = reader.GetString(0); // Customer ID
                                var temp = Convert.ToInt32(CustID);
                                return temp;
                            }
                            return -1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return -1;
            }
        }
        
        private static int getEquipmentID(string equipmentName, string customerName)
        {
            int eqpID;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    //   var custID = getCustomerID(customer);
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT EquipmentID FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = @cname AND EquipmentName = @eqpname;", connection))
                    {
                        cmd.Parameters.AddWithValue("@cname", customerName);
                        cmd.Parameters.AddWithValue("@eqpname", equipmentName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                eqpID = Convert.ToInt32(reader.GetString(0)); // Customer ID
                                return eqpID;
                            }
                            return -1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return -1;
            }
        }


        private static int getUserID(string UserName)
        {
            int userID;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    //   var custID = getCustomerID(customer);
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT UserID FROM users WHERE Username = @username;", connection))
                    {
                        cmd.Parameters.AddWithValue("@username", UserName);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                userID = Convert.ToInt32(reader.GetString(0)); // Customer ID
                                return userID;
                            }
                            return -1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return -1;
            }
        }

        
        public static void updateAnswer(string title, string answer)
        {
            try
            {
                string qaid = string.Empty;

                if (title.Contains("-- NY --"))
                {
                    title = title.Remove(0, 9);
                }

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT QAID FROM QA WHERE QTitle = @title;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@title", title);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                qaid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "UPDATE QA SET Answer = @answer, Answered = '1' WHERE QAID = @qaid;";

                    comm.Parameters.AddWithValue("@qaid", qaid);
                    comm.Parameters.AddWithValue("@answer", answer);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void addSummation(string time, string User, string comment)
        {
            try
            {
                var temp = DateTime.Now.ToString();
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO Summation (WorkTime, Comment, User, Date) VALUES(@time, @comment, @user, @date);";

                    comm.Parameters.AddWithValue("@time", time);
                    comm.Parameters.AddWithValue("@user", User);
                    comm.Parameters.AddWithValue("@date", temp);
                    comm.Parameters.AddWithValue("@comment", comment);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static void addMissedCalls(string User, int NrofCalls, string pass)
        {
            try
            {
                var temp = DateTime.Now.ToString();
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO missedLogging (TimeStamp, User, MissedCalls, Pass) VALUES (@time, @user, @calls, @pass);";

                    comm.Parameters.AddWithValue("@time", temp);
                    comm.Parameters.AddWithValue("@user", User);
                    comm.Parameters.AddWithValue("@calls", NrofCalls);
                    comm.Parameters.AddWithValue("@pass", pass);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void AddCommonUser(string name)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO users_for_logging (UserName) VALUES(@Name);";

                    comm.Parameters.AddWithValue("@Name", name);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static List<MissedCallsObject> getMissedCallList()
        {
            string retString = string.Empty;

            List<MissedCallsObject> mco = new List<MissedCallsObject>();
            
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT TimeStamp, User, MissedCalls, Pass FROM missedLogging;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MissedCallsObject missedObj = new MissedCallsObject();

                                missedObj.Tid = reader.GetString("TimeStamp");
                                missedObj.Användare = reader.GetString("User");
                                missedObj.Pass = reader.GetString("Pass");
                                missedObj.Missade = reader.GetString("MissedCalls");
                                mco.Add(missedObj);
                            }
                        }
                        return mco;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return mco;
            }
        }

        public class MissedCallsObject
        {
            public string Tid { get; set; }
            public string Användare { get; set; }
            public string Missade { get; set; }
            public string Pass { get; set; }
        }
    }
}
