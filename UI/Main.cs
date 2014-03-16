using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows;


namespace UI
{
    public partial class Main : Form
    {
        public string HSpath;
        private string extPath;
        public string rootPath;

        #region -[ Eventos ]-

        public Main()
        {
            InitializeComponent();
            this.extPath = "ext";
            string exePath = Assembly.GetExecutingAssembly().CodeBase;
            rootPath = exePath.Substring(0, exePath.LastIndexOf("/") + 1).Replace("file:///", "");
            if (rootPath.Contains("bin/Debug"))
                rootPath = rootPath.Replace("/UI/bin/Debug", "");
            if (File.Exists(Path.Combine(rootPath, "Config.cfg")))
            {
                HSpath = File.ReadAllText(Path.Combine(rootPath, "Config.cfg"));
                if (HSpath != "")
                {
                    btnInject.Enabled = true;
                    lblPath.Text = HSpath;
                    return;
                }
            }
            btnInject.Enabled = false;
        }

        private void btnStartBot_Click(object sender, EventArgs e)
        {
            SendConnectCmd("stbot");
        }

        private void btnStartBotRanked_Click(object sender, EventArgs e)
        {
            SendConnectCmd("stran");
        }

        private void btnStartBotvsAI_Click(object sender, EventArgs e)
        {
            SendConnectCmd("stain");
        }

        private void btnStartBotvsAIExpert_Click(object sender, EventArgs e)
        {
            SendConnectCmd("staie");
        }

        private void btnStopBot_Click(object sender, EventArgs e)
        {
            SendConnectCmd("stopb");
        }

        private void btnStopBotAfterThis_Click(object sender, EventArgs e)
        {
            SendConnectCmd("stopa");
        }

        private void btnSay_Click(object sender, EventArgs e)
        {
            SendConnectCmd("saywo" + txtSay.Text.Length.ToString() + txtSay.Text);
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            Inject();
        }

        #endregion

