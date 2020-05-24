namespace NetMQ_test_project
{
    public class Indicator
    {
        private string _id;
        private string _name;
        private double _consensus;

        public TradingRules TradingRules { get; set; }
        public string Signal { get; set; }



        public Indicator(string id)
        {
            this._id = id;
        }
        public Indicator(string id, string name)
            :this(id)
        {
            this._name = name;
        }
        public Indicator(string id, string name, double consensus)
            :this(id, name)
        {
            this._consensus = consensus;
        }

        public string Id()
        {
            return _id;
        }

        public string DisplayTradingRules()
        {
            return TradingRules.DisplayRules();
        }

    }
}
