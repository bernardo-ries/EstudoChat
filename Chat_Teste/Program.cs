using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Chat_Teste
{
    internal class Program
    {
        static void Recebedor(object Reader)
        {
            StreamReader streamReader = ( StreamReader ) Reader;
            while (true)
            {
                Console.WriteLine("Mensagem do amigo:");
                string texto_2 = streamReader.ReadLine();
                Console.WriteLine(texto_2);
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
                streamWriter.WriteLine(texto);
                streamWriter.Flush();
                sw.Restart();
            }
        }
        const int porta=12345;
        static void Main(string[] args)
        {
            
            TcpListener listener = new(IPAddress.Any,porta);
            listener.Start();
            TcpClient client = listener.AcceptTcpClient();
            Console.WriteLine("Conectado a: "+client.Client.RemoteEndPoint.ToString());
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
    }
}

//Console.WriteLine("Digite sua mensagem:");
//string texto = Console.ReadLine()!;
//streamWriter.WriteLine(texto);
//streamWriter.Flush();
//Console.WriteLine("Aguardando Mensagem...");
//string texto_2 = streamReader.ReadLine();
//Console.WriteLine(texto_2);
