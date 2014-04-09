using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SharpUpdate;

namespace UpdateApp
{
    public partial class AutoUpdater : Form, ISharpUpdatable
    {
        private SharpUpdater updater;

        public AutoUpdater()
        {
            InitializeComponent();

            this.label1.Text = this.ApplicationAssembly.GetName().Version.ToString();
            updater = new SharpUpdater(this);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            updater.DoUpdate();
        }

        #region SharpUpdate
        public string ApplicationName
        {
            get { return "TestApp"; }
        }

        public string ApplicationID
        {
            get { return "TestApp"; }
        }

        public Assembly ApplicationAssembly
        {
            get { return Assembly.GetExecutingAssembly(); }
        }

        public Icon ApplicationIcon
        {
            get { return this.Icon; }
        }

        public Uri UpdateXmlLocation
        {
            get { return new Uri("http://www.stonerbotbd.co.nf/update.xml"); }
        }

        public Form Context
        {
            get { return this; }
        }
        #endregion

        private void Close2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        
    }
}
