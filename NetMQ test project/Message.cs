using System.Collections.Generic;
using System;
using System.Diagnostics;

namespace NetMQ_test_project
{
    public class Message
    {
        private string _originalString;
        public string _id;
        public string _dataString;
        public double _data;  

        public string GetID(string msg)
        {

            return _id;
        }
        public static string GetData(string indi, string msg)
        {
            var GetDataSW = new Stopwatch();
            GetDataSW.Start();

            int index = FastIndexOf(msg, indi);
            GetDataSW.Stop();
            string data = "";//msg.Substring(startIndex,msg.IndexOf(" ",startIndex, StringComparison.OrdinalIgnoreCase) - startIndex);
            
            Console.WriteLine(GetDataSW.Elapsed.TotalMilliseconds);
            return data;
        }
        public string GetOriginalString()
        {
            return _originalString;
        }

        public static int SearchString(string strToSearch, string strKeyToLookFor)
        {
            return (strToSearch.IndexOf(strKeyToLookFor, 0, StringComparison.OrdinalIgnoreCase));
            //return (strToSearch.Length - strToSearch.Replace(strKeyToLookFor, String.Empty).Length) / strKeyToLookFor.Length;
        }


        static int FastIndexOf(string source, string pattern)
        {
            if (pattern == null) throw new ArgumentNullException();
            if (pattern.Length == 0) return 0;
            if (pattern.Length == 1) return source.IndexOf(pattern[0]);
            bool found;
            int limit = source.Length - pattern.Length + 1;
            if (limit < 1) return -1;
            // Store the first 2 characters of "pattern"
            char c0 = pattern[0];
            char c1 = pattern[1];
            // Find the first occurrence of the first character
            int first = source.IndexOf(c0, 0, limit);
            while (first != -1)
            {
                // Check if the following character is the same like
                // the 2nd character of "pattern"
                if (source[first + 1] != c1)
                {
                    first = source.IndexOf(c0, ++first, limit - first);
                    continue;
                }
                // Check the rest of "pattern" (starting with the 3rd character)
                found = true;
                for (int j = 2; j < pattern.Length; j++)
                    if (source[first + j] != pattern[j])
                    {
                        found = false;
                        break;
                    }
                // If the whole word was found, return its index, otherwise try again
                if (found) return first;
                first = source.IndexOf(c0, ++first, limit - first);
            }
            return -1;
        }
    }

}
