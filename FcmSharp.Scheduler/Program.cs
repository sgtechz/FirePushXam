using System;
using System.Threading;
using System.Threading.Tasks;
using FcmSharp.Scheduler.Database;
using FcmSharp.Scheduler.Services;
using FcmSharp.Settings;

namespace FcmSharp.Scheduler
{
    class Program
    {
        private static readonly TimeSpan PollingInterval = TimeSpan.FromMinutes(1);

        static async Task Main(string[] args)
        {
            var cancellationToken = CancellationToken.None;

            // Inicializa la base de datos
            await CreateDatabase(cancellationToken);

            // Inicia el ciclo de programacion para programar los mensjaes
            await ProcessMessages(cancellationToken);
        }


        public static async Task ProcessMessages(CancellationToken cancellationToken)
        {
            var service = CreateSchedulerService();

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(PollingInterval, cancellationToken);

                if (!cancellationToken.IsCancellationRequested)
                {
                    DateTime scheduledTime = DateTime.UtcNow;

                    Console.WriteLine($"[${DateTime.Now}] [INFO] Enviando mensajes agendados a las {scheduledTime}");

                    await service.SendScheduledMessagesAsync(scheduledTime, cancellationToken);
                }
            }

            service.Dispose();
        }

        private static Task CreateDatabase(CancellationToken cancellationToken)
        {
            using (var context = new ApplicationDbContext())
            {
                return context.Database.EnsureCreatedAsync(cancellationToken);
            }
        }

        private static ISchedulerService CreateSchedulerService()
        {
            // lee las credenciales de este servicio
            var settings = FileBasedFcmClientSettings.CreateFromFile(@"C:\sample-notification-2c161-firebase-adminsdk-hjxo7-17f677fd81.json");

            // contruir el cliente
            var client = new FcmClient(settings);
            
            return new SchedulerService(client);
        }
    }
}
