using Logging.Aspects.Legacy;
using MethodDecorator.Fody.Interfaces;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace Logging.Aspects
{
    /// <summary>
    /// Logs calls to the decorated method or all methods of the decorated class.
    /// When applied to a class, all methods are logged except constructors.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Assembly | AttributeTargets.Module)]
    public class LogMethodsAttribute : Attribute, IMethodDecorator
    {
        public static ILoggerService LoggingService { get; set; } = new AppLoggerService();        
        private const string FAILED_TAG = "💥Exception in";

        private MethodBase Method = null;
        private object[] Args = new object[0];

        public void Init(object instance, MethodBase method, object[] args)
        {
            Method = method;
            Args = args;
        }

        /// <summary>
        /// OnEntry() is called when marked method is executed. Gets the method name and parametters to log
        /// </summary>
        public virtual void OnEntry()
        {
            if(LoggingService == null)
            {
                return;
            }

            var excludeAttr = Method.GetCustomAttribute<ExcludeFromLogAttribute>();
            if (excludeAttr != null)
            {
                return;
            }

            var itemsCountAttr = Method.GetCustomAttribute<ItemsCountAttribute>();
            int itemsCount = 10;
            if (itemsCountAttr != null)
            {
                itemsCount = itemsCountAttr.Count;
            }

            string fullMethodName = GetMethodFullName(Method);
            try
            {
                if (!EnsureIsMethod(fullMethodName))
                    return;

                if (Args.Length > 0)
                {
                    var parameters = Method.GetParameters().ToDictionary(key => key.Name, value => Args[value.Position]);
                    LoggingService.LogMethodStarted($"{fullMethodName}({parameters.ToDebugString(itemsCount)});");                    
                }
                else
                {
                    LoggingService.LogMethodStarted($"{fullMethodName}();");
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex);
            }
        }

        
        /// <summary>
        /// OnExit() is called when execution of marked method is completed
        /// Curently OnExit works only for "void Method" it  doesn't work good for Task<> method, so we don't need it
        /// </summary>        
        public void OnExit()
        {
           
        }

        /// <summary>
        /// OnException() is called when marked method crashed
        /// </summary>
        public virtual void OnException(Exception exception)
        {
            string fullMethodName = GetMethodFullName(Method);
            try
            {
                if (Args.Length > 0)
                {
                    var parameters = Method.GetParameters().ToDictionary(key => key.Name, value => Args[value.Position]);
                    LoggingService.Log($"{FAILED_TAG} {fullMethodName}({parameters.ToDebugString()}); Error: {exception}");
                }
                else
                {
                    LoggingService.Log($"{FAILED_TAG} {fullMethodName}(); Error: {exception}");
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogError(ex);
            }            
        }


        #region Helpers
        private string GetMethodFullName(MethodBase method)
        {
            string methodName = method.Name;
            string className = method.ReflectedType.Name;
            string fullMethodName = $"{className}.{methodName}";

            return fullMethodName;
        }

        private bool EnsureIsMethod(string fullMethodName)
        {
            //accept only method and exclude property or event
            //Property and events will have <> or set_, get_
            if (fullMethodName.Contains("ctor"))
                return false;
            if (fullMethodName.Contains("get_") || fullMethodName.Contains("set_"))
                return false;
            if (fullMethodName.Contains("add_") || fullMethodName.Contains("remove_"))
                return false;
            if(fullMethodName.Contains("<"))
                return false;
            return true;
        }
        #endregion
    }  

    public static class ParamsExtensions
    {
        public static string ToDebugString(this IDictionary<string, object> dictionary, int count = 10)
        {
            if (dictionary == null)
                return "";

            if (dictionary.Count == 0)
                return "";

            var kvalues = dictionary.Select(kv => GetKeyValueString(kv, count)).ToArray();
            return string.Join(", ", kvalues);
        }

        private static string GetKeyValueString(KeyValuePair<string, object> kv, int count = 10)
        {
            string kvStr = null;
            if (kv.Value == null)
            {
                kvStr = $"{kv.Key} = null";
            }
            else if (kv.Value is string)
            {
                kvStr = $"{kv.Key} = '{kv.Value}'";
            }
            else
            {
                if (kv.Value is IList)
                {
                    var array = (IList)kv.Value;
                    var arrayTypeName = array.GetType().Name;
                    string arrayValue = null;
                    if (array.Count > 0)
                    {
                        var implementedToString = array as IToStringImplemented;
                        if (implementedToString == null)
                        {
                            var itemsStrings = string.Empty;
                            for (int i = 0; i < count; i++)
                            {
                                if (i >= array.Count)
                                    break;
                                itemsStrings += $"{array[i].ToString()}, ";
                            }
                            arrayValue = $"{arrayTypeName}[{array.Count}] = {{ {itemsStrings} .... }}";
                        }
                        else
                        {
                            arrayValue = array.ToString();
                        }
                    }
                    else
                    {
                        arrayValue = $"{arrayTypeName}[{array.Count}]";
                    }

                    kvStr = $"{kv.Key} = {arrayValue}";
                }
                else if (kv.Value is PropertyChangedEventArgs)
                {
                    var propChanged = kv.Value as PropertyChangedEventArgs;
                    if (propChanged != null)
                    {
                        kvStr = $"{kv.Key} = {propChanged.PropertyName}";
                    }
                    else
                    {
                        kvStr = $"{kv.Key} = {kv.Value}";
                    }
                }
                else
                {
                    kvStr = $"{kv.Key} = {kv.Value}";
                }
            }

            return kvStr;
        }
    }
}
