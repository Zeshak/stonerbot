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
            this.SuspendLayout();
            // 
            // btnStartBot
            // 
            this.btnStartBot.Location = new System.Drawing.Point(162, 12);
            this.btnStartBot.Name = "btnStartBot";
            this.btnStartBot.Size = new System.Drawing.Size(146, 23);
            this.btnStartBot.TabIndex = 0;
            this.btnStartBot.Text = "Start Bot";
            this.btnStartBot.UseVisualStyleBackColor = true;
            this.btnStartBot.Click += new System.EventHandler(this.btnStartBot_Click);
            // 
            // btnInject
            // 
            this.btnInject.Location = new System.Drawing.Point(12, 12);
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
            this.lblStatus.Location = new System.Drawing.Point(104, 227);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(47, 13);
            this.lblStatus.TabIndex = 2;
            this.lblStatus.Text = "lblStatus";
            // 
            // lblBotStatus
            // 
            this.lblBotStatus.AutoSize = true;
            this.lblBotStatus.Location = new System.Drawing.Point(104, 240);
            this.lblBotStatus.Name = "lblBotStatus";
            this.lblBotStatus.Size = new System.Drawing.Size(63, 13);
            this.lblBotStatus.TabIndex = 3;
            this.lblBotStatus.Text = "lblBotStatus";
            // 
            // btnStopBot
            // 
            this.btnStopBot.Location = new System.Drawing.Point(314, 12);
            this.btnStopBot.Name = "btnStopBot";
            this.btnStopBot.Size = new System.Drawing.Size(146, 23);
            this.btnStopBot.TabIndex = 4;
            this.btnStopBot.Text = "Stop Bot";
            this.btnStopBot.UseVisualStyleBackColor = true;
            this.btnStopBot.Click += new System.EventHandler(this.btnStopBot_Click);
            // 
            // btnStartBotRanked
            // 
            this.btnStartBotRanked.Location = new System.Drawing.Point(162, 41);
            this.btnStartBotRanked.Name = "btnStartBotRanked";
            this.btnStartBotRanked.Size = new System.Drawing.Size(146, 23);
            this.btnStartBotRanked.TabIndex = 5;
            this.btnStartBotRanked.Text = "Start Bot Ranked";
            this.btnStartBotRanked.UseVisualStyleBackColor = true;
            this.btnStartBotRanked.Click += new System.EventHandler(this.btnStartBotRanked_Click);
            // 
            // btnStartBotvsAIExpert
            // 
            this.btnStartBotvsAIExpert.Location = new System.Drawing.Point(162, 99);
            this.btnStartBotvsAIExpert.Name = "btnStartBotvsAIExpert";
            this.btnStartBotvsAIExpert.Size = new System.Drawing.Size(144, 23);
            this.btnStartBotvsAIExpert.TabIndex = 6;
            this.btnStartBotvsAIExpert.Text = "Start Bot vs AI Expert";
            this.btnStartBotvsAIExpert.UseVisualStyleBackColor = true;
            this.btnStartBotvsAIExpert.Click += new System.EventHandler(this.btnStartBotvsAIExpert_Click);
            // 
            // btnStartBotvsAI
            // 
            this.btnStartBotvsAI.Location = new System.Drawing.Point(162, 70);
            this.btnStartBotvsAI.Name = "btnStartBotvsAI";
            this.btnStartBotvsAI.Size = new System.Drawing.Size(146, 23);
            this.btnStartBotvsAI.TabIndex = 7;
            this.btnStartBotvsAI.Text = "Start Bot vs AI";
            this.btnStartBotvsAI.UseVisualStyleBackColor = true;
            this.btnStartBotvsAI.Click += new System.EventHandler(this.btnStartBotvsAI_Click);
            // 
            // btnStopBotAfterThis
            // 
            this.btnStopBotAfterThis.Location = new System.Drawing.Point(314, 41);
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
            this.label1.Location = new System.Drawing.Point(12, 227);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 13);
            this.label1.TabIndex = 9;
            this.label1.Text = "Injection Status: ";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 240);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 13);
            this.label2.TabIndex = 10;
            this.label2.Text = "Bot Status: ";
            // 
            // txtSay
            // 
            this.txtSay.Location = new System.Drawing.Point(12, 167);
            this.txtSay.Name = "txtSay";
            this.txtSay.Size = new System.Drawing.Size(144, 20);
            this.txtSay.TabIndex = 11;
            // 
            // btnSay
            // 
            this.btnSay.Location = new System.Drawing.Point(162, 163);
            this.btnSay.Name = "btnSay";
            this.btnSay.Size = new System.Drawing.Size(144, 23);
            this.btnSay.TabIndex = 12;
            this.btnSay.Text = "Say";
            this.btnSay.UseVisualStyleBackColor = true;
            this.btnSay.Click += new System.EventHandler(this.btnSay_Click);
            // 
            // folderPath
            // 
            this.folderPath.Location = new System.Drawing.Point(15, 293);
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
            this.lblPath.Location = new System.Drawing.Point(179, 298);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(235, 13);
            this.lblPath.TabIndex = 18;
            this.lblPath.Text = "\"Before Inject, configure your Hearthstone path\"";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(471, 343);
            this.Controls.Add(this.lblPath);
            this.Controls.Add(this.folderPath);
            this.Controls.Add(this.btnSay);
            this.Controls.Add(this.txtSay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnStopBotAfterThis);
            this.Controls.Add(this.btnStartBotvsAI);
            this.Controls.Add(this.btnStartBotvsAIExpert);
            this.Controls.Add(this.btnStartBotRanked);
            this.Controls.Add(this.btnStopBot);
            this.Controls.Add(this.lblBotStatus);
            this.Controls.Add(this.lblStatus);
            this.Controls.Add(this.btnInject);
            this.Controls.Add(this.btnStartBot);
            this.ForeColor = System.Drawing.Color.Black;
            this.Name = "Main";
            this.Text = "StonerBot";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

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
    }
}

