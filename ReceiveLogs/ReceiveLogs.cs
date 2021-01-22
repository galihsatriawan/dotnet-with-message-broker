using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
namespace ReceiveLogs
{
    class Program
    {
        static void Main(string[] args)
        {
            //cretate connection
            var factory = new ConnectionFactory{
                UserName = "user",
                Password = "user",
                HostName = "192.168.1.11"
            };
            using (var connection = factory.CreateConnection()){
                using (var channel = connection.CreateModel()){
                    var exchangeName = "logs";
                    channel.ExchangeDeclare(exchangeName, type: ExchangeType.Fanout);

                    var queueName = channel.QueueDeclare().QueueName;

                    // Binding 
                    channel.QueueBind(queue: queueName,
                        exchange: exchangeName,
                        routingKey: "",
                        arguments:null
                    );

                    Console.WriteLine(" [*] Waiting for logs.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>{
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);

                        Console.WriteLine(" [x] {0}", message);
                        channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple:false);
                    };

                    channel.BasicConsume(
                        queue: queueName,
                        autoAck:false,
                        consumer: consumer
                    );
                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                    
                }
            }
        }
    }
}
