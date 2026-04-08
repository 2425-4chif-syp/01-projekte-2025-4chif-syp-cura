using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace Core.Entities
{
    public partial class Party : EntityObject
    {
        public string PartyName { get; set; } = string.Empty;

    }
}
