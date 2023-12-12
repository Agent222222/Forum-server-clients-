using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

class Server
{
    private static IPAddress ipAddress = IPAddress.Parse("10.10.10.104"); // Замініть на свій статичний IP
    private const int port = 12345;

    static async Task Main(string[] args)
    {
        TcpListener server = new TcpListener(ipAddress, port);
        server.Start();
        Console.WriteLine($"Сервер слухає на {ipAddress}:{port}");

        while (true)
        {
            TcpClient client = await server.AcceptTcpClientAsync();
            _ = HandleClientAsync(client);
        }
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

                        for (int i = 0; i< nonEmptyWords.Count(); i+=2)
                        {
                            if (check[1] == nonEmptyWords[i] && check[2] == nonEmptyWords[i+1])
                            {
                                string saved = File.ReadAllText("received_data.txt");

                                ch++;
                                byte[] response = Encoding.UTF8.GetBytes($"1 {saved}");
                                Console.WriteLine($"Вивід на екран клієнта: {response}");
                                await stream.WriteAsync(response, 0, response.Length);
                            }
                        }

                       
                        if(ch == 0)
                        {
                            byte[] response = Encoding.UTF8.GetBytes($"0 log in error");
                            Console.WriteLine($"Вивід помилки на екран клієнта: {response}");
                            await stream.WriteAsync(response, 0, response.Length);
                        }

                    }else if(data[0] == 'S')
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
                                byte[] response = Encoding.UTF8.GetBytes($"0 sign in error");
                                Console.WriteLine($"Вивід на екран клієнта: {response}");
                                await stream.WriteAsync(response, 0, response.Length);
                                
                            }
                        }

                        if (ch == 0)
                        {
                            string modifiedData = data.Substring(2);
                            System.IO.File.AppendAllText("users_data.txt", $"\n{modifiedData}");
                            string saved = File.ReadAllText("received_data.txt");

                            byte[] response = Encoding.UTF8.GetBytes($"1 {saved}");
                            Console.WriteLine($"Вивід на екран клієнта: {response}");
                            await stream.WriteAsync(response, 0, response.Length);
                        }
                    }
                    else if (data[0] == 'T')
                    {
                        int ch = 0;
                        string filePath = "received_data.txt";
                        string modifiedData = data.Substring(2);
                        System.IO.File.AppendAllText(filePath, $"\n{modifiedData}");

                        byte[] response = Encoding.UTF8.GetBytes($"{modifiedData}");
                        Console.WriteLine($"Вивід на екран клієнта: {response}");
                        await stream.WriteAsync(response, 0, response.Length);

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
            client.Close();
        }
    }

    static void SaveDataToFile(string data)
    {
        try
        {
            // Збереження даних у файл
            string filePath = "received_data.txt";
            System.IO.File.AppendAllText(filePath, $"{DateTime.Now}: {data}\n");
            Console.WriteLine($"Дані збережено в файлі: {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка збереження даних у файл: {ex.Message}");
        }
    }
}