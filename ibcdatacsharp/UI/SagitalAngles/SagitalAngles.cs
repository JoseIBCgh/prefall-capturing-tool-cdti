using System;
using System.Numerics;

namespace ibcdatacsharp.UI.SagitalAngles
{
    public class SagitalAngles
    {
        private int TOTAL_SENSORS = 4;
        private int TOTAL_JOINTS = 3;
        private Quaternion[] mQ_sensors_raw;
        private Quaternion[] mQ_segments;
        private Quaternion[] mQ_sensors_ref;
        private Quaternion[] mQ_compensate;
        private Quaternion idenQuat = new Quaternion(0, 0, 0, 1); // x = 1 o w = 1??

        private Quaternion mQ_virtual;
        private Quaternion mQ_left;
        private Quaternion mQ_right;

        private Quaternion[] mQ_joints;

        private Vector3 eulerAnglesZ;
        public void quaternionCalcsConnect()
        {
            mQ_sensors_raw = new Quaternion[TOTAL_SENSORS];
            mQ_segments = new Quaternion[TOTAL_SENSORS];
            mQ_sensors_ref = new Quaternion[TOTAL_SENSORS];
            mQ_compensate = new Quaternion[TOTAL_SENSORS];

            mQ_joints = new Quaternion[TOTAL_JOINTS];

            for (int i = 0; i < TOTAL_SENSORS; i++)
            {
                mQ_sensors_raw[i] = idenQuat;
                mQ_segments[i] = idenQuat;
                mQ_sensors_ref[i] = idenQuat;
                mQ_compensate[i] = idenQuat;
            }
        }
        public void calculateMounting()
        {
            mQ_sensors_ref = mQ_sensors_raw;
            for (int iSen = 0; iSen <= 3; ++iSen)
            {
                mQ_sensors_ref[iSen] = Quaternion.Normalize(mQ_sensors_ref[iSen]);
            }
        }
        public void calculateVirtualOrientation()
        {
            //Virtual axis

            Quaternion Q_virtual_creator = mQ_sensors_raw[0];

            Vector3 world_X = new Vector3(1,0,0);
            Vector3 world_Z = new Vector3(0,0,1);

             
            Vector3 vc_Z = Vector3.Transform(world_Z, Q_virtual_creator);       // Apply rotation to Z axis of the sensor
            Vector3 vc_Z_proy_XY = vc_Z - (Vector3.Dot(vc_Z, world_Z) * world_Z); // Projection: rotated Y axis on the XY plane
            Vector3 forward_line = Vector3.Normalize(vc_Z_proy_XY);              // Normalize projection to create a "forward line"

            double dotx = Vector3.Dot(world_X, forward_line);
            double detx = Vector3.Dot(world_Z, Vector3.Cross(world_X, forward_line));
            double angle = Math.Atan2(detx, dotx);                           // Determine angle between w_X and the forward line

            Quaternion Qvirtual = new Quaternion((float)angle, 0, 0, 1 );   // Construct a quat with the needed rotation
                                                                            // result quaternion = (angle, 0, 0, 1)

            mQ_virtual = Quaternion.Normalize(Qvirtual);
        }
        public void updateLeftAndRightQuats()
        {

            Quaternion Q_isb = new Quaternion(new Vector3(1, 0, 0), 90 * (float)Utils.M_PI / 180);
            Q_isb = Quaternion.Normalize(Q_isb);

            Quaternion mQ_left = Quaternion.Conjugate(Q_isb) * Quaternion.Conjugate(mQ_virtual);
            mQ_left = Quaternion.Normalize(mQ_left);

            mQ_right = mQ_virtual * Q_isb;
            mQ_right = Quaternion.Normalize(mQ_right);
        }
        public void updateSegmentsAndJoints()
        {

            //this->compensateRawQuats(mQ_sensors_raw);

            Quaternion refLiveDifference, operateLeft;

            // Store segments (ISB)

            for (int iSensor = 0; iSensor <= (TOTAL_SENSORS - 1); ++iSensor)
            {

                // Raw quats were not normalized. Ref quats were (in ModelCalibrationPanel).

                refLiveDifference = Quaternion.Normalize(mQ_sensors_raw[iSensor]) * Quaternion.Conjugate(mQ_sensors_ref[iSensor]);
                refLiveDifference = Quaternion.Normalize(refLiveDifference);

                operateLeft = mQ_left * refLiveDifference;
                operateLeft = Quaternion.Normalize(operateLeft);

                mQ_segments[iSensor] = operateLeft * mQ_right;
                mQ_segments[iSensor] = Quaternion.Normalize(mQ_segments[iSensor]);

            }

            // Store joints (ISB)

            Quaternion Q_proximal, Q_distal;

            for (int iJoint = 0; iJoint <= (TOTAL_JOINTS - 1); ++iJoint)
            {

                Q_proximal = mQ_segments[iJoint];
                Q_distal = mQ_segments[iJoint + 1];

                // 0 and 1 : right hip   [0]
                // 1 and 2 : right knee  [1]
                // 2 and 3 : right ankle [2]
                // 0 and 4 : left hip    [3]  <--- 0 and 4!

                mQ_joints[iJoint] = Quaternion.Conjugate(Q_proximal) * Q_distal;
                mQ_joints[iJoint] = Quaternion.Normalize(mQ_joints[iJoint]);

                //Tenemos los joints
            }

            /// Quaternion to euler angles conversion.

            double[] eulerAnglesZ = new double[] { 0, 0, 0 }; // 3 sagittal angles, init them with 0.0
                                                         // esto sería un array con 6 ángulos correspondientes a las articulaciones e incializados a 0.0

            for (int iAngle = 0; iAngle <= (eulerAnglesZ.Length - 1); ++iAngle)
            {
                double[] res = new double[3];
                Utils.quaternion2euler(mQ_joints[iAngle], ref res, Utils.RotSeq.yxz); // rotation sequence conventions are backwards

                // Change the angle signs according to clinical conventions

                int sign = 1;

                // change signs for right and left knees
                if (iAngle == 1 || iAngle == 4)
                {
                    sign = -1;
                }

                // LE QUITO EL SIGNO POR AHORA

                eulerAnglesZ[iAngle] = sign * res[0] * 180 / Utils.M_PI; // otherlib
            }

            this.eulerAnglesZ = new Vector3((float)eulerAnglesZ[0], (float)eulerAnglesZ[1], (float)eulerAnglesZ[2]);
            //Esto es lo que obtenemos: eulerAnglesZ , mQ_sensors_raw, mQ_segments, mQ_joints 
        }
    }
}
