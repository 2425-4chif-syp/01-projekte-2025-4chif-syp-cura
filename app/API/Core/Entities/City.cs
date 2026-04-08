using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    public class City: EntityObject
    {
        public string CityName { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public int ElegibleVoters { get; set; } //Anzahl der wahlberechtigten BürgerInnen
    }
}
