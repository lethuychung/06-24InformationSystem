using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace _06_24InformationSystem
{
    public partial class MapForm : Form
    {
        public MapForm(string base64Map)
        {
            InitializeComponent();
            this.WindowState = FormWindowState.Maximized;
            showMap(base64Map);
        }

        private void showMap(string MapData)
        {
            Image img;
            try
            {
               // var tempimg = MySQLWorker.getCompanyMap(MapCustomerComboBox.Text);
                byte[] imageBytes = Convert.FromBase64String(MapData);

                MemoryStream ms = new MemoryStream(imageBytes);

                img = Image.FromStream(ms, true, true);
                MapPictureBox.Image = img;
            }
            catch (Exception)
            {
                // ignored
                MapPictureBox.Image = null;
            }
        }

        private void MapForm_Load(object sender, EventArgs e)
        {
            
        }
    }
}
