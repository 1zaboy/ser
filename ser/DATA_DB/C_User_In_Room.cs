//class for databace table _User_In_Room (Entity Framework)
namespace ser.DATA_DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("_User_In_Room")]
    public partial class C_User_In_Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_User_In_Room()
        {
            message_on_room = new HashSet<message_on_room>();
        }

        [Key]
        public int TableId { get; set; }

        public int Room_U { get; set; }

        public int User_U { get; set; }

        public virtual C_Room C_Room { get; set; }

        public bool Participant { get; set; }

        public bool Admin { get; set; }

        public virtual UserNotType UserNotType { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<message_on_room> message_on_room { get; set; }
    }
}
