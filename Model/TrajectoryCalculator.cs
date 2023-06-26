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
    /// Статический класс для вычисления положения датчика в пространстве и его траектории движения
    /// </summary>
    public static class TrajectoryCalculator
    {
        #region Methods to work with angles
        /// <summary>
        /// Вычисляет угол альфа
        /// </summary>
        /// <param name="a">Амплитуда</param>
        /// <param name="w">Угловая частота</param>
        /// <param name="t">Время</param>
        /// <returns>Угол альфа в радианах</returns>
        /// <exception cref="TimeNegativeException">Исключение, если время отрицательно</exception>
        public static double CalculateAngleAlpha(double a, double w, double t) => t > 0 ? a * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");

        /// <summary>
        /// Вычисляет угол бета
        /// </summary>
        /// <param name="b"></param>
        /// <param name="w"></param>
        /// <param name="t">Время</param>
        /// <returns>Угол бета в радианах</returns>
        /// <exception cref="TimeNegativeException">Исключение, если время отрицательно</exception>
        public static double CalculateAngleBeta(double b, double w, double t) => t > 0 ? b * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");
        
        /// <summary>
        /// Переводит угол из градусов в радианы
        /// </summary>
        /// <param name="degrees">Угол в градусах</param>
        /// <returns>Угол в радианах</returns>
        public static double ToRadians(this double degrees) => Math.PI / 180 * degrees;
        #endregion

        #region Methods to calculate location
        /// <summary>
        /// Вычисляет координату X положения датчика
        /// </summary>
        /// <param name="radius">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату X положения датчика</returns>
        public static double CalculateLocationCoordinateX(double radius, double alphaAngle, double betaAngle) => radius * Math.Sin(alphaAngle) * Math.Cos(betaAngle);

        /// <summary>
        /// Вычисляет координату Y положения датчика
        /// </summary>
        /// <param name="radius">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату Y положения датчика</returns>
        public static double CalculateLocationCoordinateY(double radius, double alphaAngle, double betaAngle) => radius * Math.Sin(alphaAngle) * Math.Sin(betaAngle);

        /// <summary>
        /// Вычисляет координату Z положения датчика
        /// </summary>
        /// <param name="radius">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату Z положения датчика</returns>
        public static double CalculateLocationCoordinateZ(double radius, double alphaAngle, double betaAngle) => radius * Math.Cos(alphaAngle);

        /// <summary>
        /// Вычисляет положение датчика в пространстве
        /// </summary>
        /// <param name="radius">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Положение датчика в виде Vector3D</returns>
        public static Vector3D CalculateLocationInSpace(double radius, double alphaAngle, double betaAngle)
        {
            Vector3D point3D;
            point3D.X = CalculateLocationCoordinateX(radius, alphaAngle, betaAngle);
            point3D.Y = CalculateLocationCoordinateY(radius, alphaAngle, betaAngle);
            point3D.Z = CalculateLocationCoordinateZ(radius, alphaAngle, betaAngle);
            return point3D;
        }

        /// <summary>
        /// Вычисляет положение датчика в пространстве
        /// </summary>
        /// <param name="point3D">Точка для записи положения</param>
        /// <param name="radius">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
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
            Quaternion resultQuaternion = rotationQuaternion * initialVector.ToQuaternion() * rotationQuaternion.ToInverse();
            return resultQuaternion.ToVector3D();
        }

        #endregion

        #region Methods to calculate displacement and trajectory of sensor
        /// <summary>
        /// Вычисляет двойной интеграл и возвращает перемещение датчика
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Вектор перемещения</returns>
        public static Vector3D CalculateDisplacement(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            double deltaT = 1.0 / sampleRate;
            Vector3D velocity;
            Vector3D displacement;
            for(int i = start; i < start + countOfSamples; i++)
            {
                velocity += accelerationVectors[i] * deltaT;
                displacement += velocity * deltaT;
            }
            return displacement;
        }
        /// <summary>
        /// Вычисляет двойной интеграл и возвращает список перемещений датчика
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Список векторов перемещения</returns>
        public static List<Vector3D> CalculateAndReturnListOfDisplacements(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            double deltaT = 1.0 / sampleRate;
            Vector3D velocity;
            Vector3D displacement;
            List<Vector3D> displacementVectors = new();
            for (int i = start; i < start + countOfSamples; i++)
            {
                velocity += accelerationVectors[i] * deltaT;
                displacement += velocity * deltaT;
                displacementVectors.Add(displacement);
            }
            return displacementVectors;
        }
        /// <summary>
        /// Вычисляет точки для построения траектории движения по сфере
        /// </summary>
        /// <param name="startingPoint">Начальная точка</param>
        /// <param name="endPoint">Конечная точка</param>
        /// <param name="radius">Радиус сферы</param>
        /// <returns>Список точек в пространстве, по которым строится траектория</returns>
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
        #endregion

        #region Methods to calculate radius of sphere
        /// <summary>
        /// Рассчитывает расстояние между 2-мя точками
        /// </summary>
        /// <param name="point1">Точка 1</param>
        /// <param name="point2">Точка 2</param>
        /// <returns>Расстояние между точками</returns>
        public static double CalculateDistanceBetweenTwoPoints(Vector3D point1, Vector3D point2) => (point1 - point2).Length;
        /// <summary>
        /// Рассчитывает вектор на единичной сфере
        /// </summary>
        /// <param name="rotation">Кватернион вращения</param>
        /// <returns>Вектор на единичной сфере</returns>
        public static Vector3D CalculateUnitVector(Quaternion rotation)
        {
            Vector3D yAxis = new(0, 1, 0);
            return RotateVectorInSpace(yAxis, rotation.ToInverse());
        }
        /// <summary>
        /// Рассчитывает радиус сферы
        /// </summary>
        /// <param name="t1">Начало интегрирования</param>
        /// <param name="t2">Конец интегрирования</param>
        /// <param name="accVectors">Список векторов ускорений</param>
        /// <param name="quaternion">Кватернион</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Радиус сферы</returns>
        public static double CalculateRadius(int t1, int t2, List<Vector3D> accVectors, List<Quaternion> quaternion, int sampleRate)
        {
            double d2 = CalculateDisplacement(accVectors, t1, t2 - t1, sampleRate).Length;
            double d1 = CalculateDistanceBetweenTwoPoints(CalculateUnitVector(quaternion[t1]), CalculateUnitVector(quaternion[t2]));
            return d2 / d1;
        }
        #endregion
    }
}
