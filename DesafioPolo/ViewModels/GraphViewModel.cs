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
        private string _selectedDataReferencia;
        private ObservableCollection<string> _dataReferencias;
        private ObservableCollection<IndicadorModel> _indicadores;

        public string SelectedIndicador
        {
            get { return _selectedIndicador; }
            set { SetProperty(ref _selectedIndicador, value); }
        }

        public string SelectedDataReferencia
        {
            get { return _selectedDataReferencia; }
            set { SetProperty(ref _selectedDataReferencia, value); }
        }

        public ObservableCollection<string> DataReferencias
        {
            get { return _dataReferencias; }
            set { SetProperty(ref _dataReferencias, value); }
        }

        public ObservableCollection<IndicadorModel> Indicadores
        {
            get { return _indicadores; }
            set { SetProperty(ref _indicadores, value); }
        }


        public GraphViewModel()

        {
            DataReferencias = new ObservableCollection<string>();
            Indicadores = new ObservableCollection<IndicadorModel>();
            LoadData();
        }

        private void LoadData()
        {
            // Carregar dados dos indicadores (simulação)
            Indicadores = new ObservableCollection<IndicadorModel>
            {
               
            };

            var referencias = Indicadores.Select(ind => ind.DataReferencia).Distinct().OrderBy(d => d).ToList();
            foreach (var referencia in referencias)
            {
                DataReferencias.Add(referencia);

            }
        }

        public void UpdateChart()
        {
            if (string.IsNullOrEmpty(SelectedIndicador) || string.IsNullOrEmpty(SelectedDataReferencia))
                return;

            var indicadoresFiltrados = Indicadores
                .Where(ind => ind.Indicador == SelectedIndicador && ind.DataReferencia == SelectedDataReferencia)
                .OrderBy(ind => ind.Data)
                .ToList();

            if (indicadoresFiltrados.Count == 0)
                return;

            var datas = indicadoresFiltrados.Select(ind => ind.Data.ToDateTime(TimeOnly.MinValue)).ToArray();
            var medias = indicadoresFiltrados.Select(ind => ind.Media).ToArray();

            // Atualizar o gráfico
            //var graphView = App.Current.MainWindow.FindName("plot") as WpfPlot;
            //graphView.Plot.Clear();
            //graphView.Plot.Add.Scatter(datas.Select(d => d.ToOADate()).ToArray(), medias);
            //graphView.Plot.Axes.AddBottomAxis();
            //graphView.Refresh();
        }
    }
}
