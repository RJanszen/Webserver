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
        // Define the folder locations for response messages
        public const string MSG_DIR = "/root/msg/";
        public const string WEB_DIR = "/root/web/";

        // Define the main headers
        public const string VERSION = "HTTP/1.1";
        public const string SERVERNAME = "Sogyo webserver v1";
        public const string METHOD = "GET";

        private bool running = false;

        private TcpListener listener;

        // Start listening on port ..
        public HTTPServer(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
        }

        // Start new thread, so different clients can be handled simultaneously
        public void Start()
        {
            Thread serverThread = new Thread(new ThreadStart(Run));
            serverThread.Start();
        }

        // New thread
        private void Run()
        {
            running = true;
            listener.Start();

            while(running)
            {
                Console.WriteLine("Waiting for connection");

                // Client connected?
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connected");

                HandleClient(client); // Handle the request
                client.Close(); // Close Tcp connection after handling
            }

            running = false;
            listener.Stop();
        }

        // Handle client
        private void HandleClient(TcpClient client)
        {
            StreamReader reader = new StreamReader(client.GetStream());

            String msg = "";
            try // Try to extract the request message (all headers and parameters)
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

            Debug.WriteLine("Request: \n" + msg); // Print each line

            Request rq = Request.GetRequest(msg); // Process the request (stored in String msg)
            Response rs = Response.From(rq); // Form a response from this request
            rs.Post(client.GetStream()); // Post the response
        }
    }
}
