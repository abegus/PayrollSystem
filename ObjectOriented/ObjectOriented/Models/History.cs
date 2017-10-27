namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("History")]
    public partial class History
    {
        [Key]
        public string Hid { get; set; }

        [Display(Name = "StartDate")]
        public DateTime? StartDate { get; set; }

        [Display(Name = "EndDate")]
        public DateTime? EndDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? Pay { get; set; }

        [System.ComponentModel.DefaultValue(1)]
        public int Level { get; set; }

        [StringLength(50)]
        public string Position { get; set; }

        public int Witholdings { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        //my added fileds
        [StringLength(128)]
        public string Mid { get; set; }

        [StringLength(50)]
        public string DepartmentName { get; set; }

        [StringLength(128)]
        public string ManagerName { get; set; }

        public virtual AspNetUsers AspNetUser { get; set; }
    }
}
