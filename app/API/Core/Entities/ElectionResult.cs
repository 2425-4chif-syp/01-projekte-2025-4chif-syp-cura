using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;
using System.Text;

namespace Core.Entities
{
    public class ElectionResult : EntityObject
    {
        [ForeignKey(nameof(Party_Id))]
        public Party? Party{ get; set; }
        public int Party_Id { get; set; }
        [ForeignKey(nameof(City_Id))]
        public City? City { get; set; }
        public int City_Id { get; set; }     

        public int NrOfVotes { get; set; }   //Anzahl der Stimmen einer Party in einem Ort

    }
}
