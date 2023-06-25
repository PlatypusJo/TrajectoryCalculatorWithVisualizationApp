using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;
using System.Windows.Media;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// Класс для определения сферических/круглых 3D объектов
    /// </summary>
    public abstract class RoundMesh
    {
        #region Fields
        /// <summary>
        /// Кол-во сегментов
        /// </summary>
        protected int n = 10;
        /// <summary>
        /// Радиус
        /// </summary>
        protected double r = 20;
        /// <summary>
        /// Кисть
        /// </summary>
        protected Brush brush;
        /// <summary>
        /// Точки 3D
        /// </summary>
        protected Point3DCollection points;
        /// <summary>
        /// Индексы треугольников
        /// </summary>
        protected Int32Collection triangleIndices;
        #endregion
        
        #region Properties
        /// <summary>
        /// Возвращает/устанавливает радиус
        /// </summary>
        public virtual double Radius
        {
            get { return r; }
            set { r = value; CalculateGeometry(); }
        }
        /// <summary>
        /// Возвращает/устанавливает кол-во сегментов 
        /// </summary>
        public virtual int Separators
        {
            get { return n; }
            set { n = value; CalculateGeometry(); }
        }
        /// <summary>
        /// Возвращает кисть
        /// </summary>
        public virtual Brush Brush
        {
            get { return brush; }
        }
        /// <summary>
        /// Возвращает список точек
        /// </summary>
        public Point3DCollection Points
        {
            get { return points; }
        }
        /// <summary>
        /// Возвращает список индексов треугольников
        /// </summary>
        public Int32Collection TriangleIndices
        {
            get { return triangleIndices; }
        }
        #endregion

        #region Methods
        /// <summary>
        /// Метод для вычисления геометрии объекта
        /// </summary>
        protected abstract void CalculateGeometry();
        #endregion
        
    }
}
