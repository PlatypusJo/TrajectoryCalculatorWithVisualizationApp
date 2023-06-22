using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    public class Trajectory3DModel
    {
        #region Constructors
        public Trajectory3DModel(Point3D initialPoint)
        {
            pointsInSpace = new();
            triangleIndexes = new();
            AddPointInSpace(initialPoint);
        }
        #endregion

        #region Private Fields
        private Point3DCollection pointsInSpace;
        private Int32Collection triangleIndexes;
        #endregion

        #region Public Methods
        public void AddPointInSpace(Point3D pointInSpace) => PointsInSpace.Add(pointInSpace);
        public void RemovePointInSpace(Point3D pointInSpace) => PointsInSpace.Remove(pointInSpace);
        public void RemoveAllPoints() => PointsInSpace.Clear();
        public void AddTriangleIndex(int index) => TriangleIndexes.Add(index);
        public void RemoveTriangleIndex(int index) => TriangleIndexes.Remove(index);
        public void RemoveAllIndexes() => TriangleIndexes.Clear();
        #endregion

        #region Properties
        public Point3DCollection PointsInSpace => pointsInSpace;
        public Int32Collection TriangleIndexes => triangleIndexes;
        #endregion
    }
}
