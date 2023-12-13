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
    public partial class authorization : Form
    {
        public string login;
        public string password;
        public string IP;

        public authorization()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            login = tbLogin.Text;
            password = tbPasswd.Text;
            IP = IP_inpt.Text;
            this.Close();
        }

    }
}
