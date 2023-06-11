using OxyPlot;
using OxyPlot.Axes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace TrajectoryOfSensorVisualization.ViewModel
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private PlotModel plotModelXYPlane;
        private PlotModel plotModelXZPlane;
        private PlotModel plotModelYZPlane;

        public PlotModel PlotModelXYPlane 
        { 
            get { return plotModelXYPlane; } 
            set { plotModelXYPlane = value; OnPropertyChanged(nameof(PlotModelXYPlane));} 
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

        public MainWindowViewModel()
        {
            PlotModelXYPlane = new PlotModel();
            PlotModelXZPlane = new PlotModel();
            PlotModelYZPlane = new PlotModel();
            SetUpModelXY();
            SetUpModelXZ();
            SetUpModelYZ();
        }

        public void SetUpModelXY()
        {
            var xAxis = new LinearAxis()
            {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Position = AxisPosition.Left,
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
                Position = AxisPosition.Bottom,
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
