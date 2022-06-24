# .NET WeatherFlow Tempest API Websocket Example
This project originally started as a proof of concept looking at using .NET Core's native ClientWebSocket class (https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets.clientwebsocket?view=netcore-3.1).  

After a little research into the .NET WebSocket Client and a great article I found on The Codegarden (https://thecodegarden.net/websocket-client-dotnet) that provided the following code it became obvious that I needed a class wrapper providing Async use of the ClientWebSocket class.

```
static async Task Main(string[] args)
{
    do
    {
        using (var socket = new ClientWebSocket())
            try
            {
                await socket.ConnectAsync(new Uri(Connection), CancellationToken.None);

                await Send(socket, "data");
                await Receive(socket);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR - {ex.Message}");
            }
    } while (true);
}

static async Task Send(ClientWebSocket socket, string data) =>
    await socket.SendAsync(Encoding.UTF8.GetBytes(data), 
    WebSocketMessageType.Text, true, CancellationToken.None);

static async Task Receive(ClientWebSocket socket)
{
    var buffer = new ArraySegment<byte>(new byte[2048]);
    do
    {
        WebSocketReceiveResult result;
        using (var ms = new MemoryStream())
        {
            do
            {
                result = await socket.ReceiveAsync(buffer, CancellationToken.None);
                ms.Write(buffer.Array, buffer.Offset, result.Count);
            } while (!result.EndOfMessage);

            if (result.MessageType == WebSocketMessageType.Close)
                break;

            ms.Seek(0, SeekOrigin.Begin);
            using (var reader = new StreamReader(ms, Encoding.UTF8))
                Console.WriteLine(await reader.ReadToEndAsync());
        }
    } while (true);
}
```

Further research lead me to an existing NuGet package solution (Websocket.Client, https://github.com/Marfusios/websocket-client, https://www.nuget.org/packages/Websocket.Client/) with a wrapper class that offers built-in reconnection, error handling and implements Reactive Extensions (Rx) (https://github.com/dotnet/reactive) for composing asynchronous and event-based programs using observable sequences and LINQ-style query operators.

In searching GitHub for WeatherFlow code it became overwhelmingly obvious that there were a ton of Python examples, but little .NET.  Nothing against Python.  Part of my job involves Python coding on IoT devices, but I though it would be useful to post this example for those looking for an alternative.

The example provides both a simple message reader and a targeted detailed message processor.  The code provides for automatic connection/reconnection to the WeatherFlow Smart Weather API WebSocket turning on listening for a device and station.  In the detailed message processing, there are examples of deserializing the message JSON in to known object classes or dynamic class objects.  Several of the message object classes (i.e. LightningStrikeEvent) have code for converting Epoch time to .NET DateTime.

Before running the example you will need to enter the following in to User Secrets for the project.

```
{
  "AppSettings": {
    "Token": "28330565-1b0b-40f2-a34c-bab064ff6032",
    "StationId": 12345,
    "DeviceId": 67890
  }
}
```

In Visual Studio you can do this by right clicking the project in solution explorer and select Manage User Secrets from the menu.  In Visual Studio Code install the .NET Core User Secrets extension by Adrian Wilczyński (adrianwilczynski.user-secrets) and follow the provided instructions.  

You will need to replace the token, station id and device id with your access token, station and device id.  You can get an access token by signing in to https://tempestwx.com/, going to settings, scroll down to More and selecting Data Authorizations.  Your station and device id can be pulled from the URL.  For example, go to weather, select you station (right corner icon) and click on the temperature.  When the graph appears the URL (i.e. https://tempestwx.com/station/12345/graph/67890/temp/2) will contain the station id and device id.

I hope this is of some help to those using the WeatherFlow Smart Weather API WebSocket (https://weatherflow.github.io/Tempest/api/ws.html) or wanting to understand WebSocket processing in .NET core.  If you have any question or issues with the example please contact me through the GitHub issues feature.
