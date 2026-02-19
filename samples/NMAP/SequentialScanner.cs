using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using log4net;

namespace NMAP
{
	public class SequentialScanner : IPScanner
	{
		protected virtual ILog log => LogManager.GetLogger(typeof(SequentialScanner));

		public async virtual Task Scan(IPAddress[] ipAddrs, int[] ports)
		{
			foreach(var ipAddr in ipAddrs)
			{
				if(await PingAddr(ipAddr) != IPStatus.Success)
					continue;

				foreach(var port in ports)
					await CheckPort(ipAddr, port);
			}
		}
		
		protected async Task<IPStatus> PingAddr(IPAddress ipAddr, int timeout = 3000)
		{
			Console.Out.WriteLineAsync($"Pinging {ipAddr}");
			using(var ping = new Ping())
			{
				var status = await ping.SendPingAsync(ipAddr, timeout);
				Console.Out.WriteLineAsync($"Pinged {ipAddr}: {status.Status}");
				return status.Status;
			}
		}

		protected async Task CheckPort(IPAddress ipAddr, int port, int timeout = 3000)
		{
			using(var tcpClient = new TcpClient())
			{
				Console.Out.WriteLineAsync($"Checking {ipAddr}:{port}");

				var connectTask = await tcpClient.ConnectWithTimeoutAsync(ipAddr, port, timeout);
				PortStatus portStatus;
				switch(connectTask.Status)
				{
					case TaskStatus.RanToCompletion:
						portStatus = PortStatus.OPEN;
						break;
					case TaskStatus.Faulted:
						portStatus = PortStatus.CLOSED;
						break;
					default:
						portStatus = PortStatus.FILTERED;
						break;
				}
				Console.Out.WriteLineAsync($"Checked {ipAddr}:{port} - {portStatus}");
			}
		}
	}
}