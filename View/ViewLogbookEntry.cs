using System;
using System.Windows.Forms;

namespace _06_24InformationSystem.View
{
    public partial class ViewLogbookEntry : Form
    {
        public ViewLogbookEntry(string errorText, string commentText)
        {
            InitializeComponent();
            RenderText(errorText, commentText);
        }

        private void RenderText(string err, string comm)
        {
            entryErrorText.Text = err;
            entryCommentText.Text = comm;
        }

        private void closeBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