        private void Inject()
        {
            if (!this.HSpath.Equals(""))
            {
                string injPath = Path.Combine(rootPath + extPath + "/Injector.exe");
                if (File.Exists(injPath))
                {
                    try
                    {
                        string destAssemblyPath = Path.Combine(this.HSpath, "Hearthstone_Data\\Managed\\Assembly-CSharp.dll");
                        string destAssemblyUnity = Path.Combine(this.HSpath, "Hearthstone_Data\\Managed\\UnityEngine.dll");
                        string destMonoCecil = Path.Combine(this.HSpath, "Hearthstone_Data\\Managed\\Mono.Cecil.dll");
                        string destHearthstone = Path.Combine(this.HSpath, "Hearthstone_Data\\Managed\\Hearthstone.dll");
                        string assemblyMonoCecil = Path.Combine(rootPath, extPath, "Mono.Cecil.dll");
                        string assemblyUnityExt = Path.Combine(rootPath, extPath, "UnityEngine.dll");
                        string assemblyCSharpExtOrig = Path.Combine(rootPath, extPath, "Assembly-CSharp.orig.dll");
                        string assemblyCSharpExtPatched = Path.Combine(rootPath, extPath, "Assembly-CSharp.dll");
                        string assemblyHearthstoneExt = Path.Combine(rootPath, extPath, "Hearthstone.dll");

                        if (File.Exists(destAssemblyPath) && File.Exists(destAssemblyUnity))
                        {
                            if (!File.Exists(assemblyCSharpExtOrig) || !File.Exists(assemblyUnityExt))
                            {
                                this.lblStatus.Text = "Copying files...";
                                File.Copy(destAssemblyPath, assemblyCSharpExtOrig, true);
                                File.Copy(destAssemblyUnity, assemblyUnityExt, true);
                            }
                            Process process = new Process();
                            process.StartInfo.FileName = injPath;
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.CreateNoWindow = true;
                            process.StartInfo.WorkingDirectory = Path.Combine(rootPath + extPath + "/");
                            process.Start();
                            this.lblStatus.Text = "Injecting...";
                            process.WaitForExit();
                            if (process.ExitCode == 0)
                            {
                                File.Copy(assemblyHearthstoneExt, destHearthstone, true);
                                File.Copy(assemblyCSharpExtPatched, destAssemblyPath, true);
                                File.Copy(assemblyMonoCecil, destMonoCecil, true);
                                File.WriteAllText(Path.Combine(this.HSpath + "plugins.txt"), Path.Combine(rootPath, "plugins"));
                                File.WriteAllText(Path.Combine(this.HSpath + "root.txt"), rootPath);
                                this.lblStatus.Text = "Injection done";
                            }
                            else
                                this.lblStatus.Text = "Error " + (object)process.ExitCode + " , check log file...";
                        }
                        else
                        {
                            int num = (int)MessageBox.Show("Impossible to locate some game DLLS..." + Environment.NewLine + "Please select right Hearthstone path in Edit menu." + Environment.NewLine
                                + "The selected folder MUST contains both 'Data' and 'Hearthstone_Data' folders..." + Environment.NewLine + this.HSpath + "/Hearthstone_Data/Managed/Assembly-CSharp.dll", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                        }
                    }
                    catch (IOException ex)
                    {
                        int num = (int)MessageBox.Show("Error during file copy. Maybe the game is running ?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                    }
                    catch (Exception ex)
                    {
                        int num = (int)MessageBox.Show(ex.StackTrace);
                    }
                }
                else
                {
                    int num1 = (int)MessageBox.Show("Impossible to find " + Path.Combine(rootPath + extPath + "/Injector.exe"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                int num2 = (int)MessageBox.Show("You have to select the HearthStone main path before injecting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.lblStatus.Text = "Awaiting";
            this.lblBotStatus.Text = "No status";
        }

        private void SendConnectCmd(string message)
        {
            try
            {
                TcpClient client = new TcpClient("localhost", 8888);

                Byte[] data = new Byte[256];
                data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                UpdateBotStatus(responseData);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                UpdateBotStatus("Error");
            }
        }

        private void UpdateBotStatus(string responseData)
        {
            switch (responseData)
            {
                case "stbot":
                    lblBotStatus.Text = "Bot started";
                    UpdateButtons(true);
                    break;
                case "stran":
                    lblBotStatus.Text = "Bot started ranked";
                    UpdateButtons(true);
                    break;
                case "stain":
                    lblBotStatus.Text = "Bot started vs AI";
                    UpdateButtons(true);
                    break;
                case "staie":
                    lblBotStatus.Text = "Bot started vs AI Expert";
                    UpdateButtons(true);
                    break;
                case "stopb":
                    lblBotStatus.Text = "Bot stopped";
                    UpdateButtons(false);
                    break;
                case "stopa":
                    lblBotStatus.Text = "Bot will stop after finish this game";
                    UpdateButtons(false);
                    break;
                default:
                    lblBotStatus.Text = "Error";
                    UpdateButtons(false);
                    break;
            }
        }

        private void UpdateButtons(bool isPlaying)
        {
            btnStartBot.Enabled = !isPlaying;
            btnStartBotRanked.Enabled = !isPlaying;
            btnStartBotvsAI.Enabled = !isPlaying;
            btnStartBotvsAIExpert.Enabled = !isPlaying;
            btnStopBot.Enabled = isPlaying;
            btnStopBotAfterThis.Enabled = isPlaying;
        }

        private void folderPath_Click(object sender, EventArgs e)
        {
            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK)
            {
                this.lblPath.Text = folderBrowserDialog1.SelectedPath;
                this.HSpath = this.lblPath.Text;
                File.WriteAllText(Path.Combine(rootPath, "Config.cfg"), this.HSpath);
                btnInject.Enabled = true;
            }
        }
    }
}
