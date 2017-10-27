namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Work
    {
        [Key]
        [Column(Order = 0)]
        public string DepId { get; set; }

        [Key]
        [Column(Order = 1)]
        public string UserId { get; set; }

        public int? IsManager { get; set; }

        public int? IsEmployee { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }

        public virtual Department Department { get; set; }
    }
}
