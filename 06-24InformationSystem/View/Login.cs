using System;
using System.Windows.Forms;
using _06_24InformationSystem.Controller;
using _06_24InformationSystem.Model.MySQL.UserHandler;
using _06_24InformationSystem.Model.MySQL.News;
using _06_24InformationSystem.Model;

namespace _06_24InformationSystem
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Login button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LoginBtn_Click(object sender, EventArgs e)
        {
            if (checkIfPassWordExists(usernameTextBox.Text) == true)
            {
                int value;
                if (int.TryParse(IntercomID.Text, out value))
                {
                    var userName = usernameTextBox.Text;
                    var password = passwordTextBox.Text;
                    var intercom = IntercomID.Text;
                    if (checkLogin(userName, password, intercom) == true) // Open new form
                    {
                        var level = Convert.ToInt32(MySQLHandler.getUserLevel(userName));

                        var interConvert = Convert.ToInt32(intercom);
                        MainView main = new MainView(level, interConvert, userName);
                        this.Hide();
                        main.Closed += (s, args) => this.Close();
                        main.Show();
                        if (!NewsMySQLHandler.checkIfDisplayNews(userName))
                        {
                            NewsForm news = new NewsForm(userName);
                            news.Show(); // disp news popup
                        }
                    }
                    else
                    {
                        errorLabel.Visible = true;
                    }
                }
                else
                {
                    errorLabel2.Visible = true;
                }
            }
            else
            {
                if (userController.checkIfUserExists(usernameTextBox.Text))
                {
                    View.ChoosePassPormpt cpp = new View.ChoosePassPormpt(usernameTextBox.Text);
                    cpp.Show();
                }
                else
                {
                    MessageBox.Show("Användaren finns inte");
                }
            }

        }

        private void showWarning(string text)
        {
            MessageBox.Show(text, "Varning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button1);
        }

        private bool checkIfPassWordExists(string username)
        {
            return userController.checkIfPasswordExists(username);
        }

        /// <summary>
        /// Checks username and password against MySQL database
        /// </summary>
        /// <param name="Username"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        private bool checkLogin(string Username, string Password, string intercom)
        {
            if (Username == "" || Password == "" || intercom == "") { showWarning("Du måste fylla i alla fält."); return false; }
            return MySQLWorker.getUserLogin(Username, Password);
        }
    }
}