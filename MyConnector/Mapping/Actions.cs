using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace MyConnector.Mapping
{
    public enum ActionType
    {
        None,
        Add,
        Update
    }

    public enum OnDelete
    {
        [Description("RESTRICT")]
        Restrict = 0,
        [Description("CASCADE")]
        Cascade = 1,
        [Description("SET NULL")]
        SetNull = 2,
        [Description("SET DEFAULT")]
        SetDefault = 3,
        [Description("NO ACTION")]
        NoAction = 4
    }

    public enum OnUpdate
    {
        [Description("RESTRICT")]
        Restrict = 0,
        [Description("CASCADE")]
        Cascade = 1,
        [Description("SET NULL")]
        SetNull = 2,
        [Description("SET DEFAULT")]
        SetDefault = 3,
        [Description("NO ACTION")]
        NoAction = 4
    }
}
