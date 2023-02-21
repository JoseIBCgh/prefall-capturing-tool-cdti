using System.Collections.Generic;

namespace ibcdatacsharp.UI.Filters
{
    public class FilterManager
    {
        public List<Filter> filters;
        private int ind;
        public FilterManager()
        {
            filters = new List<Filter>();
            filters.Add(new None());
            filters.Add(new EKF());
            filters.Add(new Mahoney());
            filters.Add(new Madgwick());
        }
        public void filter(ref WisewalkSDK.WisewalkData data)
        {
            filters[ind].filter(ref data);
        }
        public void changeFilter(Filter newFilter)
        {
            ind = filters.IndexOf(newFilter);
        }
    }
}
