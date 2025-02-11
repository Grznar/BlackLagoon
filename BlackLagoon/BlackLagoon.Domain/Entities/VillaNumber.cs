using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackLagoon.Domain.Entities
{
    public class VillaNumber
    {
        
        [Key,DatabaseGenerated(DatabaseGeneratedOption.None)]
        [DisplayName("Villa Number")]
        public int Villa_Number { get; set; }

        [ValidateNever]
        public Villa Villa { get; set; }
        [ForeignKey("Villa")]
        public int VillaId { get; set; }
        public string? SpecialDetails { get; set; }

    }
}
