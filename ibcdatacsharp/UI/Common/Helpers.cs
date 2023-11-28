//#define NORMALIZE_ANGLE_STATS

using System.Windows.Media;
using System.Windows;
using System.Numerics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Runtime.CompilerServices;
using System.Windows.Navigation;

namespace ibcdatacsharp.UI.Common
{
    /// <summary>
    /// Serie de metodos auxiliares compartidos por varias clases
    /// </summary>
    public static class Helpers
    {
        /// <summary>
        /// Devuelve un descendiente de tipo T
        /// </summary>
        /// <param name="depObj">Objeto base</param>
        /// <returns>Devuelve el primer descendiente de tipo T que encuentra</returns>
        public static T GetChildOfType<T>(this DependencyObject depObj)
            where T : DependencyObject
        {
            if (depObj == null) return null;

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
            {
                var child = VisualTreeHelper.GetChild(depObj, i);

                var result = (child as T) ?? GetChildOfType<T>(child);
                if (result != null) return result;
            }
            return null;
        }
        /// <summary>
        /// Genera un float aleatorio entre 2 valores
        /// </summary>
        /// <param name="min">Valor minimo a generar</param>
        /// <param name="max">Valor maximo a generar</param>
        /// <returns>Valor generado</returns>
        public static float NextFloat(float min, float max)
        {
            System.Random random = new System.Random();
            double val = (random.NextDouble() * (max - min) + min);
            return (float)val;
        }
        /// <summary>
        /// Convierte un quaternion a angulos de Euler
        /// </summary>
        /// <param name="q">Quaternion a convertir</param>
        /// <returns>Angulos de Euler</returns>
        public static Vector3 ToEulerAngles(Quaternion q)
        {
            Vector3 angles = new();

            // roll / x
            double sinr_cosp = 2 * (q.W * q.X + q.Y * q.Z);
            double cosr_cosp = 1 - 2 * (q.X * q.X + q.Y * q.Y);
            angles.X = (float)Math.Atan2(sinr_cosp, cosr_cosp);

            // pitch / y
            double sinp = 2 * (q.W * q.Y - q.Z * q.X);
            if (Math.Abs(sinp) >= 1)
            {
                angles.Y = (float)Math.CopySign(Math.PI / 2, sinp);
            }
            else
            {
                angles.Y = (float)Math.Asin(sinp);
            }

            // yaw / z
            double siny_cosp = 2 * (q.W * q.Z + q.X * q.Y);
            double cosy_cosp = 1 - 2 * (q.Y * q.Y + q.Z * q.Z);
            angles.Z = (float)Math.Atan2(siny_cosp, cosy_cosp);

            return angles;
        }
        /// <summary>
        /// Convierte m/s^2 a G
        /// </summary>
        /// <param name="ms2">Valor en m/s^2</param>
        /// <returns>Valor en G</returns>
        public static float ToGs(float ms2)
        {
            return ms2 / 9.8f;
        }
        /// <summary>
        /// Convierte radianes a grados
        /// </summary>
        /// <param name="radians">Valor en radianes</param>
        /// <returns>Valor en grados</returns>
        public static float ToDegrees(float radians)
        {
            float degrees = (180 / (float)Math.PI) * radians;
            return (degrees);
        }
        /// <summary>
        /// Convierte grados a radianes
        /// </summary>
        /// <param name="degrees">Valor en grados</param>
        /// <returns>Valor en radianes</returns>
        public static float ToRadians(float degrees)
        {
            float radians = ((float)Math.PI / 180) * degrees;
            return radians;
        }
        /// <summary>
        /// Normaliza un angulo
        /// </summary>
        /// <param name="angle">Angulo a normalizar</param>
        /// <returns>Angulo normalizado</returns>
        public static float NormalizeAngle(float angle)
        {
            /*
            while(angle > 360)
            {
                angle -= 360;
            }
            while(angle < -360)
            {
                angle += 360;
            }
            */
            return angle % 360;
        }
        /// <summary>
        /// Encuentra el angulo en el rango (-360, 360) equivalente a angle mas cercano a closeAngle
        /// </summary>
        /// <param name="angle">Angulo a transformar</param>
        /// <param name="closeAngle">Angulo mas cercano</param>
        /// <returns>Angulo en el rango (-360,360) equivalente a angle mas cercano a closeAngle</returns>
        public static float ClosestAngle360(float angle, float closeAngle)
        {
            angle = angle % 360;
            float dif = Math.Abs(angle - closeAngle);
            if (angle < 0)
            {
                float angleTest = angle + 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < dif)
                {
                    return angleTest;
                }
                return angle;
            }
            if(angle > 0)
            {
                float angleTest = angle - 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < dif)
                {
                    return angleTest;
                }
                return angle;
            }
            // Si llega aqui angle == 0
            if(closeAngle < -180)
            {
                return -360;
            }
            if(closeAngle > 180)
            {
                return 360;
            }
            return 0;
        }
#if NORMALIZE_ANGLE_STATS
        private static int NormalizeAngleWhileIter = 0;
        private static int NormalizeAngleCalls = 0;
