using System;

namespace HTTPServer
{
    public class Program
    {
        static void Main(string[] args)
        {
            HTTPServer server = new HTTPServer(443);
            server.Start();
        }
    }
}
