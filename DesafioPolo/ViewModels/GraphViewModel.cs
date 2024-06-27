using DesafioPolo.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using ScottPlot.WPF;
using System.Text;
using System.Threading.Tasks;

namespace DesafioPolo.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        private string _selectedIndicador;
        private string _selectedData;
        private ObservableCollection<string> _datas;
        private ObservableCollection<IndicadorModel> _indicadores;

        public string SelectedIndicador
        {
            get { return _selectedIndicador; }
            set
            {
                if (SetProperty(ref _selectedIndicador, value))
                {
                    UpdateData();
                    OnPropertyChanged(nameof(IsDataEnabled));
                }
            }
        }

        public string SelectedData
        {
            get { return _selectedData; }
            set
            {
                if (SetProperty(ref _selectedData, value))
                {
                    UpdateChart();
                }
            }
        }

        public ObservableCollection<string> Datas
        {
            get { return _datas; }
            set { SetProperty(ref _datas, value); }
        }

        public bool IsDataEnabled
        {
            get { return !string.IsNullOrEmpty(SelectedIndicador); }
        }

        public GraphViewModel(ObservableCollection<IndicadorModel> indicadores)
        {
            _indicadores = indicadores;
        }

        public void UpdateData()
        {
            if (string.IsNullOrEmpty(SelectedIndicador))
            {
                Datas = new ObservableCollection<string>(); // Limpa a lista
                return;
            }

            var datas = _indicadores
                .Where(ind => ind.Indicador == SelectedIndicador)
                .Select(ind => ind.Data.ToString())
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            Datas = new ObservableCollection<string>(datas);
        }


        public void UpdateChart()
        {
            if (string.IsNullOrEmpty(SelectedIndicador) || string.IsNullOrEmpty(SelectedData))
                return;

            var selectedDate = DateOnly.FromDateTime(DateTime.Parse(SelectedData));

            var indicadoresFiltrados = _indicadores
                .Where(ind => ind.Indicador == SelectedIndicador && ind.Data == selectedDate)
                .OrderBy(ind => ind.Data)
                .ToList();

            if (indicadoresFiltrados.Count == 0)
                return;

            var datasReferencia = indicadoresFiltrados.Select(ind => DateTime.Parse(ind.DataReferencia)).ToArray();
            var medias = indicadoresFiltrados.Select(ind => ind.Media).ToArray();

            var graphView = App.Current.MainWindow.FindName("plot") as WpfPlot;
            if (graphView != null)
            {
                graphView.Plot.Clear();
                graphView.Plot.Add.Scatter(datasReferencia.Select(d => d.ToOADate()).ToArray(), medias);
                //graphView.Plot.XAxis.DateTimeFormat(true);
                graphView.Refresh();
            }
        }
    }
}
