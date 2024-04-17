using System.Text.Json;

static void TestJSON() {
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    string text = File.ReadAllText("config.json");
    var config = JsonSerializer.Deserialize<Config>(text, options);

    Console.WriteLine($"MimeMappings:{config.MimeTypes [".html"]}");
    Console.WriteLine($"IndexFiles:{config.IndexFiles[0]}");
}
static void TestJSON2() {
    var options = new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    };

    string text = File.ReadAllText(@"json/books.json");
    var books = JsonSerializer.Deserialize<List<Book>>(text, options);

    Book book = books[4];
    Console.WriteLine($"title: {book.Title}");
    Console.WriteLine($"authors: {book.Authors[0]}");
}

static void TestServer() {
    SimpleHTTPServer server = new SimpleHTTPServer("files", 8080,"config.json");
    string helpMessage = @"Server started. You can try the following commands:
    stop - stop the server
";
    Console.WriteLine(helpMessage);
    while (true)
    {
        // read line from console
        String command = Console.ReadLine();
        if (command.Equals("stop"))
        {
            server.Stop();
            break;
        }else if(command.Equals("test")){
              TestJSON();
        }else if(command.Equals("numreqs")){
            Console.WriteLine(server.NumRequests);
        }else if(command.Equals("paths")){
           foreach(var pat in server.Paths){
              Console.WriteLine($"{pat.Key}: {pat.Value}");
           }
        }else if(command.Equals("404paths")){
           foreach(var pat in server.FOFPaths){
              Console.WriteLine($"URL {pat.Key} has {pat.Value} number of requests so far.");
           }
        }else if(command.Equals("category")){
           foreach(var pat in server.CategorySearch){
              Console.WriteLine($"{pat.Key} has {pat.Value} number of requests so far.");
           }
        }else{
            Console.WriteLine($"{command} is an invalid Command ");
        }
    }
}

//TestJSON();
TestServer();
