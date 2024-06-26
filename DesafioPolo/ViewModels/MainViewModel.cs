using DesafioPolo.Data;
using DesafioPolo.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
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
        public ICommand ExportarCsvCommand { get; set; }
        public ICommand SaveDBCommand { get; set; }
        public ICommand LoadDBCommand { get; set; }

        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(async param => await LoadDataAsync(), param => CanExecuteLoadData());
            Indicadores = new ObservableCollection<IndicadorModel>();
            ExportarCsvCommand = new RelayCommand(param => ExportarCsv());
            IndicadorTipos = new ObservableCollection<string> { "IPCA", "IGP-M", "Selic" };
            SaveDBCommand = new RelayCommand(async param => await SaveToDB());
            LoadDBCommand = new RelayCommand(async param => await LoadDB());
            LoadDataInitiate();
        }

        private void ExportarCsv()
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
            }
        }

        private async Task SaveToDB()
        {
            using (var context = new AppDbContext())
            {
                foreach (var indicador in Indicadores)
                {
                    if (!context.Indicadores.Any(i => i.Indicador == indicador.Indicador && i.Data.ToString() == indicador.Data.ToString() && i.DataReferencia == indicador.DataReferencia))
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
            var url = MontarUrl();
            using (HttpClient client = new HttpClient())
            {
                var response = await client.GetStringAsync(url);

                var responseObject = JsonConvert.DeserializeObject<ResponseObject>(response);

                if (responseObject == null)
                    return;

                if (responseObject.Indicadores.Count == 0)
                    return;

                var indicadores = responseObject.Indicadores;


                foreach (var indicador in indicadores)
                {
                    Indicadores.Add(indicador);
                }

            }
        }

        private string MontarUrl()
        {
            return $"https://olinda.bcb.gov.br/olinda/servico/Expectativas/versao/v1/odata/ExpectativaMercadoMensais?$filter=Indicador eq '{SelectedIndicador}' and Data ge '{DataInicio:yyyy-MM-dd}' and Data le '{DataFim:yyyy-MM-dd}'";
        }

        public class ResponseObject
        {
            [JsonProperty("value")]
            public List<IndicadorModel> Indicadores { get; set; }
        }


    }
}

