using ibcdatacsharp.Common;
using ibcdatacsharp.UI.DeviceList.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Documents;
using static WisewalkSDK.Protocol_v3;

namespace ibcdatacsharp.DeviceList.TreeClasses
{
    /// <summary>
    /// Guarda la informacion de un IMU
    /// </summary>
    public class IMUInfo : BaseObject
    {
        private static HashSet<int> idsUsed = new HashSet<int>();
        private static Dictionary<Joint, IMUInfo> jointsUsed = new Dictionary<Joint, IMUInfo>();
        /// <summary>
        /// Comrpueba si se han usado todas las Joints
        /// </summary>
        /// <returns>true si se han usado todas, false si falta alguna</returns>
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
        /// <summary>
        /// id del IMU (null si no se ha asignado)
        /// </summary>
        public int? id
        {
            get { return GetValue<int>("id"); }
            set { SetValue("id", value); }
        }
        /// <summary>
        /// Nombre del IMU
        /// </summary>
        public string name
        {
            get { return GetValue<string>("name"); }
            set { SetValue("name", value); }
        }
        /// <summary>
        /// Direccion MAC del IMU
        /// </summary>
        public string address
        {
            get { return GetValue<string>("address"); }
            set { SetValue("address", value); }
        }
        /// <summary>
        /// Batteria del IMU. null si no se sabe
        /// </summary>
        public int? battery
        {
            get { return GetValue<int?>("battery"); }
            set { SetValue("battery", value); }
        }
        /// <summary>
        /// Si el IMU esta conectado
        /// </summary>
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
        /// <summary>
        /// Si el IMU se esta usando
        /// </summary>
        public bool used
        {
            get { return GetValue<bool>("used"); }
            set { SetValue("used", value); }
        }
        /// <summary>
        /// Version firmware del IMU. Null si no se sabe aun
        /// </summary>
        public string? fw
        {
            get { return GetValue<string>("fw"); }
            set { SetValue("fw", value); }
        }
        /// <summary>
        /// Indice para el sagital angles
        /// </summary>
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
        /// <summary>
        /// Joint que esta usando el IMU (qbase o qmob) solo hay 1 IMU con cada una
        /// </summary>
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
        /// <summary>
        /// Cambia la joint si queda alguna libre. Si no la setea a null
        /// </summary>
        public void replaceJoint()
        {
            Joint? oldJoint = this.joint;
            Joint? unusedJoint = getUnusedJoint();
            jointsUsed.Remove(oldJoint.Value);
            joint = unusedJoint;
        }
        /// <summary>
        /// Obtiene una joint sin usar
        /// </summary>
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
        /// <summary>
        /// Handler de IMU (sirve para la API)
        /// </summary>
        public byte? handler { get; set; }
        /// <summary>
        /// Notifica que connected ha cambiado
        /// </summary>
        public void checkJAUpdate()
        {
            NotifyChange("connected");
        }
        /// <summary>
        /// Relacionado con el tipo de paquete del IMU
        /// </summary>
        public bool DISMAG
        {
            get { return GetValue<bool>("DISMAG"); }
            set { SetValue("DISMAG", value); }
        }
        /// <summary>
        /// Constructor vacio
        /// </summary>
        public IMUInfo() { }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name">Valor (tiene que ser un bool)</param>
        /// <param name="address">Type</param>
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
        /// <summary>
        /// Asigna un id al IMU (el mas pequeño sin usar)
        /// </summary>
        public void setID()
        {
            id = getNextID();
        }
        /// <summary>
        /// Obtiene el ID mas pequeño sin usar
        /// </summary>
        /// <returns>Id mas pequeño sin usar</returns>
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
        /// <summary>
        /// Libera el id del IMU para que pueda volver a usarse
        /// </summary>
        /// <param name="imu">IMU a liberar</param>
        public static void removeIMU(IMUInfo imu)
        {
            //Trace.WriteLine("removeIMU");
            //Trace.WriteLine(imu.id);
            idsUsed.Remove((int)imu.id);
        }
    }
}
