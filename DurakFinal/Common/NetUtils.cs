using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;

namespace Durak.Common
{
    // Утіліта для створення гри
    public static class NetUtils
    {
        // Отримує перший відкритий порт
        public static int GetOpenPort(int startPort = NetSettings.DEFAULT_SERVER_PORT)
        {
            int portStartIndex = startPort;
            int count = 99;

            // Отримує список портів
            IPGlobalProperties properties = IPGlobalProperties.GetIPGlobalProperties();
            IPEndPoint[] udpEndPoints = properties.GetActiveUdpListeners();

            // Отримує список усіх використовуваних портів
            List<int> usedPorts = udpEndPoints.Select(p => p.Port).ToList<int>();

            // Визначте результат (за умовчанням повертає 0)
            int unusedPort = Enumerable.Range(portStartIndex, count).Where(port => !usedPorts.Contains(port)).FirstOrDefault();

            return unusedPort;
        }

        // Отримує IP-адресу локальної машини
        public static IPAddress GetAddress()
        {
            IPAddress ip = System.Net.Dns.GetHostEntry(Environment.MachineName).AddressList.Where(i => i.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork).FirstOrDefault();

            return ip;
        }
    }
}
