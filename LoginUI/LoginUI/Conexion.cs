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

namespace Login
{
    class Conexion:HIDManagement
    {

        protected internal bool trylogin(string username, string password, HIDManagement hdi)
        {
            MySqlConnection con = connect();

            //Old
            //MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE User_Name = '" + username + "' AND Password = '" + password + "';");

            //New con transformación de parametros a literal, evitando la inyección de sql.
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM users WHERE User_Name =@username AND Password=@password");

            cmd.Parameters.Add("@username", MySqlDbType.VarChar, 32).Value = username;
            cmd.Parameters.Add("@password", MySqlDbType.VarChar, 64).Value = password;
      
            cmd.Connection = con;
            con.Open();

            MySqlDataReader reader = cmd.ExecuteReader();

            if (reader.Read() != false)
            {
                if (reader.IsDBNull(0) == true)
                {
                    cmd.Connection.Close();
                    reader.Dispose();
                    cmd.Dispose();
                    return false;
                }
                else
                {
                    hdi.setHDIFromDB(reader["Hardware_ID"].ToString());
                    hdi.setUserNameFromDB(reader["User_Name"].ToString());
                    hdi.setPassword(reader["Password"].ToString());
                    cmd.Connection.Close();
                    reader.Dispose();
                    cmd.Dispose();

                    if (hdi.verificarHDIS())
                    {                    
                        return true;
                    }
                    else
                    {            
                        return false;
                    }
             

                }

            }
            else
            {
                return false;
            }

        }

        public static MySqlConnection connect()
        {
            MySqlConnection con = new MySqlConnection("host=db4free.net;user=stoner;password=pepito;database=bdbot;");
            return con;
        }
    }
}
