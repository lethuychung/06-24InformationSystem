using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _06_24InformationSystem.Model.MySQL.Summation
{
    class EventsMySQLHandler : LogMaster
    {
        static string conString = XMLReader.getMySqlConnectionString();
        
        public class LogObject
        {
            public string Tid { get; set; }
            public string Kund { get; set; }
            public string Utrustning { get; set; }
            public string Swap { get; set; }
            public string Regnr { get; set; }
            public string Fel { get; set; }
            public string Åtgärd { get; set; }
            public string Kommentar { get; set; }
            public string Användare { get; set; }
        }

        public class SummationObject
        {
            public string Pass { get; set; }
            public string Användare { get; set; }
            public string Kommentar { get; set; }
            
        }

        public static DataTable getLogBookDataAdapter(DateTime selectedDay)
        {
            DataTable dt = new DataTable();
            // 2016-03-01 07:52:33
            var startDate = selectedDay.Year + "-" + selectedDay.Month.ToString("00") + "-" + selectedDay.Day.ToString("00") + " 03:00:00";
            selectedDay = selectedDay.AddDays(1);
            var endDate = selectedDay.Year +  "-" + selectedDay.Month.ToString("00") + "-" + selectedDay.Day.ToString("00") + " 01:00:00";


            MySqlConnection cn = new MySqlConnection(conString);

            try
            {
                // change order on select values to get correct order in datagridview
                string sqlCmd = "SELECT LogBook.IncDate AS Tid, customers.CustomerName AS Kund, equipment.EquipmentName AS  Utrustning, LogBook.SwapID AS Swap, LogBook.RegNr, LogBook.Error AS Fel, LogActions.Action AS Åtgärd, LogBook.Comment AS Kommentar, users.Username AS Användare FROM customers INNER JOIN LogBook ON customers.CustomerID = LogBook.Customer INNER JOIN equipment ON LogBook.Equipment = equipment.EquipmentID INNER JOIN LogActions ON LogActions.ActionID = LogBook.ActionTaken INNER JOIN users ON LogBook.User = users.UserID WHERE IncDate BETWEEN @startdate AND @enddate ORDER BY IncDate DESC;";

                
                MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, cn);
                adr.SelectCommand.Parameters.AddWithValue("@startdate", startDate);
                adr.SelectCommand.Parameters.AddWithValue("@enddate", endDate);
                adr.SelectCommand.CommandType = CommandType.Text;

                adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)
                

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                cn.Dispose(); // return connection to pool
            }
            return dt;
        }
        
        public static List<LogObject> getLogBook(DateTime selectedDay) // return dataset instead (fix)
        {

            var watch = Stopwatch.StartNew();

            string retString = string.Empty;

            List<LogObject> lo = new List<LogObject>();

            var startDate = selectedDay.Day.ToString("00") + "-" + selectedDay.Month.ToString("00") + "-" + selectedDay.Year + " 03:00:00";
            selectedDay = selectedDay.AddDays(1);
            var endDate = selectedDay.Day.ToString("00") + "-" + selectedDay.Month.ToString("00") + "-" + selectedDay.Year + " 01:00:00";


            try
            {
                string swapbool = string.Empty;
                string regnr = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT customers.CustomerName, LogBook.User, LogBook.Error, LogBook.Comment, LogBook.SwapID, LogBook.RegNr, LogBook.IncDate, equipment.EquipmentName, LogActions.Action FROM customers INNER JOIN LogBook ON customers.CustomerID = LogBook.Customer INNER JOIN equipment ON LogBook.Equipment = equipment.EquipmentID INNER JOIN LogActions ON LogActions.ActionID = LogBook.ActionTaken WHERE IncDate BETWEEN @startdate AND @enddate ORDER BY IncDate DESC;", connection))
                    {
                        cmd.Parameters.AddWithValue("@startdate", startDate);
                        cmd.Parameters.AddWithValue("@enddate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            var fixString = string.Empty;
                            while (reader.Read())
                            {
                                LogObject logObject = new LogObject();

                                try
                                {
                                    logObject.Regnr = reader.GetString("RegNr");
                                    logObject.Swap = swapbool = reader.GetString("SwapID");
                                }
                                catch (Exception)
                                {
                                    swapbool = "";
                                    regnr = "";
                                }

                                if (logObject.Swap == "1")
                                {
                                    logObject.Swap = "JA";
                                }

                                if (logObject.Swap == "0")
                                {
                                    logObject.Swap = "";
                                }

                                logObject.Användare = getUsernameFromID(reader.GetString("User"));
                                logObject.Fel = reader.GetString("Error");

                                var temp3 = reader.GetString("Error");
                                byte[] bytes4 = Encoding.Default.GetBytes(temp3);
                                temp3 = Encoding.UTF8.GetString(bytes4);
                                logObject.Fel = temp3;

                                var temp = reader.GetString("Comment");
                                byte[] bytes2 = Encoding.Default.GetBytes(temp);
                                temp = Encoding.UTF8.GetString(bytes2);
                                logObject.Kommentar = temp;


                                logObject.Kund = reader.GetString("CustomerName");

                                logObject.Tid = reader.GetString("IncDate");
                                logObject.Utrustning = reader.GetString("EquipmentName");
                                logObject.Åtgärd = reader.GetString("Action");

                                lo.Add(logObject);


                            }
                        }
                        watch.Stop();
                        double elapsedMs = watch.ElapsedMilliseconds;
                        Console.WriteLine("Elapsed time: " + elapsedMs / 1000 + " Seconds");

                        return lo;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return lo;
            }
        }

        public static string getUsernameFromID(string userID)
        {
            try
            {
                string username = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Username FROM users WHERE UserID = @ID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", userID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetString(0); // username

                            }
                            return "error";
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "error";
            }
        }

        public static List<SummationObject> getSummation(DateTime selectedDay)
        {
            string retString = string.Empty;

            List<SummationObject> so = new List<SummationObject>();

            DateTime startDate = selectedDay.AddHours(3);

            DateTime endDate = selectedDay.AddDays(1);
            endDate = endDate.AddHours(1); // ökar på en timme för att vara säker på att få sista passet på kvällen

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT WorkTime, Comment, User FROM Summation WHERE Date BETWEEN @startDate AND @endDate;", connection))
                    {
                        cmd.Parameters.AddWithValue("@startDate", startDate);
                        cmd.Parameters.AddWithValue("@endDate", endDate);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                SummationObject sumObj = new SummationObject();

                                sumObj.Användare = reader.GetString("User");
                                sumObj.Pass = reader.GetString("WorkTime");
                                sumObj.Kommentar = reader.GetString("Comment");
                                so.Add(sumObj);
                            }
                        }
                        return so;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return so;
            }
        }
    }
}
