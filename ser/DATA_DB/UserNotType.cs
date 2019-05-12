namespace ser.DATA_DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("UserNotType")]
    public partial class UserNotType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UserNotType()
        {
            C_User_In_Room = new HashSet<C_User_In_Room>();
        }

        public int Id { get; set; }

        [Required]
        public string NameUser { get; set; }

        [Required]
        public string Password { get; set; }
       

        public int? index_in_list { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<C_User_In_Room> C_User_In_Room { get; set; }
    }
}
