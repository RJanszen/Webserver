using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace HTTPServer
{
    public class HTTPServer
    {
        public const string MSG_DIR = "/root/msg/";
        public const string WEB_DIR = "/root/web/";

        public const string VERSION = "HTTP/1.1";
        public const string SERVERNAME = "Sogyo webserver v1";
        public const string METHOD = "GET";

        private bool running = false;

        private TcpListener listener;


        public HTTPServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        private void Run()
        {
            running = true;
            listener.Start();

            while(running)
            {
                Console.WriteLine("Waiting for connection");

                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connected");

                HandleClient(client);
                client.Close();
            }

            running = false;

            listener.Stop();
        }

        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            String msg = "";
            try
            {
                while (reader.Peek() != -1)
                {
                    msg += reader.ReadLine() + "\n";
                }
            }
            catch
            {
                Console.WriteLine("Couldn't read request form.");
            }

            Debug.WriteLine("Request: \n" + msg);

            Request rq = Request.GetRequest(msg);
            Response rs = Response.From(rq);
            rs.Post(client.GetStream());
        }
    }
}
