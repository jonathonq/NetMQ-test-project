using System.Collections.Generic;

namespace NetMQ_test_project
{
    public class Message
    {
        private string _originalString;
        private string _id;
        private string _data;  
        public Message(string message)
        {
            this._originalString = message;
            this._id = message.Substring(0, 5);
            this._data = message.Substring(6);
        }
        public string GetID()
        {
            return _id;
        }
        public string GetData()
        {
            return _data;
        }
        public string GetOriginalString()
        {
            return _originalString;
        }
    }

}
