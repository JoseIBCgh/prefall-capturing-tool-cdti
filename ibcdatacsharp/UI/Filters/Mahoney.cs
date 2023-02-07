using AHRS;
using ibcdatacsharp.UI.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using WisewalkSDK;

namespace ibcdatacsharp.UI.Filters
{
    public class Mahoney : Filter
    {
        private MahonyAHRS MahonyCalc;
        public Mahoney()
        {
            name = "MAH";
            MahonyCalc = new MahonyAHRS(1f / 256f, 5f);
        }
        public override void filter(ref WisewalkData data)
        {
            for (int index = 0; index < data.Quat.Count; index++)
            {
                Quaternion q = new Quaternion((float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z, (float)data.Quat[index].W);

                MahonyCalc.Quaternion = new float[] { q.W, q.X, q.Y, q.Z };
                ImuSensor imu = data.Imu[index];
                /*
                MahonyCalc.Update(
                    imu.gyro_x, imu.gyro_y, imu.gyro_z,
                    imu.acc_x, imu.acc_y, imu.acc_z,
                    imu.mag_x, imu.mag_y, imu.mag_z
                    );
                */
                MahonyCalc.Update(
                    imu.gyro_x, imu.gyro_y, imu.gyro_z,
                    Helpers.ToGs(imu.acc_x), Helpers.ToGs(imu.acc_y), Helpers.ToGs(imu.acc_z),
                    imu.mag_x, imu.mag_y, imu.mag_z
                    );
                float[] result = MahonyCalc.Quaternion;
                data.Quat[index].X = result[1];
                data.Quat[index].Y = result[2];
                data.Quat[index].Z = result[3];
                data.Quat[index].W = result[0];
            }
        }
    }
}
