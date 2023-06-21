using Microsoft.VisualBasic.FileIO;
using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
//using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using TrajectoryOfSensorVisualization.Model;

namespace TrajectoryOfSensorVisualization.ViewModel
{
    public class PlotsViewModel : INotifyPropertyChanged
    {
        private PlaneModel planeXY;
        private PlaneModel planeXZ;
        private PlaneModel planeYZ;
        private Point3DCollection pointsInSpace;
        private Point3DCollection pointsInSpaceToDisplay3DPlots;
        private Int32Collection triangleIndices;

        

        public PlotsViewModel()
        {
            PlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -0.5, 0.5);
            PlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -0.5, 0.5);
            PlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -0.5, 0.5);
            pointsInSpace = new()
            {
                new Point3D(0, 0, 0.5)
            };
            pointsInSpaceToDisplay3DPlots = new()
            {
                new Point3D(0, 0, 0.5)
            };
            triangleIndices = new();
            GenerateData();
            LoadData();
            //CalculateIntegral();
            //double d = 180;
            //d.ToRadians();
            //Vector3D v = new(0, 1, 0);
            //Vector3D v1 = new(1, 0, 0);
            //Quaternion q = new(v1, Math.PI / 180 * 90);
            //Vector3D v2 = TrajectoryCalculator.RotateVector3D(v, q);
        }

        #region Methods For Work With ViewModel Data
        public void CalculateIntegral()
        {
            List<Vector3D> accelerationVectors = new();
            List<Quaternion> quaternions = new();
            
            using (StreamReader reader = new(@"TestFile.csv"))
            {
                double gLength = Convert.ToDouble(reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None)[0]);
                while(!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    string[] separators = new string[] { ";" };
                    line = line.Replace('.', ',');
                    string[] data = line.Split(separators, StringSplitOptions.None);
                    accelerationVectors.Add(new() { X = Convert.ToDouble(data[0]), Y = Convert.ToDouble(data[1]), Z = Convert.ToDouble(data[2]) });
                    quaternions.Add(new() { X = Convert.ToDouble(data[4]), Y = Convert.ToDouble(data[5]), Z = Convert.ToDouble(data[6]), W = Convert.ToDouble(data[3]) });
                }
                accelerationVectors = RotateVectors(accelerationVectors, quaternions, gLength);
            }
            Vector3D v = TrajectoryCalculator.CalculateDisplacementWithIntegral(accelerationVectors, 100, 200);
            Debug.WriteLine($"Интеграл равен: {v.X}, {v.Y}, {v.Z}");
        }

        public List<Vector3D> RotateVectors(List<Vector3D> axellerateVectors, List<Quaternion> quaternions, double gLength)
        {
            List<Vector3D> result = new List<Vector3D>();
            for (int i = 0; i < axellerateVectors.Count; i++)
            {
                Vector3D temporaryVector = new(0.0, 1.0, 0.0);
                axellerateVectors[i] -= temporaryVector * gLength;
            }
            for (int i = 0; i < axellerateVectors.Count; i++)
            {
                result.Add(TrajectoryCalculator.RotateVector3D(axellerateVectors[i], quaternions[i]));
            }
            return result;
        }

        public void GenerateData()
        {
            Random random = new Random();
            double radius = 0.5;
            List<Vector3D> pointsForApproximation = new()
            {
                new Vector3D(0, 0, radius),
            };
            List<(double alphaAngle, double betaAngle)> angles = new()
            {
                new (0, 0),
            };
            int k = 0;
            for (double t = 0.01; t < 0.1; t += 0.01)
            {
                double w1 = random.NextDouble() * 500.0 + 1.0;
                double w2 = random.NextDouble() * 500.0 + 1.0;
                double a = random.NextDouble() * 140.0 + 1.0;
                double b = random.NextDouble() * 140.0 + 1.0;
                
                double alphaAngle = TrajectoryCalculator.CalculateAngleAlpha(a, w1, t);
                double betaAngle = TrajectoryCalculator.CalculateAngleBeta(b, w2, t); // Math.PI / 180 * 63
                angles.Add((alphaAngle, betaAngle));
                pointsForApproximation.Add(TrajectoryCalculator.CalculateLocationInSpace(radius, alphaAngle, betaAngle));
                if (pointsForApproximation.Count == 2)
                {
                    CalculateTrajectory(pointsForApproximation, angles, radius, ref k);
                    pointsForApproximation.RemoveAt(0);
                    angles.RemoveAt(0);
                }
            }
        }

        public void CalculateTrajectory(List<Vector3D> pointsForApproximation, List<(double alphaAngle, double betaAngle)> angles, double radius, ref int k)
        {
            int countStepsAlpha = (int)((angles[1].alphaAngle - angles[0].alphaAngle) / 0.1);
            int countStepsBeta = (int)((angles[1].betaAngle - angles[0].betaAngle) / 0.1);
            double stepAlpha = countStepsBeta < 0 ? -0.1 : 0.1;
            double stepBeta = countStepsBeta < 0 ? -0.1 : 0.1;
            //int countStepsAlpha = 100; 
            //int countStepsBeta = 100;
            //double stepAlpha = (angles[1].alphaAngle - angles[0].alphaAngle) / 100.0;
            //double stepBeta = (angles[1].betaAngle - angles[0].betaAngle) / 100.0;
            double currentAlphaAngle = angles[0].alphaAngle + stepAlpha;
            double currentBetaAngle = angles[0].betaAngle + stepBeta;
            pointsInSpace.Add((Point3D)pointsForApproximation.First());
            pointsInSpaceToDisplay3DPlots.Add((Point3D)pointsForApproximation.First());
            pointsInSpaceToDisplay3DPlots.Add(new Point3D(pointsForApproximation.First().X, pointsForApproximation.First().Y - 0.02, pointsForApproximation.First().Z));
            triangleIndices.Add(k);
            triangleIndices.Add(k + 1);
            k++;
            for (int i = 0, j = 0; i < Math.Abs(countStepsAlpha) || j < Math.Abs(countStepsBeta); i++, j++, k++) // до 30 по i без j выводит дугу 
            {
                Vector3D currentPointInSpace = TrajectoryCalculator.CalculateLocationInSpace(radius, currentAlphaAngle, currentBetaAngle);
                pointsInSpace.Add((Point3D)currentPointInSpace);
                pointsInSpaceToDisplay3DPlots.Add((Point3D)currentPointInSpace);
                pointsInSpaceToDisplay3DPlots.Add(new Point3D(currentPointInSpace.X, currentPointInSpace.Y - 0.02, currentPointInSpace.Z));
                triangleIndices.Add(k);
                triangleIndices.Add(k - 1); 
                triangleIndices.Add(k + 1);
                triangleIndices.Add(k);
                triangleIndices.Add(k);
                triangleIndices.Add(k + 1);
                k++;
                if (i < Math.Abs(countStepsAlpha))
                {
                    currentAlphaAngle += stepAlpha;
                }
                if (j < Math.Abs(countStepsBeta))
                {
                    currentBetaAngle += stepBeta;
                }
            }
            pointsInSpace.Add((Point3D)pointsForApproximation.Last());
            pointsInSpaceToDisplay3DPlots.Add((Point3D)pointsForApproximation.Last());
            pointsInSpaceToDisplay3DPlots.Add(new Point3D(pointsForApproximation.Last().X, pointsForApproximation.Last().Y - 0.02, pointsForApproximation.Last().Z));
            triangleIndices.Add(k);
            triangleIndices.Add(k - 1);
            triangleIndices.Add(k + 1);
            triangleIndices.Add(k);
            triangleIndices.Add(k);
            triangleIndices.Add(k + 1);
            k++;
        }

        #region Find trajectory with approximation
        //public void CalculateTrajectory(List<Vector3D> pointsForApproximation, ref int k)
        //{
        //    for (double t = 0.0; t < 1.0; t += 0.01, k++)
        //    {
        //        Vector3D approximatedPoint = TrajectoryCalculator.Approximate(pointsForApproximation[0], pointsForApproximation[1], pointsForApproximation[2], pointsForApproximation[3], t);
        //        pointsInSpace.Add((Point3D)approximatedPoint);
        //        pointsInSpaceToDisplay3DPlots.Add((Point3D)approximatedPoint);
        //        pointsInSpaceToDisplay3DPlots.Add(new Point3D(approximatedPoint.X, approximatedPoint.Y - 0.02, approximatedPoint.Z));
        //        if (k > 0)
        //        {
        //            triangleIndices.Add(k);
        //            triangleIndices.Add(k - 1);
        //            triangleIndices.Add(k + 1);
        //            triangleIndices.Add(k);
        //        }
        //        triangleIndices.Add(k);
        //        triangleIndices.Add(k + 1);
        //        k++;
        //    }
        //}
        #endregion
        #endregion

        public void LoadData()
        {
            foreach(Point3D pointInSpace in pointsInSpace)
            {
                PlaneXY.AddDataPoint(new(pointInSpace.X, pointInSpace.Y));
                PlaneXZ.AddDataPoint(new(pointInSpace.X, pointInSpace.Z));
                PlaneYZ.AddDataPoint(new(pointInSpace.Y, pointInSpace.Z));
            }
        }

        #region Properties
        public PlaneModel PlaneXY
        {
            get { return planeXY; }
            set { planeXY = value; OnPropertyChanged(nameof(PlaneXY)); }
        }

        public PlaneModel PlaneXZ
        {
            get { return planeXZ; }
            set { planeXZ = value; OnPropertyChanged(nameof(PlaneXZ)); }
        }

        public PlaneModel PlaneYZ
        {
            get { return planeYZ; }
            set { planeYZ = value; OnPropertyChanged(nameof(PlaneYZ)); }
        }

        public Point3DCollection Points
        {
            get { return pointsInSpaceToDisplay3DPlots; }
        }

        public Int32Collection TriangleIndices
        {
            get { return triangleIndices; }
        }
        #endregion

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
