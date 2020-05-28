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
                        indicators[UserDefinedIndicators.IndexOf(id)].TradingRules = TradingRules.DefineByInput(indicators[UserDefinedIndicators.IndexOf(id)]);
                        break;
                    case "no":
                    case "n":
                        foreach(var indicator in indicators)
                        {
                            indicator.TradingRules = new TradingRules(id, RulesInputType.Deviation, 1, "<", 10, ">", 10);
                        }
                        break;

                    default:
                        Console.WriteLine("Invalid input. No Rules set for {0}", id);
                        break;
                }
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
            var indiArray = new string[UserDefinedIndicators.Count];
            for (int i = 0; i < indiArray.Length; i++)
            {
                indiArray[i] = UserDefinedIndicators[i];
            }
           



            //List<string> Messages = connection.ListenForMessages(UserDefinedIndicators);
            string[] Messages = connection.ListenForMessages(indiArray);


            var stopWatch = new Stopwatch();


            foreach (string message in Messages)
            {

                Message currentMessage = new Message(message);

                //Console.WriteLine(Messages.Count);

                //CadRetailSalesMoM.GenerateSignal(newMessage.GetData());
                //Console.WriteLine("\nID: {0} |  Data: {1}", currentMessage.GetID(), currentMessage.GetData());

                stopWatch.Start();


                for (int i=0; i<indicators.Length;i++)
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
                        TimeSpan ts = stopWatch.Elapsed;

                        string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                        ts.Hours, ts.Minutes, ts.Seconds,
                        ts.Milliseconds / 10);

                        Console.WriteLine("RunTime " + elapsedTime);


                        //Console.WriteLine("StopWatch: {0}", stopWatch.Elapsed);


                    }
                }

                
                



                //Console.WriteLine("Signal is: {0}", CadRetailSalesMoM.GenerateSignal(newMessage.GetData()));


            }

        }
    }
}
