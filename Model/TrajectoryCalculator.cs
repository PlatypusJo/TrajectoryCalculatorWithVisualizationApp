using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// 
    /// </summary>
    public static class TrajectoryCalculator
    {
        #region Methods to work with angles
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="w"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="TimeNegativeException"></exception>
        public static double CalculateAngleAlpha(double a, double w, double t) => t > 0 ? a * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="b"></param>
        /// <param name="w"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        /// <exception cref="TimeNegativeException"></exception>
        public static double CalculateAngleBeta(double b, double w, double t) => t > 0 ? b * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="degrees"></param>
        /// <returns></returns>
        public static double ToRadians(this double degrees) => Math.PI / 180 * degrees;
        #endregion
        
        #region Methods to calculate location
        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="alphaAngle"></param>
        /// <param name="betaAngle"></param>
        /// <returns></returns>
        public static double CalculateLocationCoordinateX(double radius, double alphaAngle, double betaAngle) => radius * Math.Sin(alphaAngle) * Math.Cos(betaAngle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="alphaAngle"></param>
        /// <param name="betaAngle"></param>
        /// <returns></returns>
        public static double CalculateLocationCoordinateY(double radius, double alphaAngle, double betaAngle) => radius * Math.Sin(alphaAngle) * Math.Sin(betaAngle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="alphaAngle"></param>
        /// <param name="betaAngle"></param>
        /// <returns></returns>
        public static double CalculateLocationCoordinateZ(double radius, double alphaAngle, double betaAngle) => radius * Math.Cos(alphaAngle);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="alphaAngle"></param>
        /// <param name="betaAngle"></param>
        /// <returns></returns>
        public static Vector3D CalculateLocationInSpace(double radius, double alphaAngle, double betaAngle)
        {
            Vector3D point3D;
            point3D.X = CalculateLocationCoordinateX(radius, alphaAngle, betaAngle);
            point3D.Y = CalculateLocationCoordinateY(radius, alphaAngle, betaAngle);
            point3D.Z = CalculateLocationCoordinateZ(radius, alphaAngle, betaAngle);
            return point3D;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="point3D"></param>
        /// <param name="radius"></param>
        /// <param name="alphaAngle"></param>
        /// <param name="betaAngle"></param>
        public static void CalculateLocationInSpace(ref Vector3D point3D, double radius, double alphaAngle, double betaAngle)
        {
            point3D.X = CalculateLocationCoordinateX(radius, alphaAngle, betaAngle);
            point3D.Y = CalculateLocationCoordinateY(radius, alphaAngle, betaAngle);
            point3D.Z = CalculateLocationCoordinateZ(radius, alphaAngle, betaAngle);
        }
        #endregion
        
        #region Methods to work with Quaternions
        /// <summary>
        /// Производит поворот вектора в пространстве
        /// </summary>
        /// <param name="initialVector">Начальный вектор</param>
        /// <param name="rotationQuaternion">Кватернион поворота</param>
        /// <returns>Повёрнутый в пространстве вектор</returns>
        public static Vector3D RotateVectorInSpace(Vector3D initialVector, Quaternion rotationQuaternion)
        {
            Quaternion vectorQuaternion = new(initialVector.X, initialVector.Y, initialVector.Z, 0);
            double cosOfRotationAngle = Math.Cos(rotationQuaternion.W / 2.0);
            double sinOfRotationAngle = Math.Sin(rotationQuaternion.W / 2.0);
            rotationQuaternion = new()
            {
                X = rotationQuaternion.X * sinOfRotationAngle,
                Y = rotationQuaternion.Y * sinOfRotationAngle,
                Z = rotationQuaternion.Z * sinOfRotationAngle,
                W = cosOfRotationAngle
            };
            Quaternion invertRotationQuaternion = rotationQuaternion;
            invertRotationQuaternion.Invert();
            Quaternion resultQuaternion = rotationQuaternion * vectorQuaternion * invertRotationQuaternion;
            return new(resultQuaternion.X, resultQuaternion.Y, resultQuaternion.Z);
        }
        #endregion
        
        public static Vector3D CalculateDisplacement(List<Vector3D> accelerationVectors, int countOfSamples, int sampleRate)
        {
            double deltaT = 1 / sampleRate;
            Vector3D velocity;
            Vector3D displacement;
            for(int i = 0; i < countOfSamples; i++)
            {
                velocity += accelerationVectors[i] * deltaT;
                displacement += velocity * deltaT;
            }
            return displacement;
        }

        public static List<Vector3D> CalculatePointsOfTrajectoryInSpace()
        {
            List<Vector3D> points = new List<Vector3D>();
            return points;
        }
    }
}
