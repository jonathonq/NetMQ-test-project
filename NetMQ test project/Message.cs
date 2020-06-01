using System.Collections.Generic;

namespace NetMQ_test_project
{
    public class Message
    {
        private string _originalString;
        public string _id;
        public double _data;  
        public Message(string message)
        {
            this._originalString = message;
            this._id = message.Substring(0, 5);
            this._data = double.Parse(message.Substring(6));
        }
        public string GetID()
        {
            return _id;
        }
        public  double GetData(string msg)
        {
            return _data;
        }
        public string GetOriginalString()
        {
            return _originalString;
        }
    }

}
