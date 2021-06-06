using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using SerialMQTTInterface.Extensions;
using System.Collections.Specialized;
using System.Threading.Tasks;

namespace SerialMQTTInterface.IO.MQTT
{
	internal static class MQTT
	{
		internal class MQTTInitializeOptions
		{
			public MQTTInitializeOptions(System.Net.IPAddress serverIp, bool useTls, bool allowUntrustedCerts, string clientId, string username, string password)
			{
				ServerIp = serverIp;
				UseTls = useTls;
				AllowUntrustedCerts = allowUntrustedCerts;
				ClientId = clientId;
				Username = username;
				Password = password;
			}

			public System.Net.IPAddress ServerIp { get; private set; }
			public bool UseTls { get; private set; }
			public bool AllowUntrustedCerts { get; private set; }
			public string ClientId { get; private set; }
			public string Username { get; private set; }
			public string Password { get; private set; }
		}

		private static string SourceName => "MQTT";

		private static IMqttClientOptions ConnectOptions { get; set; }

		private static IMqttClient Client { get; set; }

		public static async void Initialize(MQTTInitializeOptions options)
		{
			if (options == null)
			{
				throw new System.ArgumentNullException(nameof(options));
			}

			Client = new MqttFactory().CreateMqttClient();
			Client.UseApplicationMessageReceivedHandler(OnMessageReceived);
			Client.UseConnectedHandler(OnConnected);
			Client.UseDisconnectedHandler(OnDisconnect);

			MqttClientOptionsBuilder mqttClientOptionsBuilder = new MqttClientOptionsBuilder()
				.WithClientId(options.ClientId)
				.WithTcpServer(options.ServerIp.ToString())
				.WithCleanSession()
				.WithTls(new MqttClientOptionsBuilderTlsParameters
				{
					UseTls = true,
					SslProtocol = System.Security.Authentication.SslProtocols.Tls12,

					AllowUntrustedCertificates = true
				});

			if (!string.IsNullOrWhiteSpace(options.Username) && !string.IsNullOrWhiteSpace(options.Password))
			{
				Console.Print(SourceName, $"Using credentials for MQTT.");
				mqttClientOptionsBuilder.WithCredentials(options.Username, options.Password);
			}
			else
			{
				Console.Print(SourceName, $"Not using credentials for MQTT.");
			}

			ConnectOptions = mqttClientOptionsBuilder.Build();
			Console.Print(SourceName, $"Connecting to {options.ServerIp}...");
			try
			{
				MqttClientAuthenticateResult result = await Client.ConnectAsync(ConnectOptions);
				if (result.ResultCode == MqttClientConnectResultCode.Success)
				{
					Console.Print(SourceName, $"Connected to {options.ServerIp}.");
				}
			}
			catch (System.Exception e)
			{
				Console.Print(SourceName, "Connecting failed.", e);
			}
		}

		public static async void ExecuteCommand(Commands.IMQTTCommand command)
		{
			try
			{
				Console.Print(SourceName, command.ExecutionMessage);
				await command.Execute(Client);
			}
			catch (System.Exception e)
			{
				Console.Print(SourceName, $"An exception occured while executing an MQTT command.", e);
			}
			
		}

		private static /*async Task*/ void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
		{
			Console.Print(SourceName, $"Received message with topic {e.ApplicationMessage.Topic} and payload {e.ApplicationMessage.PayloadToUTF8String()}");

			Serial.Write(e.ApplicationMessage.ToSerialString());
		}

		private static async void OnConnected(MqttClientConnectedEventArgs e)
		{
			NameValueCollection topics = System.Configuration.ConfigurationManager.GetSection("subscribeToTopics") as NameValueCollection;
			foreach(string topic in topics)
			{
				await Client.SubscribeAsync(new MqttTopicFilterBuilder()
					.WithTopic(topics[topic])
					.Build()
				);
			}
		}

		private static async Task OnDisconnect(MqttClientDisconnectedEventArgs e)
		{
			Console.Print(SourceName, "Disconnected from server. Reconnecting in 5 seconds...");
			await Task.Delay(System.TimeSpan.FromSeconds(5));

			try
			{
				MqttClientAuthenticateResult result = await Client.ConnectAsync(ConnectOptions);
				if (result.ResultCode == MqttClientConnectResultCode.Success)
				{
					Console.Print(SourceName, "Reconnected.");
				}
			}
			catch (System.Exception ex)
			{
				Console.Print(SourceName, "Reconnecting failed.", ex);
			}
		}
	}
}