#endif
        /// <summary>
        /// Encuentra el angulo equivalente a angle mas cercano a closeAngle
        /// </summary>
        /// <param name="angle">Angulo a transformar</param>
        /// <param name="closeAngle">Angulo mas cercano</param>
        /// <returns>Angulo equivalente a angle mas cercano a closeAngle</returns>
        public static float ClosestAngle(float angle, float closeAngle)
        {
#if NORMALIZE_ANGLE_STATS
            NormalizeAngleCalls++;
#endif
            float dif = Math.Abs(angle - closeAngle);
            float angleRes = angle;
            float difRes = dif;
            float angleTest = angle;
            while(angleTest <= closeAngle)
            {
#if NORMALIZE_ANGLE_STATS
                NormalizeAngleWhileIter++;
#endif
                angleTest += 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if(difTest < difRes)
                {
                    angleRes = angleTest;
                    difRes = difTest;
                }
                else if(difTest > dif)
                {
                    break;
                }
            }
            angleTest = angle;
            while (angleTest >= closeAngle)
            {
#if NORMALIZE_ANGLE_STATS
                NormalizeAngleWhileIter++;
#endif
                angleTest -= 360;
                float difTest = Math.Abs(angleTest - closeAngle);
                if (difTest < difRes)
                {
                    angleRes = angleTest;
                    difRes = difTest;
                }
                else if (difTest > dif)
                {
                    break;
                }
            }
#if NORMALIZE_ANGLE_STATS
            if(NormalizeAngleCalls % 1000 == 0)
            {
                Trace.WriteLine("average while iterations per call: " + (float)NormalizeAngleWhileIter / NormalizeAngleCalls);
            }
#endif
            return angleRes;
        }
        public static void ClosestAngleTest()
        {
            float[] angles = new float[] {10, -180, 320, -250};
            float[] prevs = new float[] {20, 0, 280, 0};
            float[] res = new float[] { 10, -180, 320, 110 };
            Trace.WriteLine("Normalize Angle Test");
            for(int i = 0; i < angles.Length; i++)
            {
                float a = ClosestAngle(angles[i], prevs[i]);
                if(a != res[i])
                {
                    Trace.WriteLine("res = " + a + ", angle = " + angles[i] + ", prev = " + prevs[i]);
                }
            }
        }
        /// <summary>
        /// Calcula la velocidad angular al pasar de q1 a q2 en un espacio de tiempo dt
        /// </summary>
        /// <param name="q1">Primer quaternion</param>
        /// <param name="q2">Segundo quaternion</param>
        /// <param name="dt">Diferencia de tiempo entre los dos quaterniones</param>
        /// <returns>Velocidad angular</returns>
        public static Vector3 AngularVelocityFromQuaternions(Quaternion q1, Quaternion q2, double dt)
        {


            Vector3 v = new Vector3();
            v.X = (float)((2 / dt) * (q1.W * q2.X - q1.X * q2.W - q1.Y * q2.Z + q1.Z * q2.Y));
            v.Y = (float)((2 / dt) * (q1.W * q2.Y + q1.X * q2.Z - q1.Y * q2.W - q1.Z * q2.X));
            v.Z = (float)((2 / dt) * (q1.W * q2.Z - q1.X * q2.Y + q1.Y * q2.X - q1.Z * q2.W));

            return v;

        }
        private static float maxDiference = 0;
        /// <summary>
        /// Calcula la velocidad angular al pasar de angle0 a angle1 en un espacio de tiempo dt
        /// </summary>
        /// <param name="angle1">Segundo angulo en grados</param>
        /// <param name="angle0">Primer angulo en grados</param>
        /// <param name="dt">Diferencia de tiempo entre los dos angulos</param>
        /// <returns>Velocidad angular</returns>
        public static float AngularVelocityFromDegrees(float angle1, float angle0, float dt)
        {
            float angle1Rad = ToRadians(angle1);
            float angle0Rad = ToRadians(angle0);
            return AngularVelocity(angle1Rad, angle0Rad, dt);
        }
        /// <summary>
        /// Calcula la velocidad angular al pasar de angle0 a angle1 en un espacio de tiempo dt
        /// </summary>
        /// <param name="angle0">Primer angulo en radianes</param>
        /// <param name="angle1">Segundo angulo en radianes</param>
        /// <param name="dt">Diferencia de tiempo entre los dos angulos</param>
        /// <returns>Velocidad angular</returns>
        public static float AngularVelocity(float angle1, float angle0, float dt)
        {
            return (angle1 - angle0) / dt;
        }
        /// <summary>
        /// Calcula la aceleracion angular a partir de la velocidad angular en 2 instantes (como vectores)
        /// </summary>
        /// <param name="w1">Velocidad angular en el instante 1</param>
        /// <param name="w0">Velocidad angular en el instante 0</param>
        /// <param name="dt">Diferencia de tiempo entre los dos instantes</param>
        /// <returns>Aceleracion angular</returns>
        public static Vector3 AngularAcceleration(Vector3 w1, Vector3 w0, float dt)
        {
            return (w1 - w0) / dt;
        }
        /// <summary>
        /// Calcula la aceleracion angular a partir de la velocidad angular en 2 instantes (como numeros)
        /// </summary>
        /// <param name="w1">Velocidad angular en el instante 1</param>
        /// <param name="w0">Velocidad angular en el instante 0</param>
        /// <param name="dt">Diferencia de tiempo entre los dos instantes</param>
        /// <returns>Aceleracion angular</returns>
        public static float AngularAcceleration(float w1, float w0, float dt)
        {
            return (w1 - w0) / dt;
        }
        /// <summary>
        /// Imprime por Trace un diccionario de WisewalkSDK.Device
        /// </summary>
        /// <param name="dict">Diccionario de string,WisewalkSDK.Device </param>
        public static void printDict(Dictionary<string, WisewalkSDK.Device> dict)
        {
            foreach (var kvp in dict)
            {
                string s = string.Format("Key = {0}, Value = {1}", kvp.Key, kvp.Value.Id);
                Trace.WriteLine(s);
            }
        }
        /// <summary>
        /// Imprime por trace una array de 2 dimensiones
        /// </summary>
        /// <param name="array">Array de 2 dimensiones</param>
        public static void printArray(float[,] array)
        {
            Trace.WriteLine("array");
            for(int i = 0; i < array.GetLength(0); i++)
            {
                string temp = "[";
                for(int j = 0; j < array.GetLength(1); j++)
                {
                    temp += array[i, j] + ", ";
                }
                temp += "]";
                Trace.WriteLine(temp);
            }
        }
        /// <summary>
        /// Comprueba se 2 floats casi iguales
        /// </summary>
        /// <param name="a">primer float</param>
        /// <param name="b">segundo float</param>
        /// <param name="epsilon">constante pequeña</param>
        /// <returns>True si son casi iguales</returns>
        public static bool NearlyEqual(float a, float b, float epsilon = 0.1f)
        {
            float absA = Math.Abs(a);
            float absB = Math.Abs(b);
            float diff = Math.Abs(a - b);

            if (a == b)
            { // shortcut, handles infinities
                return true;
            }
            else if (a == 0 || b == 0 || absA + absB < float.MinValue)
            {
                // a or b is zero or both are extremely close to it
                // relative error is less meaningful here
                return diff < (epsilon * float.MaxValue);
            }
            else
            { // use relative error
                return diff / (absA + absB) < epsilon;
            }
        }
        /// <summary>
        /// Imprime por trace los dispositivos conectados a la api
        /// </summary>
        public static void printDevicesConnected()
        {
            Trace.WriteLine("devices connected:");
            var devicesConnected = ((MainWindow)Application.Current.MainWindow).api.GetDevicesConnected();
            foreach (KeyValuePair<string, WisewalkSDK.Device> entry in devicesConnected)
            {
                Trace.WriteLine("handler: " + entry.Key);
                Trace.WriteLine(entry.Value.Id);
            }
        }
        /// <summary>
        /// Llama a una funcion cuando un frame se ha inicializado
        /// </summary>
        /// <param name="frame">frame</param>
        /// <param name="f">funcion a llamar</param>
        public static void callWhenNavigated(Frame frame, Action f)
        {
            if(frame.Content == null)
            {
                frame.Navigated += (sender, args) =>
                {
                    f();
                };
            }
            else
            {
                f();
            }
        }
        /// <summary>
        /// Genera un float aleatorio entre 2 valores
        /// </summary>
        /// <param name="min">Valor minimo a generar</param>
        /// <param name="max">Valor maximo a generar</param>
        /// <returns>float aleatorio entre min y max</returns>
        public static float randomFloat(float min, float max)
        {
            Random random = new Random();
            float f = random.NextSingle();
            f = f * (max - min);
            return f + min;
        }
        /// <summary>
        /// Genera un Quaternio n aleatorio
        /// </summary>
        /// <returns>Quaternion aleatorio</returns>
        public static Quaternion random_quaternion()
        {
            float x, y, z, u, v, w, s;
            do { x = randomFloat(-1,1); y = randomFloat(-1, 1); z = x * x + y * y; } while (z > 1);
            do { u = randomFloat(-1, 1); v = randomFloat(-1, 1); w = u * u + v * v; } while (w > 1);
            s = (float)Math.Sqrt((1 - z) / w);
            return new Quaternion(x, y, s * u, s * v);
        }
        /// <summary>
        /// Imprime por trace una lista de WisewalkSDK.QuatSensor
        /// </summary>
        /// <param name="quats">lista de WisewalkSDK.QuatSensor</param>
        public static void print(List<WisewalkSDK.QuatSensor> quats)
        {
            for(int i = 0; i < quats.Count; i++)
            {
                Trace.WriteLine(quats[i].W + " " + quats[i].X + " " + quats[i].Y + " " +
                    quats[i].Z);
            }
        }
    }
}