namespace ObjectOriented.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class PayrollDbConnection : DbContext
    {
        public PayrollDbConnection()
            : base("name=PayrollDbConnection")
        {
        }

        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Mng> Mngs { get; set; }
        public virtual DbSet<Paystub> Paystubs { get; set; }
        public virtual DbSet<Work> Works { get; set; }

        //myAdditions
        public virtual DbSet<History> Historys { get; set; }//was just History...?
        public virtual DbSet<DepToProj> DepToProjs { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<UserToProj> UserToProjs { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRole>()
                .HasMany(e => e.AspNetUsers)
                .WithMany(e => e.AspNetRoles)
                .Map(m => m.ToTable("AspNetUserRoles").MapLeftKey("RoleId").MapRightKey("UserId"));

            modelBuilder.Entity<AspNetUsers>()
                .Property(e => e.Salary)
                .HasPrecision(19, 4);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserClaims)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.AspNetUserLogins)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Mngs)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.EmpId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Mngs1)
                .WithRequired(e => e.AspNetUser1)
                .HasForeignKey(e => e.Mid)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Paystubs)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId);

            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Works)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);

            //MyAddition
            modelBuilder.Entity<AspNetUsers>()
                .HasMany(e => e.Historys)
                .WithRequired(e => e.AspNetUser)
                .HasForeignKey(e => e.UserId)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<AspNetUsers>()
               .HasMany(e => e.UserToProjs)
               .WithRequired(e => e.AspNetUser)
               .HasForeignKey(e => e.UserId)
               .WillCascadeOnDelete(false);
            //end addition

            modelBuilder.Entity<Department>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<Department>()
                .HasMany(e => e.Works)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);
            //added
            modelBuilder.Entity<Department>()
                .HasMany(e => e.DepToProjs)
                .WithRequired(e => e.Department)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Mng>()
                .Property(e => e.Temp)
                .IsUnicode(false);
            //added
            modelBuilder.Entity<DepToProj>()
                .Property(e => e.Temp)
                .IsUnicode(false);
            modelBuilder.Entity<UserToProj>()
                .Property(e => e.Temp)
                .IsUnicode(false);
            modelBuilder.Entity<UserToProj>()
                .Property(e => e.Percentage)
                .HasPrecision(19, 4);

            modelBuilder.Entity<Paystub>()
                .Property(e => e.Pay)
                .HasPrecision(19, 4);

            //added
            modelBuilder.Entity<Project>()
                .HasMany(e => e.DepToProjs)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Project>()
                .HasMany(e => e.UserToProjs)
                .WithRequired(e => e.Project)
                .WillCascadeOnDelete(false);
            modelBuilder.Entity<Project>()
               .Property(e => e.OtherCosts)
               .HasPrecision(19, 4);
            modelBuilder.Entity<Project>()
               .Property(e => e.Total)
               .HasPrecision(19, 4);
        }
    }
}
