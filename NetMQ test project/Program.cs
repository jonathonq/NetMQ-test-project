using System;
using NetMQ;
using NetMQ.Sockets;
using System.Threading;
using System.Collections;

namespace NetMQ_test_project
{
    internal static class Program
    {
        private static  void Main()
        {
            Console.Title = "test";
            
            var subscriber = new NetMQ.Sockets.SubscriberSocket();



            //subscriber.Connect("tcp://173.214.169.6:44012"); // NY4 server
            subscriber.Connect("tcp://66.70.190.253:44012"); // test server
            Console.WriteLine("Connecting to tcp://66.70.190.253:44012");  
            subscriber.SubscribeToAnyTopic();

            // IMPORTANT: configure TCP keepalive settings
            subscriber.Options.TcpKeepalive = true;
            subscriber.Options.TcpKeepaliveIdle = new System.TimeSpan(0, 0, 75);
            subscriber.Options.TcpKeepaliveInterval = new System.TimeSpan(0, 0, 75);

            while (true)
            {
                Console.WriteLine("Waiting for message... \n");
 
                string messageReceived = subscriber.ReceiveFrameString();
                Console.WriteLine(messageReceived.ToString() + " received at: " + DateTime.Now.ToString());
                
                break; // if you received everything you need, break or leave it
            }
        }
    }
}
