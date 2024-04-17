// SimpleServer based on code by Can Güney Aksakalli
// MIT License - Copyright (c) 2016 Can Güney Aksakalli
// https://aksakalli.github.io/2014/02/24/simple-http-server-with-csparp.html
// modifications by Jaime Spacco

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Web;
using System.Text.Json;


/// <summary>
/// Interface for simple servlets.
/// 
/// </summary>
interface IServlet {
    void ProcessRequest(HttpListenerContext context);
}
/// <summary>
/// BookHandler: Servlet that reads a JSON file and returns a random book
/// as an HTML table with one row.
/// TODO: search for specific books by author or title or whatever
/// </summary>
class BookHandler : IServlet {
    public void ProcessRequest(HttpListenerContext context) {
        // we want to use case-insensitive matching for the JSON properties
        // the json files use lowercae letters, but we want to use uppercase in our C# code
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string text = File.ReadAllText(@"json/books.json");
        var books = JsonSerializer.Deserialize<List<Book>>(text, options);
         
        string cmd = context.Request.QueryString["cmd"]; 
        if(cmd.Equals("list")){
             int start = Int32.Parse(context.Request.QueryString["s"]);
             int end = Int32.Parse(context.Request.QueryString["e"]);
             
             List<Book> sublist = books.GetRange(start, end-start);
             string response = $@"
                    <table border=1>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Short Description</th>
                        <th>Long Description</th>
                    </tr>";
                 foreach (Book book2 in sublist){
                    string authors5 = String.Join(",<br>",book2.Authors); 
                     response +=$@"
                        <tr>
                            <td>{book2.Title}</td>
                            <td>{authors5}</td>
                            <td>{book2.ShortDescription}</td>
                            <td>{book2.LongDescription}</td>
                        </tr>
                        ";
                 }     
                 response += "</table>";
                 byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response);
                 context.Response.ContentType = "text/html";
                 context.Response.ContentLength64 = bytes.Length;
                 context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                 context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                 context.Response.StatusCode = (int)HttpStatusCode.OK;
                 context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                 context.Response.OutputStream.Flush();
                 context.Response.OutputStream.Close();           
        }else if(cmd.Equals("random")){
               Random rand = new Random();
               int index = rand.Next(books.Count);
               Book book4 = books[index];
               string authors2 = String.Join(", <br>", book4.Authors);
               string response = $@"
                    <table border=1>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Short Description</th>
                        <th>Long Description</th>
                    </tr>
                    <tr>
                            <td>{book4.Title}</td>
                            <td>{authors2}</td>
                            <td>{book4.ShortDescription}</td>
                            <td>{book4.LongDescription}</td>
                    </tr>
                        </table>
                        ";  
                 response += "</table>";
                        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response);
                        context.Response.ContentType = "text/html";
                        context.Response.ContentLength64 = bytes.Length;
                        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                        context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                        context.Response.StatusCode = (int)HttpStatusCode.OK;
                        context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                        context.Response.OutputStream.Flush();
                        context.Response.OutputStream.Close();
        } 

        // grab a random book
        int bookNum = 0;
        if(context.Request.QueryString.AllKeys.Contains("n")){
               bookNum = Int32.Parse(context.Request.QueryString["n"]);
        }
        Book book = books[bookNum];

        // convert book.Authors, which is a list, into a string with ", <br>" in between each author
        // string.Join() is a very useful method
        string delimiter = ",<br> ";
        string authors = string.Join(delimiter, book.Authors);

        // build the HTML response
        // @ means a multiline string (Java doesn't have this)
        // $ means string interpolation (Java doesn't have this either)
        
       
        // write HTTP response to the output stream
        // all of the context.response stuff is setting the headers for the HTTP response
        

    }
}
/// <summary>
/// FooHandler: Servlet that returns a simple HTML page.
/// </summary>
class FooHandler : IServlet {

