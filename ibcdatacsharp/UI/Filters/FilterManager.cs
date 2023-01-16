using System.Collections.Generic;

namespace ibcdatacsharp.UI.Filters
{
    public class FilterManager
    {
        public List<Filter> filters;
        public FilterManager()
        {
            filters = new List<Filter>();
            filters.Add(new None());
            filters.Add(new EKF());
        }
    }
}
