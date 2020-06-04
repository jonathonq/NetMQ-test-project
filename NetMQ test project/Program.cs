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
                "30000"
                
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
            var IndicatorIds = UserDefinedIndicatorIds.ToArray();

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
                        indicator.TradingRules = new TradingRules(indicator.Id, TriggerType.Deviation, 1, "<", 10, ">", 10);
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





            var connection = new Connection("testserver", "44031", IndicatorIds);

            // Create thread to listen for messages seperate from trade execution thread
            var ListenerThread = new Thread(connection.ListenForMessages) { Name="ListenerThread" };
            ListenerThread.Start();
            //var testThread = new Thread(connection.PrintRecieved) { Name = "testThread" };

            var stopWatch = new Stopwatch();
            stopWatch.Start();
            AutoClicker.Click();
            stopWatch.Stop();


            stopWatch.Reset();

            while (true)
            {
                var msg = connection._recievedSb.ToString();

                if (msg.Length>0)
                {
                    stopWatch.Start();
                    for (int i = 0; i < indicators.Length; i++)
                    {
                        if (msg.Substring(0, 5) == indicators[i].Id)
                        {
                            
                            //var signal = indicators[i].TradingRules.GenerateSignal(double.Parse(msg.Substring(6)));
                            switch (indicators[i].TradingRules.GenerateSignal(double.Parse(msg.Substring(6))))
                            {
                                case Signal.Sell:
                                    stopWatch.Stop();
                                    AutoClicker.ClickSell();
                                    Console.WriteLine("Sell signal");
                                    break;

                                case Signal.Buy:
                                    stopWatch.Stop();

                                    AutoClicker.ClickBuy();
                                    Console.WriteLine("Buy signal");
                                    break;

                                default:
                                    Console.WriteLine("No Signal");
                                    break;
                            }
                            stopWatch.Stop();
                            Console.WriteLine($"Recieved: {msg} | Signal passed: {indicators[i].TradingRules.GenerateSignal(double.Parse(msg.Substring(6)))}");
                            Console.WriteLine($"Contains {IndicatorIds[i]}");
                        }
                    }
                    
                }

                

                #region List version
                /*
                if (connection._recievedList.Count >0)
                {
                    //var str = connection._recieved.ToString();
                    stopWatch.Start();
                    for (int i = 0; i < IndicatorIds.Length; i++)
                    {
                        if (connection._recievedList[i].Contains(IndicatorIds[i]))
                        {
                            Console.WriteLine(IndicatorIds[i]);
                        }
                    }

                    stopWatch.Stop();
                    
                    break;
                }*/
                #endregion
                //break;
            }
            connection.StopListening();
            /*
            foreach (string message in Messages)
            {
                Message currentMessage = new Message(message);

                //Console.WriteLine(Messages.Count);

                //CadRetailSalesMoM.GenerateSignal(newMessage.GetData());
                //Console.WriteLine("\nID: {0} |  Data: {1}", currentMessage.GetID(), currentMessage.GetData());

                stopWatch.Start();

                for (int i = 0; i < indicators.Length; i++)
                {
                    if (currentMessage._id == indicators[i].Id)
                    {
                        switch (indicators[i].TradingRules.GenerateSignal(currentMessage._data))
                        {
                            case Signal.Sell:
                                AutoClicker.ClickSell();
                                //Console.WriteLine("Sell signal");
                                break;

                            case Signal.Buy:
                                AutoClicker.ClickBuy();
                                //Console.WriteLine("Buy signal");
                                break;

                            default:
                                //Console.WriteLine("No Signal");
                                break;
                        }
                        stopWatch.Stop();
                        Console.WriteLine("Signal: " + (indicators[i].TradingRules.GenerateSignal(currentMessage._data)).ToString());
                        TimeSpan ts = stopWatch.Elapsed;

                        string elapsedTime = String.Format("{0:00}ms | {1:00} microseconds | {2:00} ticks",
                        ts.TotalMilliseconds, ts.TotalMilliseconds * 1000, ts.Ticks);

                        Console.WriteLine("RunTime: " + elapsedTime);
                    }
                }
            }*/
        }
    }
}