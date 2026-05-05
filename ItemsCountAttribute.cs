using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Aspects
{
    /// <summary>
    /// Specifies that if the method has an <see cref="IList"/> parameter,
    /// only up to the given number of items should be logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ItemsCountAttribute : Attribute
    {
        public ItemsCountAttribute(int count) 
        {
            Count = count;
        }

        public int Count { get; }
    }
}
