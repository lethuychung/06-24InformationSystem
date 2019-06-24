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
    public partial class PreviewHTML : Form
    {
        public PreviewHTML()
        {
            InitializeComponent();
        }

        public PreviewHTML(string htmlText)
        {
            InitializeComponent();
            webBrowser1.DocumentText = htmlText;
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {

        }
    }
}
