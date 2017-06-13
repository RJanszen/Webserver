namespace HTTPServer
{
    public class Request
    {
        // Request parameter definitions
        public string Type { get; set; }
        public string URL { get; set; }
        public string Host { get; set; }
        public string Referer { get; set; }
        private static int counter = 0;

        // Request constructor
        private Request(string type, string url, string host, string referer)
        {
            Type = type;
            URL = url;
            Host = host;
            Referer = referer;
            toFile();
        }

        // Log each request to "test.txt"
        private void toFile()
        {
            // Compose a string that consists of three lines.
            counter++;
            string lines = "Host: " + this.Host + "\nReferer: " + this.Referer + "\nCount: " + counter;

            // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter("test.txt");
            file.WriteLine(lines);

            file.Close();
        }

        // Extract headers 'type', 'url', 'host', and 'referer' from the request, and set those parameters
        public static Request GetRequest(string request)
        {
            if (string.IsNullOrEmpty(request))
                return null;

            string[] tokens = request.Split(' ');
            string type = tokens[0];
            string url = tokens[1];
            string host = tokens[4];
            string referer = "";

            for (int i = 0; i < tokens.Length; i++)
            {
                if (tokens[i] == "Referer:")
                {
                    referer = tokens[i + 1];
                    break;
                }
            }
            return new Request(type, url, host, referer);
        }
        
    }
}
