using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;

namespace Server_interface
{
    public partial class Form1 : Form
    {
        private static IPAddress ipAddress;// Замініть на свій статичний IP
        private const int port = 12345;
        private static List<TcpClient> clients = new List<TcpClient>();
        private static List<string> logined = new List<string>();
        private static Form1 instance;
        private static bool moderate = true;
        private static int bdwordcount = 0;

        public Form1()
        {
            instance = this;
            InitializeComponent();
        }

        static async Task FindIP()
        {
            try
            {
                // Запускаємо ipconfig та отримуємо вивід
                string ipConfigOutput = ExecuteCommand("ipconfig", "/all");

                // Знаходимо глобальну IPv4-адресу для адаптера Wi-Fi
                string wirelessIPv4Address = FindIPv4Address(ipConfigOutput, "Wireless LAN adapter Wi-Fi");
                instance.label3.Text = wirelessIPv4Address;
                ipAddress = IPAddress.Parse(wirelessIPv4Address);
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
        static bool ContainFuck(string message)
        {
            // List of bad words (replace with your actual list)
            List<string> badWords = new List<string> { "fuck", "fucking", "fuck off"};

            // Split the message into words
            string[] words = message.Split(' ', '-', '\n', '\r');

            // Check if any word in the message is a bad word
            foreach (string word in words)
            {
                if (badWords.Contains(word.ToLower()))
                {
                    bdwordcount++;
                }
            }
            if (bdwordcount > 0)
            {
                return true;
            }
            else
            {
                return false; 
            }
        }
        static bool ContainShit(string message)
        {
            // List of bad words (replace with your actual list)
            List<string> badWords = new List<string> { "shit", "shitty" };

            // Split the message into words
            string[] words = message.Split(' ', '-', '\n', '\r');

            // Check if any word in the message is a bad word
            foreach (string word in words)
            {
                if (badWords.Contains(word.ToLower()))
                {
                    bdwordcount++;
                }
            }
            if (bdwordcount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        static bool ContainAss(string message)
        {
            // List of bad words (replace with your actual list)
            List<string> badWords = new List<string> { "ass", "Ass", "ASS", "asshole" };

            // Split the message into words
            string[] words = message.Split(' ', '-', '\n', '\r');

            // Check if any word in the message is a bad word
            foreach (string word in words)
            {
                if (badWords.Contains(word.ToLower()))
                {
                    bdwordcount++;
                }
            }
            if (bdwordcount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        static async Task Main()
        {
            FindIP();

            TcpListener server = new TcpListener(ipAddress, port);
            server.Start();
            Console.WriteLine($"Сервер слухає на {ipAddress}:{port}");

            // Створення спільного потоку даних для всіх клієнтів


            while (true)
            {
                TcpClient client = await server.AcceptTcpClientAsync();
                clients.Add(client);
                _ = HandleClientAsync(client);
            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            Main();
        }
        

        static async Task HandleClientAsync(TcpClient client)
        {
            try
            {

                using (NetworkStream stream = client.GetStream())
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;

                    while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
                    {
                        string data = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Console.WriteLine($"Отримано від клієнта: {data}");

                        if (data[0] == 'L')
                        {
                            int ch = 0;
                            string filePath = "users_data.txt";
                            string[] check = data.Split(' ');

                            string content = File.ReadAllText(filePath);
                            string[] words = content.Split(' ', '\r', '\n');
                            string[] nonEmptyWords = words.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                            for (int i = 0; i < nonEmptyWords.Count(); i += 2)
                            {
                                if ((check[1] == nonEmptyWords[i] && check[2] == nonEmptyWords[i + 1]))
                                {
                                    int logcheck = 0;
                                    for (int j = 0; j < logined.Count(); j += 2)
                                    {
                                        if ((check[1] == logined[j] && check[2] == logined[j + 1]))
                                        {
                                            logcheck++;
                                        }
                                    }
                                    if (logcheck == 0)
                                    {
                                        string saved = File.ReadAllText("received_data.txt");
                                        logined.Add(check[1]);
                                        logined.Add(check[2]);
                                        logined.Add(check[3]);
                                        instance.dataGridView1.RowCount = 1;
                                        for (int h = 0; h < logined.Count / 3; h++)
                                        {
                                            instance.dataGridView1.RowCount++;
                                            instance.dataGridView1.Rows[h].Cells[0].Value = logined[2 * h + h];
                                            instance.dataGridView1.Rows[h].Cells[1].Value = logined[2 * h + h + 2];
                                        }

                                        ch++;
                                        byte[] response = Encoding.UTF8.GetBytes($"1 {saved}");
                                        Console.WriteLine($"Вивід на екран клієнта: {response}");
                                        await stream.WriteAsync(response, 0, response.Length);
                                    }
                                    else
                                    {
                                        byte[] response = Encoding.UTF8.GetBytes($"2 ");
                                        await stream.WriteAsync(response, 0, response.Length);
                                    }
                                }
                            }


                            if (ch == 0)
                            {
                                byte[] response = Encoding.UTF8.GetBytes($"0 log in error");
                                Console.WriteLine($"Вивід помилки на екран клієнта: {response}");
                                await stream.WriteAsync(response, 0, response.Length);
                            }

                        }
                        else if (data[0] == 'S')
                        {
                            int ch = 0;
                            string filePath = "users_data.txt";
                            string[] check = data.Split(' ');

                            string content = File.ReadAllText(filePath);
                            string[] words = content.Split(' ', '\r', '\n');
                            string[] nonEmptyWords = words.Where(s => !string.IsNullOrEmpty(s)).ToArray();

                            for (int i = 0; i < nonEmptyWords.Count(); i += 2)
                            {
                                if (check[1] == nonEmptyWords[i])
                                {
                                    ch++;
                                    byte[] response = Encoding.UTF8.GetBytes($"2 sign in error");
                                    Console.WriteLine($"Вивід на екран клієнта: {response}");
                                    await stream.WriteAsync(response, 0, response.Length);

                                }
                            }

                            if (ch == 0)
                            {
                                DialogResult result = MessageBox.Show($"Client: {check[1]} want to register (Confirm ?)", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                                if (result == DialogResult.Yes)
                                {
                                    string modifiedData = data.Substring(2);
                                    System.IO.File.AppendAllText("users_data.txt", $"\n{modifiedData}");
                                    string saved = File.ReadAllText("received_data.txt");

                                    logined.Add(check[1]);
                                    logined.Add(check[2]);
                                    logined.Add(check[3]);

                                    instance.dataGridView1.RowCount = 1;
                                    for (int h = 0; h < logined.Count / 3; h++)
                                    {
                                        instance.dataGridView1.RowCount++;
                                        instance.dataGridView1.Rows[h].Cells[0].Value = logined[2 * h + h];
                                        instance.dataGridView1.Rows[h].Cells[1].Value = logined[2 * h + h + 2];
                                    }

                                    byte[] response = Encoding.UTF8.GetBytes($"1 {saved}");
                                    Console.WriteLine($"Вивід на екран клієнта: {response}");
                                    await stream.WriteAsync(response, 0, response.Length);
                                }
                                else
                                {

                                }
                               
                            }
                        }
                        else if (data[0] == 'T')
                        {
                            int ch = 0;
                            string filePath = "received_data.txt";

                            if (moderate)
                            {
                                string modifiedData = data.Substring(2);
                                ContainFuck(modifiedData);
                                ContainAss(modifiedData);
                                ContainShit(modifiedData);

                                if (bdwordcount < 3)
                                {
                                    System.IO.File.AppendAllText(filePath, $"\n{modifiedData}");
                                    string saved = File.ReadAllText("received_data.txt");


                                    byte[] response = Encoding.UTF8.GetBytes($"1S {saved}");
                                    Console.WriteLine($"Вивід на екран клієнта: {response}");


                                    foreach (var connectedClient in clients)
                                    {
                                        NetworkStream connectedStream = connectedClient.GetStream();
                                        // Sending the length of the message first
                                        byte[] responseLength = BitConverter.GetBytes(response.Length);
                                        await connectedStream.WriteAsync(responseLength, 0, responseLength.Length);
                                        // Sending the actual message
                                        await connectedStream.WriteAsync(response, 0, response.Length);
                                    }
                                }
                                else
                                {
                                    byte[] response = Encoding.UTF8.GetBytes($"5");

                                    foreach (var connectedClient in clients)
                                    {
                                        NetworkStream connectedStream = connectedClient.GetStream();
                                        // Sending the length of the message first
                                        byte[] responseLength = BitConverter.GetBytes(response.Length);
                                        await connectedStream.WriteAsync(responseLength, 0, responseLength.Length);
                                        // Sending the actual message
                                        await connectedStream.WriteAsync(response, 0, response.Length);
                                    }
                                }
                                bdwordcount = 0;
                            }
                            else
                            {
                                string modifiedData = data.Substring(2);
                                System.IO.File.AppendAllText(filePath, $"\n{modifiedData}");
                                string saved = File.ReadAllText("received_data.txt");


                                byte[] response = Encoding.UTF8.GetBytes($"1 {saved}");

                                foreach (var connectedClient in clients)
                                {
                                    NetworkStream connectedStream = connectedClient.GetStream();
                                    // Sending the length of the message first
                                    byte[] responseLength = BitConverter.GetBytes(response.Length);
                                    await connectedStream.WriteAsync(responseLength, 0, responseLength.Length);
                                    // Sending the actual message
                                    await connectedStream.WriteAsync(response, 0, response.Length);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка обробки клієнта: {ex.Message}");
            }
            finally
            {
                clients.Remove(client);
                client.Close();
            }
        }

        private async void button3_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to disconnect this client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {

                    int selectedIndex = dataGridView1.SelectedRows[0].Index;

                    byte[] response = Encoding.UTF8.GetBytes($"3");
                    NetworkStream connectedStream = clients[selectedIndex].GetStream();
                    // Sending the length of the message first
                    byte[] responseLength = BitConverter.GetBytes(response.Length);
                    await connectedStream.WriteAsync(responseLength, 0, responseLength.Length);
                    // Sending the actual message
                    await connectedStream.WriteAsync(response, 0, response.Length);

                    clients.RemoveAt(selectedIndex);

                    for(int i = 0; i < 3; i++)
                    {
                        logined.RemoveAt(selectedIndex * 2 + selectedIndex);
                    }

                    instance.dataGridView1.RowCount = 1;
                    for (int h = 0; h < logined.Count / 3; h++)
                    {
                        instance.dataGridView1.RowCount++;
                        instance.dataGridView1.Rows[h].Cells[0].Value = logined[2 * h + h];
                        instance.dataGridView1.Rows[h].Cells[1].Value = logined[2 * h + h + 2];
                    }
                }
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure you want to disconnect this client?", "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                if (dataGridView1.SelectedRows.Count > 0)
                {

                    int selectedIndex = dataGridView1.SelectedRows[0].Index;

                    byte[] response = Encoding.UTF8.GetBytes($"4");
                    NetworkStream connectedStream = clients[selectedIndex].GetStream();
                    // Sending the length of the message first
                    byte[] responseLength = BitConverter.GetBytes(response.Length);
                    await connectedStream.WriteAsync(responseLength, 0, responseLength.Length);
                    // Sending the actual message
                    await connectedStream.WriteAsync(response, 0, response.Length);

                    clients.RemoveAt(selectedIndex);
                    string filePath = "users_data.txt";
                    string[] lines = File.ReadAllLines(filePath);

                    var newLines = new System.Collections.Generic.List<string>();

                    // Iterate through each line and exclude lines containing the specified word
                    foreach (string line in lines)
                    {
                        if (!line.Contains(logined[selectedIndex * 2 + selectedIndex]))
                        {
                            newLines.Add(line);
                        }
                    }

                    // Write the modified lines back to the file
                    File.WriteAllLines(filePath, newLines);

                    for (int i = 0; i < 3; i++)
                    {
                        logined.RemoveAt(selectedIndex * 2 + selectedIndex);
                    }

                    instance.dataGridView1.RowCount = 1;
                    for (int h = 0; h < logined.Count / 3; h++)
                    {
                        instance.dataGridView1.RowCount++;
                        instance.dataGridView1.Rows[h].Cells[0].Value = logined[2 * h + h];
                        instance.dataGridView1.Rows[h].Cells[1].Value = logined[2 * h + h + 2];
                    }
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if(moderate == false)
            {
                label5.Text = "ON";
                label5.ForeColor = Color.Green;
                moderate = true;
            }
            else
            {
                label5.Text = "OFF";
                label5.ForeColor = Color.Red;
                moderate = false;
            }
           
        }
    }
}
