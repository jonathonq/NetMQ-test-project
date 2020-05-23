using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;


namespace NetMQ_test_project
{

    internal static partial class Program
    {
        
        private static  void Main()
        {
            Console.Title = "News Trader 2020";

            string title = @"

  _   _                     _____              _             ____   ___ ____   ___  
 | \ | | _____      _____  |_   _| __ __ _  __| | ___ _ __  |___ \ / _ \___ \ / _ \ 
 |  \| |/ _ \ \ /\ / / __|   | || '__/ _` |/ _` |/ _ \ '__|   __) | | | |__) | | | |
 | |\  |  __/\ V  V /\__ \   | || | | (_| | (_| |  __/ |     / __/| |_| / __/| |_| |
 |_| \_|\___| \_/\_/ |___/   |_||_|  \__,_|\__,_|\___|_|    |_____|\___/_____|\___/ 


 v0.1 (Alpha)
                                                                                    

"; // ascii art
            Console.WriteLine(title);

            // Create instance of Connection passing values server, port, Id
            var connection = new Connection("testserver", "44012");




            List<string> UserDefinedIndicators = new List<string> // Which topics/indicator IDs to listen for
            {
                "10260",
                "10261",
                "10262"
            };



            /*List<TradingRules> AllTradingRules = new List<TradingRules>();

            foreach (var id in UserDefinedIndicators)
            {
                AllTradingRules[UserDefinedIndicators.IndexOf(id)] = new TradingRules(id,1,">", 10, "<", 10);
                Console.WriteLine(AllTradingRules[UserDefinedIndicators.IndexOf(id)]);

            }*/




            var CadRetailSalesMoM = new TradingRules("10260", 0, ">", 0.2, "<", 0.2);

            List<string> Messages = connection.ListenForMessages(UserDefinedIndicators);
            foreach(string message in Messages)
            {
                Message newMessage = new Message(message);
                CadRetailSalesMoM.GenerateSignal(newMessage.GetData());
                Console.WriteLine("ID: {0} |  Data: {1}", newMessage.GetID(),  newMessage.GetData());

                Console.WriteLine("Signal for {0} is: {1}",newMessage.GetID(), CadRetailSalesMoM.GenerateSignal(newMessage.GetData()));


            }
        }
    }
}
