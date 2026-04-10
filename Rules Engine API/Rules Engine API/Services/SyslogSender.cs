
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Text.Json;
//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Services;

//public static class SyslogSender
//{
//    // ⚠️ غيري القيم دي حسب Wazuh Manager
//    private static readonly string WazuhIp = "192.168.146.132";
//    private static readonly int WazuhPort = 514;

//    public static void Send(SecurityEvent securityEvent)
//    {
//        try
//        {
//            using var udpClient = new UdpClient();

//            var json = JsonSerializer.Serialize(
//                securityEvent,
//                new JsonSerializerOptions
//                {
//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                }
//            );

//            // RFC 5424 basic syslog format
//            var syslogMessage = $"<134>{json}";
//            var data = Encoding.UTF8.GetBytes(syslogMessage);

//            udpClient.Send(
//                data,
//                data.Length,
//                new IPEndPoint(IPAddress.Parse(WazuhIp), WazuhPort)
//            );
//        }
//        catch (Exception ex)
//        {
//            // في البروجيكت الحقيقي يتحط Logging
//            Console.WriteLine($"Syslog send failed: {ex.Message}");
//        }
//    }
//}

//##################################################################


//using System.Net.Sockets;
//using System.Text;
//using System.Text.Json;
//using Rules_Engine_API.Models;

//namespace Rules_Engine_API.Services;

//public static class SyslogSender
//{
//    // ⚠️ IP و Port بتوع Wazuh Manager
//    private static readonly string WazuhIp = "192.168.1.13";
//    private static readonly int WazuhPort = 514; // TCP syslog port

//    public static void Send(SecurityEvent securityEvent)
//    {
//        try
//        {
//            using var tcpClient = new TcpClient();
//            tcpClient.Connect(WazuhIp, WazuhPort);

//            using var networkStream = tcpClient.GetStream();

//            var json = JsonSerializer.Serialize(
//                securityEvent,
//                new JsonSerializerOptions
//                {
//                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
//                }
//            );

//            // RFC 5424 basic framing
//            var syslogMessage = $"<134>{json}\n";
//            var data = Encoding.UTF8.GetBytes(syslogMessage);

//            networkStream.Write(data, 0, data.Length);
//            networkStream.Flush();
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Syslog TCP send failed: {ex.Message}");
//        }
//    }
//}

/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
/////////////////////////////////////////
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using Rules_Engine_API.Models;

namespace Rules_Engine_API.Services;

public static class SyslogSender
{
    // ⚠️ IP و Port بتوع Wazuh Manager
    private static readonly string WazuhIp = "100.81.129.48";
    private static readonly int WazuhPort = 514; // TCP syslog port

    public static void Send(SecurityEvent securityEvent)
    {
        try
        {
            // Serialize event to JSON (pretty print)
            var json = JsonSerializer.Serialize(
                securityEvent,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    WriteIndented = false
                }
            );

            // 🔹 Debug: Print Syslog message to console
            Console.WriteLine("------ SYSLOG MESSAGE ------");
            Console.WriteLine(json);
            Console.WriteLine("----------------------------");

            // Prepare RFC 5424 style message
            //var syslogMessage = $"<134>{DateTime.UtcNow:MMM dd HH:mm:ss} webtrap-backend {json}\n";
            var syslogMessage = $"<134>webtrap-backend {DateTime.UtcNow:MMM dd HH:mm:ss} {json}\n";
            var data = Encoding.UTF8.GetBytes(syslogMessage);

            // Send over TCP
            //using var tcpClient = new TcpClient();
            //tcpClient.Connect(WazuhIp, WazuhPort);
            using var tcpClient = new TcpClient(AddressFamily.InterNetwork); // Force IPv4
            tcpClient.Connect(WazuhIp, WazuhPort);

            using var networkStream = tcpClient.GetStream();
            networkStream.Write(data, 0, data.Length);
            networkStream.Flush();

            Console.WriteLine("[SYSLOG] Message sent successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[SYSLOG ERROR] TCP send failed: {ex.Message}");
        }
    }
}
