namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Project")]
    public partial class Project
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Project()
        {
            DepToProjs = new HashSet<DepToProj>();
            UserToProjs = new HashSet<UserToProj>();
        }

        [Key]
        public string ProjId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [Column(TypeName = "money")]
        public decimal OtherCosts { get; set; }

        [Column(TypeName = "money")]
        public decimal Total { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DepToProj> DepToProjs { get; set; }
        public virtual ICollection<UserToProj> UserToProjs { get; set; }
    }
}