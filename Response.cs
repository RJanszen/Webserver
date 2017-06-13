using System;
using System.IO;
using System.Net.Sockets;

namespace HTTPServer
{
    public class Response
    {
        // Response parameter definitions
        private byte[] content = null;
        private string serverStatus;
        private string timeStamp;
        private static string mimeType;

        // Response constructor
        private Response(string status, string mime, byte[] data)
        {
            serverStatus = status;
            content = data;
            DateTime time = DateTime.Now;
            timeStamp = time.ToString("ddd, dd MMM yyyy HH:mm:ss G\\M\\T");
        }

        // From a request, establish a response
        public static Response From(Request request)
        {
            if (request == null)
                return MakeNullRequest(); // If request is empty -> 400 bad request

            if (request.Type=="GET")
            {
                string file = Environment.CurrentDirectory + HTTPServer.WEB_DIR + request.URL; // Find which file is requested
                mimeType = GetMimeType(file); // Get mimetype (jpg, txt, etc.)
                FileInfo f = new FileInfo(file);
                if (f.Exists && f.Extension.Contains("."))
                {
                    return MakeFromFile(f); // Go to file fetcher and return it as binary 200 response
                }
                else
                {
                    DirectoryInfo di = new DirectoryInfo(f + "/");
                    if (!di.Exists) // If file doesn't exist -> 404 not found
                        return MakePNFRequest();
                    FileInfo[] files = di.GetFiles();
                    foreach(FileInfo ff in files)
                    {
                        string n = ff.Name;
                        if (n.Contains("default.html") || n.Contains("default.htm") || n.Contains("index.html") || n.Contains("index.htm"))
                            return MakeFromFile(ff); // Find a default page to return as 200 if it's unspecified
                    }
                    return serverError(); // If all fails -> 500 server error
                }
            }
            else
                return serverError(); // If all fails -> 500 server error
        }

        // Get file and make 200 response message with binary data
        private static Response MakeFromFile(FileInfo f)
        {
            FileStream fs = f.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] data = new Byte[fs.Length];
            reader.Read(data, 0, data.Length);
            fs.Close();
            return new Response("200 OK", "html/text", data);
        }

        private static Response MakeNullRequest()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "400.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fs = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] data = new Byte[fs.Length];
            reader.Read(data, 0, data.Length);
            fs.Close();
            return new Response("400 Bad Request", "html/text", data);
        }

        private static Response MakePNFRequest()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "404.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fs = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] data = new Byte[fs.Length];
            reader.Read(data, 0, data.Length);
            fs.Close();
            return new Response("404 Page Not Found", "html/text", data);
        }

        private static Response serverError()
        {
            String file = Environment.CurrentDirectory + HTTPServer.MSG_DIR + "500.html";
            FileInfo fileInfo = new FileInfo(file);
            FileStream fs = fileInfo.OpenRead();
            BinaryReader reader = new BinaryReader(fs);
            Byte[] data = new Byte[fs.Length];
            reader.Read(data, 0, data.Length);
            fs.Close();
            return new Response("500 Server Error", "html/text", data);
        }

        // Get mimetype based on extension
        private static string GetMimeType(string fileName)
        {
            string mimeType = "application/unknown";
            string ext = System.IO.Path.GetExtension(fileName).ToLower();
            Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext); // Fetch from registry
            if (regKey != null && regKey.GetValue("Content Type") != null)
                mimeType = regKey.GetValue("Content Type").ToString(); // Set mimetype correctly if possible
            return mimeType;
        }

        // Post (flush) the reponse to the networkstream (client)    
        public void Post(NetworkStream stream)
        {
            StreamWriter writer = new StreamWriter(stream);
            // The response message:
            Console.WriteLine(string.Format("{0} {1}\r\nDate: {2}\r\nServer: {3}\r\nConnection: close\r\nContent-Type: {4}\r\nContent-Length: {5}\r\n",
                HTTPServer.VERSION, serverStatus, timeStamp, HTTPServer.SERVERNAME, mimeType, content.Length));
            writer.WriteLine(string.Format("{0} {1}\r\nDate: {2}\r\nServer: {3}\r\nConnection: close\r\nContent-Type: {4}\r\nContent-Length: {5}\r\n",
                HTTPServer.VERSION, serverStatus, timeStamp, HTTPServer.SERVERNAME, mimeType, content.Length));

            writer.Flush();
            try
            {
                stream.Write(content, 0, content.Length); // The content
            }
            catch
            {
                Console.WriteLine("Server Error - Failed to write datastream.");
                serverError();
            }
            
            writer.Close();
        }
    }
}
