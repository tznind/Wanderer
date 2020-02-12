using System;

namespace Tests
{
    public class OptionNotAvailableException : Exception
    {
        public OptionNotAvailableException(string s):base(s)
        {
            
        }
    }
}