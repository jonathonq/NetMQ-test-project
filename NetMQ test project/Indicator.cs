namespace NetMQ_test_project
{
    public class Indicator
    {
        private string _id;
        private float _consensus = 100; //default value 100 for testing. Remove later

        public Indicator(string id)
        {
            this._id = id;
        }
        public Indicator(string id, float cons)
            :this(id)
        {
            this._consensus = cons;
        }

        public string Id()
        {
            return _id;
        }
        public float Consensus()
        {
            return _consensus;
        }
    }
}
