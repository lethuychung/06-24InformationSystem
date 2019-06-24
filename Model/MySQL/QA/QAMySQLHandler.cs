using GPLED.Logic;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace _06_24InformationSystem.Model.MySQL.QA
{
    class QAMySQLHandler : LogMaster
    {
        static string conString = XMLReader.getMySqlConnectionString();


        public static void updateAnswer(string title, string answer, string user)
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
                    
                    comm.CommandText = "UPDATE QA SET Answer = @answer, Answered = '1', AnsweredBy = @user WHERE QAID = @qaid;";

                    comm.Parameters.AddWithValue("@qaid", qaid);
                    comm.Parameters.AddWithValue("@answer", answer);
                    comm.Parameters.AddWithValue("@user", user);

                    comm.ExecuteNonQuery();

                    connection.Close();
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }

        public static void AskQuestion(string Question, string User, string Title, string Answer)
        {
            try
            {
                if (Answer == null)
                {
                    Answer = "Inget svar än";
                }
                string cid = string.Empty;
                string timeNow = DateTime.Now.ToString();
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();

                    MySqlCommand comm = connection.CreateCommand();
                    comm.CommandText = "INSERT INTO QA (Question, Answer, QTitle, AskingUser, TimeAsked) VALUES (@Q, @Answer, @QT, @User, @dateTime);";

                    comm.Parameters.AddWithValue("@Q", Question);
                    comm.Parameters.AddWithValue("@QT", Title);
                    comm.Parameters.AddWithValue("@User", User);
                    comm.Parameters.AddWithValue("@dateTime", timeNow);
                    comm.Parameters.AddWithValue("@Answer", Answer);

                    comm.ExecuteNonQuery();

                    connection.Close();

                }

            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
            }
        }


        public static List<string> getQuestionTitle()
        {
            List<string> QuestList = new List<string>();
            string temp = string.Empty;
            string temp2 = string.Empty;
            string answeredTemp = string.Empty;

            string unanswered = "-- NY -- ";

            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT QTitle, Answered FROM 0624system.QA;", connection))
                    {
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                QuestionsTitle QuestNfo = new QuestionsTitle();
                                temp = reader.GetString("QTitle");
                                answeredTemp = reader.GetString("Answered");
                                if (answeredTemp == "0")
                                {
                                    temp2 = unanswered + temp;
                                    QuestList.Add(temp2);
                                }
                                else {
                                    QuestList.Add(temp);
                                }
                            }
                        }
                        return QuestList;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return QuestList;
            }
        }


        public static string getQuestion(string title)
        {
            var tempQuest = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Question FROM 0624system.QA WHERE QTitle = @title;", connection))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                tempQuest = reader.GetString("Question");
                            }
                        }
                        return tempQuest;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }

        public static string getQuestionAnswer(string title)
        {
            var temp = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT Answer FROM 0624system.QA WHERE QTitle = @title;", connection))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                temp = reader.GetString("Answer");
                            }
                        }
                        return temp;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }

        public static string getQuestionUser(string title)
        {
            var temp = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT AskingUser FROM 0624system.QA WHERE QTitle = @title;", connection))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                temp = reader.GetString("AskingUser");
                            }
                        }
                        return temp;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }


        public static string getAnswerUser(string title)
        {
            var temp = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT AnsweredBy FROM QA WHERE QTitle = @title;", connection))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                temp = reader.GetString("AnsweredBy");
                            }
                        }
                        return temp;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }

        public static string getQuestionTime(string title)
        {
            var temp = string.Empty;
            try
            {
                using (MySqlConnection connection = new MySqlConnection(conString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = new MySqlCommand(@"SELECT TimeAsked FROM 0624system.QA WHERE QTitle = @title;", connection))
                    {
                        cmd.Parameters.AddWithValue("@title", title);
                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                temp = reader.GetString("TimeAsked");
                            }
                        }
                        return temp;
                    }
                }
            }
            catch (Exception e)
            {
                log.Error("Exception thrown from method " + GetCurrentMethod(), e);
                return "null";
            }
        }



        public class Questions
        {
            public string Question { get; set; }
            public string Answer { get; set; }
            public string TimeAsked { get; set; }
        }

        public class QuestionsTitle
        {
            public string Title { get; set; }
        }

    }
}
