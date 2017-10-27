namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Mng
    {
        [Key]
        [Column(Order = 0)]
        public string Mid { get; set; }

        [Key]
        [Column(Order = 1)]
        public string EmpId { get; set; }

        [StringLength(50)]
        public string Temp { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }

        public virtual AspNetUsers AspNetUser1 { get; set; }
    }
}
