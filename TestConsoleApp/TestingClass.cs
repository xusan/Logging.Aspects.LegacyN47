using Logging.Aspects;
using System;

namespace TestConsoleApp
{
    [LogMethods]
    public class TestingClass
    {
        int d;
        public void Method1()
        {
            d++;
        }

        public void Method2()
        {
            d++;
        }

        public void Method3()
        {
            d++;
            throw new Exception("Test error");
        }
    }
}
