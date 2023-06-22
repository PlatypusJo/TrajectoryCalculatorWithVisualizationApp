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
        #region Private Fields
        private PlaneModel planeXY;
        private PlaneModel planeXZ;
        private PlaneModel planeYZ;
        private Trajectory3DModel trajectoryInSpace;
        #endregion
        
        #region Constructors
        public PlotsViewModel()
        {
            PlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -0.5, 0.5);
            PlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Red, "X", -0.5, 0.5);
            PlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -0.5, 0.5);
            TrajectoryInSpace = new(new(0, 0, 0.5));
            GenerateData();
            CalculateIntegral();
            Vector3D v = new(1, 2, 0);
            Vector3D v1 = new(1, 0, 0);
            double angle = 90;
            Quaternion q = new(v1.X, v1.Y, v1.Z, angle.ToRadians());
            Vector3D v2 = TrajectoryCalculator.RotateVectorInSpace(v, q);
            Debug.WriteLine($"{v2.X}, {v2.Y}, {v2.Z}");
        }
        #endregion
        
        #region Public Methods
        public void GenerateData()
        {
            Random random = new Random();
            double radius = 0.5;
            Vector3D initialPoint = new(0, 0, 0.5);
            int k = 0;
            for (double t = 0.01; t < 0.1; t += 0.01)
            {
                double w1 = random.NextDouble() * 500.0 + 1.0;
                double w2 = random.NextDouble() * 500.0 + 1.0;
                double a = random.NextDouble() * 140.0 + 1.0;
                double b = random.NextDouble() * 140.0 + 1.0;
                
                double alphaAngle = TrajectoryCalculator.CalculateAngleAlpha(a, w1, t);
                double betaAngle = TrajectoryCalculator.CalculateAngleBeta(b, w2, t); // Math.PI / 180 * 63
                Vector3D endPoint = TrajectoryCalculator.CalculateLocationInSpace(radius, alphaAngle, betaAngle);
                foreach (Point3D pointInSpace in TrajectoryCalculator.CalculatePointsOfTrajectoryInSpace(initialPoint, endPoint, radius))
                {
                    PlaneXY.AddDataPoint(new(pointInSpace.X, pointInSpace.Y));
                    PlaneXZ.AddDataPoint(new(pointInSpace.X, pointInSpace.Z));
                    PlaneYZ.AddDataPoint(new(pointInSpace.Y, pointInSpace.Z));
                    TrajectoryInSpace.AddPointInSpace(pointInSpace);
                    TrajectoryInSpace.AddPointInSpace(new(pointInSpace.X, pointInSpace.Y - 0.02, pointInSpace.Z));
                    if (k > 0)
                    {
                        TrajectoryInSpace.AddTriangleIndex(k);
                        TrajectoryInSpace.AddTriangleIndex(k - 1);
                        TrajectoryInSpace.AddTriangleIndex(k + 1);
                        TrajectoryInSpace.AddTriangleIndex(k);
                    }
                    TrajectoryInSpace.AddTriangleIndex(k);
                    TrajectoryInSpace.AddTriangleIndex(k + 1);
                    k += 2;
                }
                initialPoint = endPoint;
            }
        }

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
            Vector3D v = TrajectoryCalculator.CalculateDisplacement(accelerationVectors, 100, 200);
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
                result.Add(TrajectoryCalculator.RotateVectorInSpace(axellerateVectors[i], quaternions[i]));
            }
            return result;
        }
        #endregion

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

        public Trajectory3DModel TrajectoryInSpace
        {
            get { return trajectoryInSpace; }
            set { trajectoryInSpace = value; OnPropertyChanged(nameof(TrajectoryInSpace)); }
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
