using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// Статический класс для вычисления положения датчика в пространстве и его траектории движения
    /// </summary>
    public static class MathToolsFor3D
    {
        #region Methods to work with angles
        /// <summary>
        /// Функция для вычисления угол альфа
        /// </summary>
        /// <param name="a">Амплитуда</param>
        /// <param name="w">Угловая частота</param>
        /// <param name="t">Время</param>
        /// <returns>Угол альфа в радианах</returns>
        /// <exception cref="TimeNegativeException">Исключение, если время отрицательно</exception>
        public static double CalculateAngleAlpha(double a, double w, double t) => t > 0 ? a * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");

        /// <summary>
        /// Функция для вычисления угол бета
        /// </summary>
        /// <param name="b">Fvgkbnelf</param>
        /// <param name="w">Угловая частота</param>
        /// <param name="t">Время</param>
        /// <returns>Угол бета в радианах</returns>
        /// <exception cref="TimeNegativeException">Исключение, если время отрицательно</exception>
        public static double CalculateAngleBeta(double b, double w, double t) => t > 0 ? b * Math.Sin(w * t) : throw new TimeNegativeException("Отрицательное значение времени");
        
        /// <summary>
        /// Функция для перевода угла из градусов в радианы
        /// </summary>
        /// <param name="degrees">Угол в градусах</param>
        /// <returns>Угол в радианах</returns>
        public static double ToRadians(this double degrees) => Math.PI / 180 * degrees;
        /// <summary>
        /// Функция для перевода угла из радиан в градусы
        /// </summary>
        /// <param name="radians">Угол в радианах</param>
        /// <returns>Угол в градусах</returns>
        public static double ToDegrees(this double radians) => 180 * radians / Math.PI;
        /// <summary>
        /// Функция для вычисления знакового угла между трёхмерными векторами (от -π до π) (рад.) в плоскости заданной осью
        /// </summary>
        /// <param name="firstVector">Первый вектор</param>
        /// <param name="secondVector">Второй вектор</param>
        /// <param name="normalVector">Вектор нормали к плоскости, в которой лежат векторы</param>
        /// <returns>Угол между векторами (от -π до π) (рад.)</returns>
        public static double SignedAngle(Vector3D firstVector, Vector3D secondVector, Vector3D normalVector)
        {
            double dot = Vector3D.DotProduct(firstVector, secondVector);
            double det = ScalarTripleProduct(normalVector, firstVector, secondVector);
            return Math.Atan2(det, dot);
        }
        /// <summary>
        /// Функция для вычисления смешанного произведения (скалярного) трёх векторов (a * (b x c))
        /// </summary>
        /// <param name="firstVector"></param>
        /// <param name="secondVector"></param>
        /// <param name="thirdVector"></param>
        /// <returns>Смешанное произведение (скаляр)</returns>
        public static double ScalarTripleProduct(Vector3D firstVector, Vector3D secondVector, Vector3D thirdVector) => Vector3D.DotProduct(firstVector, Vector3D.CrossProduct(secondVector, thirdVector));
        #endregion

        #region Methods to calculate location
        /// <summary>
        /// Функция для вычисления координаты X положения датчика в сферических координатах
        /// </summary>
        /// <param name="radiusOfSphere">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату X положения датчика</returns>
        public static double CalcCoordinateXInSphericalCoordinates(double radiusOfSphere, double alphaAngle, double betaAngle) => radiusOfSphere * Math.Sin(alphaAngle) * Math.Cos(betaAngle);
        /// <summary>
        /// Функция для вычисления координаты Y положения датчика в сферических координатах
        /// </summary>
        /// <param name="radiusOfSphere">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату Y положения датчика</returns>
        public static double CalcCoordinateYInSphericalCoordinates(double radiusOfSphere, double alphaAngle, double betaAngle) => radiusOfSphere * Math.Sin(alphaAngle) * Math.Sin(betaAngle);
        /// <summary>
        /// Функция для вычисления координаты Z положения датчика в сферических координатах
        /// </summary>
        /// <param name="radiusOfSphere">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Координату Z положения датчика</returns>
        public static double CalcCoordinateZInSphericalCoordinates(double radiusOfSphere, double alphaAngle, double betaAngle) => radiusOfSphere * Math.Cos(alphaAngle);
        /// <summary>
        /// Функция для вычисления положение датчика в пространстве в сферических координатах
        /// </summary>
        /// <param name="radiusOfSphere">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        /// <returns>Положение датчика в виде Vector3D</returns>
        public static Vector3D CalcLocationInSphericalCoordinates(double radiusOfSphere, double alphaAngle, double betaAngle)
        {
            Vector3D point3D;
            point3D.X = CalcCoordinateXInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
            point3D.Y = CalcCoordinateYInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
            point3D.Z = CalcCoordinateZInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
            return point3D;
        }
        /// <summary>
        /// Функция для вычисления положение датчика в пространстве в сферических координатах
        /// </summary>
        /// <param name="point3D">Точка для записи положения</param>
        /// <param name="radiusOfSphere">Радиус сферы, по которой движется датчик</param>
        /// <param name="alphaAngle">Угол альфа в радианах</param>
        /// <param name="betaAngle">Угол бета в радианах</param>
        public static void CalculateLocationInSpaceInSphericalCoordinates(ref Vector3D point3D, double radiusOfSphere, double alphaAngle, double betaAngle)
        {
            point3D.X = CalcCoordinateXInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
            point3D.Y = CalcCoordinateYInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
            point3D.Z = CalcCoordinateZInSphericalCoordinates(radiusOfSphere, alphaAngle, betaAngle);
        }
        /// <summary>
        /// Функция для вычисления положения датчика в пространстве по радиусу и углам
        /// </summary>
        /// <param name="radiusOfSphere">Радиус сферы</param>
        /// <param name="alphaAngle">Угол альфа</param>
        /// <param name="betaAngle">Угол бета</param>
        /// <returns>Положение датчика в пространстве</returns>
        public static Vector3D CalculateLocationInSpaceByRadiusAndAngles(double radiusOfSphere, double alphaAngle, double betaAngle)
        {
            Vector3D point3D;
            point3D.X = radiusOfSphere * Math.Sin(alphaAngle);
            point3D.Z = -radiusOfSphere * Math.Sin(betaAngle);
            point3D.Y = Math.Sqrt(radiusOfSphere * radiusOfSphere - point3D.X * point3D.X - point3D.Z * point3D.Z);
            return point3D;
        }
        /// <summary>
        /// Функция для вычисления положения датчика в пространстве в локальных координатах
        /// </summary>
        /// <param name="rotationQuaternion">Кватернион поворота</param>
        /// <param name="radiusOfSphere">Радиус сферы</param>
        /// <returns>Положение датчика в пространстве в локальных координатах</returns>
        public static Vector3D CalculateSensorPosition(Quaternion rotationQuaternion, double radiusOfSphere)
        {
            Vector3D yAxis = new(0, 1, 0);
            Vector3D yAxisInLocal = RotateVectorInSpace(yAxis, rotationQuaternion.ToInverse());
            double xAngle = SignedAngle(yAxis, yAxisInLocal, new(1, 0, 0));            
            double zAngle = SignedAngle(yAxis, yAxisInLocal, new(0, 0, 1));
            return CalculateLocationInSpaceByRadiusAndAngles(radiusOfSphere, zAngle, xAngle);
        }
        #endregion
        
        #region Methods to work with Quaternions and Vectors
        /// <summary>
        /// Функция для преобразования вектора в кватернион
        /// </summary>
        /// <param name="vector">Исходный вектор</param>
        /// <returns>Кватернион</returns>
        public static Quaternion ToQuaternion(this Vector3D vector) => new(vector.X, vector.Y, vector.Z, 0);
        /// <summary>
        /// Функция для преобразования кватерниона в вектор
        /// </summary>
        /// <param name="quaternion">Исходный кватернион</param>
        /// <returns>Вектор</returns>
        public static Vector3D ToVector3D(this Quaternion quaternion) => new(quaternion.X, quaternion.Y, quaternion.Z);
        /// <summary>
        /// Функция для преобразования кватерниона в обратный
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
        /// Функция для поворота вектора в пространстве
        /// </summary>
        /// <param name="initialVector">Начальный вектор</param>
        /// <param name="rotationQuaternion">Кватернион поворота</param>
        /// <returns>Повёрнутый в пространстве вектор</returns>
        public static Vector3D RotateVectorInSpace(Vector3D initialVector, Quaternion rotationQuaternion)
        {
            Quaternion resultQuaternion = rotationQuaternion * initialVector.ToQuaternion() * rotationQuaternion.ToInverse();
            return resultQuaternion.ToVector3D();
        }
        /// <summary>
        /// Функция для поворота списка векторов с помощью кватернионов
        /// </summary>
        /// <param name="vectors">Список векторов</param>
        /// <param name="quaternions">Список кватернионов поворота</param>
        /// <param name="gLength">Длина вектора ускорения свободного падения</param>
        /// <returns>Список повёрнутых векторов ускорений</returns>
        public static List<Vector3D> RotateVectorsInSpace(List<Vector3D> vectors, List<Quaternion> quaternions, double gLength)
        {
            List<Vector3D> rotatedVectors = new List<Vector3D>();

            for (int i = 0; i < vectors.Count; i++)
            {
                rotatedVectors.Add(RotateVectorInSpace(vectors[i], quaternions[i]));
            }
            Vector3D temporaryVector = new(0.0, 1.0, 0.0);
            for (int i = 0; i < rotatedVectors.Count; i++)
            {
                rotatedVectors[i] -= temporaryVector * gLength;
            }

            return rotatedVectors;
        }
        #endregion

        #region Methods to calculate integrals
        /// <summary>
        /// Функция для вычисления двойного интеграла, которая возвращает перемещение датчика
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Вектор перемещения</returns>
        public static Vector3D CalculateDisplacement(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            Vector3D velocity;
            Vector3D displacement;
            double deltaT = 1.0 / sampleRate;

            for(int i = start; i < start + countOfSamples; i++)
            {
                velocity += accelerationVectors[i] * deltaT;
                displacement += velocity * deltaT;
            }

            return displacement;
        }
        /// <summary>
        /// Функция для вычисления двойного интеграла, которая возвращает список перемещений датчика
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало - с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Список векторов перемещения</returns>
        public static List<Vector3D> CalculateAndReturnListOfDisplacements(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            Vector3D velocity;
            Vector3D displacement;
            double deltaT = 1.0 / sampleRate;
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
        /// Функция для вычисления интеграла методом прямоугольников в среднем
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Список векторов перемещения</returns>
        public static List<Vector3D> CalculateIntegralAvgRectangle(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            Vector3D velocity;
            Vector3D displacement;
            double deltaT = 1.0 / sampleRate;
            List<Vector3D> velocityVectors = new();
            List<Vector3D> displacementVectors = new();

            for (int i = start; i < start + countOfSamples; i++)
            {
                velocity += (accelerationVectors[i] * (deltaT + 0.5)) * deltaT;
                velocityVectors.Add(velocity);
            }

            for(int i = 0; i < velocityVectors.Count; i++)
            {
                displacement += (velocityVectors[i] * (deltaT + 0.5)) * deltaT;
                displacementVectors.Add(displacement);
            }

            return displacementVectors;
        }
        /// <summary>
        /// Функция для вычисления интеграла методом трапеций
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Список векторов перемещения</returns>
        public static List<Vector3D> CalculateIntegralTrapezoidal(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            Vector3D velocity;
            Vector3D displacement;
            double deltaT = 1.0 / sampleRate;
            List<Vector3D> velocityVectors = new();
            List<Vector3D> displacementVectors = new();

            for (int i = start; i < start + countOfSamples - 1; i++)
            {
                velocity += (accelerationVectors[i] + accelerationVectors[i + 1]) * 0.5 * deltaT;
                velocityVectors.Add(velocity);
            }

            for (int i = 0; i < velocityVectors.Count - 1; i++)
            {
                displacement += (velocityVectors[i] + velocityVectors[i + 1]) * 0.5 * deltaT;
                displacementVectors.Add(displacement);
            }

            return displacementVectors;
        }
        /// <summary>
        /// Функция для вычисления интеграла методом парабол
        /// </summary>
        /// <param name="accelerationVectors">Список векторов ускорений</param>
        /// <param name="start">Начало с какого вектора начинаются вычисления</param>
        /// <param name="countOfSamples">Количество отсчётов</param>
        /// <param name="sampleRate">Частота дискретизации</param>
        /// <returns>Список векторов перемещения</returns>
        public static List<Vector3D> CalculateIntegralParabols(List<Vector3D> accelerationVectors, int start, int countOfSamples, int sampleRate)
        {
            Vector3D velocity;
            Vector3D displacement;
            double deltaT = 1.0 / sampleRate;
            List<Vector3D> velocityVectors = new();
            List<Vector3D> displacementVectors = new();

            for (int i = start; i < start + countOfSamples - 2; i++)
            {
                velocity += (accelerationVectors[i] + 4 * accelerationVectors[i + 1] + accelerationVectors[i + 2]) * deltaT / 3;
                velocityVectors.Add(velocity);
            }

            for (int i = 0; i < velocityVectors.Count - 2; i++)
            {
                displacement += (velocityVectors[i] + 4 * velocityVectors[i + 1] + velocityVectors[i + 2]) * deltaT / 3;
                displacementVectors.Add(displacement);
            }

            return displacementVectors;
        }
        #endregion

        #region Methods to calculate trajectory
        /// <summary>
        /// Функция для нахождения точек траектории перемещения с "шумами"
        /// </summary>
        /// <param name="rotationQuaternions">Список кватернионов поворота</param>
        /// <param name="noiseVectors">Список векторов "шума"</param>
        /// <param name="radiusOfSphere">Радиус сферы</param>
        /// <returns>Список векторов перемещения с шумами</returns>
        public static List<Vector3D> CalculateTrajectoryPointsWithNoise(List<Quaternion> rotationQuaternions, List<Vector3D> noiseVectors, double radiusOfSphere)
        {
            List<Vector3D> points = new();
            for (int i = 0; i < rotationQuaternions.Count; i++)
            {
                points.Add(CalculateSensorPosition(rotationQuaternions[i], radiusOfSphere));
            }
            for (int i = 0; i < noiseVectors.Count; i++)
            {
                points[i] += noiseVectors[i];
            }
            return points;
        }
        /// <summary>
        /// Функция для фильтрации списка векторов перемещения методом "плавающего" окна 
        /// </summary>
        /// <param name="displacementVectors">Список векторов перемещения</param>
        /// <param name="sizeOfWindow">Размер окна фильтрация</param>
        /// <returns>Список отфильтрованных векторов</returns>
        public static List<Vector3D> FiltrationByFloatingWindow(List<Vector3D> displacementVectors, int sizeOfWindow)
        {
            int remainder = sizeOfWindow / 2;
            List<Vector3D> filtredDisplacementVectors = new();
            Vector3D sum;
            for (int i = 0; i < displacementVectors.Count - sizeOfWindow; i++)
            {
                sum = displacementVectors[i];
                for (int j = i + 1; j < i + sizeOfWindow; j++)
                {
                    sum += displacementVectors[j];
                }
                filtredDisplacementVectors.Add(sum / sizeOfWindow);
            }
            for (int i = 0; i < remainder; i++)
            {
                filtredDisplacementVectors.Insert(0, filtredDisplacementVectors[0]);
                filtredDisplacementVectors.Add(filtredDisplacementVectors.Last());
            }
            return filtredDisplacementVectors;
        }
        /// <summary>
        /// Функция для нахождения "шумов" при перемещении
        /// </summary>
        /// <param name="displacementVectors">Исходный список векторов перемещения</param>
        /// <param name="filtеredDisplacementVectors">Список отфильтрованных векторов перемещения</param>
        /// <returns>Список "шумов" при перемещении</returns>
        public static List<Vector3D> CalculateListOfNoiseVectors(List<Vector3D> displacementVectors, List<Vector3D> filtеredDisplacementVectors)
        {
            List<Vector3D> result = new();
            for(int i = 0; i < displacementVectors.Count - 50; i++)
            {
                result.Add(displacementVectors[i] - filtеredDisplacementVectors[i]);
            }
            return result;
        }
        /// <summary>
        /// Функция для вычисления точки для построения траектории движения по сфере
        /// </summary>
        /// <param name="startingPoint">Начальная точка представленная вектором</param>
        /// <param name="endPoint">Конечная точка представленная вектором</param>
        /// <param name="radiusOfSphere">Радиус сферы</param>
        /// <returns>Список точек в пространстве, по которым строится траектория движения по сфере</returns>
        public static List<Point3D> CalculateTrajectoryPointsOnSphere(Vector3D startingPoint, Vector3D endPoint, double radiusOfSphere)
        {
            List<Point3D> points = new();
            points.Add((Point3D)startingPoint);
            Vector3D vectorBetweenTwoPoints = endPoint - startingPoint;
            for (double i = 0.01; i < 1.0; i += 0.01)
            {
                Vector3D pointInSpace = vectorBetweenTwoPoints * i + startingPoint;
                double coefficient = radiusOfSphere / pointInSpace.Length;
                points.Add((Point3D)(pointInSpace * coefficient));
            }
            points.Add((Point3D)endPoint);
            return points;
        }
        #endregion
    }
}
