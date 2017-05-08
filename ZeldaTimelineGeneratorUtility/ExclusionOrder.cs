using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZeldaTimelineGeneratorUtility
{
    [TypeConverter(typeof(EnumBindingSourceExtension.EnumDescriptionTypeConverter))]
    public enum ExclusionOrder
    {
        [Description("No Data")]
        NoData,
        [Description("Must Be After")]
        MustBeAfter,
        [Description("Must Be Before")]
        MustBeBefore,
        [Description("Can't Be After")]
        CantBeAfter,
        [Description("Can't Be Before")]
        CantBeBefore
    }
}