    public void ProcessRequest(HttpListenerContext context) {
        string response = $@"
            <H1>This is a Servlet Test.</H1>
            <h2>Servlets are a Java thing; there is probably a .NET equivlanet but I don't know it</h2>
            <h3>I am but a humble Java programmer who wrote some Servlets in the 2000s</h3>
            <p>Request path: {context.Request.Url.AbsolutePath}</p>
";
        foreach ( String s in context.Request.QueryString.AllKeys )
            response += $"<p>{s} -> {context.Request.QueryString[s]}</p>\n";

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response);

        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = bytes.Length;
        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
        context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        context.Response.OutputStream.Write(bytes, 0, bytes.Length);

        context.Response.OutputStream.Flush();
        context.Response.OutputStream.Close();
    }
}

class fourHandler : IServlet {

    public void ProcessRequest(HttpListenerContext context) {
        string response = $@"
    <!DOCTYPE html>
    <html lang='en'>

    <head>
         <title>404 Error</title>
        <style>
            body {{
                font-family: Arial, sans-serif;
                background-color: #f7f7f7;
                text-align: center;
            }}
            .cont {{
                background-color: #ffffff;
                border-radius: 10px;
                box-shadow: 0px 0px 10px 0px rgba(0, 0, 0, 0.1);
                padding: 30px;
            }}

            h1 {{
                color: #FF5733;
                font-size: 36px;
            }}

            h2,h3 {{
                color: #333;
                font-size: 24px;
            }}


        </style>
    </head>

    <body>
        <div class='cont'>
            <h1>404 Error!</h1>
            <h2>You are seeing this page because you have entered an incorrect URL or path.</h2>
            <h3>Please fix the typos and re-enter the URL again.</h3>
        </div>
    </body>

    </html>
";

        foreach ( String s in context.Request.QueryString.AllKeys )
            response += $"<p>{s} -> {context.Request.QueryString[s]}</p>\n";

        byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response);

        context.Response.ContentType = "text/html";
        context.Response.ContentLength64 = bytes.Length;
        context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
        context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
        context.Response.StatusCode = (int)HttpStatusCode.OK;

        context.Response.OutputStream.Write(bytes, 0, bytes.Length);

        context.Response.OutputStream.Flush();
        context.Response.OutputStream.Close();
    }
}


class SimpleHTTPServer
{
    // bind servlets to a path
    // for example, this means that /foo will be handled by an instance of FooHandler
    // TODO: put these mappings into a configuration file
    private IDictionary<string, IServlet> _servlets = new Dictionary<string, IServlet>() {
        {"foo", new FooHandler()},
        {"books", new BookHandler()},
        {"fourOfour", new fourHandler()},
        {"filter", new filterHandler()},
        {"category", new categoryHandler()}
    };

    // list of default index files
    // if the client requests a directory (e.g. http://localhost:8080/), 
    // we will look for one of these files
    private string[] _indexFiles;
    
    // map extensions to MIME types
    // TODO: put this into a configuration file
    private static IDictionary<string, string> _mimeTypeMappings;

    // instance variables
    private Thread _serverThread;
    private string _rootDirectory;
    private HttpListener _listener;
    private int _port;
    private bool _done = false;
    private int _numreqs = 0;
    private Dictionary<string, int> urls = new Dictionary<string, int>(); 
    private Dictionary<string, int> numFOF = new Dictionary<string, int>(); 
    public static Dictionary<string, int> categoryL = new Dictionary<string, int>();
    
    public int Port
    {
        get { return _port; }
        private set { }
    }

    public int NumRequests
    {
        get{return _numreqs;}
        private set {_numreqs=value;}
    }

    public Dictionary<string,int> Paths
    {
        get{return urls;}
    }

    public Dictionary<string,int> CategorySearch
    {
        get{return categoryL;}
    }


     public Dictionary<string,int> FOFPaths
    {
        get{return numFOF;}
    }

    


    /// <summary>
    /// Construct server with given port.
    /// </summary>
    /// <param name="path">Directory path to serve.</param>
    /// <param name="port">Port of the server.</param>
    public SimpleHTTPServer(string path, int port, string configFilename)
    {
        this.Initialize(path, port, configFilename);
    }

