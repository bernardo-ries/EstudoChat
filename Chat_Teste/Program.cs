using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Chat_Teste.Models;

namespace Chat_Teste
{
    internal class Program
    {
        static void ShowMessage(Message message)
        {
            Console.WriteLine($"[{message.Sender}] :{message.Text}");
        }
        static void Recebedor(object ns1)
        {
            NetworkStream ns = ( NetworkStream ) ns1;
            StreamReader sr = new StreamReader(ns, Encoding.ASCII);
            while (true)
            {
                string json = sr.ReadLine();
                Message message = JsonSerializer.Deserialize<Message>(json);
                ShowMessage(message);
            }
        }
        static void Enviador(object Writer)
        {
            StreamWriter streamWriter = ( StreamWriter ) Writer;
            Stopwatch sw = Stopwatch.StartNew();
            while (true)
            {   
                if(sw.ElapsedMilliseconds < 500)
                {
                    Thread.Sleep((int)(500 - sw.ElapsedMilliseconds));
                }
                Console.WriteLine("Digite sua mensagem:");
                string texto = Console.ReadLine()!;
                Message message = new Message();
                message.Sender = username;
                message.Text = texto;
                
                string json = JsonSerializer.Serialize(message);

                Console.WriteLine(json);


                streamWriter.WriteLine(json);
                streamWriter.Flush();
                sw.Restart();
            }
        }
        const int porta=12345;
        static string username;
        static void Main(string[] args)
        {
            Console.WriteLine("Digite seu nombre hermanito:");
            username = Console.ReadLine();
            Console.WriteLine("Digite 1 para ser o host, e 2 para se conectar: ");
            int opcao = int.Parse(Console.ReadLine());
            if (opcao == 1)
            {
                TcpListener listener = new(IPAddress.Any, porta);
                listener.Start();
                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Conectado a: " + client.Client.RemoteEndPoint.ToString());
                NetworkStream ns = client.GetStream();
                StreamReader streamReader = new StreamReader(ns, Encoding.ASCII);
                StreamWriter streamWriter = new StreamWriter(ns, Encoding.ASCII);
                Thread trtR = new Thread(Recebedor);
                Thread trtE = new Thread(Enviador);
                trtR.Start(streamReader);
                trtE.Start(streamWriter);
                while (true)
                {
                    Thread.Sleep(100);
                }

            }
            else
            {
                Console.WriteLine("Digite o ip do candango: ");
                string hostname = Console.ReadLine();
                TcpClient client = new TcpClient(hostname, porta);
                Console.WriteLine("Conectado a: " + client.Client.RemoteEndPoint.ToString());
                NetworkStream ns = client.GetStream();
                StreamWriter streamWriter = new StreamWriter(ns, Encoding.ASCII);
                Thread trtR = new Thread(Recebedor);
                Thread trtE = new Thread(Enviador);
                trtR.Start(ns);
                trtE.Start(streamWriter);
                while (true)
                {
                    Thread.Sleep(100);
                }
            }
        }
    }
}

//Console.WriteLine("Digite sua mensagem:");
//string texto = Console.ReadLine()!;
//streamWriter.WriteLine(texto);
//streamWriter.Flush();
//Console.WriteLine("Aguardando Mensagem...");
//string texto_2 = streamReader.ReadLine();
//Console.WriteLine(texto_2);
