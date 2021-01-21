using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace Receive
{
    class Program
    {
        static void Main(string[] args)
        {
            var nameClient = "tester";
            var factory = new ConnectionFactory(){ 
                UserName = "user",
                Password = "user",
                Port = Protocols.DefaultProtocol.DefaultPort,
                HostName = "192.168.1.11"
                 }; 
            using (var connection = factory.CreateConnection()){
                using (var channel = connection.CreateModel()){
                    channel.QueueDeclare(
                      queue:  nameClient,
                      durable: false,
                      exclusive: false,
                      autoDelete: false,
                      arguments: null
                    );
                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (ModuleHandle, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        Console.WriteLine(" [x] Received {0}", message);
                    };
                    channel.BasicConsume(
                        queue: nameClient,
                        autoAck:true,
                        consumer: consumer
                    );
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }     
        }
    }
}
