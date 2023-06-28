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
    /// Класс представляющий сферу в пространстве
    /// </summary>
    public class SphereGeometry3D : RoundMesh
    {
        #region Constructors
        /// <summary>
        /// Конструктор класса 3D сферы
        /// </summary>
        /// <param name="colorOfBrush">Цвет сферы</param>
        /// <param name="opacityOfBrush">Прозрачность сферы от 0 до 1</param>
        public SphereGeometry3D(double radius, Color colorOfBrush, double opacityOfBrush)
        { 
            Radius = radius;
            Separators = (int)(radius * 50);
            brush = new SolidColorBrush(colorOfBrush)
            {
                Opacity = opacityOfBrush
            };
        }
        #endregion

        #region Methods
        /// <summary>
        /// Метод для вычисления геометрии объекта
        /// </summary>
        protected override void CalculateGeometry()
        {
            int e;
            double segmentRad = Math.PI / 2 / (n + 1);
            int numberOfSeparators = 4 * n + 4;

            points = new Point3DCollection();
            triangleIndices = new Int32Collection();

            for (e = -n; e <= n; e++)
            {
                double r_e = r * Math.Cos(segmentRad * e);
                double y_e = r * Math.Sin(segmentRad * e);

                for (int s = 0; s <= (numberOfSeparators - 1); s++)
                {
                    double z_s = r_e * Math.Sin(segmentRad * s) * (-1);
                    double x_s = r_e * Math.Cos(segmentRad * s);
                    points.Add(new Point3D(x_s, y_e, z_s));
                }
            }
            points.Add(new Point3D(0, r, 0));
            points.Add(new Point3D(0, -1 * r, 0));

            for (e = 0; e < 2 * n; e++)
            {
                for (int i = 0; i < numberOfSeparators; i++)
                {
                    triangleIndices.Add(e * numberOfSeparators + i);
                    triangleIndices.Add(e * numberOfSeparators + i +
                                        numberOfSeparators);
                    triangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                        numberOfSeparators + numberOfSeparators);

                    triangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                        numberOfSeparators + numberOfSeparators);
                    triangleIndices.Add(e * numberOfSeparators +
                                       (i + 1) % numberOfSeparators);
                    triangleIndices.Add(e * numberOfSeparators + i);
                }
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                triangleIndices.Add(e * numberOfSeparators + i);
                triangleIndices.Add(e * numberOfSeparators + (i + 1) %
                                    numberOfSeparators);
                triangleIndices.Add(numberOfSeparators * (2 * n + 1));
            }

            for (int i = 0; i < numberOfSeparators; i++)
            {
                triangleIndices.Add(i);
                triangleIndices.Add((i + 1) % numberOfSeparators);
                triangleIndices.Add(numberOfSeparators * (2 * n + 1) + 1);
            }
        }
        #endregion
    }
}
