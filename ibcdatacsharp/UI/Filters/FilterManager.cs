using System.Collections.Generic;
using System.Diagnostics;

namespace ibcdatacsharp.UI.Filters
{
    public class FilterManager
    {
        public List<Filter> filters;
        private int index;
        public FilterManager()
        {
            filters = new List<Filter>();
            filters.Add(new None());
            filters.Add(new EKF());
        }
        public void filter(ref WisewalkSDK.WisewalkData data)
        {
            filters[index].filter(ref data);
        }
        public void changeFilter(Filter newFilter)
        {
            index = filters.IndexOf(newFilter);
            Trace.WriteLine("changeFilter");
            Trace.WriteLine(index);
        }
    }
}
