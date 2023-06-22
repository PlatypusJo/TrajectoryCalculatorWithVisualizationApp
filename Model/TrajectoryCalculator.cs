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
    /// Класс для вычисления положения датчика в пространстве и его траектории движения
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
        /// Преобразует вектор в кватернион
        /// </summary>
        /// <param name="vector">Исходный вектор</param>
        /// <returns>Кватернион</returns>
        public static Quaternion ToQuaternion(this Vector3D vector) => new(vector.X, vector.Y, vector.Z, 0);

        /// <summary>
        /// Преобразует кватернион в вектор
        /// </summary>
        /// <param name="quaternion">Исходный кватернион</param>
        /// <returns>Вектор</returns>
        public static Vector3D ToVector3D(this Quaternion quaternion) => new(quaternion.X, quaternion.Y, quaternion.Z);

        /// <summary>
        /// Преобразует кватернион в обратный
        /// </summary>
        /// <param name="quaternion">Исходный кватернион</param>
        /// <returns>Обратный кватернион</returns>
        public static Quaternion ToInverse(this Quaternion quaternion)
        {
            Quaternion invertQuaternion = quaternion;
            invertQuaternion.Invert();
            return invertQuaternion;
        }

        /// <summary>
        /// Производит поворот вектора в пространстве
        /// </summary>
        /// <param name="initialVector">Начальный вектор</param>
        /// <param name="rotationQuaternion">Кватернион поворота</param>
        /// <returns>Повёрнутый в пространстве вектор</returns>
        public static Vector3D RotateVectorInSpace(Vector3D initialVector, Quaternion rotationQuaternion)
        {
            double cosOfRotationAngle = Math.Cos(rotationQuaternion.W / 2.0);
            double sinOfRotationAngle = Math.Sin(rotationQuaternion.W / 2.0);
            rotationQuaternion = new()
            {
                X = rotationQuaternion.X * sinOfRotationAngle,
                Y = rotationQuaternion.Y * sinOfRotationAngle,
                Z = rotationQuaternion.Z * sinOfRotationAngle,
                W = cosOfRotationAngle
            };
            Quaternion resultQuaternion = rotationQuaternion * initialVector.ToQuaternion() * rotationQuaternion.ToInverse();
            return resultQuaternion.ToVector3D();
        }

        #endregion
        
        public static Vector3D CalculateDisplacement(List<Vector3D> accelerationVectors, int countOfSamples, int sampleRate)
        {
            double deltaT = 1.0 / sampleRate;
            Vector3D velocity;
            Vector3D displacement;
            for(int i = 0; i < countOfSamples; i++)
            {
                velocity += accelerationVectors[i] * deltaT;
                displacement += velocity * deltaT;
            }
            return displacement;
        }

        public static List<Point3D> CalculatePointsOfTrajectoryInSpace(Vector3D startingPoint, Vector3D endPoint, double radius)
        {
            List<Point3D> points = new();
            points.Add((Point3D)startingPoint);
            Vector3D vectorBetweenTwoPoints = endPoint - startingPoint;
            for (double i = 0.01; i < 1.0; i += 0.01)
            {
                Vector3D pointInSpace = vectorBetweenTwoPoints * i + startingPoint;
                double coefficient = radius / pointInSpace.Length;
                points.Add((Point3D)(pointInSpace * coefficient));
            }
            points.Add((Point3D)endPoint);
            return points;
        }
    }
}
