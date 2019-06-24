using System;
using System.Windows.Forms;

namespace _06_24InformationSystem.View
{
    public partial class viewCommentEntry : Form
    {
        public viewCommentEntry(string passComment)
        {
            InitializeComponent();
            renderText(passComment);
        }

        private void renderText(string comment)
        {
            viewCommentBox.Text = comment;
        }

        private void closeCommentBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
