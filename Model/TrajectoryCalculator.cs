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

        public static Vector3D LinearInterpolate(Vector3D a, Vector3D b, double t)
        {
            return a + (b - a) * t;
        }

        public static Vector3D Approximate(Vector3D point1, Vector3D point2, Vector3D point3, Vector3D point4, double t)
        {
            Vector3D interpolatedPoint1 = LinearInterpolate(point1, point2, t);
            Vector3D interpolatedPoint2 = LinearInterpolate(point2, point3, t);
            Vector3D interpolatedPoint3 = LinearInterpolate(point3, point4, t);

            Vector3D interpolatedPoint4 = LinearInterpolate(interpolatedPoint1, interpolatedPoint2, t);
            Vector3D interpolatedPoint5 = LinearInterpolate(interpolatedPoint2, interpolatedPoint3, t);

            Vector3D approximatedPoint = LinearInterpolate(interpolatedPoint4, interpolatedPoint5, t);

            return approximatedPoint;
        }

        public static Vector3D ReprojectVector3D(Vector3D initialVector, Quaternion quaternionOrientation)
        {
            Vector3D temporaryVector = new(-quaternionOrientation.X, -quaternionOrientation.Y, -quaternionOrientation.Z);
            Vector3D intermediateVector = 2 * Vector3D.CrossProduct(temporaryVector, initialVector);
            Vector3D resultVector = initialVector + quaternionOrientation.W * intermediateVector + Vector3D.CrossProduct(temporaryVector, intermediateVector);
            return resultVector;
        }

        public static Vector3D RotateVector3D(Vector3D initialVector, Quaternion quaternionOrientation)
        {
            Vector3D temporaryVector = new(quaternionOrientation.X, quaternionOrientation.Y, quaternionOrientation.Z);
            Vector3D intermediateVector = 2 * Vector3D.CrossProduct(quaternionOrientation.Axis, initialVector);
            Vector3D resultVector = initialVector + quaternionOrientation.W * intermediateVector + Vector3D.CrossProduct(quaternionOrientation.Axis, intermediateVector);
            return resultVector;
        }

        public static Vector3D CalculateDisplacementWithIntegral(List<Vector3D> accelerationVectors, int countOfSamples, int sampleRate)
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
