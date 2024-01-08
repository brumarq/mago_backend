using System.Net.Sockets;

namespace HealthCheckApp;

class Program
{
    static async Task Main(string[] args)
    {
        var servicesToCheck = new (string Host, int Port)[]
        {
            ("firmware-microservice-service-mago-backend-test.apps.ocp4-inholland.joran-bergfeld.com", 6969)
        };

        foreach (var service in servicesToCheck)
        {
            await CheckServiceHealth(service.Host, service.Port);
        }
    }

    static async Task CheckServiceHealth(string host, int port)
    {
        using var client = new TcpClient();
        try
        {
            await client.ConnectAsync(host, port);
            Console.WriteLine($"Success: Connected to {host}:{port}");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}