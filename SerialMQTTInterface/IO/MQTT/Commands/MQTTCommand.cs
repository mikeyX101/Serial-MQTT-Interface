using System;
using System.Threading.Tasks;

namespace SerialMQTTInterface.IO.MQTT.Commands
{
	public interface IMQTTCommand
	{
		public abstract Task Execute(MQTTnet.Client.IMqttClient client);
		public abstract string ExecutionMessage { get; }
	}

	internal abstract class MQTTCommand : IMQTTCommand
	{
		public abstract string ExecutionMessage { get; }

		internal MQTTCommand() { }

		public static bool TryParse(string command, out IMQTTCommand mqttCommand)
		{
			IMQTTCommand commandInstance = null;
			bool success = false;

			if (command != null)
			{
				string[] parts = command.Split(' ', StringSplitOptions.RemoveEmptyEntries);
				//!mqtt_publish <string topic> <string payload> <bool retained>
				if (parts.Length == 4 && parts[0] == MQTTPublish.CommandName && bool.TryParse(parts[3], out bool retained))
				{
					commandInstance = new MQTTPublish(parts[1], parts[2], retained);
					success = true;
				}
			}

			mqttCommand = commandInstance;
			return success;
		}

		public abstract Task Execute(MQTTnet.Client.IMqttClient client);
	}
}
