using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Connecting;
using MQTTnet.Client.Disconnecting;
using MQTTnet.Client.Options;
using System.Threading.Tasks;

namespace SerialMQTTInterface.IO.MQTT
{
	internal static class MQTT
	{
		private static string SourceName => "MQTT";

		private static IMqttClientOptions ConnectOptions { get; set; }

		private static IMqttClient Client { get; set; }

		public static async void Initialize(System.Net.IPAddress serverIp)
		{
			Client = new MqttFactory().CreateMqttClient();
			Client.UseApplicationMessageReceivedHandler(OnMessageReceived);
			Client.UseDisconnectedHandler(OnDisconnect);

			ConnectOptions = new MqttClientOptionsBuilder()
				.WithClientId("AlarmSystem")
				.WithTcpServer(serverIp.ToString())
				.Build();

			Console.Print(SourceName, $"Connecting to {serverIp}...");
			try
			{
				MqttClientAuthenticateResult result = await Client.ConnectAsync(ConnectOptions);
				if (result.ResultCode == MqttClientConnectResultCode.Success)
				{
					Console.Print(SourceName, $"Connected to {serverIp}.");
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
				Console.Print(SourceName, $"An exception occured while publishing a message.", e);
			}
			
		}

		private static /*async Task*/ void OnMessageReceived(MqttApplicationMessageReceivedEventArgs e)
		{
			Console.Print(SourceName, $"Received message with topic {e.ApplicationMessage.Topic}");
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
