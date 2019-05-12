namespace ser.DATA_DB
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class message_on_room
    {
        [Key]
        public int TableId { get; set; }

        public string text_mess { get; set; }

        public DateTime? time_mess { get; set; }

        public int Room_U { get; set; }
        

        public virtual C_User_In_Room C_User_In_Room { get; set; }
    }
}
