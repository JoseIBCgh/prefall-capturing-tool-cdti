using OpenCvSharp.Flann;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WisewalkSDK;

namespace ibcdatacsharp.UI.Filters
{
    public class None : Filter
    {
        public None()
        {
            name = "None";
        }
        public override void filter(ref WisewalkData data)
        {
            /*
            Quaternion[] result = new Quaternion[data.Quat.Count];
            for(int index = 0; index < data.Quat.Count; index++)
            {
                result[index] = new Quaternion((float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z, (float)data.Quat[index].W);
            }
            return result;
            */
        }
    }
}
