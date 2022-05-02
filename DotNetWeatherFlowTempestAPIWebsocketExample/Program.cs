using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reflection;
using Websocket.Client;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace WeatherFlowSmartWeatherAPIWebsocketDotNetExample
{
    /// <summary>
    /// A WeatherFlow Smart Weather API Websocket .Net example.
    /// </summary>
    /// <remarks>
    /// This is an example of techniques for accessing the WeatherFlow Smart Weather API Websocket. 
    /// https://weatherflow.github.io/SmartWeather/api/ws.html
    /// 
    /// The example uses the WebSocket .NET Client (Websocket.Client, https://github.com/Marfusios/websocket-client, https://www.nuget.org/packages/Websocket.Client/)
    /// which a is wrapper over the native .NET ClientWebSocket class (https://docs.microsoft.com/en-us/dotnet/api/system.net.websockets.clientwebsocket?view=netcore-3.1).
    /// The class offers built-in reconnection, error handling and implements Reactive Extensions (Rx) (https://github.com/dotnet/reactive) for composing asynchronous
    /// and event-based programs using observable sequences and LINQ-style query operators.
    /// </remarks>
    class Program
    {
        /// <summary>
        /// The entry point of a C# application.
        /// </summary>
        /// <param name="args">Command-line arguments</param>
        static void Main(string[] args)
        {
            #region Get application settings.
            // This section of code is a lot of work to implement the configuration in ASP.NET Core (https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1).
            // This allows the application to pull things like your access token, station id and device id from settings files, such as appsettings.json, app secrets
            // (https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-3.1&tabs=windows), environment variables, Azure Key Vault, Azure App Configuration,
            // and Command-line arguments.

            // I especially made it a point to implement app secrets so that you might not check in your access token. Remember that once you commit code to a Git repository with
            // secrets like passwords and access tokens there is no going back and erasing them.  Check out https://geekflare.com/github-credentials-scanner/ or Google scan git for passwords.

            // Determine Environment (not a console application thing).
            var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            // Environment can also be passes as the first argument.
            if (args != null && args.Count() == 1)
                environmentName = args[0];

            // And if not found it defaults to Development.
            if (string.IsNullOrWhiteSpace(environmentName))
                environmentName = "Development";

            // Get appsettings file(s).
            // ReSharper disable once InconsistentNaming
            var builder = new ConfigurationBuilder()
                .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))
                .AddJsonFile($"appsettings.json", true, true)
                .AddJsonFile($"appsettings.{environmentName}.json", true, true);

            if (environmentName.Equals("Development"))
            {
                builder.AddUserSecrets<Program>();
            }
                        
            builder.AddEnvironmentVariables();

            if (args != null)
            {
                builder.AddCommandLine(args);
            }

            IConfiguration configuration = builder.Build();
            
            // Create a service collection.
            IServiceCollection services = new ServiceCollection();

            // Setup configuration.
            services.AddOptions();
            services.Configure<AppSettingsOptions>(configuration.GetSection("AppSettings"));
            
            // Create service provider.
            var serviceProvider = services.BuildServiceProvider();

            IOptions<AppSettingsOptions> appSettingsOptions = serviceProvider.GetService<IOptions<AppSettingsOptions>>();
            AppSettingsOptions appSettings = appSettingsOptions.Value;
            #endregion

            var url = new Uri($"wss://ws.weatherflow.com/swd/data?token={appSettings.Token}");
            int stationId = appSettings.StationId;
            int deviceId = appSettings.DeviceId;

            using (var client = new WebsocketClient(url))
            {
                Console.WriteLine("Press ESC to stop WebSocket connection...");

                client.ReconnectTimeout = TimeSpan.FromMinutes(2);
                client.ReconnectionHappened.Subscribe(info =>
                {
                    Console.WriteLine($"Reconnection happened, type: {info.Type}");

                    var startListeningForDevice = new
                    {
                        type = "listen_start",
                        device_id = deviceId,
                        id = deviceId.ToString()
                    };

                    client.Send(JsonSerializer.Serialize(startListeningForDevice));

                    var startListeningForStation = new
                    {
                        type = "listen_start_events",
                        station_id = stationId,
                        id = stationId.ToString()
                    };

                    client.Send(JsonSerializer.Serialize(startListeningForStation));
                });

                client.DisconnectionHappened.Subscribe(info => Console.WriteLine($"Disconnection happened, type: {info.Type}"));

                Console.WriteLine("Press \"S\" for simple message processing.  Press \"D\" for detailed message processing.");

                ConsoleKey keyPressed = WaitForKeyStroke(new List<ConsoleKey> {ConsoleKey.S, ConsoleKey.D});

                if (keyPressed.Equals(ConsoleKey.S))
                {
                    // This is a simple configuration for processing each message.
                    client.MessageReceived
                        .Subscribe(msg => Console.WriteLine($"Message: {msg}"));
                }
                else
                {
                    // This is a more detailed configuration for processing each specific message in parallel with synchronization (Asynchronous in order on separate threads).
                    // In this example it's a bit over kill for the code executed in each Subscribe(), however in production code each Subscribe() would normally call a specific
                    // method that you would want to run on it's own thread. Reference readme.md file for Websocket.Client for more details.
                    // https://github.com/Marfusios/websocket-client

                    object gate = new object();

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"connection_opened\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            Console.WriteLine($"Connection Opened: {msg}");
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"ack\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            // Example of deserializing JSON in to an object using the new .NET Core JsonSerializer.
                            Acknowledgement ack = JsonSerializer.Deserialize<Acknowledgement>(msg.Text);

                            string stationDevice = ack.Id.Equals(stationId.ToString()) ? "station" : "device";
                            Console.WriteLine($"Start/Stop Listening for {stationDevice}: {msg}");
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"obs_st\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            // Example of deserializing JSON in to a dynamic object using Newtonsoft Json.NET. 
                            // Note: The new .NET Core JsonSerializer cannot deserialize to a dynamic.
                            // https://github.com/dotnet/runtime/issues/29690
                            dynamic message = JsonConvert.DeserializeObject(msg.Text);

                            string messageType = message.status != null ? "Status" : "Summary";
                           
                            Console.WriteLine($"{messageType}: {msg}");

                            // An alternative way to deserialize messages with the same message type ("type":"obs_st") but different contents
                            // without the need for a dynamic.  This works because a status message has both a status and summary.
                            // A summary message does not have a status.
                            StatusMessage statusMessage = JsonSerializer.Deserialize<StatusMessage>(msg.Text);

                            if (statusMessage.Status != null)
                            {
                                // Use/process status message.
                            }
                            else
                            {
                                // Use/process summary message.
                                SummaryMessage summaryMessage = JsonSerializer.Deserialize<SummaryMessage>(msg.Text);
                            }
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"evt_strike\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            // Example of deserializing JSON in to an object using the new .NET Core JsonSerializer.
                            LightningStrikeEvent lightningStrikeEvent = JsonSerializer.Deserialize<LightningStrikeEvent>(msg.Text);

                            Console.WriteLine($"Lightning strike event occured {lightningStrikeEvent.OccuredAt:f}: {msg}");
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"evt_precip\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            // Example of deserializing JSON in to an object using the new .NET Core JsonSerializer.
                            RainStartEvent rainStartEvent = JsonSerializer.Deserialize<RainStartEvent>(msg.Text);

                            Console.WriteLine($"Rain event occured {rainStartEvent.OccuredAt:f}: {msg}");
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"evt_station_online\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            Console.WriteLine($"Station Online: {msg}");
                        });

                    client.MessageReceived
                        .Where(msg => msg.Text != null)
                        .Where(msg => msg.Text.Contains("\"evt_station_offline\""))
                        .ObserveOn(TaskPoolScheduler.Default)
                        .Synchronize(gate)
                        .Subscribe(msg =>
                        {
                            Console.WriteLine($"Station Offline: {msg}");
                        });
                }

                client.Start();

                WaitForKeyStroke(new List<ConsoleKey>{ ConsoleKey.Escape });

                var stopListeningForDevice = new
                {
                    type = "listen_stop",
                    device_id = deviceId,
                    id = deviceId.ToString()
                };

                client.Send(JsonSerializer.Serialize(stopListeningForDevice));

                var stopListeningForStation = new
                {
                    type = "listen_stop_events",
                    station_id = stationId,
                    id = stationId.ToString()
                };

                client.Send(JsonSerializer.Serialize(stopListeningForStation));
            }

            // This final confirmation is here so you can see the DisconnectionHappened message before the window is closed.
            Console.WriteLine("Press RETURN to close application...");
            Console.ReadLine();
        }

        /// <summary>
        /// Method that waits for the user to press a key.
        /// </summary>
        /// <param name="keys">The key(s) that we are expecting the user to press.</param>
        /// <returns>The key pressed.</returns>
        static ConsoleKey WaitForKeyStroke(List<ConsoleKey> keys)
        {
            ConsoleKey keyPressed;
            bool keyMatch = true;

            do
            {
                while (!Console.KeyAvailable)
                {
                    // Just waiting around for the user to press a key.
                    // Everything else is running on Async threads.
                }

                keyPressed = Console.ReadKey(true).Key;

                foreach (ConsoleKey key in keys)
                {
                    if (key.Equals(keyPressed))
                    {
                        keyMatch = false;
                        break;
                    }
                }
            } while (keyMatch);

            return keyPressed;
        }
    }
}
