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
    /// <summary>
    /// ViewModel класс главного экрана, хранит данные отображаемые во View
    /// </summary>
    public class PlotsViewModel : INotifyPropertyChanged
    {
        #region Private Fields
        /// <summary>
        /// График на плоскости XY
        /// </summary>
        private PlaneModel planeXY;
        /// <summary>
        /// График на плоскости XZ
        /// </summary>
        private PlaneModel planeXZ;
        /// <summary>
        /// График на плоскости YZ
        /// </summary>
        private PlaneModel planeYZ;
        /// <summary>
        /// Траектория в пространстве
        /// </summary>
        private Trajectory3DModel trajectoryInSpace;
        /// <summary>
        /// Сфера в пространстве
        /// </summary>
        private SphereGeometry3D sphere;
        #endregion
        
        #region Constructors
        /// <summary>
        /// Конструктор ViewModel
        /// </summary>
        public PlotsViewModel()
        {
            PlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -0.5, 0.5);
            PlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Red, "X", -0.5, 0.5);
            PlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -0.5, 0.5);
            TrajectoryInSpace = new(new(0, 0, 0));
            Sphere = new(Color.FromRgb(255, 0, 0), 0.3) { Radius = 0.5, Separators = 25 };
            //GenerateData();
            CalculateIntegral();
            #region Test rotation
            //Vector3D v = new(1, 2, 0);
            //Vector3D v1 = new(1, 0, 0);
            //double angle = 90;
            //Quaternion q = new(v1, angle);
            //Vector3D v2 = TrajectoryCalculator.RotateVectorInSpace(v, q);
            //Debug.WriteLine($"{v2.X}, {v2.Y}, {v2.Z}");
            #endregion
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Генирирует данные для отображения во View
        /// </summary>
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
                        TrajectoryInSpace.AddTriangleIndice(k);
                        TrajectoryInSpace.AddTriangleIndice(k - 1);
                        TrajectoryInSpace.AddTriangleIndice(k + 1);
                        TrajectoryInSpace.AddTriangleIndice(k);
                    }
                    TrajectoryInSpace.AddTriangleIndice(k);
                    TrajectoryInSpace.AddTriangleIndice(k + 1);
                    k += 2;
                }
                initialPoint = endPoint;
            }
        }
        /// <summary>
        /// Вычисляет перемещение датчика по данным из файла
        /// </summary>
        public void CalculateIntegral()
        {
            List<Vector3D> accelerationVectors = new();
            List<Quaternion> quaternions = new();
            List <Vector3D> gyrVectors = new();
            #region Old but gold
            //using (StreamReader reader = new(@"sensor_data.csv"))
            //{
            //    reader.ReadLine();
            //    string[] initialLine = reader.ReadLine().ToString().Replace('.', ',').Split(';', StringSplitOptions.None);
            //    double gLength = Convert.ToDouble(initialLine[5]);
            //    sampleFreq = Convert.ToInt32(initialLine[0]);
            //    calibCount = Convert.ToInt32(initialLine[1]);
            //    for(int i = 0; i < calibCount + 3; i++)
            //    {
            //        reader.ReadLine();
            //    }
            //    while(!reader.EndOfStream)
            //    {
            //        string line = reader.ReadLine();
            //        string[] separators = new string[] { ";" };
            //        line = line.Replace('.', ',');
            //        string[] data = line.Split(separators, StringSplitOptions.None);
            //        accelerationVectors.Add(new() { X = Convert.ToDouble(data[0]), Y = Convert.ToDouble(data[1]), Z = Convert.ToDouble(data[2]) });
            //        quaternions.Add(new() { X = Convert.ToDouble(data[7]), Y = Convert.ToDouble(data[8]), Z = Convert.ToDouble(data[9]), W = Convert.ToDouble(data[6]) });
            //    }
            //    accelerationVectors = RotateVectors(accelerationVectors, quaternions, gLength);
            //}
            //Vector3D v = TrajectoryCalculator.CalculateDisplacement(accelerationVectors, 100, 200, sampleFreq);
            //Debug.WriteLine($"Интеграл равен: {v.X}, {v.Y}, {v.Z}");
            #endregion
            DataReader.ReadVectorsAndQuaternionsFromFile("sensor_data.csv", ref accelerationVectors, ref gyrVectors, ref quaternions);
            int sampleFreq = DataReader.ReadSampleFreqFromFile("sensor_data.csv");
            double gLength = DataReader.ReadGVectorLengthFromFile("sensor_data.csv");
            accelerationVectors = RotateVectors(accelerationVectors, quaternions, gLength);
            double avgR = 0;
            int count = 0;
            for (int i = 0; i < accelerationVectors.Count - 101; i += 100)
            {
                double r = TrajectoryCalculator.CalculateRadius(i, i + 100, accelerationVectors, quaternions, sampleFreq);
                avgR += r;
                count++;
            }
            Debug.WriteLine($"Радиус = {avgR / count}");
            DrawDisplacement(accelerationVectors, sampleFreq);
        }
        /// <summary>
        /// Заполняет точками перемещение датчика в пространстве
        /// </summary>
        /// <param name="accelerationVectors">Векторы ускорений</param>
        /// <param name="sampleFreq">Кол-во отсчётов в секунду</param>
        public void DrawDisplacement(List<Vector3D> accelerationVectors, int sampleFreq)
        {
            int k = 0;
            foreach (Vector3D vectorDisplacement in TrajectoryCalculator.CalculateAndReturnListOfDisplacements(accelerationVectors, 0, 300, sampleFreq))
            {
                PlaneXY.AddDataPoint(new(vectorDisplacement.X, vectorDisplacement.Y));
                PlaneXZ.AddDataPoint(new(vectorDisplacement.X, vectorDisplacement.Z));
                PlaneYZ.AddDataPoint(new(vectorDisplacement.Y, vectorDisplacement.Z));
                TrajectoryInSpace.AddPointInSpace((Point3D)vectorDisplacement);
                TrajectoryInSpace.AddPointInSpace(new(vectorDisplacement.X, vectorDisplacement.Y - 0.02, vectorDisplacement.Z));
                if (k > 0)
                {
                    TrajectoryInSpace.AddTriangleIndice(k);
                    TrajectoryInSpace.AddTriangleIndice(k - 1);
                    TrajectoryInSpace.AddTriangleIndice(k + 1);
                    TrajectoryInSpace.AddTriangleIndice(k);
                }
                TrajectoryInSpace.AddTriangleIndice(k);
                TrajectoryInSpace.AddTriangleIndice(k + 1);
                k += 2;
                Debug.WriteLine($"{vectorDisplacement.X}, {vectorDisplacement.Y}, {vectorDisplacement.Z}");
            }
        }
        /// <summary>
        /// Поворот векторов ускорений с помощью кватернионов
        /// </summary>
        /// <param name="accellerationVectors">Список векторов ускорений</param>
        /// <param name="quaternions">Список кватернионов поворота</param>
        /// <param name="gLength">Длина вектора ускорения свободного падения</param>
        /// <returns>Список повёрнутых векторов ускорений</returns>
        public List<Vector3D> RotateVectors(List<Vector3D> accellerationVectors, List<Quaternion> quaternions, double gLength)
        {
            List<Vector3D> result = new List<Vector3D>();
            
            for (int i = 0; i < accellerationVectors.Count; i++)
            {
                result.Add(TrajectoryCalculator.RotateVectorInSpace(accellerationVectors[i], quaternions[i]));
            }
            for (int i = 0; i < result.Count; i++)
            {
                Vector3D temporaryVector = new(0.0, 1.0, 0.0);
                result[i] -= temporaryVector * gLength;
            }
            return result;
        }
        #endregion

        #region Properties
        /// <summary>
        /// Возвращает/устанавливает график в плоскости XY
        /// </summary>
        public PlaneModel PlaneXY
        {
            get { return planeXY; }
            set { planeXY = value; OnPropertyChanged(nameof(PlaneXY)); }
        }
        /// <summary>
        /// Возвращает/устанавливает график в плоскости XZ
        /// </summary>
        public PlaneModel PlaneXZ
        {
            get { return planeXZ; }
            set { planeXZ = value; OnPropertyChanged(nameof(PlaneXZ)); }
        }
        /// <summary>
        /// Возвращает/устанавливает график в плоскости YZ
        /// </summary>
        public PlaneModel PlaneYZ
        {
            get { return planeYZ; }
            set { planeYZ = value; OnPropertyChanged(nameof(PlaneYZ)); }
        }
        /// <summary>
        /// Возвращает/устанавливает траекторию движения в пространстве датчика
        /// </summary>
        public Trajectory3DModel TrajectoryInSpace
        {
            get { return trajectoryInSpace; }
            set { trajectoryInSpace = value; OnPropertyChanged(nameof(TrajectoryInSpace)); }
        }
        /// <summary>
        /// Возвращает/устанавливает сферу, по которой движется датчик
        /// </summary>
        public SphereGeometry3D Sphere
        {
            get { return sphere; }
            set { sphere = value; OnPropertyChanged(nameof(Sphere)); }
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
