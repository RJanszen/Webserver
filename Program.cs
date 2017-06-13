using System;

namespace HTTPServer
{
    public class Program
    {
        static void Main(string[] args) // Start a new server on localhost:443
        {
            HTTPServer server = new HTTPServer(443);
            server.Start();
        }
    }
}