    /// <summary>
    /// Construct server with any open port.
    /// </summary>
    /// <param name="path">Directory path to serve.</param>
    public SimpleHTTPServer(string path, string configFilename)
    {
        //get an empty port
        TcpListener l = new TcpListener(IPAddress.Loopback, 0);
        l.Start();
        int port = ((IPEndPoint)l.LocalEndpoint).Port;
        l.Stop();
        this.Initialize(path, port, configFilename);
    }

    /// <summary>
    /// Stop server and dispose all functions.
    /// </summary>
    public void Stop()
    {
        _done = true;
        _listener.Close();
    }

    private void Listen()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:" + _port.ToString() + "/");
        _listener.Start();
        while (!_done)
        {
            Console.WriteLine("Waiting for connection...");
            try
            {
                NumRequests++;
                HttpListenerContext context = _listener.GetContext();
                Process(context);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        Console.WriteLine("Server stopped!");
    }

    /// <summary>
    /// Process an incoming HTTP request with the given context.
    /// </summary>
    /// <param name="context"></param>
    private void Process(HttpListenerContext context)
    {
        string filename = context.Request.Url.AbsolutePath;
        filename = filename.Substring(1);
        Console.WriteLine($"{filename} is the path");
        
        urls[filename] = urls.GetValueOrDefault(filename,0) +1;
        Console.WriteLine($"{filename} is added to the map");

        // check if the path is mapped to a servlet
        if (_servlets.ContainsKey(filename))
        {
            _servlets[filename].ProcessRequest(context);
            return;
        }

        // if the path is empty (i.e. http://blah:8080/ which yields hte path /)
        // look for a default index filename
        if (string.IsNullOrEmpty(filename))
        {
            foreach (string indexFile in _indexFiles)
            {
                if (File.Exists(Path.Combine(_rootDirectory, indexFile)))
                {
                    filename = indexFile;
                    break;
                }
            }
        }

        // search for the file in the root directory
        // this means we are serving the file, if we can find it
        filename = Path.Combine(_rootDirectory, filename);

        if (File.Exists(filename))
        {
            try
            {
                Stream input = new FileStream(filename, FileMode.Open);
                
                //Adding permanent http response headers
                string mime;
                context.Response.ContentType = _mimeTypeMappings.TryGetValue(Path.GetExtension(filename), out mime) ? mime : "application/octet-stream";
                context.Response.ContentLength64 = input.Length;
                context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                context.Response.AddHeader("Last-Modified", System.IO.File.GetLastWriteTime(filename).ToString("r"));

                byte[] buffer = new byte[1024 * 16];
                int nbytes;
                while ((nbytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    context.Response.OutputStream.Write(buffer, 0, nbytes);
                input.Close();
                
                context.Response.StatusCode = (int)HttpStatusCode.OK;
                context.Response.OutputStream.Flush();
                context.Response.OutputStream.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            }

        }
        
        else
        {
            // This sends a 404 if the file doesn't exist or cannot be read
            // TODO: customize the 404 page
            //context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            numFOF[filename] = numFOF.GetValueOrDefault(filename,0) +1;
            Console.WriteLine($"{filename} is added to the 404 map");
            _servlets["fourOfour"].ProcessRequest(context);
            return;
        }
        
        context.Response.OutputStream.Close();
    }

    /// <summary>
    /// Initializes the server by setting up a listener thread on the given port
    /// </summary>
    /// <param name="path">the path of the root directory to serve files</param>
    /// <param name="port">the port to listen for connections</param>
    private void Initialize(string path, int port, string configFilename)
    {
        this._rootDirectory = path;
        this._port = port;
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string text = File.ReadAllText(configFilename);
        var config = JsonSerializer.Deserialize<Config>(text, options);

        _mimeTypeMappings = config.MimeTypes;
        _indexFiles = config.IndexFiles.ToArray();

        _serverThread = new Thread(this.Listen);
        _serverThread.Start();
    }
  }
  class categoryHandler : IServlet {
    public void ProcessRequest(HttpListenerContext context) {
        // we want to use case-insensitive matching for the JSON properties
        // the json files use lowercae letters, but we want to use uppercase in our C# code
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string text = File.ReadAllText(@"json/books.json");
        var books = JsonSerializer.Deserialize<List<Book>>(text, options);
       
      //Uri uri = new Uri(context);

        string[] segments = context.Request.Url.AbsolutePath.Split('/');

        // Get the last segment
        //string lastSegment = segments.LastOrDefault();
 
        string a = context.Request.QueryString["c"]; 
        
        if(1<2){
             //string n = context.Request.QueryString["n"];
             
             List<Book> sublist9 = books.Where(book => book.Categories.Any(categorie => categorie.ToLower().Contains(a.ToLower()))).ToList();
             Console.WriteLine(sublist9[0]);
             string response9 = $@"
                    <table border=1>
                    <tr>
                        <th>Title</th>
                        <th>Category</th>
                        <th>Short Description</th>
                        <th>Long Description</th>
                    </tr>";
                 foreach (Book book9 in sublist9){
                    string cat9 = String.Join(",<br>",book9.Categories); 
                    SimpleHTTPServer.categoryL[cat9] = SimpleHTTPServer.categoryL.GetValueOrDefault(cat9,0) +1;
                    
                     response9 +=$@"
                        <tr>
                            <td>{book9.Title}</td>
                            <td>{cat9}</td>
                            <td>{book9.ShortDescription}</td>
                            <td>{book9.LongDescription}</td>
                        </tr>
                        ";
                 }     
                 response9 += "</table>";
                 byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response9);
                 context.Response.ContentType = "text/html";
                 context.Response.ContentLength64 = bytes.Length;
                 context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                 context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                 context.Response.StatusCode = (int)HttpStatusCode.OK;
                 context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                 context.Response.OutputStream.Flush();
                 context.Response.OutputStream.Close();           
        }
    }
    
  }
  class filterHandler : IServlet {
    public void ProcessRequest(HttpListenerContext context) {
        // we want to use case-insensitive matching for the JSON properties
        // the json files use lowercae letters, but we want to use uppercase in our C# code
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        string text = File.ReadAllText(@"json/books.json");
        var books = JsonSerializer.Deserialize<List<Book>>(text, options);
       
      //Uri uri = new Uri(context);

        string[] segments = context.Request.Url.AbsolutePath.Split('/');

        // Get the last segment
        //string lastSegment = segments.LastOrDefault();
 
        string a = context.Request.QueryString["a"]; 
        
        if(1<2){
             //string n = context.Request.QueryString["n"];
             
             List<Book> sublist9 = books.Where(book => book.Authors.Any(author => author.ToLower().Contains(a.ToLower()))).ToList();
             Console.WriteLine(sublist9[0]);
             string response9 = $@"
                    <table border=1>
                    <tr>
                        <th>Title</th>
                        <th>Author</th>
                        <th>Short Description</th>
                        <th>Long Description</th>
                    </tr>";
                 foreach (Book book9 in sublist9){
                    string authors9 = String.Join(",<br>",book9.Authors); 
                     response9 +=$@"
                        <tr>
                            <td>{book9.Title}</td>
                            <td>{authors9}</td>
                            <td>{book9.ShortDescription}</td>
                            <td>{book9.LongDescription}</td>
                        </tr>
                        ";
                 }     
                 response9 += "</table>";
                 byte[] bytes = System.Text.Encoding.UTF8.GetBytes(response9);
                 context.Response.ContentType = "text/html";
                 context.Response.ContentLength64 = bytes.Length;
                 context.Response.AddHeader("Date", DateTime.Now.ToString("r"));
                 context.Response.AddHeader("Last-Modified", DateTime.Now.ToString("r"));
                 context.Response.StatusCode = (int)HttpStatusCode.OK;
                 context.Response.OutputStream.Write(bytes, 0, bytes.Length);
                 context.Response.OutputStream.Flush();
                 context.Response.OutputStream.Close();           
        }
    
    
  }

  
} 
