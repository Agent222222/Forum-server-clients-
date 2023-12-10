using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Messenger_interface
{
    public partial class Form1 : Form
    {
        authorization form = new authorization();
        public Form1()
        {
            InitializeComponent();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            form.ShowDialog();
        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {

            rtbChat.Text = form.login;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            form.ShowDialog();
        }
    }
}
