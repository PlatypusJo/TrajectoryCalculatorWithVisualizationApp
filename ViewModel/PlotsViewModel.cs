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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
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
        #region Properties and fields
        /// <summary>
        /// График на плоскости XY
        /// </summary>
        private PlaneModel planeXY;
        public PlaneModel PlaneXY
        {
            get { return planeXY; }
            set { planeXY = value; OnPropertyChanged(nameof(PlaneXY)); }
        }
        /// <summary>
        /// График на плоскости XZ
        /// </summary>
        private PlaneModel planeXZ;
        public PlaneModel PlaneXZ
        {
            get { return planeXZ; }
            set { planeXZ = value; OnPropertyChanged(nameof(PlaneXZ)); }
        }
        /// <summary>
        /// График на плоскости YZ
        /// </summary>
        private PlaneModel planeYZ;
        public PlaneModel PlaneYZ
        {
            get { return planeYZ; }
            set { planeYZ = value; OnPropertyChanged(nameof(PlaneYZ)); }
        }
        /// <summary>
        /// Траектория в пространстве
        /// </summary>
        private Trajectory3DModel trajectoryInSpace;
        public Trajectory3DModel TrajectoryInSpace
        {
            get { return trajectoryInSpace; }
            set { trajectoryInSpace = value; OnPropertyChanged(nameof(TrajectoryInSpace)); }
        }
        /// <summary>
        /// Сфера в пространстве
        /// </summary>
        private SphereGeometry3D sphere;
        public SphereGeometry3D Sphere
        {
            get { return sphere; }
            set { sphere = value; OnPropertyChanged(nameof(Sphere)); }
        }
        /// <summary>
        /// Путь к файлу с данными
        /// </summary>
        private string filePath;
        public string SelectedFilePath
        {
            get { return filePath; }
            set { filePath = value; OnPropertyChanged(nameof(SelectedFilePath)); }
        }
        /// <summary>
        /// Радиус сферы (метры)
        /// </summary>
        private double radius;
        public string Radius
        {
            get => radius.ToString();
            set
            {
                try
                {
                    radius = Convert.ToDouble(value.Replace(".", ","));
                }
                catch
                {
                    const string message = "Неправильный формат введённых данных. В поле ввода \"Радиус сферы\" допустимы только действительные числа";
                    const string caption = "Ошибка в формате данных";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
                }
            }
        }
        /// <summary>
        /// Размер окна фильтрации (условные единицы)
        /// </summary>
        private int sizeOfFloatingWindow;
        public string SizeOfFloatingWindow
        {
            get => sizeOfFloatingWindow.ToString();
            set 
            { 
                try
                {
                    sizeOfFloatingWindow = Convert.ToInt32(value); 
                }
                catch
                {
                    const string message = "Неправильный формат введённых данных. В поле ввода \"Плавающее окно\" допустимы только целые числа";
                    const string caption = "Ошибка в формате данных";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
                }
            }
        }
        /// <summary>
        /// Время интегрироввания (секунды)
        /// </summary>
        private double integrationTime;
        public string IntegrationTime
        {
            get => integrationTime.ToString();
            set
            {
                try
                {
                    integrationTime = Convert.ToDouble(value.Replace(".", ","));
                }
                catch
                {
                    const string message = "Неправильный формат введённых данных. В поле ввода \"Время интегрирования\" допустимы только действительные числа";
                    const string caption = "Ошибка в формате данных";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
                }
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Конструктор ViewModel
        /// </summary>
        public PlotsViewModel()
        {
            PlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -100, 100);
            PlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Red, "X", -100, 100);
            PlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -100, 100);
            TrajectoryInSpace = new();
            Sphere = new(0, Color.FromRgb(255, 0, 0), 0.3);
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
                        if (success.HasValue && success.Value == true)
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
                    (calculateTrajectoryCommand = new RelayCommand(async obj =>
                    {

                        PlaneModel newPlaneXY = new(OxyColors.Green, "Y", OxyColors.Red, "X", -radius, radius);
                        PlaneModel newPlaneXZ = new(OxyColors.Blue, "Z", OxyColors.Red, "X", -radius, radius);
                        PlaneModel newPlaneYZ = new(OxyColors.Blue, "Z", OxyColors.Green, "Y", -radius, radius);
                        Trajectory3DModel newTrajectoryInSpace = new();
                        List<Vector3D> vectorCollection = new();
                       
                        vectorCollection = await CalculateTrajectory();

                        if (vectorCollection.Count <= 0)
                        {
                            return;
                        }
                        else
                        {
                            FillModels(vectorCollection, newPlaneXY, newPlaneXZ, newPlaneYZ, newTrajectoryInSpace);
                            UpdateBindings(newPlaneXY, newPlaneXZ, newPlaneYZ, newTrajectoryInSpace);
                        }
                    }));
            }
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Вычисляет траекторию перемещения датчика
        /// </summary>
        private async Task<List<Vector3D>> CalculateTrajectory()
        {
            return await Task<List<Vector3D>>.Run(() =>
            {
                List<Vector3D> accelerationVectors = new();
                List<Quaternion> quaternions = new();
                int sampleFreq = 0;
                double gLength = 0;
                try
                {
                    accelerationVectors = DataReader.ReadAccVectorsFromFile(SelectedFilePath);
                    quaternions = DataReader.ReadQuaternionsFromFile(SelectedFilePath);
                    sampleFreq = DataReader.ReadSampleFreqFromFile(SelectedFilePath);
                    gLength = DataReader.ReadGVectorLengthFromFile(SelectedFilePath);
                }
                catch
                {
                    accelerationVectors = null;
                    quaternions = null;
                    string message = "Отсутствует файл для чтения или данные в файле имеют некорректный вид.";
                    string caption = "Ошибка при чтении файла";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
                    return new List<Vector3D>();
                }
                List<Vector3D> accelerationVectorsRot = MathToolsFor3D.RotateVectorsInSpace(accelerationVectors, quaternions, gLength);
                List<Vector3D> displacementVectors = new();
                List<Vector3D> filtredVectors = new();
                List<Vector3D> noiseVectors = new();
                try
                {
                    displacementVectors = MathToolsFor3D.CalculateIntegralAvgRectangle(accelerationVectorsRot, 0, (int)(integrationTime * sampleFreq), sampleFreq);
                    filtredVectors = MathToolsFor3D.FiltrationByFloatingWindow(displacementVectors, sizeOfFloatingWindow);
                    noiseVectors = MathToolsFor3D.CalculateListOfNoiseVectors(displacementVectors, filtredVectors);
                }
                catch
                {
                    displacementVectors = null;
                    filtredVectors = null;
                    noiseVectors = null;
                    string message = "Размер окна фильтрации превышает кол-во отсчётов или время интегрирования превышает время в файле.";
                    string caption = "Ошибка при чтении файла";
                    MessageBoxButton button = MessageBoxButton.OK;
                    MessageBoxImage icon = MessageBoxImage.Error;
                    MessageBox.Show(message, caption, button, icon);
                    return new List<Vector3D>();
                }
                quaternions.RemoveRange(noiseVectors.Count, quaternions.Count - noiseVectors.Count);
                List<Vector3D> vectorsCollection = MathToolsFor3D.CalculateTrajectoryPointsWithNoise(quaternions, noiseVectors, radius);
                return vectorsCollection;
            }); 
        }
        /// <summary>
        /// Функция для заполнения моделей данными, которые будут отображены во View
        /// </summary>
        /// <param name="displacementVectors">Список векторов перемещений</param>
        /// <param name="planeXY">Модель плоскости XY</param>
        /// <param name="planeXZ">Модель плоскости XZ</param>
        /// <param name="planeYZ">Модель плоскости YZ</param>
        /// <param name="trajectory3D">Модель траектории в пространстве</param>
        private void FillModels(List<Vector3D> displacementVectors, PlaneModel planeXY, PlaneModel planeXZ, PlaneModel planeYZ, Trajectory3DModel trajectory3D)
        {
            int k = 0;
            foreach (Vector3D displacementVector in displacementVectors)
            {
                planeXY.AddDataPoint(new(displacementVector.X, displacementVector.Y));
                planeXZ.AddDataPoint(new(displacementVector.X, displacementVector.Z));
                planeYZ.AddDataPoint(new(displacementVector.Y, displacementVector.Z));
                trajectory3D.AddPointInSpace((Point3D)displacementVector);
                trajectory3D.AddPointInSpace(new(displacementVector.X, displacementVector.Y - 0.02, displacementVector.Z));
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
            }
        }
        /// <summary>
        /// Функция для обновления привязанных данных во View
        /// </summary>
        /// <param name="planeXY">Модель плоскости XY</param>
        /// <param name="planeXZ">Модель плоскости XZ</param>
        /// <param name="planeYZ">Модель плоскости YZ</param>
        /// <param name="trajectory">Модель траектории в пространстве</param>
        private void UpdateBindings(PlaneModel planeXY, PlaneModel planeXZ, PlaneModel planeYZ, Trajectory3DModel trajectory)
        {
            PlaneXY = planeXY;
            PlaneXZ = planeXZ;
            PlaneYZ = planeYZ;
            TrajectoryInSpace = trajectory;
            Sphere = new(radius, Color.FromRgb(255, 0, 0), 0.3);
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
