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



namespace Login
{
    public partial class LoginApp : Form
    {

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
                MessageBox.Show("Error de autentificacion  :(");
            }
        }

        private bool isCorrectLogin(Conexion con, HIDManagement hdi)
        {
            return con.trylogin(txtuser.Text, txtpass.Text, hdi);
        }

        private void LogIn()
        {
            MessageBox.Show("Entro!");
            this.playSimpleSound();
            UI.Main main = new UI.Main();
            this.Hide();
            main.ShowDialog();
            this.Close();
        }

        private void LoginApp_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            this.Close();
        }

        private void playSimpleSound()
        {
            SoundPlayer simpleSound = new SoundPlayer(@"G:\stonerbot\LoginUI\murloc sound.wav");
            simpleSound.Play();
        }

       
    }
}
