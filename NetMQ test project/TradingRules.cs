using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
namespace NetMQ_test_project
{

    public class TradingRules
    {
        //This class is for defining rules to a specific indicator
        public string IndicatorID { get; set; }
        public static string Signal;
        private double _buyTrigger;
        private string _buyOperator;
        private double _sellTrigger;
        private string _sellOperator;
        private double _consensus;
        private string _displayRules;


        public TradingRules(string id,double cons, string buyOper, double buyDev, string sellOper, double sellDev)
        {
            this.IndicatorID = id;
            this._buyOperator = buyOper;
            this._buyTrigger = buyDev;
            this._sellOperator = sellOper;
            this._sellTrigger = sellDev;
            this._consensus = cons;
            this._displayRules = String.Format("ID: {0}, If Dev {1} {2}, BUY. If Dev {3} {4}, SELL",IndicatorID,_buyOperator,_buyTrigger,_sellOperator,_sellTrigger);
        }


        private static Boolean Operator(string logic, double x, double y)
        {
            switch (logic)
            {
                case ">": return x > y;
                case "<": return x < y;
                case ">=": return x >= y;
                case "<=": return x <= y;
                case "==": return x == y;
                default: throw new Exception("invalid logic set in trigger");
            }
        }

        public void SetBuyTrigger(string oprtr, double dev)
        {
            this._buyOperator = oprtr;
            this._buyTrigger = dev;
        }
        public void SetSellTrigger(string oprtr, double dev)
        {
            this._sellOperator = oprtr;
            this._sellTrigger = dev;
        }

        private static double GetDeviation(double cons, double act)
        {
            return act - cons;
        }

        public string GenerateSignal(string data)
        {
            double doubleData = double.Parse(data); // data comes as string so convert to double
            double dev = GetDeviation(_consensus, doubleData);
            if (Operator(_buyOperator, dev, _buyTrigger))
            {
                
                // if deviation matches buy trigger criteria then buy signal
                return String.Format("buy (Deviation: {0} is: {1} Trigger: {2})",dev, _buyOperator,_buyTrigger);
            }
            else if (Operator(_sellOperator, dev, _sellTrigger))
            {
                return String.Format("sell (Deviation: {0} is: {1} Trigger: {2})", dev, _sellOperator, _sellTrigger);
            }
            else
            {
                return "No signal";
            }

        }

        public string DisplayRules()
        {
            return _displayRules;
        }

        public static TradingRules DefineByInput(Indicator indicator)
        {

            float cons;
            string buyOp;
            float buyDev;
            string sellOp;
            float sellDev;

            Console.WriteLine("\nDefining Trade Rules for {0}.", indicator.Id());
            Console.WriteLine("\nWhat is the consensus figure?");
            cons = float.Parse(Console.ReadLine());
            Console.WriteLine("Buy Trigger: Please set Operator ( < | > | <= | >= | == )");
            buyOp = Console.ReadLine();
            Console.WriteLine("Buy Trigger: Please set deviation");
            buyDev = float.Parse(Console.ReadLine());
            Console.WriteLine("Sell Trigger: Please set Operator ( < | > | <= | >= | == )");
            sellOp = Console.ReadLine();
            Console.WriteLine("Sell Trigger: Please set deviation");
            sellDev = float.Parse(Console.ReadLine());


            return new TradingRules(indicator.Id(),cons,buyOp,buyDev,sellOp,sellDev);
        }

    }

}
