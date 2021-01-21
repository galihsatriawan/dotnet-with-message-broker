using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
namespace Worker
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory{
                UserName = "user",
                Password = "user",
                Port = Protocols.DefaultProtocol.DefaultPort,
                HostName = "192.168.1.11"
            };
            var nameClient = "multiple-workers";
            using (var connection = factory.CreateConnection()){
                using (var channel = connection.CreateModel()){
                    // make sure the channel is available
                    channel.QueueDeclare(
                        queue: nameClient,
                        durable: false,
                        exclusive: false,
                        autoDelete:false,
                        arguments:null
                    );

                    // Consume
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) => {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [x] Received {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);
                        Console.WriteLine(" [x] Done ");

                    };
                    channel.BasicConsume(
                        queue: nameClient,
                        autoAck: true,
                        consumer:consumer
                    );
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
