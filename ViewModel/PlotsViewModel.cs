using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
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
using TrajectoryOfSensorVisualization.Util;

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
        /// <summary>
        /// 
        /// </summary>
        private string filePath;

        //private PlaneModel planeXY1;
        //private PlaneModel planeXZ1;
        //private PlaneModel planeYZ1;
        //private Trajectory3DModel trajectoryInSpace1;
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
            Sphere = new(0.5, Color.FromRgb(255, 0, 0), 0.3);
            //GenerateData();
            //CalculateIntegral();
            #region Test rotation
            //Vector3D v = new(0.010925709, 0.999916115, -0.006956365);
            //Vector3D v1 = new(1, 0, 0);
            //double angle = 90;
            //Quaternion q = new(0.003478152, -1.90E-05, 0.005463035, 0.999979028);
            //Vector3D v2 = TrajectoryCalculator.RotateVectorInSpace(v, q);
            //Debug.WriteLine($"{v2.X}, {v2.Y}, {v2.Z}");
            #endregion
        }
        #endregion

        #region Commands
        private RelayCommand openFileCommand;
        public RelayCommand OpenFileCommand
        {
            get
            {
                return openFileCommand ??
                    (openFileCommand = new RelayCommand(obj =>
                    {
                        OpenFileDialog fileDialog = new OpenFileDialog();
                        fileDialog.Filter = "CSV files (*.csv) | *.csv";
                        fileDialog.Title = "Please pick a CSV source file";

                        bool? success = fileDialog.ShowDialog();
                        if (success == true)
                        {
                            SelectedFilePath = fileDialog.FileName;
                        }
                    }));
            }
        }

        private RelayCommand calculateTrajectoryCommand;
        public RelayCommand CalculateTrajectoryCommand
        {
            get
            {
                return calculateTrajectoryCommand ??
                    (calculateTrajectoryCommand = new RelayCommand(obj =>
                    {
                        CalculateTrajectory();
                        //Task.Run(CalculateTrajectory).ContinueWith(delegate
                        //{
                        //    PlaneXY = planeXY1;
                        //    PlaneXZ = planeXZ1;
                        //    PlaneYZ = planeYZ1;
                        //    TrajectoryInSpace = trajectoryInSpace1;
                        //});
                    }));
            }
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
            string file = "sensor_data (вперёд назад 3).csv";
            DataReader.ReadVectorsAndQuaternionsFromFile(file, ref accelerationVectors, ref gyrVectors, ref quaternions);
            int sampleFreq = DataReader.ReadSampleFreqFromFile(file);
            double gLength = DataReader.ReadGVectorLengthFromFile(file);
            var accelerationVectorsRot = RotateVectors(accelerationVectors, quaternions, gLength);
            double avgR = 0;
            int count = 0;
            for (int i = 0; i < accelerationVectorsRot.Count - 300; i += 100)
            {
                double r = TrajectoryCalculator.CalculateRadius(i, i + 300, accelerationVectorsRot, quaternions, sampleFreq);
                avgR += r;
                count++;
            }
            Debug.WriteLine($"Радиус = {avgR / count}");
            //DrawDisplacement(accelerationVectorsRot, sampleFreq);
        }
        /// <summary>
        /// Заполняет точками перемещение датчика в пространстве
        /// </summary>
        /// <param name="accelerationVectors">Векторы ускорений</param>
        /// <param name="sampleFreq">Кол-во отсчётов в секунду</param>
        public void DrawDisplacement(List<Vector3D> accelerationVectors, int sampleFreq, ref PlaneModel planeXY, ref PlaneModel planeXZ, ref PlaneModel planeYZ, ref Trajectory3DModel trajectory3D)
        {
            int k = 0;
            List<Vector3D> displacementVectors = TrajectoryCalculator.CalculateIntegralAvgRectangle(accelerationVectors, 0, accelerationVectors.Count, sampleFreq);
            List<Vector3D> avgVectors = TrajectoryCalculator.AverageFloatingWindow(displacementVectors, 101);
            foreach (Vector3D vectorDisplacement in TrajectoryCalculator.SubstractListsOfVector3D(displacementVectors, avgVectors))
            {
                planeXY.AddDataPoint(new(vectorDisplacement.X, vectorDisplacement.Y));
                planeXZ.AddDataPoint(new(vectorDisplacement.X, vectorDisplacement.Z));
                planeYZ.AddDataPoint(new(vectorDisplacement.Y, vectorDisplacement.Z));
                trajectory3D.AddPointInSpace((Point3D)vectorDisplacement);
                trajectory3D.AddPointInSpace(new(vectorDisplacement.X, vectorDisplacement.Y - 0.02, vectorDisplacement.Z));
                if (k > 0)
                {
                    trajectory3D.AddTriangleIndice(k);
                    trajectory3D.AddTriangleIndice(k - 1);
                    trajectory3D.AddTriangleIndice(k + 1);
                    trajectory3D.AddTriangleIndice(k);
                }
                trajectory3D.AddTriangleIndice(k);
                trajectory3D.AddTriangleIndice(k + 1);
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

        #region Private Methods
        private void CalculateTrajectory()
        {
            PlaneModel newPlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -0.5, 0.5);
            PlaneModel newPlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Red, "X", -0.5, 0.5);
            PlaneModel newPlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -0.5, 0.5);
            Trajectory3DModel newTrajectoryInSpace = new(new(0, 0, 0));
            List<Vector3D> accelerationVectors = new();
            List<Quaternion> quaternions = new();
            List<Vector3D> gyrVectors = new();
            DataReader.ReadVectorsAndQuaternionsFromFile(filePath, ref accelerationVectors, ref gyrVectors, ref quaternions);
            int sampleFreq = DataReader.ReadSampleFreqFromFile(filePath);
            double gLength = DataReader.ReadGVectorLengthFromFile(filePath);
            var accelerationVectorsRot = RotateVectors(accelerationVectors, quaternions, gLength);
            double avgR = 0;
            int count = 0;
            for (int i = 0; i < 400; i += 10)
            {
                double r = TrajectoryCalculator.CalculateRadius(i, i + 100, accelerationVectorsRot, quaternions, sampleFreq);
                avgR += r;
                count++;
            }
            Debug.WriteLine($"Радиус = {avgR / count}");
            DrawDisplacement(accelerationVectorsRot, sampleFreq, ref newPlaneXY, ref newPlaneXZ, ref newPlaneYZ, ref newTrajectoryInSpace);
            PlaneXY = newPlaneXY;
            PlaneXZ = newPlaneXZ;
            PlaneYZ = newPlaneYZ;
            TrajectoryInSpace = newTrajectoryInSpace;
            //App.Current.Dispatcher.Invoke(() =>
            //{
            //    NewMethod(newPlaneXY, newPlaneXZ, newPlaneYZ, newTrajectoryInSpace);
            //});
        }

        //private void NewMethod(PlaneModel p1, PlaneModel p2, PlaneModel p3, Trajectory3DModel trajectory)
        //{
        //    planeXY1 = p1;
        //    planeXZ1 = p2;
        //    planeYZ1 = p3;
        //    trajectoryInSpace1 = trajectory;
        //}
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
        /// <summary>
        /// 
        /// </summary>
        public string SelectedFilePath 
        { 
            get { return filePath; } 
            set { filePath = value; OnPropertyChanged(nameof(SelectedFilePath)); } 
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
