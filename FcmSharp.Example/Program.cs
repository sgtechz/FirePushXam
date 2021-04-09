using System;
using System.Collections.Generic;
using System.Threading;
using FcmSharp.Requests;
using FcmSharp.Settings;
using FcmSharp.Example;

namespace FcmSharp.Example
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Leemos las credenciales de la cuenta de google services
            var settings = FileBasedFcmClientSettings.CreateFromFile(@"763532726491", @"C:\sample-notification-2c161-firebase-adminsdk-hjxo7-17f677fd81.json");

            // Construimos el cliente
            using (var client = new FcmClient(settings))
            {
                var notification = new Notification
                {
                    Title = "Plataforma Estudio Mobile",
                    Body = "Notificando"
                };

                //Console.Write("Device Token: ");
                Console.Write("Ingresa el token del dispositivo: ");

                string registrationId = Console.ReadLine();

                 // EL TOKEN VENCE CADA MEDIA HR MAS O MENOS. RECORDAR USAR EL METODO ON NEW TOKEN
                //  string registrationId = "";

                // Enviamos el mensaje al dispositivo correspondiente
                var message = new FcmMessage()
                {
                    ValidateOnly = false,
                    Message = new Message
                    {
                        Token = registrationId,
                        Notification = notification
                    }
                };

                // Enviamos el mensaje y esperamos el resultado
                CancellationTokenSource cts = new CancellationTokenSource();

                // Respuesta asincronica 
                var result = client.SendAsync(message, cts.Token).GetAwaiter().GetResult();

                // mostrar resultado en la consola 

                Console.WriteLine("Message ID = {0}", result.Name);
                Console.ReadLine();
               // Console.WriteLine("Mensaje enviado correctamente al dispositivo " + registrationId);                
            }
        }
    }
}
