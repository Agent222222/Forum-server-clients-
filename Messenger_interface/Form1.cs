using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Messenger_interface
{

    public partial class Form1 : Form
    {

        string serverIp = "192.168.253.204"; // Замініть на статичний IP вашого сервера
        const int serverPort = 12345;
        bool connection = false;

        TcpClient client = new TcpClient();
        authorization form = new authorization();
        public Form1()
        {
            InitializeComponent();
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {

            form.ShowDialog();

            client.Connect(serverIp, serverPort);
            NetworkStream stream = client.GetStream();
            string inputData = "L " + form.login + " " + form.password;

            byte[] data = Encoding.UTF8.GetBytes(inputData);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (response[0] == '1')
            {
                MessageBox.Show("Successful login!", "MessageBox Example", MessageBoxButtons.OK);
                connection = true;
                response = response.Substring(2);

                rtbChat.ReadOnly = false;
                rtbChat.Text += response;
                rtbMessage.Text = "";
                rtbChat.ReadOnly = true;
            }
            else
            {
                connection = false;
                MessageBox.Show(response, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {

            try
            {
                if (connection == true)
                {
                    NetworkStream stream = client.GetStream();

                    string inputData = "T (" + form.login + ") " + rtbMessage.Text;

                    byte[] data = Encoding.UTF8.GetBytes(inputData);
                    stream.Write(data, 0, data.Length);

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    rtbChat.ReadOnly = false;
                    rtbChat.Text += "\n" + response;
                    rtbMessage.Text = "";
                    rtbChat.ReadOnly = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Помилка підключення: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            form.ShowDialog();

            client.Connect(serverIp, serverPort);
            NetworkStream stream = client.GetStream();
            string inputData = "S " + form.login + " " + form.password;

            byte[] data = Encoding.UTF8.GetBytes(inputData);
            stream.Write(data, 0, data.Length);

            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
            if (response[0] == '1')
            {
                MessageBox.Show("Successful registration!", "MessageBox Example", MessageBoxButtons.OK);
                connection = true;
                response = response.Substring(2);

                rtbChat.ReadOnly = false;
                rtbChat.Text += response;
                rtbMessage.Text = "";
                rtbChat.ReadOnly = true;
            }
            else
            {
                connection = false;
                MessageBox.Show(response, "MessageBox Example", MessageBoxButtons.OK);
            }
        }
    }
}
