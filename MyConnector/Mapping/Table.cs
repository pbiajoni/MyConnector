using System;
using System.Collections.Generic;
using System.Text;

namespace MyConnector.Mapping
{
    public class Table
    {
        public string Name { get; set; }
        public List<Field> Fields { get; set; }
        public Table()
        {
            if(Fields is null)
            {
                Fields = new List<Field>();
            }
        }

        public Table(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));

            if (Fields is null)
            {
                Fields = new List<Field>();
            }
        }

       
    }
}
