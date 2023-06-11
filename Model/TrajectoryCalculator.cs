using System;
using System.Collections.Generic;
using System.Linq;
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
        public static double ConvertDegreesToRadians(int degrees) => Math.PI / 180 * degrees;

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
        public static Point3D CalculateLocationInSpace(double radius, double alphaAngle, double betaAngle)
        {
            Point3D point3D;
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
        public static void CalculateLocationInSpace(ref Point3D point3D, double radius, double alphaAngle, double betaAngle)
        {
            point3D.X = CalculateLocationCoordinateX(radius, alphaAngle, betaAngle);
            point3D.Y = CalculateLocationCoordinateY(radius, alphaAngle, betaAngle);
            point3D.Z = CalculateLocationCoordinateZ(radius, alphaAngle, betaAngle);
        }
    }
}
