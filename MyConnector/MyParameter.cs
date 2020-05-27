using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector
{
    public class MyParameter
    {
        public MyParameter()
        {
        }

        public MyParameter(string parameterName, object parameterValue)
        {
            ParameterName = parameterName ?? throw new ArgumentNullException(nameof(parameterName));
            ParameterValue = parameterValue ?? throw new ArgumentNullException(nameof(parameterValue));
        }

        public string ParameterName { get; set; }
        public object ParameterValue { get; set; }
    }
}
