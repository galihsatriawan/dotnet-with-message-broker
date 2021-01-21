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
                        durable: true, // Set true for keep it although rabbitmq restart
                        exclusive: false,
                        autoDelete:false,
                        arguments:null
                    );
                    // Prevent worker fetch message before complete the job
                    channel.BasicQos(prefetchSize: 0, prefetchCount: 1, global: false);

                    // Consume
                    var consumer = new EventingBasicConsumer(channel);

                    consumer.Received += (model, ea) => {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [x] Received {0}", message);

                        int dots = message.Split('.').Length - 1;
                        Thread.Sleep(dots * 1000);
                        Console.WriteLine(" [x] Done ");
                        // Note: it is possible to access the channel via
                        //       ((EventingBasicConsumer)sender).Model here
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false); // confirmation that task have been processed
                    };
                    channel.BasicConsume(
                        queue: nameClient,
                        autoAck: false, // Manually add the flag
                        consumer:consumer
                    );
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
        }
    }
}
