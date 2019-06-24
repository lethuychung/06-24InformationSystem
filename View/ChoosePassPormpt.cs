using System;
using System.Windows.Forms;

namespace _06_24InformationSystem.View
{
    public partial class ChoosePassPormpt : Form
    {

        string un = string.Empty;

        public ChoosePassPormpt(string username)
        {
            InitializeComponent();
            un = username;
        }

        private void savePassBtn_Click(object sender, EventArgs e)
        {
            if(pass1.Text == pass2.Text)
            {
                Model.MySQL.UserHandler.userController.updatePassword(un, pass1.Text);
                MessageBox.Show("Lösenordet är ändrat");
                this.Close();
            }
            else
            {
                MessageBox.Show("Dina lösenord stämmer inte överrens");
            }
        }
    }
}
