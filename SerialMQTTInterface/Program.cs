using System.Net;

namespace SerialMQTTInterface
{
	class Program
	{

		public static void Main(string[] args)
		{
			// Args: uint comPortNumber, int baudRate, string mqttServerIp
			if (args.Length >= 3 && uint.TryParse(args[0], out uint comPortNumber) && int.TryParse(args[1], out int baudRate) && IPAddress.TryParse(args[2], out IPAddress ipAddress))
			{
				IO.MQTT.MQTT.Initialize(ipAddress);
				IO.Serial.ReadPort(comPortNumber, baudRate);
			}
			else
			{
				IO.Console.Print("App", "Invalid Args. Usage: SerialMQTTInterface.exe <COMPortNumber> <BaudRate> <MQTTServerIp>");
				System.Environment.Exit(1);
			}
		}
	}
}
