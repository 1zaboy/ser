namespace ser.DATA_DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("_Room")]
    public partial class C_Room
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public C_Room()
        {
            C_User_In_Room = new HashSet<C_User_In_Room>();
        }

        [Key]
        public int TableId { get; set; }

        [Required]
        public string NameRoom { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_User_In_Room> C_User_In_Room { get; set; }
    }
}
