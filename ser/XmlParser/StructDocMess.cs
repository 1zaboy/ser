using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ser.XmlParser
{
    //struct(class) for main type packets data on network
    class StructDocMess
    {
        public string index_command;

        public string index_user;
        public string name_user;
        public string password_user;

        public int index_room;
        public string name_room;
        public int count_users_in_room;

        public string text_message;
        public string time_message;
    }
}
