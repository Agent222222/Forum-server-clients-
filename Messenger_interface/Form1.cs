using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text.RegularExpressions;
using System.IO;
using System.Runtime.CompilerServices;

namespace Messenger_interface
{

    public partial class Form1 : Form
    {
       
        string serverIp = "10.10.10.80"; // Замініть на статичний IP вашого сервера
        const int serverPort = 12345;
        bool connection = false;


        private NetworkStream stream;
        TcpClient client = new TcpClient();
        authorization form = new authorization();
        private static Form1 instance;
        private static string wirelessIPv4Address;

        public Form1()
        {
            instance = this;

            InitializeComponent();
            client = new TcpClient();
            CloseConnection();
           // InitializeConnection();
        }

        private void CloseConnection()
        {
            try
            {
                if (client != null && client.Connected)
                {
                    stream.Close();
                    client.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error closing the connection: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }
        private void InitializeConnection()
        {
            try
            {
                if (!client.Connected)
                {
                    client.Connect(serverIp, serverPort);
                    stream = client.GetStream();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error connecting to the server: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        static async Task FindIP()
        {
            try
            {
                // Запускаємо ipconfig та отримуємо вивід
                string ipConfigOutput = ExecuteCommand("ipconfig", "/all");

                // Знаходимо глобальну IPv4-адресу для адаптера Wi-Fi
                wirelessIPv4Address = FindIPv4Address(ipConfigOutput, "Wireless LAN adapter Wi-Fi");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка: {ex.Message}");
            }
        }
        static string ExecuteCommand(string command, string arguments)
        {
            System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.FileName = command;
            process.StartInfo.Arguments = arguments;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();
            string output = process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            return output;
        }
        static string FindIPv4Address(string ipConfigOutput, string adapterName)
        {
            string pattern = $"{adapterName}[\\s\\S]*?IPv4 Address[.\\s]+: ([0-9\\.]+)";
            Match match = Regex.Match(ipConfigOutput, pattern);
            return match.Success ? match.Groups[1].Value : "Не знайдено";
        }

        private async Task ReceiveMessagesAsync()
        {
            try
            {
                byte[] buffer = new byte[1024];
                int bytesRead;

                while (true)
                {
                    bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (response[0] == '1')
                    {
                        response = response.Substring(2);

                        rtbChat.ReadOnly = false;
                        rtbChat.Text += response;
                        rtbMessage.Text = "";
                        rtbChat.ReadOnly = true;
                    }
                    else if (response[0] == '3')
                    {
                        MessageBox.Show("You were disconnected", "MessageBox Example", MessageBoxButtons.OK);
                        this.Close();
                    }
                    else if (response[0] == '4')
                    {
                        MessageBox.Show("You were disconnected and deleted from the list of login", "MessageBox Example", MessageBoxButtons.OK);
                        this.Close();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error receiving messages: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void logInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                form.ShowDialog();
                serverIp = form.IP;
                InitializeConnection();

                string inputData = "L " + form.login + " " + form.password + " " + wirelessIPv4Address;
                SendMessage(inputData);

                string response = ReceiveMessage();
                if (response[0] == '1')
                {
                    MessageBox.Show("Successful login!", "MessageBox Example", MessageBoxButtons.OK);
                    connection = true;
                    response = response.Substring(2);

                    rtbChat.ReadOnly = false;
                    rtbChat.Text += response;
                    rtbMessage.Text = "";
                    rtbChat.ReadOnly = true;

                    _ = ReceiveMessagesAsync();
                }
                else if (response[0] == '2')
                {
                    MessageBox.Show("Someone already logined by this name", "MessageBox Example", MessageBoxButtons.OK);
                }
                else if (response[0] == '0')
                {
                    connection = false;
                    MessageBox.Show(response, "MessageBox Example", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void btSendMessage_Click(object sender, EventArgs e)
        {
            try
            {
                if (connection == true)
                {
                    string inputData = "T (" + form.login + "):    " + rtbMessage.Text;
                    SendMessage(inputData);

                    NetworkStream stream2 = client.GetStream();

                    byte[] buffer = new byte[1024];
                    int bytesRead = stream2.Read(buffer, 0, buffer.Length);
                    string response = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    if (response[0] == '5')
                    {
                        MessageBox.Show("You have written too many BAD words", "MessageBox Example", MessageBoxButtons.OK);
                        return;
                    }
                    else
                    {
                        response = response.Substring(2);

                        rtbChat.ReadOnly = false;
                        rtbChat.Text = response;
                        rtbMessage.Text = "";
                        rtbChat.ReadOnly = true;
                    }
                    
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                form.ShowDialog();
                serverIp = form.IP;
                string inputData = "S " + form.login + " " + form.password + " " + wirelessIPv4Address;
                SendMessage(inputData);

                string response = ReceiveMessage();
                if (response[0] == '1')
                {
                    MessageBox.Show("Successful registration!", "MessageBox Example", MessageBoxButtons.OK);
                    connection = true;
                    response = response.Substring(2);

                    rtbChat.ReadOnly = false;
                    rtbChat.Text += response;
                    rtbMessage.Text = "";
                    rtbChat.ReadOnly = true;

                    _ = ReceiveMessagesAsync();
                }
                else if (response[0] == '2')
                {
                    MessageBox.Show("This name is already captured", "MessageBox Example", MessageBoxButtons.OK);
                }
                else if (response[0] == '0')
                {
                    connection = false;
                    MessageBox.Show(response, "MessageBox Example", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message, "MessageBox Example", MessageBoxButtons.OK);
            }
        }

        private void SendMessage(string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }

        private string ReceiveMessage()
        {
            byte[] buffer = new byte[1024];
            int bytesRead = stream.Read(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            FindIP();

        }


    }
}
