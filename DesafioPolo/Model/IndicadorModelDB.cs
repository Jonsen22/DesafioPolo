using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DesafioPolo.Model
{
    public class IndicadorModelDB
    {
        [Key]
        public string Id { get; set; }
        public string? Indicador { get; set; }
        public DateOnly Data { get; set; }
        public string? DataReferencia { get; set; }
        public double Media { get; set; }
        public double Mediana { get; set; }
        public double DesvioPadrao { get; set; }
        public double Minimo { get; set; }
        public double Maximo { get; set; }
        public int? NumeroRespondentes { get; set; }
        public int BaseCalculo { get; set; }
    }
}
