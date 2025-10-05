using System.Net.Sockets;
using System.Net;
using System.Text;
using System.ComponentModel;

namespace Server
{
    internal class Program
    {   
        

        static int porta = 12345;
        static List<TcpClient> tcpClients = [];
        static async Task Main(string[] args)
        {
            // enquanto pra sempre
            //   aceita conexoes
            //   adiciona usuario na lista de conectados
            //   _ = HandleClient(client)
            TcpListener listener = new(IPAddress.Any, porta);
            listener.Start();
            while (true)
            {
                Console.WriteLine("Esperando conexao");
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Conectado a: " + client.Client.RemoteEndPoint.ToString());
                tcpClients.Add(client);
                HandleClient(client);
            }

        }

        private static async Task HandleClient(TcpClient tcpClient)
        {
            
            NetworkStream ns = tcpClient.GetStream();
            StreamReader streamReader = new StreamReader(ns, Encoding.ASCII);
            while (tcpClient.Connected)
            {
                Console.WriteLine("Esperando mensagem de " + tcpClient.Client.RemoteEndPoint.ToString());
                string json = await streamReader.ReadLineAsync();
                Console.WriteLine(json);
                foreach(TcpClient user in tcpClients)
                {
                    if(user == tcpClient)
                    {
                        continue;
                    }
                    Console.WriteLine("Enviando Mensagem para: "+ user.Client.RemoteEndPoint.ToString());
                    NetworkStream ns2 = user.GetStream();
                    StreamWriter sw = new StreamWriter(ns2, Encoding.ASCII, leaveOpen:true);
                    sw.WriteLine(json);
                    sw.Dispose();
                    await ns2.FlushAsync();
                    
                }
                Console.WriteLine("Mensagem enviada");
            }
            streamReader.Dispose();
            tcpClients.Remove(tcpClient);
            Console.WriteLine("Usuario desconectado");
        }

        // funcao Handle Client(TcpClient)
        // espera por mensagens
        // quando receber, envia pra todos da lista de conectados
    }
}
