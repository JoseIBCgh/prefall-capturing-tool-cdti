using ibcdatacsharp.Common;
using ibcdatacsharp.UI.DeviceList.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;
using static WisewalkSDK.Protocol_v3;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    // Guarda la información de un IMU
    public class IMUInfo : BaseObject
    {
        private static HashSet<int> idsUsed = new HashSet<int>();
        private static Dictionary<Joint, IMUInfo> jointsUsed = new Dictionary<Joint, IMUInfo>();
        public static bool allRotationJointsUsed()
        {
            foreach (Joint joint in Enum.GetValues(typeof(Joint)))
            {
                if (!jointsUsed.ContainsKey(joint))
                {
                    return false;
                }
            }
            return true;
        }
        public int? id
        {
            get { return GetValue<int>("id"); }
            set { SetValue("id", value); }
        }
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        public string address
        {
            get { return GetValue<string>("address"); }
            set { SetValue("address", value); }
        }
        public int? battery
        {
            get { return GetValue<int?>("battery"); }
            set { SetValue("battery", value); }
        }
        public bool connected
        {
            get { return GetValue<bool>("connected"); }
            set { 
                SetValue("connected", value);
                if (!value)
                {
                    if (used)
                    {
                        used = false;
                    }
                }
            }
        }
        public bool used
        {
            get { return GetValue<bool>("used"); }
            set { SetValue("used", value); }
        }
        public string? fw
        {
            get { return GetValue<string>("fw"); }
            set { SetValue("fw", value); }
        }

        public string A
        {
            get { return GetValue<string>("A"); }
            set { 
                if(value != "" && !used)
                {
                    used = true;
                }
                SetValue("A", value); 
            }
        }
        public Joint? joint
        {
            get { return GetValue<Joint?>("joint"); }
            set
            {
                if (joint != null) // Libera la que estaba usando
                {
                    jointsUsed.Remove(joint.Value);
                }
                if (value != null)
                {
                    if (jointsUsed.ContainsKey(value.Value)) // Estaba usado ese side?
                    {
                        IMUInfo imuReplaced = jointsUsed[value.Value]; // Insole que usaba ese side
                        imuReplaced.replaceJoint();
                    }
                    jointsUsed[value.Value] = this;
                }
                SetValue("joint", value);
            }
        }
        public void replaceJoint()
        {
            Joint? oldJoint = this.joint;
            Joint? unusedJoint = getUnusedJoint();
            jointsUsed.Remove(oldJoint.Value);
            joint = unusedJoint;
        }
        private static Joint? getUnusedJoint()
        {
            foreach (Joint joint in Enum.GetValues(typeof(Joint)))
            {
                if (!jointsUsed.ContainsKey(joint))
                {
                    return joint;
                }
            }
            return null;
        }
        public byte? handler { get; set; }

        public void checkJAUpdate()
        {
            NotifyChange("connected");
        }
        public IMUInfo() { }
        public IMUInfo(string name, string address)
        {
            this.name = name;
            this.address = address;
            this.battery = null;
            this.connected = false;
            this.used = false;
            this.fw = null;
            this.A = "";
        }
        public void setID()
        {
            id = getNextID();
        }
        private static int getNextID()
        {
            //Trace.WriteLine("getNextID");
            //Trace.WriteLine(idsUsed.Count);
            for (int i = 0; i < idsUsed.Count; i++)
            {
                if (!idsUsed.Contains(i))
                {
                    idsUsed.Add(i);
                    return i;
                }
            }
            int id = idsUsed.Count;
            idsUsed.Add(id);
            return id;
        }
        public static void removeIMU(IMUInfo imu)
        {
            //Trace.WriteLine("removeIMU");
            //Trace.WriteLine(imu.id);
            idsUsed.Remove((int)imu.id);
        }
    }
}
