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
    class HIDManagement
    {
        public string hdiFromDB;
        public string userFromDB;
        public string passwordFromDB;

        private void insertNewHDI()
        {
            //MySqlCommand cmd = new MySqlCommand("INSERT INTO users(User_Name,Password,Hardware_ID) VALUES(@username,@password,@hardwareid)");
            MySqlCommand cmd = new MySqlCommand("UPDATE users SET Hardware_ID=@hardwareid Where User_Name=@username and Password=@password");

            cmd.Parameters.Add("@username", MySqlDbType.VarChar, 32).Value = userFromDB;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar, 64).Value = passwordFromDB;
            cmd.Parameters.Add("@hardwareid", MySqlDbType.VarChar, 64).Value = this.generarHDILocal();
            
            MySqlConnection con = Conexion.connect();
            cmd.Connection = con;
            con.Open();
            cmd.ExecuteNonQuery();
        }

        private string cpuID()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select ProcessorId From Win32_processor");
            mbsList = mbs.Get();
            string id = "";
            foreach (ManagementObject mo in mbsList)
            {
                id = mo["ProcessorId"].ToString();

            }
            return id;
        }

        private string hddID()
        {
            ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""c:""");
            dsk.Get();
            return dsk["VolumeSerialNumber"].ToString();
        }

        private string motherID()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            string serial = "";
            foreach (ManagementObject mo in moc)
            {
                serial = (string)mo["SerialNumber"];
            }
            return serial;
        }

        public string generarHDILocal()
        { 
            return this.cpuID() + this.hddID() + this.motherID();
        }

        internal bool verificarHDIS()
        {
            if (String.Equals(this.hdiFromDB, this.generarHDILocal()))
            {
                return true;
            }
            else
            {
                if (String.Equals(this.hdiFromDB, "nuevo"))
                {
                    this.insertNewHDI();//Si es nuevo lo inserto en la base.
                    return true;//Devuelvo true para que entre la primera vez que se le genero y guardo la HDI.
                }
            }
            return false;
        }

        internal void setHDIFromDB(string hdi)
        {
            this.hdiFromDB = hdi;
        }
        internal void setUserNameFromDB(string user)
        {
            this.userFromDB = user;
        }

        internal void setPassword(string password)
        {
            this.passwordFromDB = password;
        }
    }
}
