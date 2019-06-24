using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace _06_24InformationSystem.Model
{
    public class MySQLHandler
    {
        // read MySQL connection string from XML
        static string conString = XMLReader.getMySqlConnectionString();

        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string GetCurrentMethod()
        {
            StackTrace st = new StackTrace();
            StackFrame sf = st.GetFrame(1);

            return sf.GetMethod().Name;
        }

        // Regions

        #region CustomerList

        /// <summary>
        /// Creates a List of the class Customer, this is used for the combo boxes
        /// </summary>
        /// <returns></returns>
        public static List<Customer> getCustomers()
        {

            List<Customer> CustomerList = new List<Customer>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerName, CustomerID FROM customers;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Customer cust = new Customer();
                                cust.Name = reader.GetString("CustomerName");
                                cust.Value = reader.GetString("CustomerID");

                                CustomerList.Add(cust);
                            }
                        }
                        return CustomerList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return CustomerList;
            }
        }
        #endregion

        public static List<Users> getUsers()
        {

            List<Users> userList = new List<Users>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Username, Level FROM users;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Users user = new Users();
                                user.Name = reader.GetString("Username");
                                user.Value = reader.GetString("Level");

                                userList.Add(user);
                            }
                        }
                        return userList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return userList;
            }
        }


        #region Equipment

        public static string PriceOldInformation(string customer)
        {
            // SELECT InformationBLOB FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = "Linköping";
            var retVal = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(conString))
            {
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT CustPrices FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = @cname;", connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@cname", customer);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = reader.GetString(0); // Customer ID
                        }
                        return retVal;
                    }
                }
            }

        }

        public static string ContactPersonsOldInfo(string customer)
        {
            try
            {
                // SELECT InformationBLOB FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = "Linköping";
                var retVal = string.Empty;
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT CustContactPersons FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                retVal = reader.GetString(0); // Customer ID
                            }
                            return retVal;
                        }
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }


        public static string GeneralInfoOld(string customer)
        {
            // SELECT InformationBLOB FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = "Linköping";
            var retVal = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(conString))
            {
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT CustInfo FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = @cname;", connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@cname", customer);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = reader.GetString(0); // Customer ID
                        }
                        return retVal;
                    }
                }
            }

        }


        public static string EqpInfoOld(string customer, string eqpName)
        {
            // SELECT InformationBLOB FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE CustomerName = "Linköping";
            var retVal = string.Empty;
            using (MySqlConnection connection = new MySqlConnection(conString))
            {
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT InformationBLOB FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE EquipmentName = @eqpName AND CustomerName = @cname", connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@cname", customer);
                    cmd.Parameters.AddWithValue("@eqpName", eqpName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            retVal = reader.GetString(0); // Customer ID
                        }
                        return retVal;
                    }
                }
            }

        }



        /// <summary>
        /// Creates Equipment
        /// </summary>
        /// <param name="EqpName"></param>
        /// <param name="Customer"></param>
        /// <param name="entryText"></param>
        /// <param name="ICXString"></param>
        public static void createEquipment(string EqpName, string Customer, string entryText)
        {
            try
            {
                string cid = string.Empty;
                string maxInfoID = string.Empty;
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MAX(EquipmentID) FROM 0624system.equipment;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                try
                                {
                                    maxInfoID = reader.GetString(0);
                                }
                                catch (Exception e)
                                {
                                    log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                                    maxInfoID = "1";
                                }
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO equipment(CustomerID, EquipmentName, InformationID, InformationBLOB) VALUES(@cid, @eqpName2, @maxInfoID, @blob);";
                    var temp = getMaxEquipmentID();
                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@eqpName2", EqpName);
                    comm.Parameters.AddWithValue("@maxInfoID", temp);
                    comm.Parameters.AddWithValue("@blob", entryText);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void AddICXString(string Eqp, string Customer, string ICX)
        {
            try
            {
                string cid = string.Empty;
                string maxInfoID = string.Empty;
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MAX(EquipmentID) FROM 0624system.equipment;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                try
                                {
                                    maxInfoID = reader.GetString(0);
                                }
                                catch (Exception e)
                                {
                                    log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                                    maxInfoID = "1";
                                }
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();
                    //      UPDATE equipment SET ICXString = "1324" WHERE EquipmentID = 1 AND EquipmentName = "ee";
                    // comm.CommandText = "INSERT INTO equipment(CustomerID, EquipmentName, InformationID, ICXString) VALUES(@cid, @eqpName2, @maxInfoID, @ICXString);";
                    comm.CommandText = "UPDATE equipment SET ICXString = @ICXString WHERE CustomerID = @cid AND EquipmentName = @eqpName2;";
                    var temp = getMaxEquipmentID();
                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@eqpName2", Eqp);
                    comm.Parameters.AddWithValue("@ICXString", ICX);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void AddPriceInformation(string Customer, string PriceInfo)
        {
            try
            {
                string cid = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE customers SET CustPrices = @PriceNfo WHERE CustomerID = @cid;";

                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@PriceNfo", PriceInfo);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void AddCustContactInfo(string Customer, string ContactInfo)
        {
            try
            {
                string cid = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE customers SET CustContactPersons = @ContactInfo WHERE CustomerID = @cid;";

                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@ContactInfo", ContactInfo);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static void AddGeneralInfo(string Customer, string ContactInfo)
        {
            try
            {
                string cid = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE customers SET CustInfo = @ContactInfo WHERE CustomerID = @cid;";

                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@ContactInfo", ContactInfo);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static void ChangeCustName(string Customer, string newName)
        {
            string cid = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(conString))
            {
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM customers WHERE CustomerName = @cname;", connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@cname", Customer);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            cid = reader.GetString(0); // Customer ID
                        }
                    }
                }
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {

                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE customers SET CustomerName = @newName WHERE CustomerID = @cid;";

                    comm.Parameters.AddWithValue("@newName", newName);
                    comm.Parameters.AddWithValue("@cid", cid);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static void ChangeEqpInfo(string Customer, string eqpName, string newInfo)
        {
            string eqpID = string.Empty;

            using (MySqlConnection connection = new MySqlConnection(conString))
            {
                using (MySqlCommand cmd = new MySqlCommand(@"SELECT EquipmentID FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE EquipmentName = @eqpName AND CustomerName = @cname;", connection))
                {
                    connection.Open();
                    cmd.Parameters.AddWithValue("@cname", Customer);
                    cmd.Parameters.AddWithValue("@eqpName", eqpName);

                    using (MySqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            eqpID = reader.GetString(0); // Customer ID
                        }
                    }
                }
            }
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {

                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE equipment JOIN customers ON equipment.CustomerID = customers.CustomerID SET equipment.InformationBLOB = @info  WHERE EquipmentName = @eqpName AND CustomerName = @cname AND equipment.EquipmentID = @eqpID";

                    comm.Parameters.AddWithValue("@eqpID", eqpID);
                    comm.Parameters.AddWithValue("@cname", Customer);
                    comm.Parameters.AddWithValue("@eqpName", eqpName);
                    comm.Parameters.AddWithValue("@info", newInfo);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        /// <summary>
        /// Deletes selected Equipment
        /// </summary>
        /// <param name="Equipment"></param>
        /// <param name="Customer"></param>
        public static void deleteEquipment(string Equipment, string Customer)
        {
            try
            {
                var custID = getCustID(Customer);

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "DELETE FROM equipment WHERE EquipmentName = @EqpName AND CustomerID = @custID;";

                    comm.Parameters.AddWithValue("@EqpName", Equipment);
                    comm.Parameters.AddWithValue("@custID", custID);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static string getSpecificMap(string mapName)
        {
            var MapInfo = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT DISTINCT MapPath FROM customers INNER JOIN maps on maps.CustomerID = customers.CustomerID WHERE MapName = @mapName", connection))
                    {
                        cmd.Parameters.AddWithValue("@mapName", mapName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                MapInfo = reader.GetString("MapPath");
                            }
                        }
                        return MapInfo;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return MapInfo;
            }


        }

        public static List<Maps> getMaps(string Customer)
        {
            List<Maps> MapList = new List<Maps>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT maps.MapID, maps.MapName FROM customers INNER JOIN maps ON customers.CustomerID = maps.CustomerID WHERE CustomerName = @cname;", connection))
                    {
                        cmd.Parameters.AddWithValue("@cname", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Maps maps = new Maps();
                                maps.Value = reader.GetString("MapID");
                                maps.Name = reader.GetString("MapName");

                                MapList.Add(maps);
                            }
                        }
                        return MapList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return MapList;
            }
        }

        /// <summary>
        /// Gets a list of Equipments sorted by customer
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public static List<Equipment> getEquipment(string Customer)
        {
            List<Equipment> EquipmentList = new List<Equipment>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT customers.CustomerName, equipment.EquipmentID, equipment.EquipmentName, equipment.InformationBLOB FROM customers INNER JOIN equipment ON customers.CustomerID = equipment.CustomerID WHERE CustomerName = @cname;", connection))
                    {

                        cmd.Parameters.AddWithValue("@cname", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Equipment eqp = new Equipment();
                                eqp.EquipmentName = reader.GetString("EquipmentName");
                                eqp.InformationBLOB = reader.GetString("InformationBLOB");
                                eqp.EquipmentID = reader.GetString("EquipmentID");

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




        /// <summary>
        /// Gets the Equipment information from DB Blob and present it to the user
        /// </summary>
        /// <param name="EqpName"></param>
        /// <returns></returns>
        public static string getEquipmentBlob(string EqpName, string CustName)
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT equipment.InformationBLOB FROM customers INNER JOIN equipment ON customers.CustomerID = equipment.CustomerID WHERE EquipmentName = @eqpName AND CustomerName = @cname;", connection))
                    {
                        cmd.Parameters.AddWithValue("@eqpName", EqpName);
                        cmd.Parameters.AddWithValue("@cname", CustName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("InformationBLOB");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }

        #endregion

        public static void resetPassword(string username)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();
                    var level = getUserLevel(username);
                    comm.CommandText = "UPDATE users SET Pwd='', Level = @level WHERE Username = @un;";

                    comm.Parameters.AddWithValue("@level", level);
                    comm.Parameters.AddWithValue("@un", username);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static string getUserLevel(string userName)
        {
            var level = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Level FROM users WHERE Username = @user;", connection))
                    {
                        cmd.Parameters.AddWithValue("@user", userName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                level = reader.GetString("Level");
                            }
                        }
                        return level;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return level;
            }
        }
        
        public static void deleteBugReport(string ID)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "DELETE FROM bugreport WHERE BugID = @ID;";

                    comm.Parameters.AddWithValue("@ID", ID);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void deleteCommonUser(string Name)
        {
            var userID = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT userID FROM users_for_logging WHERE UserName = @Name;", connection))
                    {
                        cmd.Parameters.AddWithValue("@Name", Name);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                userID = reader.GetString("userID");
                            }
                        }

                    }
                }

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "DELETE FROM users_for_logging WHERE UserID = @ID;";

                    comm.Parameters.AddWithValue("@ID", userID);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static List<BugInfo> getBugInfo(string ID)
        {
            List<BugInfo> BugList = new List<BugInfo>();
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT DateTime, UserName, BugDesc FROM bugreport WHERE BugID = @ID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@ID", ID);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                BugInfo bugNfo = new BugInfo();
                                bugNfo.Desc = reader.GetString("BugDesc");
                                bugNfo.DateTime = reader.GetString("DateTime");
                                bugNfo.UserName = reader.GetString("UserName");
                                BugList.Add(bugNfo);
                            }
                        }
                        return BugList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return BugList;
            }
        }


        public static List<Bugs> getBugReports()
        {
            List<Bugs> BugList = new List<Bugs>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT BugID FROM bugreport;", connection))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Bugs bugz = new Bugs();
                                bugz.Value = reader.GetString("BugID");

                                BugList.Add(bugz);

                            }


                        }
                        return BugList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return BugList;
            }
        }


        public static List<Actions> getActions()
        {
            List<Actions> ActionList = new List<Actions>();

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT ActionID, Action FROM LogActions;", connection))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                Actions ac = new Actions();
                                ac.Name = reader.GetString("Action");
                                ac.Value = reader.GetString("ActionID");

                                ActionList.Add(ac);

                            }


                        }
                        return ActionList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return ActionList;
            }
        }


        public static void deleteMap(string Customer, string mapName)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    string mapID = string.Empty;
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MapID FROM maps INNER JOIN customers ON customers.CustomerID = maps.CustomerID WHERE CustomerName = @cname AND MapName = @mapName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@mapName", mapName);
                        cmd.Parameters.AddWithValue("@cname", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                mapID = reader.GetString("MapID");
                            }
                        }

                    }

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "DELETE maps FROM maps INNER JOIN customers ON customers.CustomerID = maps.CustomerID WHERE MapName = @mapName AND CustomerName = @cname AND MapID = @mapID;";

                    comm.Parameters.AddWithValue("@cname", Customer);
                    comm.Parameters.AddWithValue("@mapName", mapName);
                    comm.Parameters.AddWithValue("@mapID", mapID);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        #region Customers

        /// <summary>
        /// Deletes selected Customer
        /// </summary>
        /// <param name="Customer"></param>
        public static void deleteCustomer(string Customer)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "DELETE FROM customers WHERE CustomerName = @custName;";

                    comm.Parameters.AddWithValue("@custName", Customer);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        /// <summary>
        /// Gets information about selected Customer
        /// </summary>
        /// <param name="Customer"></param>
        /// <returns></returns>
        public static string getCustomerInfo(string Customer)
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustInfo FROM 0624system.customers WHERE CustomerName = @CustName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustName", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("CustInfo");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }


        public static string getManual()
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT manualBlob FROM Manual;", connection))
                    {

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("manualBlob");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }


        public static string getCustPriceInfo(string Customer)
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustPrices FROM 0624system.customers WHERE CustomerName = @CustName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustName", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("CustPrices");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }
        
        public static void ExportDB()
        {
            var InfoBlob = string.Empty;
            try
            {
                string shortDate = DateTime.Now.ToShortDateString();
                string file = @"C:\Swarco\0624\Export\0624DB-" + shortDate + ".sql";
                using (MySqlConnection conn = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        using (MySqlBackup mb = new MySqlBackup(cmd))
                        {
                            cmd.Connection = conn;
                            conn.Open();
                            mb.ExportToFile(file);
                            conn.Close();
                        }
                    }
                }

            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static string getCustSwapInfo(string Customer)
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustSwapAccessInfo FROM 0624system.customers WHERE CustomerName = @CustName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustName", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("CustSwapAccessInfo");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }


        public static void UpdateSwapInfo(string Customer, string text)
        {
            try
            {
                string cid = string.Empty;

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @cname;", connection))
                    {
                        connection.Open();
                        cmd.Parameters.AddWithValue("@cname", Customer);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                cid = reader.GetString(0); // Customer ID
                            }
                        }
                    }

                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "UPDATE customers SET CustSwapAccessInfo = @info WHERE CustomerID = @cid;";

                    comm.Parameters.AddWithValue("@cid", cid);
                    comm.Parameters.AddWithValue("@info", text);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static string getCustContactInfo(string Customer)
        {
            var InfoBlob = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustContactPersons FROM 0624system.customers WHERE CustomerName = @CustName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustName", Customer);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                InfoBlob = reader.GetString("CustContactPersons");
                            }
                        }
                        return InfoBlob;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return InfoBlob;
            }
        }

        /// <summary>
        /// Creates Customer
        /// </summary>
        /// <param name="CustName"></param>
        /// <param name="CustInfo"></param>
        public static void createCustomer(string CustName, string CustInfo)
        {
            int? CustID;
            try
            {
                CustID = Convert.ToInt32(getCustID(CustName));
            }
            catch (Exception e)
            {
                CustID = null;
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
            try
            {
                if (CustID != null)
                {
                    using (MySqlConnection connection = new MySqlConnection(conString))
                    {
                        connection.Open();
                        MySqlCommand comm = connection.CreateCommand();

                        comm.CommandText = "INSERT INTO customers (CustomerID, CustomerName, CustInfo) VALUES (@custID, @custName, @custInfo) ON DUPLICATE KEY UPDATE CustomerID = VALUES(CustomerID), CustomerName = VALUES(CustomerName), CustInfo = @custInfo;";

                        comm.Parameters.AddWithValue("@custName", CustName);
                        comm.Parameters.AddWithValue("@custInfo", CustInfo);
                        comm.Parameters.AddWithValue("@custID", CustID);

                        comm.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                else
                {
                    using (MySqlConnection connection = new MySqlConnection(conString))
                    {
                        connection.Open();
                        MySqlCommand comm = connection.CreateCommand();

                        comm.CommandText = "INSERT INTO customers (CustomerName, CustInfo) VALUES (@custName, @custInfo) ON DUPLICATE KEY UPDATE CustomerName = VALUES(CustomerName), CustInfo = @custInfo;";

                        comm.Parameters.AddWithValue("@custName", CustName);
                        comm.Parameters.AddWithValue("@custInfo", CustInfo);

                        comm.ExecuteNonQuery();
                        connection.Close();
                    }

                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        #endregion

        #region Misc

        /// <summary>
        /// Gets the max ID from Customer table in DB
        /// </summary>
        /// <param name="CustomerName"></param>
        /// <returns></returns>
        public static string getCustID(string CustomerName)
        {
            var CustID = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT CustomerID FROM 0624system.customers WHERE CustomerName = @CustName;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustName", CustomerName);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                CustID = reader.GetString("CustomerID");
                            }
                        }
                        return CustID;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return null;
            }
        }

        /// <summary>
        /// Gets the CustomerID from specified customer name
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Gets the Max ID from Equipment table
        /// </summary>
        /// <returns></returns>
        private static int getMaxEquipmentID()
        {
            string cid = string.Empty;
            string maxInfoID = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MAX(EquipmentID) FROM 0624system.equipment;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                maxInfoID = reader.GetString(0);
                                var temp = Convert.ToInt32(maxInfoID);
                                if (temp == 0)
                                {
                                    return 1;
                                }
                                else
                                {
                                    return temp + 1;
                                }
                            }

                        }
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return 1;
            }
        }

        /// <summary>
        /// Gets the Max ID from the maps table in DB
        /// </summary>
        /// <returns></returns>
        private static int getMaxMapID()
        {
            string cid = string.Empty;
            string maxInfoID = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MAX(MapID) FROM 0624system.maps;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {

                                maxInfoID = reader.GetString(0);
                                var temp = Convert.ToInt32(maxInfoID);
                                if (temp == 0)
                                {
                                    return 1;
                                }
                                else
                                {
                                    return temp + 1;
                                }

                            }
                            return 1;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return 1;
            }

        }

        #endregion


        #region BugReports

        /// <summary>
        /// Sends bug report to DB
        /// </summary>
        /// <param name="name"></param>
        /// <param name="bug"></param>
        public static void AddBugReport(string name, string bug)
        {
            try
            {
                DateTime now = DateTime.Now;
                var DateNow = now.ToString();

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO bugreport (DateTime, UserName, BugDesc) VALUES(@DateTime, @Name, @Bug);";

                    comm.Parameters.AddWithValue("@DateTime", DateNow);
                    comm.Parameters.AddWithValue("@Name", name);
                    comm.Parameters.AddWithValue("@Bug", bug);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }
        #endregion

        public static List<ICXInfo> getICXInfo(string ICXString)
        {
            List<ICXInfo> _ICXNfo = new List<ICXInfo>();


            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT EquipmentName, CustomerName FROM equipment INNER JOIN customers ON equipment.CustomerID = customers.CustomerID WHERE ICXString = @icxString", connection))
                    {

                        cmd.Parameters.AddWithValue("@icxString", ICXString);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {

                            while (reader.Read())
                            {
                                ICXInfo icxInfo = new ICXInfo();
                                icxInfo.EquipmentName = reader.GetString("EquipmentName");
                                icxInfo.CustomerName = reader.GetString("CustomerName");

                                _ICXNfo.Add(icxInfo);
                            }
                        }
                        return _ICXNfo;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return _ICXNfo;
            }

        }

        #region Maps

        /// <summary>
        /// Adds Customer map, PNG -> Base64 -> DB
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="customer"></param>
        /// <param name="DBPath"></param>
        public static void AddCustomerMapBase64(string filePath, string customer, string DBPath, string mapName)
        {
            try
            {
                //   deleteCompanyMap(customer);
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    var custID = getCustomerID(customer);

                    comm.CommandText = "INSERT INTO maps (MapPath, CustomerID, MapName) VALUES(@MapPath, @CustomerID, @mapName);";

                    comm.Parameters.AddWithValue("@MapPath", DBPath);
                    comm.Parameters.AddWithValue("@CustomerID", custID);
                    comm.Parameters.AddWithValue("@mapName", mapName);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }




        public static void AddManual(string text)
        {
            try
            {
                //   deleteCompanyMap(customer);
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "UPDATE Manual SET manualBlob = @text WHERE manualID = '1';";

                    comm.Parameters.AddWithValue("@text", text);

                    comm.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        /// <summary>
        /// Gets the map for the selected Customer
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        public static string getCompanyMap(string customer)
        {
            try
            {
                int CustID;
                CustID = getCustomerID(customer);

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MapPath FROM 0624system.maps WHERE CustomerID = @CustomerID;", connection))
                    {
                        cmd.Parameters.AddWithValue("@CustomerID", CustID);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return reader.GetString(0); // Customer ID

                            }
                            return "null";
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

        /// <summary>
        /// Deletes company map specified by the CustomerName
        /// </summary>
        /// <param name="customer"></param>
        public static void deleteCompanyMap(string customer)
        {
            try
            {
                int CustID;
                CustID = getCustomerID(customer);

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT MapPath FROM 0624system.maps WHERE CustomerID = @CustomerID;", connection))
                    {

                        cmd.CommandText = "DELETE FROM maps WHERE CustomerID = @CustID;";

                        cmd.Parameters.AddWithValue("@CustID", CustID);

                        cmd.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }

        }
        #endregion

        #region CreateUser

        /// <summary>
        /// Creates user with 
        /// , password and auth level
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <param name="Level"></param>
        /// <returns></returns>
        public static bool CreateUser(string Username, string Level)
        {
            try
            {
                //var hash = PasswordEncryption.GeneratePasswordHash(Password);

                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();

                    comm.CommandText = "INSERT INTO users (Username, Level, ReadNews) VALUES (@UN, @LVL, '0');";

                    comm.Parameters.AddWithValue("@UN", Username);
                  //  comm.Parameters.AddWithValue("@PWD", hash);
                    comm.Parameters.AddWithValue("@LVL", Level);

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
        #endregion

        #region LoginSystem

        /// <summary>
        /// Gets the user info from DB and checks if the login and password is legit, return false if not legit, else true
        /// </summary>
        /// <param name="username"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static bool getUserLogin(string username, string pwd)
        {
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Pwd, Level FROM users WHERE Username =@uname;", connection))
                    {

                        cmd.Parameters.AddWithValue("@uname", username);

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            string MySQLpass = string.Empty;
                            string Level = string.Empty;

                            if (reader.Read())
                            {
                                MySQLpass = reader.GetString(0);
                                Level = reader.GetInt32(1).ToString();
                            }
                            else
                                MySQLpass = "not found";

                            if (PasswordEncryption.VerifyHashPassword(pwd, MySQLpass))
                            {
                                Info.AuthLevel = Convert.ToInt32(Level);
                                return true;
                            }

                            else
                            {
                                return false;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return false;
            }
        }
        #endregion

        #region CustomerAndEquipmentClasses

        public class Customer
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class Users
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class CommonUser
        {
            public string Name { get; set; }
        }

        public class Equipment
        {
            public string EquipmentName { get; set; }
            public string EquipmentID { get; set; }
            public string InformationBLOB { get; set; }
        }
        #endregion

        public class Bugs
        {
            public string Value { get; set; }
        }

        public class Actions
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class BugInfo
        {
            public string DateTime { get; set; }
            public string UserName { get; set; }
            public string Desc { get; set; }
        }

        public class Maps
        {
            public string Name { get; set; }
            public string Value { get; set; }
        }

        public class ICXInfo
        {
            public string CustomerName { get; set; }
            public string EquipmentName { get; set; }
        }

        // End Regions
    }
}
