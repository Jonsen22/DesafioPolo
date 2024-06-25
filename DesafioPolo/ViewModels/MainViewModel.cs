using DesafioPolo.Model;
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

        public MainViewModel()
        {
            LoadDataCommand = new RelayCommand(async param => await LoadDataAsync(), param => CanExecuteLoadData());
            Indicadores = new ObservableCollection<IndicadorModel>();
            ExportarCsvCommand = new RelayCommand(param => ExportarCsv());
            IndicadorTipos = new ObservableCollection<string> { "IPCA", "IGP-M", "Selic" };
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

        private async Task LoadDataAsync()
        {
            //Indicadores.Clear();
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

        public class IndicadorModel
        {
            public string Indicador { get; set; }
            public DateTime Data { get; set; }
            public string DataReferencia { get; set; }
            public double Media { get; set; }
            public double Mediana { get; set; }
            public double DesvioPadrao { get; set; }
            public double Minimo { get; set; }
            public double Maximo { get; set; }
            public int NumeroRespondentes { get; set; }
            public int BaseCalculo { get; set; }
        }
    }
}

