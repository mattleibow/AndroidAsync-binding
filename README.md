# AndroidAsync for Xamarin.Android

[![Build status](https://ci.appveyor.com/api/projects/status/2iakdke5xn4nfq6c?svg=true)](https://ci.appveyor.com/project/mattleibow/androidasync-binding)

*Xamarin.Android binding for AndroidAsync (https://github.com/koush/AndroidAsync)*

AndroidAsync is a low level network protocol library. If you're looking for a raw Socket, 
HTTP client/server, WebSocket, and Socket.IO library for Android, AndroidAsync is it.

## Features

 * Based on NIO. One thread, driven by callbacks. Highly efficient.
 * All operations return a Future that can be cancelled
 * Socket client + socket server
 * HTTP client + server
 * WebSocket client + server

## Download

Download [the latest NuGet](https://www.nuget.org/packages/AndroidAsync/):

```bash
 PM> Install-Package AndroidAsync
```

## Using AndroidAsync

### Downloading Strings

```csharp
string url = "..."; // the URL of the string to download
AsyncHttpClient.DefaultInstance.GetString(url, (ex, response, str) => {
    if (ex != null) {
        Console.WriteLine(ex);
        return;
    }

    Console.WriteLine("Downloaded a string: " + str);
});
```

### Downloading Files

```csharp
string url = "..."; // the URL of the file to download from
string filename = "..."; // the location on the device to download to
AsyncHttpClient.DefaultInstance.GetFile(url, filename, (ex, response, file) => {
    if (ex != null) {
        Console.WriteLine(ex);
        return;
    }
    
    Console.WriteLine("Downloaded a file to: " + file.AbsolutePath);
});
```

### Supporting Caching

```csharp
// arguments are:
// 1. the http client 
// 2. the directory to store cache files 
// 3. the size of the cache in bytes
ResponseCacheMiddleware.AddCache(AsyncHttpClient.DefaultInstance,
                                 GetFileStreamPath("asynccache"),
                                 1024 * 1024 * 10);
```

### Creating Web Sockets:

```csharp
string url = "wss://echo.websocket.org"; // the web socket URL
string protocol = "my-protocol";
AsyncHttpClient.DefaultInstance.Websocket(url, protocol, (ex, webSocket) => {
    if (ex != null) {
        Console.WriteLine(ex);
        return;
    }
    
    // setting up callbacks
    webSocket.SetStringCallback((str) => {
        Console.WriteLine("Received a string: " + str);
    });

    // send data
    webSocket.Send("a string");
});
```

### Sending Multipart/Form Data

```csharp
AsyncHttpPost post = new AsyncHttpPost("http://my.server.com/postform.html");
MultipartFormDataBody body = new MultipartFormDataBody();
body.AddFilePart("my-file", new Java.IO.File("/path/to/file.txt"));
body.AddStringPart("foo", "bar");
post.Body = body;

AsyncHttpClient.DefaultInstance.ExecuteString (post, (ex, response, str) => {
    if (ex != null) {
        Console.WriteLine(ex);
        return;
    }

    Console.WriteLine("Downloaded a string: " + str);
});
```
