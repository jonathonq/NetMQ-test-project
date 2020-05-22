namespace NetMQ_test_project
{
    internal static partial class Program
    {
        public class MessageParser
        {
            public MessageParser()
            {

            }
            public static string GetID(string str)
            {
                return str.Substring(0, 5);
            }
            public static string GetData(string str)
            {
                return str.Substring(str.IndexOf(";") + 1);
            }
        }
    }
}
