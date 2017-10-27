namespace ObjectOriented.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Department")]
    public partial class Department
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Department()
        {
            Works = new HashSet<Work>();
            DepToProjs = new HashSet<DepToProj>();
        }

        [Key]
        public string DepId { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Work> Works { get; set; }
        public virtual ICollection<DepToProj> DepToProjs { get; set; }
    }
}
