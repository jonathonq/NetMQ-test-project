using System;
using System.Runtime.CompilerServices;

namespace NetMQ_test_project
{
    public class ConflictManager
    {
    }

    public enum RulesInputType
    {
        Absolute,
        Deviation
    }

    public class TradingRules
    {
        //This class is for defining rules to a specific indicator
        public string IndicatorID { get; set; }

        public double _consensus;

        private RulesInputType RulesInputType;

        private string _buyOperator;
        private string _sellOperator;
        private Operator _buyOperator1;
        private Operator _sellOperator1;

        private double _buyDevTrigger;
        private double _buyAbsTrigger;

        private double _sellDevTrigger;
        private double _sellAbsTrigger;

        private string _displayRules;

        public TradingRules(string id, RulesInputType type, double cons, string buyOper, double buyTrigger, string sellOper, double sellTrigger)
        {
            this.IndicatorID = id;
            this.RulesInputType = type;

            this._consensus = cons;
            this._buyOperator = buyOper;
            this._sellOperator = sellOper;
            //this._buyOperator1 = ParseStringToOperator(buyOper);
            // this._sellOperator1 = ParseStringToOperator(sellOper);

            if (this.RulesInputType == RulesInputType.Absolute)
            {
                //if rule is based on absolute figure then assign directly to AbsTriggers
                //and calculate what deviation would be so it can be displayed later
                this._buyAbsTrigger = buyTrigger;
                this._buyDevTrigger = buyTrigger - cons;
                this._sellAbsTrigger = sellTrigger;
                this._sellDevTrigger = sellTrigger - cons;
            }
            else if (this.RulesInputType == RulesInputType.Deviation)
            {
                //if rule is based on a deviation figure then do the opposite
                //better to calculate this now than when generating a signal
                //later when generating signal, use absolute figure since that's how it is recieved
                this._buyDevTrigger = buyTrigger;
                this._buyAbsTrigger = cons + buyTrigger;
                this._sellDevTrigger = sellTrigger;
                this._sellAbsTrigger = cons + buyTrigger;
            }

            this._displayRules = String.Format("ID: {0}, If Dev {1} {2}, BUY. If Dev {3} {4}, SELL", IndicatorID, _buyOperator, _buyDevTrigger, _sellOperator, _sellDevTrigger);
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
            this._buyOperator = oprtr;
            this._buyDevTrigger = dev;
        }

        public void SetSellTrigger(string oprtr, double dev)
        {
            this._sellOperator = oprtr;
            this._sellDevTrigger = dev;
        }

        public static double GetDeviation(double cons, double act)
        {
            return act - cons;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Signal GenerateSignal(double data)
        {
            switch (_buyOperator1)
            {
                case Operator.GreaterThan:
                    if (data > _buyAbsTrigger)
                    {
                        return Signal.Buy;
                    }
                    break;

                case Operator.LessThan:
                    if (data < _buyAbsTrigger)
                    {
                        return Signal.Buy;
                    }
                    break;

                case Operator.GreaterOrEqualTo:
                    if (data >= _buyAbsTrigger)
                    {
                        return Signal.Buy;
                    }
                    break;

                case Operator.LessOrEqualTo:
                    if (data <= _buyAbsTrigger)
                    {
                        return Signal.Buy;
                    }
                    break;

                case Operator.Equals:
                    if (data == _buyAbsTrigger)
                    {
                        return Signal.Buy;
                    }
                    break;

                default:
                    //return Signal.No_Trade;
                    break;
            }
            switch (_sellOperator1)
            {
                case Operator.GreaterThan:
                    if (data > _sellAbsTrigger)
                    {
                        return Signal.Sell;
                    }
                    break;

                case Operator.LessThan:
                    if (data < _sellAbsTrigger)
                    {
                        return Signal.Sell;
                    }
                    break;

                case Operator.GreaterOrEqualTo:
                    if (data >= _sellAbsTrigger)
                    {
                        return Signal.Sell;
                    }
                    break;

                case Operator.LessOrEqualTo:
                    if (data <= _sellAbsTrigger)
                    {
                        return Signal.Sell;
                    }
                    break;

                case Operator.Equals:
                    if (data == _sellAbsTrigger)
                    {
                        return Signal.Sell;
                    }
                    break;

                default:
                    break;
            }

            return Signal.No_Trade;

            /*
             //Checks if data matches buy absolute trigger (regardless of how it was inputted as)
             if (ParseStringToOperator(_buyOperator, data, _buyAbsTrigger))
             {
                 return Signal.Buy;
                 //return String.Format("buy (Deviation: {0} is: {1} Trigger: {2})",dev, _buyOperator,_buyTrigger);
             }
             else if (ParseStringToOperator(_sellOperator, data, _sellAbsTrigger))
             {
                 return Signal.Sell;
                 //return String.Format("sell (Deviation: {0} is: {1} Trigger: {2})", dev, _sellOperator, _sellTrigger);
             }
             else
             {
                 return Signal.No_Trade;
             }*/
        }

        public string DisplayRules()
        {
            return String.Format("Trading Rules:\n" +
                "If Data {0} {1} (Deviation {2}{3}), BUY\nIf Data {4} {5} (Deviation {6}{7}), Sell\n", _buyOperator, _buyAbsTrigger, _buyOperator, _buyDevTrigger, _sellOperator, _sellAbsTrigger, _sellOperator, _sellDevTrigger);
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
            else if (IsOperator(input.Substring(0, 1)))
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

            return new TradingRules(indicator.GetId(), RulesInputType.Deviation, cons, buyOp, buyDev, sellOp, sellDev);
        }
    }
}