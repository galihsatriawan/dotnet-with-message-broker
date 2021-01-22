using System;
using RabbitMQ.Client;
using System.Text;
namespace EmitLog
{
    class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory{
                UserName = "user",
                Password = "user",
                HostName = "192.168.1.11"
            };
            using (var connection = factory.CreateConnection()){
                using (var channel = connection.CreateModel()){
                    var exchangeName = "logs";
                    channel.ExchangeDeclare(exchangeName,ExchangeType.Fanout);
                    var message = GetMessage(args);
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(exchange: exchangeName,
                        routingKey: "",
                        basicProperties: null,
                        body: body
                    );
                    Console.WriteLine(" [x] Sent {0}", message);
                }
                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
        public static string GetMessage(string[] args){
            return ((args.Length > 0 ) ? string.Join(" ", args): "Hi");
        }
    }
}
