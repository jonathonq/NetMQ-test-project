using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
namespace NetMQ_test_project
{

    public class TradingRules
    {
        private string IndicatorId;
        private double _buyTrigger;
        private string _buyOperator;
        private double _sellTrigger;
        private string _sellOperator;

        private double _consensus;

        public TradingRules()
        {

        }


        public TradingRules(string id, double cons, string buyOper, double buyDev, string sellOper, double sellDev)
        {
            this.IndicatorId = id;
            this._buyOperator = buyOper;
            this._buyTrigger = buyDev;
            this._sellOperator = sellOper;
            this._sellTrigger = sellDev;
            this._consensus = cons;
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
            return cons - act;
        }

        public string GenerateSignal(string data)
        {
            double doubleData = double.Parse(data); // data comes as string so convert to double
            double dev = GetDeviation(_consensus, doubleData);
            if (Operator(_buyOperator, dev, _buyTrigger))
            {
                // if deviation matches buy trigger criteria then buy signal
                return "buy";
            }
            else if (Operator(_sellOperator, dev, _sellTrigger))
            {
                return "sell";
            }
            else
            {
                return "No signal";
            }

        }
        public string GetID()
        {
            return IndicatorId;
        }

    }

}
