using System;

namespace Autocad_ConcerteList.Log
{
	public static class Logger
	{
		public static readonly Log Log = new Log();
	}

	public class Log
	{
		public void Error(Exception ex, string msg)
		{
			
		}

		public void Error(string msg)
		{
			
		}
	}
}
