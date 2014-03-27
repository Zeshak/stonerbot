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
using System.Runtime.InteropServices;


namespace UI
{
    public partial class Main : Form
    {
        public string HSpath;
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
            string exePath = Assembly.GetExecutingAssembly().CodeBase;
            rootPath = exePath.Substring(0, exePath.LastIndexOf("/") + 1).Replace("file:///", "");
            if (rootPath.Contains("bin/Debug"))
                rootPath = rootPath.Replace("/UI/bin/Debug", "");

            Microsoft.Win32.RegistryKey key;
            key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey("Software").OpenSubKey("StonerBot");
            string[] quests = new string[]
            {
                "",
                "Priest Dominance", 
                "Warlock Dominance",
                "Hunter Dominance",
                "Mage Dominance",
                "Rogue Dominance",
                "Druid Dominance",
                "Shaman Dominance",
                "Paladin Dominance",
                "Warrior Dominance",
                "Total Dominance"
            };
            cmbQuests1.DataSource = quests.Clone();
            cmbQuests2.DataSource = quests.Clone();
            cmbQuests3.DataSource = quests.Clone();
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
            if (txtSay.Text.StartsWith("debug"))
                SendConnectCmd("debug" + txtSay.Text.Replace("debug ", "").Length.ToString().PadLeft(2, '0') + txtSay.Text.Replace("debug ", ""));
            else if (txtSay.Text.StartsWith("anamy"))
                SendConnectCmd(txtSay.Text);
            else if (txtSay.Text.StartsWith("anamf"))
                SendConnectCmd(txtSay.Text);
            else
                SendConnectCmd("saywo" + txtSay.Text.Length.ToString().PadLeft(2, '0') + txtSay.Text);
        }

        private void btnInject_Click(object sender, EventArgs e)
        {
            Inject();
        }

        private void btnGetQuest_Click(object sender, EventArgs e)
        {
            if (cmbQuests1.SelectedItem.ToString() == "")
                return;
            int totalLenght = cmbQuests1.SelectedItem.ToString().Length + cmbQuests2.SelectedItem.ToString().Length + cmbQuests3.SelectedItem.ToString().Length;
            string message = "quest" + totalLenght.ToString().PadLeft(2, '0');

            if (cmbQuests1.SelectedItem.ToString().Length > 0)
                message += cmbQuests1.SelectedItem.ToString();
            if (cmbQuests2.SelectedItem.ToString().Length > 0)
                message += " " + cmbQuests2.SelectedItem.ToString();
            if (cmbQuests3.SelectedItem.ToString().Length > 0)
                message += " " + cmbQuests3.SelectedItem.ToString();

            SendConnectCmd(message);
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            socketThread.Abort();
            client.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.lblStatus.Text = "Awaiting";
            this.lblBotStatus.Text = "No status";
            this.lblLastCommand.Text = string.Empty;
        }

        #endregion

        private void CopyInjectDLL(bool renew = false)
        {
            if (renew)
                DeleteOldDLL();
            string path = Path.Combine(rootPath, "ext/Injector.exe");
            if (!File.Exists(path))
            {
                MessageBox.Show("Error: Injector not found (ext/Injector.exe)!" + Environment.NewLine + "The bot instalation is not correct.");
                return;
            }
            string str = Path.Combine(rootPath, "ext/Assembly-CSharp.orig.dll");
            if (!File.Exists(str))
            {
                try
                {
                    File.Copy(Path.Combine(HSpath, "/Hearthstone_Data/Managed/Assembly-CSharp.dll"), str, true);
                    lblStatus.Text = "Getting the new Assembly-CSharp.dll";
                }
                catch (IOException ex)
                {
                    MessageBox.Show("Error: Unable to find Assembly-CSharp.dll" + Environment.NewLine + "Check that Hearthstone folder is correctly set.");
                    return;
                }
            }
            Process process = new Process();
            process.StartInfo.FileName = path;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.WorkingDirectory = Path.Combine(rootPath, "ext");
            lblStatus.Text = "Creating injection file...";
            process.Start();
            process.WaitForExit();
            if (process.ExitCode != 0)
            {
                MessageBox.Show("Error: Unsuccessfull injection file creation (Error code = " + (object)process.ExitCode + ")!");
                return;
            }
            else
                lblStatus.Text = "Injection file created.";
        }

        private void DeleteOldDLL()
        {
            string path1 = "ext/Assembly-CSharp.dll";
            if (File.Exists(path1))
                File.Delete(path1);
            string path2 = "ext/Assembly-CSharp.original.dll";
            if (!File.Exists(path2))
                return;
            File.Delete(path2);
        }

        private void Inject()
        {
            if (!this.HSpath.Equals(""))
            {
                string injPath = Path.Combine(rootPath, "ext/Injector.exe");
                if (File.Exists(injPath))
                {
                    IntPtr hwnd = FindWindow(null, "Battle.net");
                    if (hwnd == IntPtr.Zero)
                    {
                        MessageBox.Show("Battle.net launcher is not opened. Open it first.");
                        this.lblStatus.Text = "Injection Failed.";
                        return;
                    }
                    if (!File.Exists(Path.Combine(rootPath, "injector/Assembly-CSharp.dll")))
                        CopyInjectDLL(false);
                    if (!File.Exists(Path.Combine(HSpath, "Hearthstone_Data/Managed/Assembly-CSharp.dll")))
                    {
                        MessageBox.Show("Error: Unable to detect Hearthstone file to replace (Assembly-CSharp.dll)!" + Environment.NewLine + "The game may be corrupted or the bot is outdated.");
                        this.lblStatus.Text = "Injection Failed.";
                        return;
                    }
                    this.lblStatus.Text = "Injecting files.";
                    IEnumerable<string> source = Directory.EnumerateFiles(Path.Combine(rootPath, "ext"), "*.dll");
                    foreach (string str in source)
                    {
                        string destFileName = HSpath + "/Hearthstone_Data/Managed/" + Path.GetFileName(str);
                        try
                        {
                            File.Copy(str, destFileName, true);
                        }
                        catch (IOException ex)
                        {
                            MessageBox.Show("Error: Unable to copy/inject files into Hearthstone!" + Environment.NewLine + "Maybe the game is running?");
                            this.lblStatus.Text = "Injection Failed.";
                            return;
                        }
                    }
                    this.lblStatus.Text = "Injection Done.";
                    //MessageBox.Show("Injected " + Enumerable.Count<string>(source).ToString() + " files" + Environment.NewLine + "Ready to launch Hearthstone");
                    SetForegroundWindow(hwnd);
                }
                else
                {
                    int num1 = (int)MessageBox.Show("Impossible to find " + Path.Combine(rootPath + "ext/Injector.exe"), "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
            else
            {
                int num2 = (int)MessageBox.Show("You have to select the HearthStone main path before injecting!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
        }

        [DllImport("USER32.DLL", CharSet = CharSet.Unicode)]
        public static extern IntPtr FindWindow(String lpClassName, String lpWindowName);

        [DllImport("USER32.DLL")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

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
                UpdateBotStatusLabels("disco");
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
                    UpdateButtons(false);
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
