using System.Configuration;
using System.Net;

namespace SerialMQTTInterface
{
	class Program
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Args>")]
		public static void Main(string[] args)
		{
			try
			{
				string mqttServer = ConfigurationManager.AppSettings["mqttServer"];
				string mqttUseTls = ConfigurationManager.AppSettings["mqttUseTls"];
				string mqttAllowUntrustedCerts = ConfigurationManager.AppSettings["mqttAllowUntrustedCerts"];
				string mqttClientId = ConfigurationManager.AppSettings["mqttClientId"];
				string mqttUsername = ConfigurationManager.AppSettings["mqttUsername"];
				string mqttPassword = ConfigurationManager.AppSettings["mqttPassword"];

				string comPort = ConfigurationManager.AppSettings["comPort"];
				string baudRate = ConfigurationManager.AppSettings["baudRate"];

				string sendResetOnMqttStartup = ConfigurationManager.AppSettings["sendResetOnMqttStartup"];

				if (
					IPAddress.TryParse(mqttServer, out IPAddress ipAddress) &&
					bool.TryParse(mqttUseTls, out bool useTls) &&
					bool.TryParse(mqttAllowUntrustedCerts, out bool allowUntrustedCerts) &&
					mqttClientId != null &&

					uint.TryParse(comPort, out uint comPortNumber) && 
					int.TryParse(baudRate, out int baud) &&

					bool.TryParse(sendResetOnMqttStartup, out bool reset)
				)
				{
					IO.MQTT.MQTT.Initialize(new IO.MQTT.MQTT.MQTTInitializeOptions(
							ipAddress,
							useTls,
							allowUntrustedCerts,
							mqttClientId,
							mqttUsername,
							mqttPassword,
							reset
					));

					IO.Serial.ReadPort(comPortNumber, baud);
				}
				else
				{
					IO.Console.Print("App", "Invalid Settings.");
					System.Environment.Exit(1);
				}
			}
			catch (System.Exception e)
			{
				IO.Console.Print("App", "Unexpected error occured. Closing application.", e);
				System.Environment.Exit(1);
			}
		}
	}
}
