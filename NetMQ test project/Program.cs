using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Text;

namespace NetMQ_test_project
{
    internal static partial class Program
    {
        private static void Main()
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


            #region Set Indicators 
            Console.WriteLine(title);
            

            List<string> UserDefinedIndicatorIds = new List<string> // Which ZMQ topics ie. indicator IDs to listen for
            {
                "30000",
                "30001",
                "30002",
                "30003"

            };
            Console.WriteLine("Inputted Indicators are: ");
            foreach (string id in UserDefinedIndicatorIds)
            {
                Console.WriteLine(id);
            }

            // Creates array of indicator objects based on user inputted IDs
            // Not even sure if having a class/objects for indicator is necessary, could hinder performance
            var indicators = new Indicator[UserDefinedIndicatorIds.Count];

            // Convert ID List into Array for better performance?
            //var IndicatorIds = UserDefinedIndicatorIds.ToArray();


            #endregion

            #region Define Trading Rules for every indicator
            foreach (var id in UserDefinedIndicatorIds)
            {
                indicators[UserDefinedIndicatorIds.IndexOf(id)] = new Indicator(id);
                
            }
            foreach (var indicator in indicators)
            {
                Console.WriteLine("Define Rules for: {0}?  (y/n)", indicator.Id);
                switch (Console.ReadLine().ToLower())
                {
                    case "yes":
                    case "y":
                        indicator.TradingRules = TradingRules.DefineByInput(indicator);
                        break;

                    case "no":
                    case "n":
                        Console.WriteLine("Using default (hardcoded)");
                        indicator.TradingRules = new TradingRules(indicator.Id, TriggerType.Deviation, 10, "<", 1, ">", 1);
                        break;

                    default:
                        Console.WriteLine("Invalid input. No Rules set for {0}", indicator.Id);
                        break;
                }
                Console.WriteLine(indicator.TradingRules.DisplayRules());
            }
            
            
            
            List<TradingRules> AllTradingRules = new List<TradingRules>();
            #endregion


            #region Set Autoclicker Buy and Sell Locations
            AutoClicker.SetBuyPos();
            AutoClicker.SetSellPos();
            #endregion





            var connection = new Connection("testserver", "44031", indicators);

            // Create thread to listen for messages seperate from trade execution thread
            var ListenerThread = new Thread(connection.ListenForMessages) { Name="ListenerThread" };
            ListenerThread.Start();
            //var testThread = new Thread(connection.PrintRecieved) { Name = "testThread" };

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            //AutoClicker.Click();
            stopWatch.Stop();


            stopWatch.Reset();

            var tradeExecutor  = new TradeExecuter(connection, indicators);

            var TradeExecutorThread = new Thread(tradeExecutor.Start);
            TradeExecutorThread.Start();


            
            connection.StopListening();
            
        }
    }
}