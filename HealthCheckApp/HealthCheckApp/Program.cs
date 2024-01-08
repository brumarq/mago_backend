using System.Net;
using System.Net.Sockets;

namespace HealthCheckApp;

class Program
{
    static async Task Main(string[] args)
    {
        var servicesToCheck = new (string Name, string Host)[]
        {
            ("Device Service", "https://device-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("User Service", "https://user-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("Notifications Service", "https://notifications-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("Metrics Service", "https://metrics-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("Firmware Service", "https://firmware-microservice-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("Device/Metrics Orchestrator", "https://device-metrics-orchestrator-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("Device/Firmware Orchestrator", "https://device-firmware-orchestrator-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
            ("User/Notification/Device Orchestrator", "https://user-device-noti-orchestrator-service-mago-backend.apps.ocp4-inholland.joran-bergfeld.com/"),
        };

        foreach (var service in servicesToCheck)
        {
            await CheckServiceHealth(service.Name, service.Host);
        }
    }

    static async Task CheckServiceHealth(string serviceName, string host)
    {
        using var httpClient = new HttpClient();
        try
        {
            var response = await httpClient.GetAsync(host);
            if (response.IsSuccessStatusCode || response.StatusCode == HttpStatusCode.NotFound)
            {
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{serviceName}: Service at {host} is up and running ");
                Console.ResetColor();
            }
            else if (response.StatusCode == HttpStatusCode.ServiceUnavailable)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine($"{serviceName} Unavailable: Service at {host} is down");
                Console.ResetColor();
            }
            else
            {
                Console.WriteLine($"{serviceName}: Service at {host} returned status code {response.StatusCode}");
            }
        }
        catch (Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: Could not connect to {serviceName} at {host} - {e.Message}");
            Console.ResetColor();
        }
    }
}