using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;

namespace _06_24InformationSystem.Model.MySQL.Export
{
    class ExportMySQL : LogMaster
    {
        static string conString = XMLReader.getMySqlConnectionString();


        public static void getCSVFromUser(string userName, string startDate, string endDate, string filename)
        {
            try
            {
                DataTable dt = new DataTable();

                MySqlConnection cn = new MySqlConnection(conString);

                try
                {
                    // change order on select values to get correct order in datagridview
                    string sqlCmd = "SELECT LogBook.IncDate AS Tid, customers.CustomerName AS Kund, equipment.EquipmentName AS  Utrustning, LogBook.SwapID AS Swap, LogBook.RegNr, LogBook.Error AS Fel, LogActions.Action AS Åtgärd, LogBook.Comment AS Kommentar, users.Username AS Användare FROM customers INNER JOIN LogBook ON customers.CustomerID = LogBook.Customer INNER JOIN equipment ON LogBook.Equipment = equipment.EquipmentID INNER JOIN LogActions ON LogActions.ActionID = LogBook.ActionTaken INNER JOIN users ON LogBook.User = users.UserID WHERE IncDate BETWEEN @startdate AND @enddate AND users.UserName = @user ORDER BY IncDate DESC;";


                    MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, cn);
                    adr.SelectCommand.Parameters.AddWithValue("@startdate", startDate);
                    adr.SelectCommand.Parameters.AddWithValue("@enddate", endDate);
                    adr.SelectCommand.Parameters.AddWithValue("@user", userName);
                    adr.SelectCommand.CommandType = CommandType.Text;

                    adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                    var shortDate = DateTime.Now.ToShortDateString();
                    var fileName = @"C:\Swarco\0624\LogBookExport\User\LogUser -" + filename + ".csv";
                    // Open output stream
                    StreamWriter swFile = new StreamWriter(fileName, false, Encoding.UTF8);

                    // Header
                    string[] colLbls = new string[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        colLbls[i] = dt.Columns[i].ColumnName;
                        colLbls[i] = GetWriteableValueForCsv(colLbls[i]);
                    }

                    // Write labels
                    swFile.WriteLine(string.Join(";", colLbls));

                    // Rows of Data
                    foreach (DataRow rowData in dt.Rows)
                    {
                        string[] colData = new string[dt.Columns.Count];

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            object obj = rowData[i];
                            colData[i] = GetWriteableValueForCsv(obj);
                        }

                        // Write data in row
                        swFile.WriteLine(string.Join(";", colData));
                    }

                    // Close output stream
                    swFile.Close();



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    cn.Dispose(); // return connection to pool
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        private static void exportFilter(string filename, string mysqlString)
        {
            try
            {
                DataTable dt = new DataTable();

                MySqlConnection cn = new MySqlConnection(conString);

                try
                {
                    // change order on select values to get correct order in datagridview
                    string sqlCmd = mysqlString;


                    MySqlDataAdapter adr = new MySqlDataAdapter(sqlCmd, cn);


                    adr.Fill(dt); //opens and closes the DB connection automatically !! (fetches from pool)

                    var shortDate = DateTime.Now.ToShortDateString();
                    var fileName = @"C:\Swarco\0624\LogBookExport\Filter\FilterLog - " + filename + ".csv";
                    // Open output stream
                    StreamWriter swFile = new StreamWriter(fileName, false, Encoding.UTF8);

                    // Header
                    string[] colLbls = new string[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        colLbls[i] = dt.Columns[i].ColumnName;
                        colLbls[i] = GetWriteableValueForCsv(colLbls[i]);
                    }

                    // Write labels
                    swFile.WriteLine(string.Join(";", colLbls));

                    // Rows of Data
                    foreach (DataRow rowData in dt.Rows)
                    {
                        string[] colData = new string[dt.Columns.Count];

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            object obj = rowData[i];
                            colData[i] = GetWriteableValueForCsv(obj);
                        }

                        // Write data in row
                        swFile.WriteLine(string.Join(";", colData));
                    }

                    // Close output stream
                    swFile.Close();



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    cn.Dispose(); // return connection to pool
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }



        public static void exportFilterToCSV(string customer, List<string> customers, List<string> Equipment, List<string> actions, List<string> users, string startDate, string endDate, string name, bool swapChecked)
        {
            string StartString = "SELECT LogBook.IncDate AS Tid, customers.CustomerName AS Kund, equipment.EquipmentName AS  Utrustning, LogBook.SwapID AS Swap, LogBook.RegNr, LogBook.Error AS Fel, LogActions.Action AS Åtgärd, LogBook.Comment AS Kommentar, users.Username AS Användare FROM customers INNER JOIN LogBook ON customers.CustomerID = LogBook.Customer INNER JOIN equipment ON LogBook.Equipment = equipment.EquipmentID INNER JOIN LogActions ON LogActions.ActionID = LogBook.ActionTaken INNER JOIN users ON LogBook.User = users.UserID WHERE IncDate BETWEEN ";

            bool singleUser = false;
            bool singleAction = false;
            bool singleCustomer = false;

            var temp = getMySQLUserString(users, ref singleUser);
            if (!singleUser && temp != "")
            {
                var temp2 = temp.Substring(0, temp.Length - 3);
                temp = temp2 + ")";
            }


            var temp3 = getMySQLActionString(actions, ref singleAction);
            if (!singleAction)
            {
                var temp4 = temp3.Substring(0, temp3.Length - 3);
                temp3 = temp4 + ")";
            }

            var temp6 = "";
            var temp8 = "";
            var temp7 = "";
            try
            {
                var temp5 = getMySQLEquipmentString(Equipment, customers);
                temp6 = temp5.Substring(0, temp5.Length - 3);
                temp6 = temp6 + ")";
            }
            catch (Exception e)
            {
                // ignore exception
            }

            try
            {
                temp7 = getMySQLCustomerstString(customers, ref singleCustomer);
                if (!singleCustomer)
                {
                    temp8 = temp7.Substring(0, temp7.Length - 3);
                    temp8 = temp8 + ")";
                }
            }
            catch (Exception e)
            {
                // ignore exception
            }

            string custOrEqp = string.Empty;
            if (temp6 != "")
            {
                custOrEqp = temp6;
            }
            else
            {
                if (!singleCustomer)
                {
                    custOrEqp = temp8;
                }
                else
                {
                    custOrEqp = temp7;
                }
            }

            Console.WriteLine(custOrEqp);

            if (!swapChecked)
            {
                if (temp == "")
                {
                    if (custOrEqp != "")
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp3 + " AND " + custOrEqp + " ORDER BY IncDate DESC";
                    }
                    else
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp3 + " ORDER BY IncDate DESC";
                    }
                }
                else {
                    if (custOrEqp != "")
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp + " AND " + temp3 + " AND " + custOrEqp + " ORDER BY IncDate DESC";
                    }
                    else
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp + " AND " + temp3 + " ORDER BY IncDate DESC";
                    }
                }
            }
            else
            {
                if (temp == "")
                {
                    if (custOrEqp != "")
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp3 + " AND SwapID = 'JA' ORDER BY IncDate DESC";
                    }
                    else
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + " AND " + temp3 + " AND " + custOrEqp + " SwapID = 'JA' ORDER BY IncDate DESC";
                    }
                }
                else {
                    if (custOrEqp != "")
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + "AND" + temp + " AND " + temp3 + " AND " + custOrEqp + " AND SwapID = 'JA' ORDER BY IncDate DESC";
                    }
                    else
                    {
                        StartString += "'" + startDate + "'" + " AND " + "'" + endDate + "'" + "AND" + temp + " AND " + temp3 + " AND SwapID = 'JA' ORDER BY IncDate DESC";
                    }
                }
            }
            exportFilter(name, StartString);
        }

        private static string getMySQLActionString(List<string> actions, ref bool singleAction)
        {
            if (actions.Count == 0)
            {
                return "";
            }
            var retString = "(";
            var codeString = "LogActions.Action = ";
            if (actions.Count > 1)
            {
                for (int i = 0; i < actions.Count; i++)
                {
                    retString += codeString + '"' + actions[i] + '"' + " OR ";
                }
            }
            else
            {
                singleAction = true;
                retString = codeString + '"' + actions[0] + '"';
            }
            return retString;
        }

        private static string getMySQLCustomerstString(List<string> customers, ref bool singleCustomer)
        {
            if (customers.Count == 0)
            {
                return "";
            }
            var retString = "(";
            var codeString = "customers.CustomerName = ";
            if (customers.Count > 1)
            {
                for (int i = 0; i < customers.Count; i++)
                {
                    retString += codeString + '"' + customers[i] + '"' + " OR ";
                }
            }
            else
            {
                singleCustomer = true;
                retString = codeString + '"' + customers[0] + '"';
            }
            return retString;
        }

        private static string getMySQLEquipmentString(List<string> equipment, List<string> customers)
        {
            if (equipment.Count == 0)
            {
                return "";
            }
            var retString = "(";
            var codeString = "equipment.EquipmentName = ";
            if (equipment.Count > 1)
            {
                for (int i = 0; i < equipment.Count; i++)
                {
                    retString += codeString + '"' + equipment[i] + '"' + " OR ";
                }
              //  retString += " AND CustomerName = '" + customers[0] + "' ";
            }
            else
            {
                retString = codeString + '"' + equipment[0] + '"';
            }
            return retString;
        }

        private static string getMySQLUserString(List<string> users, ref bool singleUser)
        {
            if (users.Count == 0)
            {
                return "";
            }
            var retString = "(";
            var codeString = "users.UserName = ";
            if (users.Count > 1)
            {
                for (int i = 0; i < users.Count; i++)
                {
                    retString += codeString + '"' + users[i] + '"' + " OR ";
                }
            }
            else
            {
                singleUser = true;
                retString = codeString + '"' + users[0] + '"';
            }
            return retString;
        }

        public static void getCSVBetweenDates(string startDate, string endDate, string filename)
        {
            try
            {
                DataTable dt = new DataTable();

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

                    var fileName = @"C:\Swarco\0624\LogBookExport\Date\Export-Date-" + filename + ".csv";
                    // Open output stream
                    StreamWriter swFile = new StreamWriter(fileName, false, Encoding.UTF8);

                    // Header
                    string[] colLbls = new string[dt.Columns.Count];

                    for (int i = 0; i < dt.Columns.Count; i++)
                    {
                        colLbls[i] = dt.Columns[i].ColumnName;
                        colLbls[i] = GetWriteableValueForCsv(colLbls[i]);
                    }

                    // Write labels
                    swFile.WriteLine(string.Join(";", colLbls));

                    // Rows of Data
                    foreach (DataRow rowData in dt.Rows)
                    {
                        string[] colData = new string[dt.Columns.Count];

                        for (int i = 0; i < dt.Columns.Count; i++)
                        {
                            object obj = rowData[i];
                            colData[i] = GetWriteableValueForCsv(obj);
                        }

                        // Write data in row
                        swFile.WriteLine(string.Join(";", colData));
                    }

                    // Close output stream
                    swFile.Close();



                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    cn.Dispose(); // return connection to pool
                }


            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static string GetWriteableValueForCsv(object obj)
        {
            // Nullable types to blank
            if (obj == null || obj == Convert.DBNull)
                return "";

            // if string has no ','
            if (obj.ToString().IndexOf(",") == -1)
                return obj.ToString();

            // remove backslahes
            return "\"" + obj.ToString() + "\"";
        }

        public static List<MySQLHandler.Equipment> getEquipmentExport(string Customer)
        {
            List<MySQLHandler.Equipment> EquipmentList = new List<MySQLHandler.Equipment>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT EquipmentName FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = @cname;", connection))
                    {

                        cmd.Parameters.AddWithValue("@cname", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                MySQLHandler.Equipment eqp = new MySQLHandler.Equipment();
                                eqp.EquipmentName = reader.GetString("EquipmentName");

                                EquipmentList.Add(eqp);

                            }
                        }
                        return EquipmentList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return EquipmentList;
            }
        }
    }
}
