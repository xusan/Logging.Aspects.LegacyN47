using System;
using System.Collections.Generic;
using System.Text;

namespace Logging.Aspects
{
    /// <summary>
    /// Prevents the decorated method from being logged.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class ExcludeFromLogAttribute : Attribute
    {
    }
}
