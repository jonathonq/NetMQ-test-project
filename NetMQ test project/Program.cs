using System;
using System.Collections.Generic;

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
            Console.WriteLine(title);

            // Create instance of Connection passing values server, port, Id
            var connection = new Connection("testserver", "44031");

            List<string> UserDefinedIndicators = new List<string> // Which topics/indicator IDs to listen for
            {
                "30000"
            };

            Console.WriteLine("Inputted Indicators are: ");
            foreach (string id in UserDefinedIndicators)
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
                        foreach (var indicator in indicators)
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

            AutoClicker.SetBuyPos();
            AutoClicker.SetSellPos();
            var trade = new Trade(connection, indicators);
            trade.Start();
        }
    }
}