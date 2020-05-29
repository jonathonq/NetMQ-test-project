using NetMQ;
using NetMQ.Sockets;
using System;
using System.Collections.Generic;

namespace NetMQ_test_project
{
    public class Connection
    {
        private readonly string Ip;
        private readonly string Port;
        public string Address;
        public bool Listening = false;
        public List<string> Received = null;

        public Connection(string server, string port)
        {
            switch (server.ToLower()) // Haawks servers
            {
                case "ny4":
                    this.Ip = "173.214.169.6";
                    break;

                case "ny2":
                    this.Ip = "38.65.25.118";
                    break;

                case "testserver":
                    this.Ip = "66.70.190.253";
                    break;

                default:
                    Console.WriteLine("Invalid input for server. Options are \"Ny4\", \"Ny2\" or \"test\" (not case-sensitive)");
                    break;
            }

            this.Port = port;
            this.Address = String.Format("tcp://{0}:{1}", Ip, Port);
        }

        public void ListenForMessages()
        {
            Listening = true;

            // NetMQ open socket connection & subscribe
            var subscriber = new SubscriberSocket();
            subscriber.Connect(this.Address);
            Console.WriteLine("Connecting to: " + this.Address);

            subscriber.SubscribeToAnyTopic();

            // IMPORTANT: configure TCP keepalive settings
            subscriber.Options.TcpKeepalive = true;
            subscriber.Options.TcpKeepaliveIdle = new System.TimeSpan(0, 0, 75);
            subscriber.Options.TcpKeepaliveInterval = new System.TimeSpan(0, 0, 75);

            while (Listening)
            {
                Received.Add(subscriber.ReceiveFrameString());
            }
        }
    }
}