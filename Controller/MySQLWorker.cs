using _06_24InformationSystem.Model;

namespace _06_24InformationSystem.Controller
{
    public class MySQLWorker
    {
        public static bool getUserLogin(string username, string pwd)
        {
            return MySQLHandler.getUserLogin(username, pwd);
        }

        public static string getCompanyMap(string customer)
        {
            return MySQLHandler.getCompanyMap(customer);
        }
    }
}
