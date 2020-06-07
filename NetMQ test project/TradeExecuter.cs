using System;
using System.Data;
using System.Diagnostics;
using System.Threading;

namespace NetMQ_test_project
{
    public class TradeExecuter
    {
        private Connection _connection;
        private Indicator[] _indicators;
        //private string[] _indicatorIds;
        private Indicator _singleIndicator;
        private bool _multipleIndicators;
   

        

        public TradeExecuter(Connection connection, Indicator[] indicators)
        {
            this._connection = connection;
            this._indicators = indicators;
            if (indicators.Length==1)
            {
                this._singleIndicator = indicators[0];
                this._multipleIndicators = false;
            }
            else
            {
                this._indicators = indicators;
                this._multipleIndicators = true;
            }
        }

        private void SingleIndicatorExecution()
        {
            var SingleIndicatorStopWatch = new Stopwatch();
            //For a single indicator it's better to have less nested loops and save performance
            while (true)
            {
                SingleIndicatorStopWatch.Start();
                var msg = _connection._recievedSb.ToString();
                if (msg.Length > 0)
                {

                    if (msg.Substring(0, 5) == _singleIndicator.Id)
                    {
                        //var signal = indicators[i].TradingRules.GenerateSignal(double.Parse(msg.Substring(6)));
                        switch (_singleIndicator.TradingRules.GenerateSignal(double.Parse(msg.Substring(6))))
                        {
                            case Signal.Sell:
                                SingleIndicatorStopWatch.Stop();
                                //AutoClicker.ClickSell();
                                Console.WriteLine("Sell signal");
                                break;
                            case Signal.Buy:
                                SingleIndicatorStopWatch.Stop();

                                //AutoClicker.ClickBuy();
                                Console.WriteLine("Buy signal");
                                break;
                            default:
                                Console.WriteLine("No Signal");
                                break;
                        }
                        //stopWatch.Stop();
                        Console.WriteLine($"Recieved: {msg} | Signal passed: {_singleIndicator.TradingRules.GenerateSignal(double.Parse(msg.Substring(6)))}");
                    }
                }
                SingleIndicatorStopWatch.Reset();
            }
        }

        private void MultipleIndicatorExecution()
        {
            //TODO: this method must refactored/optimized but I've been up all night and I'm tired
            //Just checked and this takes 8 full seconds to execute whis is unacceptable.
            //Gonna have to bring this down to a couple of milliseconds somehow.
            //SingleIndicator method only takes 0.2ms


            var MultiIndicatorStopWatch = new Stopwatch(); // only for debug
            MultiIndicatorStopWatch.Start();
           
            var Signals = new Signal[_indicators.Length];
            var AggregateSignal = new Signal();


            //iterates through each indicator and generates signals for them
            for (int i = 0; i < Signals.Length;)
            {
                if (_connection._recievedArray[0]!=null)
                {
                    var currentMessage = _connection._recievedArray[i];
                    if (_connection._recievedArray[i].Substring(0, 5) == _indicators[i].Id)
                    {
                        switch (_indicators[i].TradingRules.GenerateSignal(double.Parse(currentMessage.Substring(6))))
                        {
                            case Signal.Sell:
                                Signals[i] = Signal.Sell;

                                break;

                            case Signal.Buy:
                                Signals[i] = Signal.Buy;
                                break;

                            default:
                                Signals[i] = Signal.No_Trade;
                                break;
                        }
                        Console.WriteLine($"Recieved: {currentMessage}\nSignal: {Signals[i]}");
                        i++;


                    }
                }
                
            }
            MultiIndicatorStopWatch.Stop(); 
            // over 7 full seconds to execute above code.. Needs improvement (O_O)



            //check only the first value to see if it's buy or sell before iterating through them
            //if all Signals are the same, then the Aggregate signal will be assigned
            //otherwise it will be considered a conflict and a no trade signal will be given
            //This code seems to perform fairly well at about 1.5ms
            

            MultiIndicatorStopWatch.Restart();
            switch (Signals[0])
            {
                case Signal.Buy:
                    if (SignalsAgree(Signal.Buy, Signals))
                    {
                        AggregateSignal = Signal.Buy;
                    }
                    else
                    {
                        AggregateSignal = Signal.Conflict;
                    }
                    break;
                case Signal.Sell:
                    if (SignalsAgree(Signal.Sell, Signals))
                    {
                        AggregateSignal = Signal.Buy;
                    }
                    else
                    {
                        AggregateSignal = Signal.Conflict;
                    }
                    break;
                case Signal.No_Trade:
                    AggregateSignal = Signal.No_Trade;
                    break;
                default:
                    break;
            }



            MultiIndicatorStopWatch.Stop();
            Console.WriteLine($"Aggregate Signal: {AggregateSignal}");

        }

        private bool SignalsAgree(Signal firstSignal, Signal[] signals)
        {
            //this starts on signals[1] since it's called after checking the first one
            for (int i = 1; i < signals.Length; i++)
            {
                if (signals[i] != firstSignal)
                {
                    return false;
                }
            }
            return true;
        }



        public void Start()
        {
            // Since the method for multi Indicators has way more overhead, use a different code path.
            // In production, a single indicator would be preferred for better performance.
            // However there may be times when having a conflict manager is needed.
            if (_multipleIndicators)
            {
                MultipleIndicatorExecution();
            }
            else
            {
                SingleIndicatorExecution();
            }

            
        }
    }
}