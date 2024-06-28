using DesafioPolo.Model;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;

namespace DesafioPolo.ViewModels
{
    public class GraphViewModel : ViewModelBase
    {
        private string _selectedIndicador;
        private string _selectedData;
        private string _selectedValor;
        private ObservableCollection<string> _datas;
        private ObservableCollection<IndicadorModel> _indicadores;

        public ObservableCollection<string> ValoresDisponiveis { get; }

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

        public string SelectedValor
        {
            get { return _selectedValor; }
            set
            {
                if (SetProperty(ref _selectedValor, value))
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

        private PlotModel _plotModel;
        public PlotModel PlotModel
        {
            get { return _plotModel; }
            set { SetProperty(ref _plotModel, value); }
        }

        public GraphViewModel(ObservableCollection<IndicadorModel> indicadores)
        {
            _indicadores = indicadores;
            PlotModel = new PlotModel { Title = "Gráfico de Indicadores" };
            PlotModel.Axes.Add(new DateTimeAxis { Position = AxisPosition.Bottom, MajorStep=80, StringFormat = "MM-yyyy" });
            PlotModel.Axes.Add(new LinearAxis { Position = AxisPosition.Left, Title = "Valor" });

            ValoresDisponiveis = new ObservableCollection<string>
        {
            "Média",
            "Mediana",
            "Máximo",
            "Mínimo",
            "Desvio Padrão"
        };

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
                .Select(ind => ind.Data.ToString("yyyy-MM-dd"))
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
                .OrderBy(ind => DateTime.ParseExact(ind.DataReferencia, "MM/yyyy", CultureInfo.InvariantCulture))
                .ToList();

            if (indicadoresFiltrados.Count == 0)
                return;

            Func<IndicadorModel, double> selector;
            switch (SelectedValor)
            {
                case "Média":
                    selector = ind => ind.Media;
                    break;
                case "Mediana":
                    selector = ind => ind.Mediana;
                    break;
                case "Máximo":
                    selector = ind => ind.Maximo;
                    break;
                case "Mínimo":
                    selector = ind => ind.Minimo;
                    break;
                case "Desvio Padrão":
                    selector = ind => ind.DesvioPadrao;
                    break;
                default:
                    selector = ind => ind.Media; 
                    break;
            }

            var datasReferencia = indicadoresFiltrados
                .Select(ind => DateTime.ParseExact(ind.DataReferencia, "MM/yyyy", CultureInfo.InvariantCulture))
                .ToArray();
            var valoresSelecionados = indicadoresFiltrados.Select(selector).ToArray();

            var lineSeries = new LineSeries { Title = SelectedIndicador };
            for (int i = 0; i < datasReferencia.Length; i++)
            {
                lineSeries.Points.Add(new DataPoint(DateTimeAxis.ToDouble(datasReferencia[i]), valoresSelecionados[i]));
            }

            PlotModel.Series.Clear();
            PlotModel.Series.Add(lineSeries);
            PlotModel.InvalidatePlot(true);
        }
    }
}
