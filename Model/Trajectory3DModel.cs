using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// Класс представляющий траекторию движения датчика в пространстве
    /// </summary>
    public class Trajectory3DModel
    {
        #region Private Fields
        /// <summary>
        /// Список точек в пространстве
        /// </summary>
        private Point3DCollection pointsInSpace;
        /// <summary>
        /// Список индексов треугольников
        /// </summary>
        private Int32Collection triangleIndices;
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор класса траектории в пространстве
        /// </summary>
        public Trajectory3DModel()
        {
            pointsInSpace = new();
            triangleIndices = new();
        }

        #endregion

        #region Public Methods
        /// <summary>
        /// Добавляет точку в пространстве в коллекцию точек
        /// </summary>
        /// <param name="pointInSpace">Точка в пространстве</param>
        public void AddPointInSpace(Point3D pointInSpace) => PointsInSpace.Add(pointInSpace);
        /// <summary>
        /// Удаляет точку в пространстве из коллекции точек
        /// </summary>
        /// <param name="pointInSpace">Точка в пространстве</param>
        public void RemovePointInSpace(Point3D pointInSpace) => PointsInSpace.Remove(pointInSpace);
        /// <summary>
        /// Удаляет все точки из коллекции
        /// </summary>
        public void RemoveAllPoints() => PointsInSpace.Clear();
        /// <summary>
        /// Добавляет индекс треугольника в коллекцию индексов треугольников
        /// </summary>
        /// <param name="index">Индекс треугольника</param>
        public void AddTriangleIndice(int index) => TriangleIndices.Add(index);
        /// <summary>
        /// Удаляет индекс треугольника из коллекции индексов треугольников
        /// </summary>
        /// <param name="index">Индекс треугольника</param>
        public void RemoveTriangleIndice(int index) => TriangleIndices.Remove(index);
        /// <summary>
        /// Удаляет все индексы треугольников из коллекции индексов треугольников
        /// </summary>
        public void RemoveAllIndices() => TriangleIndices.Clear();
        #endregion

        #region Properties
        /// <summary>
        /// Возвращает коллекцию точек в пространстве
        /// </summary>
        public Point3DCollection PointsInSpace => pointsInSpace;
        /// <summary>
        /// Возвращает коллекцию индексов треугольников
        /// </summary>
        public Int32Collection TriangleIndices => triangleIndices;
        #endregion
    }
}
