using DesafioPolo.Data;
using DesafioPolo.Model;
using DesafioPolo.View;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace DesafioPolo.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private string _selectedIndicador;
        private DateTime? _dataInicio;
        private DateTime? _dataFim;
        private ObservableCollection<IndicadorModel> _indicadores;
        private ObservableCollection<string> _indicadorTipos;

        public string SelectedIndicador
        {
            get { return _selectedIndicador; }
            set
            {
                if (SetProperty(ref _selectedIndicador, value))
                {
                    ((RelayCommand)LoadDataCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime? DataInicio
        {
            get { return _dataInicio; }
            set
            {
                if (SetProperty(ref _dataInicio, value))
                {
                    ((RelayCommand)LoadDataCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public DateTime? DataFim
        {
            get { return _dataFim; }
            set
            {
                if (SetProperty(ref _dataFim, value))
                {
                    ((RelayCommand)LoadDataCommand).RaiseCanExecuteChanged();
                }
            }
        }

        public ObservableCollection<IndicadorModel> Indicadores
        {
            get { return _indicadores; }
            set { SetProperty(ref _indicadores, value); }
        }

        public ObservableCollection<string> IndicadorTipos
        {
            get { return _indicadorTipos; }
            set { SetProperty(ref _indicadorTipos, value); }
        }

        public ICommand LoadDataCommand { get; }
        public ICommand ExportCsvCommand { get; set; }
        public ICommand ClearGridCommand { get; set; }
        public ICommand SaveDBCommand { get; set; }
        public ICommand LoadDBCommand { get; set; }
        public ICommand OpenChartCommand { get; }

        public MainViewModel()

        {
            LoadDataCommand = new RelayCommand(async param => await LoadDataAsync(), param => CanExecuteLoadData());
            Indicadores = new ObservableCollection<IndicadorModel>();
            ExportCsvCommand = new RelayCommand(param => ExportCsv());
            ClearGridCommand = new RelayCommand(param => ClearGrid());
            IndicadorTipos = new ObservableCollection<string> { "IPCA", "IGP-M", "Selic" };
            SaveDBCommand = new RelayCommand(async param => await SaveToDB());
            LoadDBCommand = new RelayCommand(async param => await LoadDB());
            OpenChartCommand = new RelayCommand(param => OpenChart());
            LoadDataInitiate();
        }

        private void ExportCsv()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.FileName = $"{SelectedIndicador}-{DataInicio:dd-MM-yyyy}-{DataFim:dd-MM-yyyy}.csv";
            saveFileDialog.Filter = "CSV file (*.csv)|*.csv";
            saveFileDialog.DefaultExt = ".csv";
            saveFileDialog.AddExtension = true;

            if (saveFileDialog.ShowDialog() == true)
            {
                StringBuilder csv = new StringBuilder();
                csv.AppendLine("Indicador;Data;DataReferencia;Media;Mediana;DesvioPadrao;Minimo;Maximo;NumeroRespondentes;BaseCalculo");

                foreach (var indicador in Indicadores)
                {
                    csv.AppendLine($"{indicador.Indicador};{indicador.Data:dd/MM/yyyy};{indicador.DataReferencia};{indicador.Media};{indicador.Mediana};{indicador.DesvioPadrao};{indicador.Minimo};{indicador.Maximo};{indicador.NumeroRespondentes};{indicador.BaseCalculo}");
                }

                File.WriteAllText(saveFileDialog.FileName, csv.ToString());
            }
        }

        private bool CanExecuteLoadData()
        {
            return !string.IsNullOrWhiteSpace(SelectedIndicador) && DataInicio.HasValue && DataFim.HasValue;
        }

        private void ClearGrid()
        {
            Indicadores.Clear();
        }

        private async void LoadDataInitiate()
        {

            using (var context = new AppDbContext())
            {
                var indicadoresDb = await context.Indicadores.ToListAsync();

                foreach (var indicadorDb in indicadoresDb)
                {

                    var indicador = new IndicadorModel
                    {
                        Indicador = indicadorDb.Indicador,
                        Data = indicadorDb.Data,
                        DataReferencia = indicadorDb.DataReferencia,
                        Media = indicadorDb.Media,
                        Mediana = indicadorDb.Mediana,
                        DesvioPadrao = indicadorDb.DesvioPadrao,
                        Minimo = indicadorDb.Minimo,
                        Maximo = indicadorDb.Maximo,
                        NumeroRespondentes = (int)indicadorDb.NumeroRespondentes,
                        BaseCalculo = indicadorDb.BaseCalculo
                    };


                    Indicadores.Add(indicador);
                }

                var lastDateDb = indicadoresDb.OrderByDescending(ind => ind.Data).FirstOrDefault();

                if (lastDateDb == null)
                    return;


            }
        }

        private async Task SaveToDB()
        {
            using (var context = new AppDbContext())
            {
                foreach (var indicador in Indicadores)
                {
                    // Verifica se já existe um registro com os mesmos valores em todos os campos
                    var existingIndicador = context.Indicadores.FirstOrDefault(i =>
                        i.Indicador == indicador.Indicador &&
                        i.Data == indicador.Data &&
                        i.DataReferencia == indicador.DataReferencia &&
                        i.Media == indicador.Media &&
                        i.Mediana == indicador.Mediana &&
                        i.DesvioPadrao == indicador.DesvioPadrao &&
                        i.Minimo == indicador.Minimo &&
                        i.Maximo == indicador.Maximo &&
                        i.NumeroRespondentes == indicador.NumeroRespondentes &&
                        i.BaseCalculo == indicador.BaseCalculo);

                    if (existingIndicador == null)
                    {
                        var indicadorDb = new IndicadorModelDB
                        {
                            Id = Guid.NewGuid().ToString(),
                            Indicador = indicador.Indicador,
                            Data = indicador.Data,
                            DataReferencia = indicador.DataReferencia,
                            Media = indicador.Media,
                            Mediana = indicador.Mediana,
                            DesvioPadrao = indicador.DesvioPadrao,
                            Minimo = indicador.Minimo,
                            Maximo = indicador.Maximo,
                            NumeroRespondentes = indicador.NumeroRespondentes,
                            BaseCalculo = indicador.BaseCalculo
                        };

                        context.Indicadores.Add(indicadorDb);
                    }
                }

                await context.SaveChangesAsync();
            }
        }

        private async Task LoadDB()
        {
            using (var context = new AppDbContext())
            {
                var indicadores = await context.Indicadores.ToListAsync();
                Indicadores.Clear();
                foreach (var indicador in indicadores)
                {

                    var indicadorModel = new IndicadorModel
                    {
                        Indicador = indicador.Indicador,
                        Data = indicador.Data,
                        DataReferencia = indicador.DataReferencia,
                        Media = indicador.Media,
                        Mediana = indicador.Mediana,
                        DesvioPadrao = indicador.DesvioPadrao,
                        Minimo = indicador.Minimo,
                        Maximo = indicador.Maximo,
                        NumeroRespondentes = (int)indicador.NumeroRespondentes,
                        BaseCalculo = indicador.BaseCalculo
                    };


                    Indicadores.Add(indicadorModel);
                }
            }
        }

        private async Task LoadDataAsync()
        {
            Indicadores.Clear();
            List<IndicadorModel> indicadoresDb = await LoadDataFromDbAsync();

            var missingDateRanges = GetMissingDateRanges(indicadoresDb);

            List<IndicadorModel> indicadoresApi = new List<IndicadorModel>();

            if (missingDateRanges.Count > 0)
            {
                foreach (var range in missingDateRanges)
                {
                    var apiData = await LoadDataFromApiAsync(range.Item1, range.Item2);
                    if (apiData != null && apiData.Count > 0)
                    {
                        indicadoresApi.AddRange(apiData);
                    }
                }
            }

            var allIndicadores = indicadoresDb.Concat(indicadoresApi).ToList();
            foreach (var indicador in allIndicadores)
            {
                Indicadores.Add(indicador);
            }
        }

        private async Task<List<IndicadorModel>> LoadDataFromDbAsync()
        {
            using (var context = new AppDbContext())
            {


                var indicadoresDb = await context.Indicadores
                    .Where(i => i.Indicador == SelectedIndicador && i.Data >= DateOnly.FromDateTime(DataInicio.Value) && i.Data <= DateOnly.FromDateTime(DataFim.Value))
                    .ToListAsync();




                var indicadores = indicadoresDb.Select(indicadorDb => new IndicadorModel
                {
                    Indicador = indicadorDb.Indicador,
                    Data = indicadorDb.Data,
                    DataReferencia = indicadorDb.DataReferencia,
                    Media = indicadorDb.Media,
                    Mediana = indicadorDb.Mediana,
                    DesvioPadrao = indicadorDb.DesvioPadrao,
                    Minimo = indicadorDb.Minimo,
                    Maximo = indicadorDb.Maximo,
                    NumeroRespondentes = (int)indicadorDb.NumeroRespondentes,
                    BaseCalculo = indicadorDb.BaseCalculo
                }).ToList();


                return indicadores;
            }
        }

        private List<Tuple<DateOnly, DateOnly>> GetMissingDateRanges(List<IndicadorModel> indicadoresDb)
        {
            List<Tuple<DateOnly, DateOnly>> missingDateRanges = new List<Tuple<DateOnly, DateOnly>>();


            var existingDates = indicadoresDb.Select(i => i.Data).Distinct().ToList();


            DateOnly startDate = DateOnly.FromDateTime(DataInicio.Value);


            DateOnly endDate = DateOnly.FromDateTime(DataFim.Value);


            if (existingDates == null || existingDates.Count == 0)
            {
                missingDateRanges.Add(Tuple.Create(startDate, endDate));
                return missingDateRanges;
            }


            bool startDateExists = existingDates.Any(d => d <= startDate);
            bool endDateExists = existingDates.Any(d => d >= endDate);

            if (!startDateExists)
            {
                missingDateRanges.Add(Tuple.Create(startDate, existingDates.Min()));
            }

            if (!endDateExists)
            {
                missingDateRanges.Add(Tuple.Create(existingDates.Max(), endDate));
            }

            return missingDateRanges;
        }

        private void OpenChart()
        {
            var window = new Window
            {
                Title = "Visualização de Gráfico",
                Content = new GraphView(Indicadores),
                Width = 800,
                Height = 600,
                WindowStartupLocation = WindowStartupLocation.CenterScreen
            };

            window.ShowDialog();
        }

        private async Task<List<IndicadorModel>> LoadDataFromApiAsync(DateOnly startDate, DateOnly endDate)
        {
            List<IndicadorModel> indicadores = new List<IndicadorModel>();

            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(MountUrl(startDate, endDate));
                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);

                if (responseObject?.Indicadores != null)
                {
                    indicadores.AddRange(responseObject.Indicadores);
                }
            }

            return indicadores;
        }

     

        private string MountUrl(DateOnly startDate, DateOnly endDate)
        {
            return $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?$filter=Indicador eq '{SelectedIndicador}' and Data ge '{startDate:yyyy-MM-dd}' and Data le '{endDate:yyyy-MM-dd}'";
        }

        public class ResponseObject
        {
            [JsonProperty("value")]
            public List<IndicadorModel> Indicadores { get; set; }
        }
    }
}

