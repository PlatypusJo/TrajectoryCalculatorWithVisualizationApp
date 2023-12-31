﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using OxyPlot;

namespace TrajectoryOfSensorVisualization.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ViewModel.PlotsViewModel viewModel;

        public MainWindow()
        {
            viewModel = new ViewModel.PlotsViewModel();
            DataContext = viewModel;
            
            InitializeComponent();

        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Camera3D.Position = new Point3D((Camera3D.Position.X - e.Delta / 360D), (Camera3D.Position.Y - e.Delta / 360D), (Camera3D.Position.Z - e.Delta / 360D));
        }
    }
}
