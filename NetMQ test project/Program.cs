using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Drawing;
using System.Diagnostics;

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

            //AutoClicker.SetBuyPos();
            //AutoClicker.SetSellPos();
           // AutoClicker.Test();
            // Create instance of Connection passing values server, port, Id
            var connection = new Connection("testserver", "44031");




            List<string> UserDefinedIndicators = new List<string> // Which topics/indicator IDs to listen for
            {
                "30000"
            };
            Console.WriteLine("Inputted Indicators are: ");
            foreach(string id in UserDefinedIndicators)
            {
                Console.WriteLine(id);
            }

            // Creates array of indicator objects based on user inputted IDs
            var indicators = new Indicator[UserDefinedIndicators.Count];
            foreach (var id in UserDefinedIndicators)
            {
                indicators[UserDefinedIndicators.IndexOf(id)] = new Indicator(id);
                Console.WriteLine("Define Rules for: {0}?  (y/n)", id);
                switch (Console.ReadLine().ToLower())
                {
                    case "yes":
                    case "y":
                        break;
                    case "no":
                    case "n":

                    default:
                        Console.WriteLine("Invalid input. No Rules set for {0}", id);
                        break;
                }
                indicators[UserDefinedIndicators.IndexOf(id)].TradingRules = TradingRules.DefineByInput(indicators[UserDefinedIndicators.IndexOf(id)]);
                Console.WriteLine(indicators[UserDefinedIndicators.IndexOf(id)].TradingRules.DisplayRules());

            }
            


            List<TradingRules> AllTradingRules = new List<TradingRules>();

            foreach (var id in UserDefinedIndicators)
            {

                AllTradingRules.Add( new TradingRules(id,RulesInputType.Deviation,1,">", 10, "<", 10));

                //Console.WriteLine(AllTradingRules[UserDefinedIndicators.IndexOf(id)].DisplayRules());

            }//possibly delete if not needed

            //var autoClicker = new AutoClicker();

            AutoClicker.SetBuyPos();
            AutoClicker.SetSellPos();



            List<string> Messages = connection.ListenForMessages(UserDefinedIndicators);

            var stopWatch = new Stopwatch();

            foreach (string message in Messages)
            {
                  
                Message currentMessage = new Message(message);
                //CadRetailSalesMoM.GenerateSignal(newMessage.GetData());
                Console.WriteLine("\nID: {0} |  Data: {1}", currentMessage.GetID(), currentMessage.GetData());



                for (int i=0; i<indicators.Length;i++)
                {
                    if (currentMessage._id == indicators[i].Id)
                    {
                        stopWatch.Start();

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



                        //Console.WriteLine("StopWatch: {0}", stopWatch.Elapsed);


                    }
                }

                
                TimeSpan ts = stopWatch.Elapsed;

                string elapsedTime = (ts.TotalMilliseconds*1000).ToString();
                Console.WriteLine("Time (microseconds):  " + elapsedTime);



                //Console.WriteLine("Signal is: {0}", CadRetailSalesMoM.GenerateSignal(newMessage.GetData()));


            }

        }
    }
}
