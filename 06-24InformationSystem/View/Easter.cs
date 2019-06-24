using System;
using System.Windows.Forms;

namespace _06_24InformationSystem.View
{
    public partial class Easter : Form
    {
        public Easter()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 5; i++)
            {
                Easter east = new Easter();
                east.Show();
            }
        }
    }
}
