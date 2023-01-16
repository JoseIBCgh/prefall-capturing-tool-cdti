using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ibcdatacsharp.UI.Filters
{
    public abstract class Filter
    {
        public string name { get; protected set; }
        public abstract Quaternion[] filter(WisewalkSDK.WisewalkData data);
    }
}
