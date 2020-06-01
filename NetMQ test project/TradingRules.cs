using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Data;
using System.Runtime.CompilerServices;
using Microsoft.VisualBasic.CompilerServices;
using System.Net.Http.Headers;

namespace NetMQ_test_project
{
    public enum TriggerType
    {
        Absolute,
        Deviation
    }
    public class TradingRules
    {
        //This class is for defining rules to a specific indicator
        public string IndicatorID { get; set; }
        public double _consensus;

        TriggerType RulesInputType;

        private string _buyOperatorStr;
        private string _sellOperatorStr;
        private Operator _buyOperator;
        private Operator _sellOperator;
        private Func<double, bool> _buySignal;
        private Func<double, bool> _sellSignal;


        private double _buyDevTrigger;
        private double _buyAbsTrigger;
        
        private double _sellDevTrigger;
        private double _sellAbsTrigger;


        private string _displayRules;

        public TradingRules(string id, TriggerType type, double cons, string buyOper, double buyTrigger, string sellOper, double sellTrigger)
        {
            this.IndicatorID = id;
            this.RulesInputType = type;

            this._consensus = cons;
            this._buyOperatorStr = buyOper;
            this._sellOperatorStr = sellOper;
            this._buyOperator = ParseStringToOperator(buyOper);
            this._sellOperator = ParseStringToOperator(sellOper);


            if (this.RulesInputType == TriggerType.Absolute)
            {
                //if rule is based on absolute figure then assign directly to AbsTriggers
                //and calculate what deviation would be so it can be displayed later 
                this._buyAbsTrigger = buyTrigger;
                this._buyDevTrigger = buyTrigger - cons;
                this._sellAbsTrigger = sellTrigger;
                this._sellDevTrigger = sellTrigger - cons;
            }
            else if (this.RulesInputType == TriggerType.Deviation)
            {
                //if rule is based on a deviation figure then do the opposite
                //better to calculate this now than when generating a signal
                //later when generating signal, use absolute figure since that's how it is recieved
                this._buyDevTrigger = buyTrigger;
                this._buyAbsTrigger = cons + buyTrigger;
                this._sellDevTrigger = sellTrigger;
                this._sellAbsTrigger = cons + buyTrigger;
            }

            this._buySignal = getFuncFromOperatorAndTrigger(_buyOperator,_buyAbsTrigger);
            this._sellSignal = getFuncFromOperatorAndTrigger(_sellOperator, _sellAbsTrigger);



            this._displayRules = String.Format("ID: {0}, If Dev {1} {2}, BUY. If Dev {3} {4}, SELL",IndicatorID,_buyOperatorStr,_buyDevTrigger,_sellOperatorStr,_sellDevTrigger);
        }


        private static Operator ParseStringToOperator(string logic)
        {
            switch (logic)
            {
                case ">": return Operator.GreaterThan;
                    //return x > y;
                case "<": return Operator.LessThan;
                     //return x < y;
                case ">=": return Operator.GreaterOrEqualTo;
                    //return x >= y;
                case "<=": return Operator.LessOrEqualTo;
                    //return x <= y;
                case "==": return Operator.Equals;
                    //return x == y;
                default: throw new Exception("invalid logic set in trigger");
            }
        }
        private static bool IsOperator(string input)
        {
            if (input == ">" || input == "<" || input == ">=" || input == "<=" || input == "==")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private enum Operator
        {
            GreaterThan,
            LessThan,
            GreaterOrEqualTo,
            LessOrEqualTo,
            Equals
        }


        public void SetBuyTrigger(string oprtr, double dev)
        {
            this._buyOperatorStr = oprtr;
            this._buyDevTrigger = dev;
        }
        public void SetSellTrigger(string oprtr, double dev)
        {
            this._sellOperatorStr = oprtr;
            this._sellDevTrigger = dev;
        }

        public static double GetDeviation(double cons, double act)
        {
            return act - cons;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Signal GenerateSignal(double data)
        {

            if (_buySignal(data))
            {
                return Signal.Buy;
            }
            else if (_sellSignal(data))
            {
                return Signal.Sell;
            }
            else
            {
                return Signal.No_Trade;
            }


        }

        private Func<double,bool> getFuncFromOperatorAndTrigger(Operator @operator, double trigger)
        {
            Func<double, bool> f = x => false;
            switch (@operator)
            {
                case Operator.GreaterThan:
                    f = data =>
                    {
                        if (data > trigger)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                    return f;

                case Operator.LessThan:
                    f = data =>
                    {
                        if (data < trigger)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                    return f;
                case Operator.GreaterOrEqualTo:
                    f = data =>
                    {
                        if (data >= trigger)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                    return f;
                case Operator.LessOrEqualTo:
                    f = data =>
                    {
                        if (data <= trigger)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                    return f;
                case Operator.Equals:
                    f = data =>
                    {
                        if (data == trigger)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    };
                    return f;
                default:
                    break;
            }
            //TODO: Throw exception here. (Figure out correct syntax)
            Console.WriteLine("getFuncFromOperatorAndTrigger: Error. Signal will always return false");
            return f;

        }

        

        public string DisplayRules()
        {
            return String.Format("Trading Rules:\n" +
                "If Data {0} {1} (Deviation {2}{3}), BUY\nIf Data {4} {5} (Deviation {6}{7}), Sell\n", _buyOperatorStr, _buyAbsTrigger, _buyOperatorStr, _buyDevTrigger, _sellOperatorStr, _sellAbsTrigger, _sellOperatorStr, _sellDevTrigger);
        }

        public static TradingRules DefineByInput(Indicator indicator)
        {

            float cons;
            string buyOp;
            float buyDev;
            string sellOp;
            float sellDev;
            string input;

            Console.WriteLine("\nDefining Trade Rules for {0}.", indicator.GetId());
            Console.WriteLine("\nWhat is the consensus figure?");
            cons = float.Parse(Console.ReadLine());

            Console.WriteLine("\nBuy Trigger: ");
            input = Console.ReadLine();
            if (IsOperator(input.Substring(0, 2)))
            {
                buyOp = input.Substring(0, 2);
                buyDev = float.Parse(input.Substring(2));
            }
            else if(IsOperator(input.Substring(0, 1)))
            {
                buyOp = input.Substring(0, 1);
                buyDev = float.Parse(input.Substring(1));
            }
            else
            {
                buyOp = "Invalid Operator";
                buyDev = 900000000;
                Console.WriteLine("Invalid Input");
            }
            Console.WriteLine("Buy Trigger set to: {0}{1}", buyOp, buyDev);



            Console.WriteLine("\nSell Trigger: ");
            input = Console.ReadLine();

            if (IsOperator(input.Substring(0, 2)))
            {
                sellOp = input.Substring(0, 2);
                sellDev = float.Parse(input.Substring(2));
            }
            else if (IsOperator(input.Substring(0, 1)))
            {
                sellOp = input.Substring(0, 1);
                sellDev = float.Parse(input.Substring(1));
            }
            else
            {
                sellOp = "Invalid Operator";
                sellDev = 900000000;
                Console.WriteLine("Invalid Input");
            }
            Console.WriteLine("\nSell Trigger set to: {0}{1}", sellOp, sellDev);



            return new TradingRules(indicator.GetId(), TriggerType.Deviation, cons,buyOp,buyDev,sellOp,sellDev);
        }

    }

}
