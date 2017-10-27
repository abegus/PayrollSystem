namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DepToProj
    {
        [Key]
        [Column(Order = 0)]
        public string DepId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string ProjId { get; set; }

        public string Temp { get; set; }

        public virtual Project Project { get; set; }

        public virtual Department Department { get; set; }
    }
}
