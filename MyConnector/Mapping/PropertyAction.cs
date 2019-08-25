using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class PropertyAction
    {
        public string Name { get; set; }
        public object Value { get; set; }
        public ActionType Action { get; set; }
    }
}
