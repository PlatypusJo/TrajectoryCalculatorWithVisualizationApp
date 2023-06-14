using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
        private PlotModel plotModelXYPlane;
        private PlotModel plotModelXZPlane;
        private PlotModel plotModelYZPlane;
        private Point3DCollection pointsInSpace;
        private Point3DCollection pointsInSpaceToDisplay3DPlots;
        private Int32Collection triangleIndices;

        public PlotModel PlotModelXYPlane
        {
            get { return plotModelXYPlane; }
            set { plotModelXYPlane = value; OnPropertyChanged(nameof(PlotModelXYPlane)); }
        }

        public PlotModel PlotModelXZPlane
        {
            get { return plotModelXZPlane; }
            set { plotModelXZPlane = value; OnPropertyChanged(nameof(PlotModelXZPlane)); }
        }

        public PlotModel PlotModelYZPlane
        {
            get { return plotModelYZPlane; }
            set { plotModelYZPlane = value; OnPropertyChanged(nameof(PlotModelYZPlane)); }
        }

        public Point3DCollection Points
        {
            get { return pointsInSpaceToDisplay3DPlots; }
        }

        public Int32Collection TriangleIndices
        {
            get { return triangleIndices; }
        }

        public PlotsViewModel()
        {
            PlotModelXYPlane = new PlotModel();
            PlotModelXZPlane = new PlotModel();
            PlotModelYZPlane = new PlotModel();
            pointsInSpace = new()
            {
                new Point3D(0, 0, 0)
            };
            pointsInSpaceToDisplay3DPlots = new()
            {
                new Point3D(0, 0, 0)
            };
            triangleIndices = new();
            SetUpModelXY();
            SetUpModelXZ();
            SetUpModelYZ();
            GenerateData();
            LoadData();
        }

        public void GenerateData()
        {
            Random random = new Random();
            double radius = 0.5;
            Point3D sensorLocation = new();
            int k = 0;
            for (double t = 0.01; t < 10; t += 0.01, k++)
            {

                double w1 = random.NextDouble() * 500.0 + 1.0;
                double w2 = random.NextDouble() * 500.0 + 1.0;
                double a = random.NextDouble() * 140.0 + 1.0;
                double b = random.NextDouble() * 140.0 + 1.0;
                double alphaAngle = TrajectoryCalculator.CalculateAngleAlpha(a, w1, t);
                double betaAngle = TrajectoryCalculator.CalculateAngleBeta(b, w2, t); // Math.PI / 180 * 63
                TrajectoryCalculator.CalculateLocationInSpace(ref sensorLocation, radius, alphaAngle, betaAngle);
                pointsInSpace.Add(sensorLocation);
                pointsInSpaceToDisplay3DPlots.Add(sensorLocation);
                pointsInSpaceToDisplay3DPlots.Add(new Point3D(sensorLocation.X, sensorLocation.Y - 0.02, sensorLocation.Z));
                if (k > 0)
                {
                    triangleIndices.Add(k);
                    triangleIndices.Add(k - 1);
                    triangleIndices.Add(k + 1);
                    triangleIndices.Add(k);
                }
                triangleIndices.Add(k);
                triangleIndices.Add(k + 1);
                k++;
            }
        }

        public void LoadData()
        {
            var lineXY = new OxyPlot.Series.LineSeries()
            {
                Color = OxyColor.FromRgb(29, 172, 214),
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle
            };
            var lineXZ = new OxyPlot.Series.LineSeries()
            {
                Color = OxyColor.FromRgb(29, 172, 214),
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle
            };
            var lineYZ = new OxyPlot.Series.LineSeries()
            {
                Color = OxyColor.FromRgb(29, 172, 214),
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle
            };
            foreach(Point3D pointInSpace in pointsInSpace)
            {
                lineXY.Points.Add(new DataPoint(pointInSpace.X, pointInSpace.Y));
                lineXZ.Points.Add(new DataPoint(pointInSpace.X, pointInSpace.Z));
                lineYZ.Points.Add(new DataPoint(pointInSpace.Y, pointInSpace.Z));
            }
            PlotModelXYPlane.Series.Add(lineXY);
            PlotModelXZPlane.Series.Add(lineXZ);
            PlotModelYZPlane.Series.Add(lineYZ);
        }

        public void SetUpModelXY()
        {
            var xAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom,
                Minimum = -0.5,
                Maximum = 0.5,
                //PositionAtZeroCrossing = true,
                TicklineColor = OxyColors.Red,
                AxislineColor = OxyColors.Red,
                TextColor = OxyColors.Red,
                MajorGridlineStyle = LineStyle.Solid, 
                MinorGridlineStyle = LineStyle.Solid, 
                Title = "X",
                TitleColor = OxyColors.Red
            };
            PlotModelXYPlane.Axes.Add(xAxis);
            var yAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
                Minimum = -0.5,
                Maximum = 0.5,
                //PositionAtZeroCrossing = true,
                TicklineColor = OxyColors.Green,
                AxislineColor = OxyColors.Green,
                TextColor = OxyColors.Green,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = "Y",
                TitleColor = OxyColors.Green
            };
            PlotModelXYPlane.Axes.Add(yAxis);
        }

        public void SetUpModelXZ()
        {
            var zAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
                Minimum = -0.5,
                Maximum = 0.5,
                TicklineColor = OxyColors.Blue,
                TextColor = OxyColors.Blue,
                AxislineColor = OxyColors.Blue,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = "Z",
                TitleColor = OxyColors.Blue
            };
            PlotModelXZPlane.Axes.Add(zAxis);
            var xAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom,
                Minimum = -0.5,
                Maximum = 0.5,
                TicklineColor = OxyColors.Red,
                AxislineColor = OxyColors.Red,
                TextColor = OxyColors.Red,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = "X",
                TitleColor = OxyColors.Red
            };
            PlotModelXZPlane.Axes.Add(xAxis);
        }

        public void SetUpModelYZ()
        {
            var zAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
                Minimum = -0.5,
                Maximum = 0.5,
                TicklineColor = OxyColors.Blue,
                TextColor = OxyColors.Blue,
                AxislineColor = OxyColors.Blue,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = "Z",
                TitleColor = OxyColors.Blue
            };
            PlotModelYZPlane.Axes.Add(zAxis);
            var yAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Bottom,
                Minimum = -0.5,
                Maximum = 0.5,
                TicklineColor = OxyColors.Green,
                AxislineColor = OxyColors.Green,
                TextColor = OxyColors.Green,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = "Y",
                TitleColor = OxyColors.Green
            };
            PlotModelYZPlane.Axes.Add(yAxis);
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
