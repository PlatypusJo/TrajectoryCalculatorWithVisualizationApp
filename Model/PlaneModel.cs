using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace TrajectoryOfSensorVisualization.Model
{
    /// <summary>
    /// Модель представляющая график на плоскости
    /// </summary>
    public class PlaneModel
    {
        #region Constructors
        /// <summary>
        /// Конструктор модели плоскости
        /// </summary>
        /// <param name="colorOfVerticalAxis">Цвет векртикальной оси</param>
        /// <param name="titleOfVerticalAxis">Название векртикальной оси</param>
        /// <param name="colorOfHorizontalAxis">Цвет горизонтальной оси</param>
        /// <param name="titleOfHorizonalAxis">Название горизонтальной оси</param>
        /// <param name="minValue">Минимальное значение из диапазона значений на осях</param>
        /// <param name="maxValue">Максимальное значение из диапазона значений на осях</param>
        public PlaneModel(OxyColor colorOfVerticalAxis, string titleOfVerticalAxis, 
            OxyColor colorOfHorizontalAxis, string titleOfHorizonalAxis,
            double minValue, double maxValue)
        {
            horizontalAxis = new LinearAxis() {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Minimum = minValue,
                Maximum = maxValue,
                Position = AxisPosition.Bottom,
                TicklineColor = colorOfHorizontalAxis,
                AxislineColor = colorOfHorizontalAxis,
                TextColor = colorOfHorizontalAxis,
                TitleColor = colorOfHorizontalAxis,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = titleOfHorizonalAxis
            };
            verticalAxis = new LinearAxis() {
                AxislineThickness = 3,
                MinorTickSize = 4,
                MajorTickSize = 7,
                AxislineStyle = LineStyle.Solid,
                Minimum = minValue,
                Maximum = maxValue,
                Position = AxisPosition.Left,
                TicklineColor = colorOfVerticalAxis,
                AxislineColor = colorOfVerticalAxis,
                TextColor = colorOfVerticalAxis,
                TitleColor = colorOfVerticalAxis,
                MajorGridlineStyle = LineStyle.Solid,
                MinorGridlineStyle = LineStyle.Solid,
                Title = titleOfVerticalAxis
            };
            values = new LineSeries() {
                Color = OxyColor.FromRgb(29, 172, 214),
                StrokeThickness = 1,
                MarkerSize = 2,
                MarkerType = MarkerType.Circle
            };
            plotModel = new PlotModel();
        }
        #endregion

        #region Private Fields
        /// <summary>
        /// Модель графика
        /// </summary>
        private PlotModel plotModel;
        /// <summary>
        /// Горизонтальная ось
        /// </summary>
        private LinearAxis horizontalAxis;
        /// <summary>
        /// Вертикальная ось
        /// </summary>
        private LinearAxis verticalAxis;
        /// <summary>
        /// Данные отображаемые на графике
        /// </summary>
        private LineSeries values;
        #endregion

        #region Private Methods
        /// <summary>
        /// Заполняет модель необходимыми данными и возвращает готовую модель
        /// </summary>
        private PlotModel SetUpPlotModelAndReturn()
        {
            plotModel.Axes.Clear();
            plotModel.Series.Clear();
            
            plotModel.Axes.Add(HorizontalAxis);
            plotModel.Axes.Add(VerticalAxis);
            plotModel.Series.Add(Values);

            return plotModel;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Метод изменения диапазона значений на осях
        /// </summary>
        /// <param name="minValue">Новое минимальное значение из диапазона значений на осях</param>
        /// <param name="maxValue">Новое максимальное значение из диапазона значений на осях</param>
        public void ChangeRangeOfValues(double minValue, double maxValue)
        {
            VerticalAxis.Maximum = maxValue;
            VerticalAxis.Minimum = minValue;

            HorizontalAxis.Maximum = maxValue;
            HorizontalAxis.Minimum = minValue;
        }
        /// <summary>
        /// Добавляет точку в список данных
        /// </summary>
        /// <param name="dataPoint">точка с данными для отображения</param>
        public void AddDataPoint(DataPoint dataPoint) => Values.Points.Add(dataPoint);
        /// <summary>
        /// Удаляет точку из списка данных
        /// </summary>
        /// <param name="dataPoint">точка с данными</param>
        public void RemoveDataPoint(DataPoint dataPoint) => Values.Points.Remove(dataPoint);
        /// <summary>
        /// Очищает список данных
        /// </summary>
        public void RemoveAllDataPoints() => Values.Points.Clear();
        #endregion

        #region Public Properties
        /// <summary>
        /// Заполняет модель и возвращает её
        /// </summary>
        public PlotModel PlotModel => SetUpPlotModelAndReturn();
        /// <summary>
        /// Возвращает горизонтальную ось
        /// </summary>
        public LinearAxis HorizontalAxis => horizontalAxis;
        /// <summary>
        /// Возвращает вертикальную ось
        /// </summary>
        public LinearAxis VerticalAxis => verticalAxis;
        /// <summary>
        /// Возвращает список данных для отображения
        /// </summary>
        public LineSeries Values => values;
        #endregion
    }
}
