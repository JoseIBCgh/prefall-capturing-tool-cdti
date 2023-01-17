using MathNet.Numerics.LinearAlgebra;
using System.Numerics;
using WisewalkSDK;
using EKFCalc = ibcdatacsharp.EKF.EKF;
using Vector3 = MathNet.Numerics.LinearAlgebra.Vector<float>;

namespace ibcdatacsharp.UI.Filters
{
    public class EKF : Filter
    {
        private EKFCalc EKFCalc;
        public EKF()
        {
            name = "EKF";
            EKFCalc = new EKFCalc(deltaT: 0.01f, spectralNoise: 0.3f * 0.3f, magnetic_dip_angle: 60f, NED: true);
        }
        public override void filter(ref WisewalkData data)
        {
            /*
            Quaternion[] result = new Quaternion[data.Quat.Count];
            for (int index = 0; index < data.Quat.Count; index++)
            {
                Quaternion q = new Quaternion((float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z, (float)data.Quat[index].W);
                Vector3 gyr = Vector3.Build.Dense(new float[] { data.Imu[index].gyro_x, data.Imu[index].gyro_y, data.Imu[index].gyro_z });
                Vector3 acc = Vector3.Build.Dense(new float[] { data.Imu[index].acc_x, data.Imu[index].acc_y, data.Imu[index].acc_z });
                Vector3 mag = Vector3.Build.Dense(new float[] { data.Imu[index].mag_x, data.Imu[index].mag_y, data.Imu[index].mag_z });
                result[index] = EKFCalc.update(q, gyr, acc, mag);
            }
            return result;
            */
            for (int index = 0; index < data.Quat.Count; index++)
            {
                Quaternion q = new Quaternion((float)data.Quat[index].X, (float)data.Quat[index].Y, (float)data.Quat[index].Z, (float)data.Quat[index].W);
                Vector3 gyr = Vector3.Build.Dense(new float[] { data.Imu[index].gyro_x, data.Imu[index].gyro_y, data.Imu[index].gyro_z });
                Vector3 acc = Vector3.Build.Dense(new float[] { data.Imu[index].acc_x, data.Imu[index].acc_y, data.Imu[index].acc_z });
                Vector3 mag = Vector3.Build.Dense(new float[] { data.Imu[index].mag_x, data.Imu[index].mag_y, data.Imu[index].mag_z });
                Quaternion result = EKFCalc.update(q, gyr, acc, mag);
                data.Quat[index].X = result.X;
                data.Quat[index].Y = result.Y;
                data.Quat[index].Z = result.Z;
                data.Quat[index].W = result.W;
            }
        }
    }
}
