using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Media;
using System.Management;
using System.IO;


namespace Login
{
    public partial class LoginApp : Form
    {
       

        public string rootPath;

        public LoginApp()
        {
            InitializeComponent();
            
        }

        private void button1_Click(object sender, EventArgs e)
        {

            Conexion con = new Conexion();
            HIDManagement hdi = new HIDManagement();
            if (isCorrectLogin(con, hdi))
            {
                LogIn();
            }
            else
            {
                MessageBox.Show("Error de autentificacion.");
            }
        }

        private bool isCorrectLogin(Conexion con, HIDManagement hdi)
        {
            return con.trylogin(txtuser.Text, txtpass.Text, hdi);
        }

        private void LogIn()
        {
            //this.playSimpleSound();
            MurlocBot.Main main = new MurlocBot.Main();
            this.Hide();
            main.ShowDialog();
            this.Close();
        }

        private void LoginApp_Load(object sender, EventArgs e)
        {
            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("MurlocBot");
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("MurlocBot");
                key.SetValue("RootPath", rootPath);
                key.Close();
            }
            else
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("MurlocBot");
                rootPath = (string)key.GetValue("RootPath");
                key.Close();
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playSimpleSound()
        {
            string soundPath = Path.Combine(rootPath, "sound1.wav");
            if (File.Exists(soundPath))
            {
                SoundPlayer simpleSound = new SoundPlayer(soundPath);
                simpleSound.Play();
            }
        }       
    }
}
