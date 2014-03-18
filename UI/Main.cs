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
using System.Timers;


namespace UI
{
    public partial class Main : Form
    {
        public string HSpath;
        private string extPath;
        public string rootPath;
        public Deck selDeck;
        public static TcpClient client;
        public static Thread socketThread;
        public class Deck
        {
            public long DeckId { get; set; }
            public string Alias { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }

            public Deck()
            {
            }
        }

        #region -[ Eventos ]-

        public Main()
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            socketThread = new Thread(new ThreadStart(UpdateBotStatus));
            socketThread.Start();
            InitializeComponent();
            this.extPath = "ext";
            string exePath = Assembly.GetExecutingAssembly().CodeBase;
            rootPath = exePath.Substring(0, exePath.LastIndexOf("/") + 1).Replace("file:///", "");
            if (rootPath.Contains("bin/Debug"))
                rootPath = rootPath.Replace("/UI/bin/Debug", "");

            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("StonerBot");
            if (key == null)
            {
                key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software", true).CreateSubKey("StonerBot");
                key.SetValue("RootPath", rootPath);
                key.Close();
            }
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
            SendConnectCmd("saywo" + txtSay.Text.Length.ToString().PadLeft(2, '0') + txtSay.Text);
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
                                File.WriteAllText(Path.Combine(this.HSpath, "plugins.txt"), Path.Combine(rootPath, "plugins"));
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
            this.lblLastCommand.Text = string.Empty;
        }

        private void UpdateBotStatus()
        {
            while (true)
            {
                Thread.Sleep(50);
                SendConnectCmd("state");
            }
        }

        private void SendConnectCmd(string message)
        {
            try
            {
                client = new TcpClient("localhost", 8888);

                Byte[] data = new Byte[256];
                data = System.Text.Encoding.ASCII.GetBytes(message);

                NetworkStream stream = client.GetStream();
                stream.Write(data, 0, data.Length);

                data = new Byte[256];
                String responseData = String.Empty;

                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                UpdateBotStatusLabels(responseData);

                stream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                UpdateBotStatusLabels("Error");
            }
        }

        private void UpdateBotStatusLabels(string responseData)
        {
            string responseInit = responseData.Substring(0, 5);
            switch (responseInit)
            {
                case "stbot":
                    lblLastCommand.Text = "Bot started";
                    UpdateButtons(true);
                    break;
                case "stran":
                    lblLastCommand.Text = "Bot started ranked";
                    UpdateButtons(true);
                    break;
                case "stain":
                    lblLastCommand.Text = "Bot started vs AI";
                    UpdateButtons(true);
                    break;
                case "staie":
                    lblLastCommand.Text = "Bot started vs AI Expert";
                    UpdateButtons(true);
                    break;
                case "stopb":
                    lblLastCommand.Text = "Bot stopped";
                    UpdateButtons(false);
                    break;
                case "stopa":
                    lblLastCommand.Text = "Bot will stop after finish this game";
                    UpdateButtons(false);
                    break;
                case "state":
                    {
                        int length = Convert.ToInt32(responseData.Substring(5, 2));
                        string message = responseData.Substring(7, length);
                        lblBotStatus.Text = message;
                        break;
                    }
                case "disco":
                    lblBotStatus.Text = "Disconnected";
                    break;
                default:
                    lblBotStatus.Text = responseData;
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

        private void cmbDecks_SelectedIndexChanged(object sender, EventArgs e)
        {
            selDeck = (Deck)cmbDecks.SelectedItem;
            lblLose.Text = selDeck.Losses.ToString();
            lblWin.Text = selDeck.Wins.ToString();
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string[] statLines = File.ReadAllLines(Path.Combine(this.HSpath, "stat.txt"));

            List<Deck> listDecks = new List<Deck>();
            Deck selDeck = new Deck();
            selDeck = null;
            foreach (string statLine in statLines)
            {
                string statDate = Convert.ToString(statLine.Substring(statLine.IndexOf("[Date]") + 6, statLine.IndexOf("[ID]") - statLine.IndexOf("[Date]") - 6));
                long statDeckId = Convert.ToInt64(statLine.Substring(statLine.IndexOf("[ID]") + 4, statLine.IndexOf("[Name]") - statLine.IndexOf("[ID]") - 4));
                string statDeckName = Convert.ToString(statLine.Substring(statLine.IndexOf("[Name]") + 6, statLine.IndexOf("[Result]") - statLine.IndexOf("[Name]") - 6));
                bool statIsWin = Convert.ToBoolean(statLine.Substring(statLine.IndexOf("[Result]") + 8));
                foreach (Deck deck in listDecks)
                {
                    if (deck.DeckId == statDeckId)
                    {
                        selDeck = deck;
                        break;
                    }
                }
                if (selDeck == null)
                {
                    selDeck = new Deck();
                    selDeck.DeckId = statDeckId;
                    selDeck.Alias = statDeckName;
                    listDecks.Add(selDeck);
                }
                if (statIsWin)
                    selDeck.Wins++;
                else
                    selDeck.Losses++;
            }

            cmbDecks.DataSource = listDecks;
            cmbDecks.ValueMember = "DeckId";
            cmbDecks.DisplayMember = "Alias";
        }
    }
}
