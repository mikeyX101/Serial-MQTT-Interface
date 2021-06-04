using MQTTnet;

namespace SerialMQTTInterface.Extensions
{
	internal static class ApplicationMessageExtensions
	{
		public static string ToSerialString(this MqttApplicationMessage message)
		{
			return $"!mqtt_data {message.Topic} {message.PayloadToUTF8String()} {message.Retain.ToString().ToLower()}";
		}

		public static string PayloadToUTF8String(this MqttApplicationMessage message)
		{
			return System.Text.Encoding.UTF8.GetString(message.Payload);
		}
	}
}
