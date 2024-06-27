using ScottPlot;
using ScottPlot.WPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using DesafioPolo.ViewModels;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DesafioPolo.View
{
    /// <summary>
    /// Lógica interna para GraphView.xaml
    /// </summary>
    public partial class GraphView : UserControl
    {
        public GraphView()
        {
            InitializeComponent();
            DataContext = new GraphViewModel();
        }

        private void OnIndicadorChecked(object sender, RoutedEventArgs e)
        {
            var viewModel = DataContext as GraphViewModel;
            if (viewModel != null)
            {

                viewModel.SelectedIndicador = (sender as RadioButton)?.Content.ToString();

                viewModel.UpdateChart();
            }
        }

        private void OnDataSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var viewModel = DataContext as GraphViewModel;
            if (viewModel != null)
            {
                viewModel.SelectedDataReferencia = (sender as ComboBox)?.SelectedItem?.ToString();

                viewModel.UpdateChart();
            }
        }
    }
}
