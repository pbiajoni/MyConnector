using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class FieldAction
    {
        public Field Field { get; set; }
        public ActionType Action { get; set; }
        public List<PropertyAction> PropertyActions { get; set; }

        public FieldAction()
        {
            if(PropertyActions is null)
            {
                PropertyActions = new List<PropertyAction>();
            }
        }
    }
}
