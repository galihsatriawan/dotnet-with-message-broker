using System;
using RabbitMQ.Client;
using System.Text;
namespace NewTask
{
    class Program
    {
        static void Main(string[] args)
        {
            // Create Connection
            var factory = new ConnectionFactory{
                UserName = "user",
                Password = "user",
                Port = Protocols.DefaultProtocol.DefaultPort,
                HostName = "192.168.1.11"
            };
            var destination = "multiple-workers";
            using (var connection = factory.CreateConnection()){
                // Create channel
                using (var channel = connection.CreateModel()){
                    channel.QueueDeclare(
                        queue: destination,
                        durable: false,
                        exclusive: false,
                        autoDelete: false,
                        arguments:null
                    );
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);

                    var properties = channel.CreateBasicProperties();
                    properties.Persistent = true;

                    channel.BasicPublish(exchange: "",
                        routingKey: destination,
                        basicProperties: properties,
                        body: body
                    );
                }
            }
        }
        public static string GetMessage(string[] args){
            return ((args.Length > 0 ) ? string.Join(" ", args): "Hi");
        }
    }
}
