using SerialMQTTInterface.IO.MQTT.Commands;
using System.IO.Ports;
using System.Threading;

namespace SerialMQTTInterface.IO
{
	internal static class Serial
	{
		private static string SourceName { get; set; }
		private static SerialPort Port { get; set; }
		private static Thread ReadThread { get; set; }
		private static bool CurrentlyReading { get; set; } = false;

		// https://docs.microsoft.com/en-us/dotnet/api/system.io.ports.serialport.open
		public static void ReadPort(uint comPort, int baudRate)
		{
			SourceName = $"COM{comPort}";
			Port = new(SourceName, baudRate);
			Port.Encoding = System.Text.Encoding.UTF8;
			// Set the read/write timeouts
			Port.ReadTimeout = 500;
			Port.WriteTimeout = 500;

			while (!Port.IsOpen)
			{
				try
				{
					Port.Open();
				}
				catch (System.Exception e)
				{
					Console.Print(SourceName, $"Error while opening port {SourceName}. Retrying in 5 seconds...", e);
					Thread.Sleep(System.TimeSpan.FromSeconds(5));
				}
			}
			
			Console.Print(SourceName, $"Press 'q' to stop reading the serial port.");
			Console.Print(SourceName, $"Reading COM{comPort} with a baud rate of {baudRate}");

			ReadThread = new(Read);
			ReadThread.Start();

			CurrentlyReading = true;
			while (CurrentlyReading)
			{
				System.ConsoleKeyInfo input = System.Console.ReadKey(true);
				if (input.Key == System.ConsoleKey.Q)
				{
					CurrentlyReading = false;
				}
			}

			ReadThread.Join();
			Port.Close();
		}

		private static void Read()
		{
			while (CurrentlyReading)
			{
				try
				{
					string message = Port.ReadLine();
					Console.Print(SourceName, message);

					if (message[0] == '!' && MQTTCommand.TryParse(message.Replace("\r", ""), out IMQTTCommand command))
					{
						MQTT.MQTT.ExecuteCommand(command);
					}
				}
				catch (System.Exception e) { 
					if (e is not System.TimeoutException && e is not System.OperationCanceledException)
					{
						Console.Print(SourceName, "An unexpected exception occured while reading the serial port.", e);
						System.Environment.Exit(1);
					}
					else if (e is System.OperationCanceledException)
					{
						Console.Print(SourceName, "Serial port is closed. Closing application.", e);
						System.Environment.Exit(0);
					}
				}
			}
		}

		public static void Write(string message)
		{
			Port.WriteLine(message);
		}
	}
}
