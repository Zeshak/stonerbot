namespace UI
{
    partial class Main
    {
        /// <summary>
        /// Variable del diseñador requerida.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Limpiar los recursos que se estén utilizando.
        /// </summary>
        /// <param name="disposing">true si los recursos administrados se deben eliminar; false en caso contrario.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código generado por el Diseñador de Windows Forms

        /// <summary>
        /// Método necesario para admitir el Diseñador. No se puede modificar
        /// el contenido del método con el editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            this.btnStartBot = new System.Windows.Forms.Button();
            this.btnInject = new System.Windows.Forms.Button();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblBotStatus = new System.Windows.Forms.Label();
            this.btnStopBot = new System.Windows.Forms.Button();
            this.btnStartBotRanked = new System.Windows.Forms.Button();
            this.btnStartBotvsAIExpert = new System.Windows.Forms.Button();
            this.btnStartBotvsAI = new System.Windows.Forms.Button();
            this.btnStopBotAfterThis = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSay = new System.Windows.Forms.TextBox();
            this.btnSay = new System.Windows.Forms.Button();
            this.folderPath = new System.Windows.Forms.Button();
            this.lblPath = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabMain = new System.Windows.Forms.TabPage();
            this.lblLastCommand = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.tabStat = new System.Windows.Forms.TabPage();
            this.btnRefresh = new System.Windows.Forms.Button();
            this.lblLose = new System.Windows.Forms.Label();
            this.lblWin = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbDecks = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnGetQuest = new System.Windows.Forms.Button();
            this.cmbQuests3 = new System.Windows.Forms.ComboBox();
            this.cmbQuests2 = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbQuests1 = new System.Windows.Forms.ComboBox();
            this.btnRenew = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabStat.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnStartBot
            // 
            this.btnStartBot.Location = new System.Drawing.Point(156, 6);
            this.btnStartBot.Name = "btnStartBot";
            this.btnStartBot.Size = new System.Drawing.Size(146, 23);
            this.btnStartBot.TabIndex = 0;
            this.btnStartBot.Text = "Start Bot";
            this.btnStartBot.UseVisualStyleBackColor = true;
            this.btnStartBot.Click += new System.EventHandler(this.btnStartBot_Click);
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(6, 6);
            this.btnInject.Name = "btnInject";
            this.btnInject.Size = new System.Drawing.Size(144, 23);
            this.btnInject.TabIndex = 1;
            this.btnInject.Text = "Inject";
            this.btnInject.UseVisualStyleBackColor = true;
            this.btnInject.Click += new System.EventHandler(this.btnInject_Click);
            // 
            // lblStatus
            // 
            this.lblStatus.AutoSize = true;
            this.lblStatus.Location = new System.Drawing.Point(98, 221);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "lblStatus";
            // 
            // lblBotStatus
            // 
            this.lblBotStatus.AutoSize = true;
            this.lblBotStatus.Location = new System.Drawing.Point(98, 234);
            this.lblBotStatus.Name = "lblBotStatus";
            this.lblBotStatus.Size = new System.Drawing.Size(63, 13);
            this.lblBotStatus.TabIndex = 3;
            this.lblBotStatus.Text = "lblBotStatus";
            // 
            // btnStopBot
            // 
            this.btnStopBot.Location = new System.Drawing.Point(308, 6);
            this.btnStopBot.Name = "btnStopBot";
            this.btnStopBot.Size = new System.Drawing.Size(146, 23);
            this.btnStopBot.TabIndex = 4;
            this.btnStopBot.Text = "Stop Bot";
            this.btnStopBot.UseVisualStyleBackColor = true;
            this.btnStopBot.Click += new System.EventHandler(this.btnStopBot_Click);
            // 
            // btnStartBotRanked
            // 
            this.btnStartBotRanked.Location = new System.Drawing.Point(156, 35);
            this.btnStartBotRanked.Name = "btnStartBotRanked";
            this.btnStartBotRanked.Size = new System.Drawing.Size(146, 23);
            this.btnStartBotRanked.TabIndex = 5;
            this.btnStartBotRanked.Text = "Start Bot Ranked";
            this.btnStartBotRanked.UseVisualStyleBackColor = true;
            this.btnStartBotRanked.Click += new System.EventHandler(this.btnStartBotRanked_Click);
            // 
            // btnStartBotvsAIExpert
            // 
            this.btnStartBotvsAIExpert.Location = new System.Drawing.Point(156, 93);
            this.btnStartBotvsAIExpert.Name = "btnStartBotvsAIExpert";
            this.btnStartBotvsAIExpert.Size = new System.Drawing.Size(144, 23);
            this.btnStartBotvsAIExpert.TabIndex = 6;
            this.btnStartBotvsAIExpert.Text = "Start Bot vs AI Expert";
            this.btnStartBotvsAIExpert.UseVisualStyleBackColor = true;
            this.btnStartBotvsAIExpert.Click += new System.EventHandler(this.btnStartBotvsAIExpert_Click);
            // 
            // btnStartBotvsAI
            // 
            this.btnStartBotvsAI.Location = new System.Drawing.Point(156, 64);
            this.btnStartBotvsAI.Name = "btnStartBotvsAI";
            this.btnStartBotvsAI.Size = new System.Drawing.Size(146, 23);
            this.btnStartBotvsAI.TabIndex = 7;
            this.btnStartBotvsAI.Text = "Start Bot vs AI";
            this.btnStartBotvsAI.UseVisualStyleBackColor = true;
            this.btnStartBotvsAI.Click += new System.EventHandler(this.btnStartBotvsAI_Click);
            // 
            // btnStopBotAfterThis
            // 
            this.btnStopBotAfterThis.Location = new System.Drawing.Point(308, 35);
            this.btnStopBotAfterThis.Name = "btnStopBotAfterThis";
            this.btnStopBotAfterThis.Size = new System.Drawing.Size(146, 23);
            this.btnStopBotAfterThis.TabIndex = 8;
            this.btnStopBotAfterThis.Text = "Stop Bot After This Game";
            this.btnStopBotAfterThis.UseVisualStyleBackColor = true;
            this.btnStopBotAfterThis.Click += new System.EventHandler(this.btnStopBotAfterThis_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 221);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Injection Status: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 234);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Bot Status: ";
            // 
            // txtSay
            // 
            this.txtSay.Location = new System.Drawing.Point(6, 161);
            this.txtSay.Name = "txtSay";
            this.txtSay.Size = new System.Drawing.Size(144, 20);
            this.txtSay.TabIndex = 11;
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(156, 157);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(144, 23);
            this.btnSay.TabIndex = 12;
            this.btnSay.Text = "Send command";
            this.btnSay.UseVisualStyleBackColor = true;
            this.btnSay.Click += new System.EventHandler(this.btnSay_Click);
            // 
            // folderPath
            // 
            this.folderPath.Location = new System.Drawing.Point(9, 287);
            this.folderPath.Name = "folderPath";
            this.folderPath.Size = new System.Drawing.Size(145, 23);
            this.folderPath.TabIndex = 17;
            this.folderPath.Text = "Hearthstone Path";
            this.folderPath.UseVisualStyleBackColor = true;
            this.folderPath.Click += new System.EventHandler(this.folderPath_Click);
            // 
            // lblPath
            // 
            this.lblPath.AutoSize = true;
            this.lblPath.ForeColor = System.Drawing.Color.Red;
            this.lblPath.Location = new System.Drawing.Point(173, 292);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(235, 13);
            this.lblPath.TabIndex = 18;
            this.lblPath.Text = "\"Before Inject, configure your Hearthstone path\"";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabMain);
            this.tabControl1.Controls.Add(this.tabStat);
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(1, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(469, 407);
            this.tabControl1.TabIndex = 19;
            // 
            // tabMain
            // 
            this.tabMain.Controls.Add(this.btnRenew);
            this.tabMain.Controls.Add(this.lblLastCommand);
            this.tabMain.Controls.Add(this.label7);
            this.tabMain.Controls.Add(this.btnInject);
            this.tabMain.Controls.Add(this.lblPath);
            this.tabMain.Controls.Add(this.btnStartBot);
            this.tabMain.Controls.Add(this.folderPath);
            this.tabMain.Controls.Add(this.lblStatus);
            this.tabMain.Controls.Add(this.btnSay);
            this.tabMain.Controls.Add(this.lblBotStatus);
            this.tabMain.Controls.Add(this.txtSay);
            this.tabMain.Controls.Add(this.btnStopBot);
            this.tabMain.Controls.Add(this.label2);
            this.tabMain.Controls.Add(this.btnStartBotRanked);
            this.tabMain.Controls.Add(this.label1);
            this.tabMain.Controls.Add(this.btnStartBotvsAIExpert);
            this.tabMain.Controls.Add(this.btnStopBotAfterThis);
            this.tabMain.Controls.Add(this.btnStartBotvsAI);
            this.tabMain.Location = new System.Drawing.Point(4, 22);
            this.tabMain.Name = "tabMain";
            this.tabMain.Padding = new System.Windows.Forms.Padding(3);
            this.tabMain.Size = new System.Drawing.Size(461, 381);
            this.tabMain.TabIndex = 0;
            this.tabMain.Text = "Main";
            this.tabMain.UseVisualStyleBackColor = true;
            // 
            // lblLastCommand
            // 
            this.lblLastCommand.AutoSize = true;
            this.lblLastCommand.Location = new System.Drawing.Point(99, 247);
            this.lblLastCommand.Name = "lblLastCommand";
            this.lblLastCommand.Size = new System.Drawing.Size(84, 13);
            this.lblLastCommand.TabIndex = 19;
            this.lblLastCommand.Text = "lblLastCommand";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 247);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(83, 13);
            this.label7.TabIndex = 20;
            this.label7.Text = "Last Command: ";
            // 
            // tabStat
            // 
            this.tabStat.Controls.Add(this.btnRefresh);
            this.tabStat.Controls.Add(this.lblLose);
            this.tabStat.Controls.Add(this.lblWin);
            this.tabStat.Controls.Add(this.label6);
            this.tabStat.Controls.Add(this.label5);
            this.tabStat.Controls.Add(this.cmbDecks);
            this.tabStat.Controls.Add(this.label3);
            this.tabStat.Location = new System.Drawing.Point(4, 22);
            this.tabStat.Name = "tabStat";
            this.tabStat.Padding = new System.Windows.Forms.Padding(3);
            this.tabStat.Size = new System.Drawing.Size(461, 381);
            this.tabStat.TabIndex = 1;
            this.tabStat.Text = "Statistics";
            this.tabStat.UseVisualStyleBackColor = true;
            // 
            // btnRefresh
            // 
            this.btnRefresh.Location = new System.Drawing.Point(359, 33);
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(75, 23);
            this.btnRefresh.TabIndex = 9;
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.UseVisualStyleBackColor = true;
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // lblLose
            // 
            this.lblLose.AutoSize = true;
            this.lblLose.Location = new System.Drawing.Point(96, 121);
            this.lblLose.Name = "lblLose";
            this.lblLose.Size = new System.Drawing.Size(13, 13);
            this.lblLose.TabIndex = 8;
            this.lblLose.Text = "0";
            // 
            // lblWin
            // 
            this.lblWin.AutoSize = true;
            this.lblWin.Location = new System.Drawing.Point(96, 90);
            this.lblWin.Name = "lblWin";
            this.lblWin.Size = new System.Drawing.Size(13, 13);
            this.lblWin.TabIndex = 7;
            this.lblWin.Text = "0";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 121);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(36, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Lose: ";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(38, 90);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Win: ";
            // 
            // cmbDecks
            // 
            this.cmbDecks.FormattingEnabled = true;
            this.cmbDecks.Location = new System.Drawing.Point(181, 33);
            this.cmbDecks.Name = "cmbDecks";
            this.cmbDecks.Size = new System.Drawing.Size(156, 21);
            this.cmbDecks.TabIndex = 1;
            this.cmbDecks.SelectedIndexChanged += new System.EventHandler(this.cmbDecks_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(38, 33);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Select your deck: ";
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnGetQuest);
            this.tabPage1.Controls.Add(this.cmbQuests3);
            this.tabPage1.Controls.Add(this.cmbQuests2);
            this.tabPage1.Controls.Add(this.label10);
            this.tabPage1.Controls.Add(this.label9);
            this.tabPage1.Controls.Add(this.label8);
            this.tabPage1.Controls.Add(this.label4);
            this.tabPage1.Controls.Add(this.cmbQuests1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(461, 381);
            this.tabPage1.TabIndex = 2;
            this.tabPage1.Text = "Quests";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnGetQuest
            // 
            this.btnGetQuest.Location = new System.Drawing.Point(66, 169);
            this.btnGetQuest.Name = "btnGetQuest";
            this.btnGetQuest.Size = new System.Drawing.Size(308, 23);
            this.btnGetQuest.TabIndex = 9;
            this.btnGetQuest.Text = "Get that quest!";
            this.btnGetQuest.UseVisualStyleBackColor = true;
            this.btnGetQuest.Click += new System.EventHandler(this.btnGetQuest_Click);
            // 
            // cmbQuests3
            // 
            this.cmbQuests3.FormattingEnabled = true;
            this.cmbQuests3.Location = new System.Drawing.Point(66, 141);
            this.cmbQuests3.Name = "cmbQuests3";
            this.cmbQuests3.Size = new System.Drawing.Size(308, 21);
            this.cmbQuests3.TabIndex = 8;
            // 
            // cmbQuests2
            // 
            this.cmbQuests2.FormattingEnabled = true;
            this.cmbQuests2.Location = new System.Drawing.Point(66, 114);
            this.cmbQuests2.Name = "cmbQuests2";
            this.cmbQuests2.Size = new System.Drawing.Size(308, 21);
            this.cmbQuests2.TabIndex = 6;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(14, 41);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(397, 13);
            this.label10.TabIndex = 4;
            this.label10.Text = "this means that maybe you cant get Total Dominance twice in a less than 3-4 days." +
    "";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(11, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(319, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "Sometimes you can\'t get a quest too many times in the same week";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 7);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(339, 13);
            this.label8.TabIndex = 2;
            this.label8.Text = "If the time is between 9PM and 2AM PST (-7) you can use this feature.";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(66, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 13);
            this.label4.TabIndex = 1;
            this.label4.Text = "Select the quests you want to get.";
            // 
            // cmbQuests1
            // 
            this.cmbQuests1.FormattingEnabled = true;
            this.cmbQuests1.Location = new System.Drawing.Point(66, 87);
            this.cmbQuests1.Name = "cmbQuests1";
            this.cmbQuests1.Size = new System.Drawing.Size(308, 21);
            this.cmbQuests1.TabIndex = 0;
            // 
            // btnRenew
            // 
            this.btnRenew.Location = new System.Drawing.Point(6, 35);
            this.btnRenew.Name = "btnRenew";
            this.btnRenew.Size = new System.Drawing.Size(144, 23);
            this.btnRenew.TabIndex = 21;
            this.btnRenew.Text = "Renew HS Files";
            this.btnRenew.UseVisualStyleBackColor = true;
            this.btnRenew.Click += new System.EventHandler(this.btnRenew_Click);
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(469, 404);
            this.Controls.Add(this.tabControl1);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "Main";
            this.Text = "StonerBot";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.tabControl1.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabMain.PerformLayout();
            this.tabStat.ResumeLayout(false);
            this.tabStat.PerformLayout();
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnStartBot;
        private System.Windows.Forms.Button btnInject;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblBotStatus;
        private System.Windows.Forms.Button btnStopBot;
        private System.Windows.Forms.Button btnStartBotRanked;
        private System.Windows.Forms.Button btnStartBotvsAIExpert;
        private System.Windows.Forms.Button btnStartBotvsAI;
        private System.Windows.Forms.Button btnStopBotAfterThis;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtSay;
        private System.Windows.Forms.Button btnSay;
        private System.Windows.Forms.Button folderPath;
        private System.Windows.Forms.Label lblPath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabMain;
        private System.Windows.Forms.TabPage tabStat;
        private System.Windows.Forms.Label lblLose;
        private System.Windows.Forms.Label lblWin;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbDecks;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnRefresh;
        private System.Windows.Forms.Label lblLastCommand;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.ComboBox cmbQuests1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button btnGetQuest;
        private System.Windows.Forms.ComboBox cmbQuests3;
        private System.Windows.Forms.ComboBox cmbQuests2;
        private System.Windows.Forms.Button btnRenew;
    }
}

