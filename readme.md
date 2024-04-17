C# web server
---
1. listen and respond with a basic web page
1. serve html from a folder
    * does this handle pages with img tags?
    * customize 404 pages
    * customize 500 pages
1. command line options for the server
    * number of requests
    * port number
    * change folder we are serving
    * restrict files
    * max/mean times to process requests
    * most popular files
    * dynamically load a class???
1. factor out how to send an HTTP response
1. handle mime types with a Dictionary and a config file
    * csv or json?
1. handle all server config with properties or json file
    * probably json because it's more common
1. fake servlets
    * list data from a json file as HTML

Threads and clients
===
1. build a simple client to connect
1. build a client with a 10 second delay 
    * show that this is very slow...
1. create a new thread for each request
1. build an evil client that creates dozens of requests
1. performance hit for creating new threads
    * spinning up/down a thread is not free
1. thread pool
1. 