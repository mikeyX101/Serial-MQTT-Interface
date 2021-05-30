using System;

namespace SerialMQTTInterface.IO
{
	internal static class Console
	{
		public static void Print(string source, string message, Exception exception = null)
		{
			System.Console.WriteLine($"{DateTime.Now:u} [{source}] - {message}{(exception != null ? $"\n{exception}" : "")}");
		}

	}
}
