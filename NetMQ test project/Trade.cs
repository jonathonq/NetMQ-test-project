using System;
using System.Diagnostics;
<<<<<<< HEAD
using System.Threading;
=======
>>>>>>> 37ea97f8c9c6cab027dc5fc24188d17b523c37be

namespace NetMQ_test_project
{
    public class Trade
    {
        private Connection _connection;
        private Indicator[] _indicators;

        public Trade(Connection connection, Indicator[] indicators)
        {
            this._connection = connection;
            this._indicators = indicators;
        }

        public void Start()
        {
<<<<<<< HEAD

            //Thread listenThread = new Thread(_connection.);
            




            /*while (true)
            {
                if (_connection.Received[0] != String.Empty)
=======
            _connection.ListenForMessages();
            while (true)
            {
                if (_connection.Received != null)
>>>>>>> 37ea97f8c9c6cab027dc5fc24188d17b523c37be
                {
                    var stopWatch = new Stopwatch();

                    for (int i = 0; i < _indicators.Length; i++)
                    {
<<<<<<< HEAD

=======
>>>>>>> 37ea97f8c9c6cab027dc5fc24188d17b523c37be
                        var msg = new Message(_connection.Received[i]);
                        if (msg._id == _indicators[i].Id)
                        {
                            switch (_indicators[i].TradingRules.GenerateSignal(msg._data))
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

                            string elapsedTime = String.Format("{0}ms | {1}Microseconds",
                            ts.TotalMilliseconds, ts.TotalMilliseconds * 1000);

                            Console.WriteLine("RunTime " + elapsedTime);
                        }
                    }
                }
<<<<<<< HEAD
            }*/
=======
            }
>>>>>>> 37ea97f8c9c6cab027dc5fc24188d17b523c37be
        }
    }
}