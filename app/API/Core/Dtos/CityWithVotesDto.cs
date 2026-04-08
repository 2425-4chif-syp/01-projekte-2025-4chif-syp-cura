using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class CityWithVotesDto
    {
        public int Id { get; set; }
        public string CityName { get; set; } = string.Empty;
        public string CityCode { get; set; } = string.Empty;
        public int ElegibleVoters { get; set; }
        public int TotalVotes { get; set; }
    }
}
