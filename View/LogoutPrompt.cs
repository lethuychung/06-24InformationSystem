using _06_24InformationSystem.Model.MySQL.Logbook;
using System;
using System.Windows.Forms;

namespace _06_24InformationSystem.View
{
    public partial class LogoutPrompt : Form
    {

        string loggedUser = string.Empty;
        int missedCalls;
        string pass = string.Empty;

        public LogoutPrompt(int uncommentedLogs, string user)
        {
            loggedUser = user;
            InitializeComponent();
            missedCalls = uncommentedLogs;
            if (uncommentedLogs > 0)
            {
                label1.Text = "Antal ologgade samtal: " + uncommentedLogs;
            }
            else
            {
                label1.Text = "Alla samtal är loggade :)";
            }

            var currPass = recommendPass();
            if (currPass == "1")
            {
                PassComboBox.SelectedIndex = 0;
            }
            else if (currPass == "2")
            {
                PassComboBox.SelectedIndex = 1;
            }
            else if (currPass == "3")
            {
                PassComboBox.SelectedIndex = 2;
            }
        }

        private void logoutPromptButton_Click(object sender, EventArgs e)
        {
            DialogResult result1;
            if(logoutCommentTextBox.Text == "")
            {
                result1 = MessageBox.Show("Är du säker på att vill logga ut utan att ha skrivit en summering?", "Fråga", MessageBoxButtons.YesNo);
            }
            else
            {
                result1 = MessageBox.Show("Är du säker på att vill logga ut?", "Fråga", MessageBoxButtons.YesNo);
            }
            
            if (result1 == DialogResult.Yes)
            {
                MainView.stop = true;
                Model.Info.AuthLevel = 0;

                LoginForm Login = new LoginForm();

                this.Hide();

                var temp = System.Reflection.Assembly.GetEntryAssembly().Location;
                System.Diagnostics.Process.Start(temp);

                MainView.ActiveForm.Close();

                if (logoutCommentTextBox.Text == "")
                {
                    LOGMySQLHandler.addSummation(PassComboBox.Text, loggedUser, "Ingen summering skriven av användare.");
                }
                else
                {
                    LOGMySQLHandler.addSummation(PassComboBox.Text, loggedUser, logoutCommentTextBox.Text);
                }

                if(missedCalls >= 1)
                {
                    LOGMySQLHandler.addMissedCalls(loggedUser, missedCalls, pass);
                }
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private string recommendPass()
        {
            TimeSpan now = DateTime.Now.TimeOfDay;

            TimeSpan time06 = new TimeSpan(06, 00, 00);
            TimeSpan time1230 = new TimeSpan(12, 30, 00);

            TimeSpan time1231 = new TimeSpan(12, 31, 00);
            TimeSpan time1830 = new TimeSpan(18, 30, 00);

            TimeSpan time1831 = new TimeSpan(18, 31, 00);
            // time 24 not needed, just check if larger than time1831 and smaller than time06

            if (now >= time06 && now <= time1230)
            {
                pass = "06-12";
                return "1";
            }
            else if (now >= time1230 && now <= time1831)
            {
                pass = "12-18";
                return "2";
            }
            else if (now >= time1831 && now <= time06)
            {
                pass = "18-24";
                return "3";
            }
            return "0";
        }
    }
}
