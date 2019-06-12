using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ser.DATA_DB;

namespace ser.XmlParser
{
    class XmlParser
    {
        public static string GetIndexCommand(string str, string inname = "index_command")
        {
            var t = XDocument.Parse(str);
            return t.Descendants(inname).First().Value;
        }
        public static StructDocMess string_to_struct(string str)
        {
            try
            {
                var XD = XDocument.Parse(str);
                StructDocMess docMess = new StructDocMess();
                docMess.index_command = XD.Descendants("index_command").First().Value;

                docMess.index_user = XD.Descendants("index_user").First().Value;
                docMess.name_user = XD.Descendants("name_user").First().Value;
                docMess.password_user = XD.Descendants("password_user").First().Value;

                docMess.index_room = Convert.ToInt32(XD.Descendants("index_room").First().Value);
                docMess.name_room = XD.Descendants("name_room").First().Value;
                docMess.count_users_in_room = Convert.ToInt32(XD.Descendants("count_users_in_room").First().Value);

                docMess.text_message = XD.Descendants("text_message").First().Value;
                docMess.time_message = XD.Descendants("time_message").First().Value;

                return docMess;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public static string struct_to_string(StructDocMess str)
        {
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem;
                iphoneCompanyElem = new XElement("index_command", str.index_command);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("index_user", str.index_user);
                element.Add(iphoneCompanyElem);

                XElement iphoneCompanyElem1 = new XElement("name_user", str.name_user);
                element.Add(iphoneCompanyElem1);

                iphoneCompanyElem = new XElement("password_user", str.password_user);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("index_room", str.index_room);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("name_room", str.name_room);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("count_users_in_room", str.count_users_in_room);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("text_message", str.text_message);
                element.Add(iphoneCompanyElem);

                iphoneCompanyElem = new XElement("time_message", str.time_message);
                element.Add(iphoneCompanyElem);

                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string struct_all_room_not_mess_string(StructDocMess mess)
        {
            dbb dbb = new dbb();
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem = new XElement("index_command", mess.index_command);
                element.Add(iphoneCompanyElem);
                Console.WriteLine(mess.index_user);

                var userAllRooms = dbb.C_User_In_Room.Where(t => t.UserNotType.Id.ToString() == mess.index_user && t.Participant).ToList();

                foreach (var uir in userAllRooms)
                {
                    XElement elementRoom = new XElement("Room");
                    iphoneCompanyElem = new XElement("index_room", uir.C_Room.TableId);
                    elementRoom.Add(iphoneCompanyElem);
                    iphoneCompanyElem = new XElement("name_room", uir.C_Room.NameRoom);
                    elementRoom.Add(iphoneCompanyElem);

                    var messallInRoom1 = dbb.message_on_room.Where(t => t.C_User_In_Room.C_Room.TableId == uir.C_Room.TableId).ToList();
                    if (messallInRoom1.Any())
                    {

                        var messallInRoom = messallInRoom1.Last();
                        XElement elementRoomMess = new XElement("Mess_in_room");

                        XElement elementOneMess = new XElement("Mess");

                        iphoneCompanyElem = new XElement("mess_str", messallInRoom.text_mess);
                        elementOneMess.Add(iphoneCompanyElem);
                        iphoneCompanyElem = new XElement("mess_time",
                            messallInRoom.time_mess.Value.ToString("yyyy.MM.dd-HH.mm.ss"));
                        elementOneMess.Add(iphoneCompanyElem);

                        XElement elementMessUser = new XElement("User_in_mess");
                        iphoneCompanyElem = new XElement("index_user", messallInRoom.C_User_In_Room.UserNotType.Id);
                        elementMessUser.Add(iphoneCompanyElem);
                        iphoneCompanyElem =
                            new XElement("name_user", messallInRoom.C_User_In_Room.UserNotType.NameUser);
                        elementMessUser.Add(iphoneCompanyElem);
                        iphoneCompanyElem = new XElement("img_user", "");
                        elementMessUser.Add(iphoneCompanyElem);
                        elementOneMess.Add(elementMessUser);
                        elementRoomMess.Add(elementOneMess);
                        elementRoom.Add(elementRoomMess);
                    }
                    var allUserInRooms = dbb.C_User_In_Room.Where(t => t.C_Room.TableId == uir.C_Room.TableId).ToList();
                    XElement elementAllUsers = new XElement("count_user", allUserInRooms.Count)
;
                    elementRoom.Add(elementAllUsers);
                    element.Add(elementRoom);
                }
                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string struct_all_mess_in_room_to_string(StructDocMess mess)
        {
            dbb dbb = new dbb();
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem = new XElement("index_command", mess.index_command);
                element.Add(iphoneCompanyElem);


                var userAllRooms = dbb.C_User_In_Room.Where(t => t.C_Room.TableId == mess.index_room).ToList();
                if (userAllRooms.Count >= 0)
                {
                    foreach (var uir in userAllRooms.Take(1))
                    {
                        XElement elementRoom = new XElement("Room");
                        iphoneCompanyElem = new XElement("index_room", uir.C_Room.TableId);
                        elementRoom.Add(iphoneCompanyElem);

                        var messallInRoom = dbb.message_on_room
                            .Where(t => t.C_User_In_Room.C_Room.TableId == uir.C_Room.TableId).ToList();
                        XElement elementRoomMess = new XElement("Mess_in_room");
                        foreach (var messageLoop in messallInRoom.Skip(messallInRoom.Count - 20))
                        {
                            XElement elementOneMess = new XElement("Mess");

                            iphoneCompanyElem = new XElement("mess_str", messageLoop.text_mess);
                            elementOneMess.Add(iphoneCompanyElem);
                            iphoneCompanyElem = new XElement("mess_time",
                                messageLoop.time_mess.Value.ToString("yyyy.MM.dd-HH.mm.ss"));
                            elementOneMess.Add(iphoneCompanyElem);

                            XElement elementMessUser = new XElement("User_in_mess");
                            iphoneCompanyElem = new XElement("index_user", messageLoop.C_User_In_Room.UserNotType.Id);
                            elementMessUser.Add(iphoneCompanyElem);
                            iphoneCompanyElem = new XElement("name_user",
                                messageLoop.C_User_In_Room.UserNotType.NameUser);
                            elementMessUser.Add(iphoneCompanyElem);
                            iphoneCompanyElem = new XElement("img_user", "");
                            elementMessUser.Add(iphoneCompanyElem);
                            elementOneMess.Add(elementMessUser);
                            elementRoomMess.Add(elementOneMess);
                        }

                        elementRoom.Add(elementRoomMess);
                        element.Add(elementRoom);
                    }
                }
                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public static string struct_one_room_to_string(StructDocMess mess)
        {
            dbb dbb = new dbb();
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem = new XElement("index_command", mess.index_command);
                element.Add(iphoneCompanyElem);


                var userAllRooms = dbb.C_User_In_Room.Where(t => t.C_Room.TableId == mess.index_room && t.Participant).ToList();

                XElement elementRoom = new XElement("Room");
                iphoneCompanyElem = new XElement("index_room", userAllRooms[0].C_Room.TableId);
                elementRoom.Add(iphoneCompanyElem);
                iphoneCompanyElem = new XElement("name_room", userAllRooms[0].C_Room.NameRoom);
                elementRoom.Add(iphoneCompanyElem);

                int i = userAllRooms[0].C_Room.TableId;
                var messallInRoom1 = dbb.message_on_room.Where(t => t.C_User_In_Room.C_Room.TableId == i).ToList();
                
                if (messallInRoom1.Any())
                {

                    var messallInRoom = messallInRoom1.Last();
                    XElement elementRoomMess = new XElement("Mess_in_room");

                    XElement elementOneMess = new XElement("Mess");

                    iphoneCompanyElem = new XElement("mess_str", messallInRoom.text_mess);
                    elementOneMess.Add(iphoneCompanyElem);
                    iphoneCompanyElem = new XElement("mess_time",
                        messallInRoom.time_mess.Value.ToString("yyyy.MM.dd-HH.mm.ss"));
                    elementOneMess.Add(iphoneCompanyElem);

                    XElement elementMessUser = new XElement("User_in_mess");
                    iphoneCompanyElem = new XElement("index_user", messallInRoom.C_User_In_Room.UserNotType.Id);
                    elementMessUser.Add(iphoneCompanyElem);
                    iphoneCompanyElem =
                        new XElement("name_user", messallInRoom.C_User_In_Room.UserNotType.NameUser);
                    elementMessUser.Add(iphoneCompanyElem);
                    iphoneCompanyElem = new XElement("img_user", "");
                    elementMessUser.Add(iphoneCompanyElem);
                    elementOneMess.Add(elementMessUser);
                    elementRoomMess.Add(elementOneMess);
                    elementRoom.Add(elementRoomMess);
                }
                elementRoom.Add(new XElement("count_user", userAllRooms.Count));

                element.Add(elementRoom);
                
                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static string struct_one_room_list_user_to_string(StructDocMess mess)
        {
            dbb dbb = new dbb();
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem = new XElement("index_command", mess.index_command);
                element.Add(iphoneCompanyElem);


                var userAllRooms = dbb.C_User_In_Room.Where(t => t.C_Room.TableId == mess.index_room && t.Participant).ToList();
                var Im = userAllRooms.Find(t => t.UserNotType.Id.ToString() == mess.index_user);
                userAllRooms.Remove(Im);
                userAllRooms.Insert(0, Im);

                XElement elementRoom = new XElement("Room");
                iphoneCompanyElem = new XElement("index_room", userAllRooms[0].C_Room.TableId);
                elementRoom.Add(iphoneCompanyElem);
                iphoneCompanyElem = new XElement("name_room", userAllRooms[0].C_Room.NameRoom);
                elementRoom.Add(iphoneCompanyElem);

                XElement elementAllUsers = new XElement("Users");
                foreach (var us in userAllRooms)
                {
                    XElement elementUsers = new XElement("User");
                    iphoneCompanyElem = new XElement("index_user", us.UserNotType.Id);
                    elementUsers.Add(iphoneCompanyElem);
                    iphoneCompanyElem = new XElement("name_user", us.UserNotType.NameUser);
                    elementUsers.Add(iphoneCompanyElem);
                    iphoneCompanyElem = new XElement("img_user", "");
                    elementUsers.Add(iphoneCompanyElem);

                    int v2 = -1;
                    if (us.UserNotType.index_in_list.HasValue)
                        v2 = us.UserNotType.index_in_list.Value;
                    if (v2 != -1)
                    {
                        iphoneCompanyElem = new XElement("in_net", "True");
                    }
                    else
                    {
                        iphoneCompanyElem = new XElement("in_net", "Flase");
                    }
                    elementUsers.Add(iphoneCompanyElem);
                    elementAllUsers.Add(elementUsers);
                }
                elementRoom.Add(elementAllUsers);
                element.Add(elementRoom);

                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        public static string struct_search_user_to_string(StructDocMess mess, int take)
        {
            dbb dbb = new dbb();
            try
            {
                var XD = new XDocument();
                XElement element = new XElement("Message");
                XElement iphoneCompanyElem = new XElement("index_command", mess.index_command);
                element.Add(iphoneCompanyElem);

                List<UserNotType> LUsers = new List<UserNotType>();
                try
                {
                    LUsers = dbb.UserNotType.Where(t => t.NameUser.Contains(mess.text_message)).OrderBy(t=>t.NameUser).Take(take).ToList();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                if (LUsers.Any())
                {
                    XElement elementAllUsers = new XElement("Users");
                    foreach (var us in LUsers)
                    {
                        XElement elementUsers = new XElement("User");
                        iphoneCompanyElem = new XElement("index_user", us.Id);
                        elementUsers.Add(iphoneCompanyElem);
                        iphoneCompanyElem = new XElement("name_user", us.NameUser);
                        elementUsers.Add(iphoneCompanyElem);
                        iphoneCompanyElem = new XElement("img_user", "");
                        elementUsers.Add(iphoneCompanyElem);
                        elementAllUsers.Add(elementUsers);
                    }
                    element.Add(elementAllUsers);
                }
                XD.Add(element);
                return XD.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
