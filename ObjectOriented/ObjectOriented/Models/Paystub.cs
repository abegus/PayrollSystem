namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Paystub")]
    public partial class Paystub
    {
        [Key]
        public string StubId { get; set; }

        [Display(Name = "Stub Month")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Pay { get; set; }

        [Column(TypeName = "money")]
        public decimal? NetPay { get; set; }

        [Column(TypeName = "money")]
        public decimal? Tax { get; set; }

        public int Witholdings { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        //my added fileds
        [StringLength(128)]
        public string Mid { get; set; }

        [StringLength(50)]
        public string DepartmentName { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }
    }
}
