using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;

namespace NetMQ_test_project
{
    internal static partial class Program
    {
        private static  void Main()
        {
            Console.Title = "test";




            // Create instance of Connection passing values server, port, Id
            var connection = new Connection("testserver", "44012");
            
            TradingRules.IndicatorIDs.Add("10260");
            TradingRules.IndicatorIDs.Add("10261");
            TradingRules.IndicatorIDs.Add("10262");

            List<string> Messages = connection.ListenForMessages(TradingRules.IndicatorIDs);
            foreach(string message in Messages)
            {
                Console.WriteLine("ID: " +  MessageParser.GetID(message) + " Data: " + MessageParser.GetData(message));
            }
            


            
        
        }
    }
}
