using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CityResultDto
    {
        public int Id { get; set; }
        public required string CityName { get; set; }
        public required string PartyName { get; set; }
        public int NumberOfVotes { get; set; }
        public double PartyPercent { get; set; }

    }
}
