using System;
using System.Xml;

namespace GPLED.Logic
{
    class XMLReader
    {
        // Author: Oscar Ternström

        #region log4net_declaration
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        #endregion

        /// <summary>
        /// gets the MySql connection string from settings.xml (this must be placed in the executable folder)
        /// </summary>
        /// <returns></returns>
        #region getMySqlConnectionString
        public static string getMySqlConnectionString()
        {
            try
            {
                var retString = "null";
                var path = AppDomain.CurrentDomain.BaseDirectory;
                using (XmlReader reader = XmlReader.Create(path + "settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "ConStringSetting":
                                    if (reader.Read())
                                    {
                                        //Console.WriteLine("  Text node: " + reader.Value.Trim());
                                        retString = reader.Value.Trim();
                                        if (retString == "")
                                        {
                                            log.Error("Returned NULL getMySqlConnectionString - XMLReader class");
                                            return "null";
                                        }
                                        return retString;
                                    }
                                    break;
                            }
                        }
                    }
                }
                log.Error("Returned NULL getMySqlConnectionString - XMLReader class");
                return "null";
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method getMySqlConnectionString - XMLReader class", e);
                return "null";
            }
        }
        #endregion


        public static string getICXIP()
        {
            try
            {
                var retString = "null";
                var path = AppDomain.CurrentDomain.BaseDirectory;
                using (XmlReader reader = XmlReader.Create(path + "settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "IcxServerIP":
                                    if (reader.Read())
                                    {
                                        //Console.WriteLine("  Text node: " + reader.Value.Trim());
                                        retString = reader.Value.Trim();
                                        if (retString == "")
                                        {
                                            log.Error("Returned NULL getICXIP - XMLReader class");
                                            return "null";
                                        }
                                        return retString;
                                    }
                                    break;
                            }
                        }
                    }
                }
                log.Error("Returned NULL getICXIP - XMLReader class");
                return "null";
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method getICXIP - XMLReader class", e);
                return "null";
            }
        }

        public static int getICXPort()
        {
            try
            {
                var retString = "null";
                var path = AppDomain.CurrentDomain.BaseDirectory;
                using (XmlReader reader = XmlReader.Create(path + "settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "IcxServerPort":
                                    if (reader.Read())
                                    {
                                        //Console.WriteLine("  Text node: " + reader.Value.Trim());
                                        retString = reader.Value.Trim();
                                        if (retString == "")
                                        {
                                            log.Error("Returned NULL IcxServerPort - XMLReader class");
                                            return -1;
                                        }
                                        var temp = Convert.ToInt32(retString);
                                        return temp;
                                    }
                                    break;
                            }
                        }
                    }
                }
                log.Error("Returned NULL IcxServerPort - XMLReader class");
                return -1;
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method IcxServerPort - XMLReader class", e);
                return -1;
            }
        }


        public static string getManualPath()
        {
            try
            {
                var retString = "null";
                var path = AppDomain.CurrentDomain.BaseDirectory;
                using (XmlReader reader = XmlReader.Create(path + "settings.xml"))
                {
                    while (reader.Read())
                    {
                        if (reader.IsStartElement())
                        {
                            switch (reader.Name)
                            {
                                case "ManualPath":
                                    if (reader.Read())
                                    {
                                        retString = reader.Value.Trim();
                                        if (retString == "")
                                        {
                                            log.Error("Returned NULL getManualPath - XMLReader class");
                                            return "null";
                                        }
                                        var temp = retString;
                                        return temp;
                                    }
                                    break;
                            }
                        }
                    }
                }
                log.Error("Returned NULL getManualPath - XMLReader class");
                return "null";
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method IcxServerPort - XMLReader class", e);
                return "null";
            }
        }

    }
}
