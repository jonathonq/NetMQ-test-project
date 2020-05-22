﻿using System;
using NetMQ;
using NetMQ.Sockets;
using System.Collections.Generic;
using System.Linq;

namespace NetMQ_test_project
{
    internal static partial class Program
    {
        public class  Connection{
            public string Ip;
            public string Port;
            

            public string Address;

            public Connection()
            {

            }
            public Connection(string server, string port)
            {
                switch (server.ToLower())
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
                this.Address = "tcp://" + Ip + ":" + port;
                
                
            }


            public List<string> ListenForMessages(List<string> indicators)
            {
                bool Listening = true;
                var OutputList = new List<string>();

                var subscriber = new SubscriberSocket();
                subscriber.Connect(this.Address);
                Console.WriteLine("Connecting to: " + this.Address);

                foreach (string indicator in indicators) 
                { 
                    subscriber.Subscribe(indicator);
                    Console.WriteLine("Subscribing to: " + indicator);
                }
                //subscriber.SubscribeToAnyTopic();

                // IMPORTANT: configure TCP keepalive settings
                subscriber.Options.TcpKeepalive = true;
                subscriber.Options.TcpKeepaliveIdle = new System.TimeSpan(0, 0, 75);
                subscriber.Options.TcpKeepaliveInterval = new System.TimeSpan(0, 0, 75);


                while (Listening == true)
                {
                    string MessageReceived = subscriber.ReceiveFrameString();
                    //Console.WriteLine(MessageReceived);
                    OutputList.Add(MessageReceived);
                    if(OutputList.Count() == indicators.Count())
                    {
                        Listening = false;
                    }

                }


                return OutputList;
            }
        }
    }
}