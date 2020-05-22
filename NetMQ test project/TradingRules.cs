using System;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
namespace NetMQ_test_project
{
    internal static partial class Program
    {
        public class TradingRules
        {
            public static List<string> IndicatorIDs = new List<string>();
            float Consensus;
            float Trigger;

            float Actual;
            float Deviation;
            string Signal;

            public TradingRules(float cons,  float trig)
            {
                this.Consensus = cons;
                this.Trigger = trig;
            }
        }
    }
}
