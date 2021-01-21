using System;
using RabbitMQ.Client;
using System.Text;
namespace Send
{
    class Program
    {
        static void Main(string[] args)
        {
            //Create connection
            var factory = new ConnectionFactory(){ 
                UserName="user",
                Password="user",
                Port= Protocols.DefaultProtocol.DefaultPort, 
                HostName = "192.168.1.11" };
            var destination = "tester";
            using (var connection = factory.CreateConnection())
            {
                using (var channel = connection.CreateModel()){
                    channel.QueueDeclare(
                      queue: destination,
                      durable: false,
                      exclusive: false,
                      autoDelete:false,
                      arguments:null  
                    );
                    string message = "Hi, my name is anonymous";
                    //Publish message
                    var body = Encoding.UTF8.GetBytes(message);
                    channel.BasicPublish(
                      exchange:"",
                      routingKey:destination,
                      basicProperties : null,
                      body: body  
                    );

                    Console.WriteLine("[x] Sent {0}", message);
                }
            }
            Console.WriteLine("Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}
