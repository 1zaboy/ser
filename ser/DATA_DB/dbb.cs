//main class(Entity Framework)
namespace ser.DATA_DB
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class dbb : DbContext
    {
        public dbb()
            : base("name=dbb")
        {
        }

        public virtual DbSet<C_Room> C_Room { get; set; }
        public virtual DbSet<C_User_In_Room> C_User_In_Room { get; set; }
        public virtual DbSet<message_on_room> message_on_room { get; set; }
        public virtual DbSet<UserNotType> UserNotType { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<C_Room>()
                .HasMany(e => e.C_User_In_Room)
                .WithRequired(e => e.C_Room)
                .HasForeignKey(e => e.Room_U)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<C_User_In_Room>()
                .HasMany(e => e.message_on_room)
                .WithRequired(e => e.C_User_In_Room)
                .HasForeignKey(e => e.Room_U)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<UserNotType>()
                .HasMany(e => e.C_User_In_Room)
                .WithRequired(e => e.UserNotType)
                .HasForeignKey(e => e.User_U)
                .WillCascadeOnDelete(false);
        }
    }
}
