using System.Net;
using System.Configuration;

namespace SerialMQTTInterface
{
	class Program
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Args>")]
		public static void Main(string[] args)
		{
			try
			{
				string comPort = ConfigurationManager.AppSettings["comPort"];
				string baud = ConfigurationManager.AppSettings["baudRate"];
				string mqttServer = ConfigurationManager.AppSettings["mqttServer"];

				if (uint.TryParse(comPort, out uint comPortNumber) && int.TryParse(baud, out int baudRate) && IPAddress.TryParse(mqttServer, out IPAddress ipAddress))
				{
					IO.MQTT.MQTT.Initialize(ipAddress);
					IO.Serial.ReadPort(comPortNumber, baudRate);
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
