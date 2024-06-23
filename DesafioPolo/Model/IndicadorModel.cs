using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioPolo.Model
{
    public class IndicadorModel
    {
        public string Indicador { get; set; }
        public DateTime Data { get; set; }
        public string DataReferencia { get; set; }
        public decimal Media { get; set; }
        public decimal Mediana { get; set; }
        public decimal DesvioPadrao { get; set; }
        public decimal Minimo { get; set; }
        public decimal Maximo { get; set; }
        public int? NumeroRespondentes { get; set; }
        public int BaseCalculo { get; set; }
    }
}
