using _06_24InformationSystem.Model.MySQL.News;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace _06_24InformationSystem
{
    public partial class NewsForm : Form
    {

        string userName = string.Empty;

        public NewsForm(string user)
        {
            InitializeComponent();
            getNews();
            userName = user;
        }

        private void getNews()
        {
            try
            {
                var InfoBlob = NewsMySQLHandler.getNews();

                Font fon = new Font("Palatino Linotype", 12.0f);
                
                newsBrowser.Font = fon;
                newsBrowser.Navigate("about:blank");
                newsBrowser.Document.Encoding = "utf-8";
                HtmlDocument doc2 = newsBrowser.Document;
                doc2.Write(String.Empty);

                newsBrowser.DocumentText = InfoBlob;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }


        }

        private void confirmReadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if(confirmReadCheckBox.Checked == true)
            {
                closeNewsPromptBtn.Enabled = true;
            }
            else
            {
                closeNewsPromptBtn.Enabled = false;
            }
        }

        private void closeNewsPromptBtn_Click(object sender, EventArgs e)
        {
            NewsMySQLHandler.updateReadNews(userName);
            this.Close();
        }
    }
}
