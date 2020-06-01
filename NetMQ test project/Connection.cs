using System;
using System.Collections.Generic;
using System.Text;
using NetMQ;
using NetMQ.Sockets;

namespace NetMQ_test_project
{
    public class  Connection{
        private readonly string Ip;
        private readonly string Port;           
        public string Address;
        private bool _listening = false;

        private string[] _topics;

        public string _lastRecieved;

        public StringBuilder _recievedSb = new StringBuilder();
        public List<string> _recievedList = new List<string>();
        public string[] _recievedArray;
        public int _recievedCount = 0;

        public Connection(string server, string port, string[] topics)
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
                    Console.WriteLine("Invalid input for server. Options are \"Ny4\", \"Ny2\" or \"test\" (not case-sensitive)" );
                    break;
            }

            this.Port = port;
            this.Address = String.Format("tcp://{0}:{1}", Ip,  Port);
            this._topics = topics;
            this._recievedArray = new string[topics.Length];
                
        }


        public void ListenForMessages()
        {
            _listening = true;
            

            // NetMQ open socket connection & subscribe
            var subscriber = new SubscriberSocket();
            subscriber.Connect(this.Address);
            Console.WriteLine("Connecting to: " + this.Address);

            foreach (string topic in _topics) 
            { 
                subscriber.Subscribe(topic);
                Console.WriteLine("Subscribing to: " + topic);
            }
            //subscriber.SubscribeToAnyTopic();

            // IMPORTANT: configure TCP keepalive settings
            subscriber.Options.TcpKeepalive = true;
            subscriber.Options.TcpKeepaliveIdle = new System.TimeSpan(0, 0, 75);
            subscriber.Options.TcpKeepaliveInterval = new System.TimeSpan(0, 0, 75);



            while (_listening)
            {
                if (subscriber.HasIn)
                {
                    _recievedSb.Append(subscriber.ReceiveFrameString()+" ");
                    _recievedList.Add(subscriber.ReceiveFrameString());
                }
                
                
            }
         
        }

        public void PrintRecieved()
        {
            while (true)
            {
                //Console.WriteLine(_recieved);

            }
        }

        public void StopListening()
        {
            _listening = false;
        }
    }
    
}
