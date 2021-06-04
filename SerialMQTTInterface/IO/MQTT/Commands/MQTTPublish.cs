using MQTTnet.Client;
using System.Threading.Tasks;

namespace SerialMQTTInterface.IO.MQTT.Commands
{
	internal sealed class MQTTPublish : MQTTCommand
	{
		public override string ExecutionMessage => $"Publishing payload \"{Payload}\" with topic \"{Topic}\". Message is {(!Retained ? "not " : "")}retained.";

		public static string CommandName => "!mqtt_publish";

		internal string Topic { get; set; }
		internal string Payload { get; set; }
		internal bool Retained { get; set; }

		internal MQTTPublish(string topic, string payload, bool retained) : base()
		{
			Topic = topic;
			Payload = payload;
			Retained = retained;
		}

		public override async Task Execute(IMqttClient client)
		{
			MQTTnet.MqttApplicationMessage message = new MQTTnet.MqttApplicationMessageBuilder()
				.WithTopic(Topic)
				.WithPayload(Payload)
				.WithRetainFlag(Retained)
				.Build();

			await client.PublishAsync(message);
		}
	}
}
