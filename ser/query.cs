using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using ser.DATA_DB;
using ser.XmlParser;

namespace ser
{
    class query
    {
        public Dictionary<string, Delegate> Dictionary = new Dictionary<string, Delegate>();
        private ClientObject _clientObject;
        public query(ClientObject clientObject)
        {
            _clientObject = clientObject;
            Dictionary.Add("0", new Func<StructDocMess, bool>(case0));
            Dictionary.Add("1", new Func<StructDocMess, bool>(case1));
            Dictionary.Add("2", new Func<StructDocMess, bool>(case2));
            Dictionary.Add("3", new Func<StructDocMess, bool>(case3));
            Dictionary.Add("5", new Func<StructDocMess, bool>(case5));
            Dictionary.Add("10", new Func<StructDocMess, bool>(case10));
            Dictionary.Add("13", new Func<StructDocMess, bool>(case13));
            Dictionary.Add("14", new Func<StructDocMess, bool>(case14));
            Dictionary.Add("15", new Func<StructDocMess, bool>(case15));
            Dictionary.Add("16", new Func<StructDocMess, bool>(case16));
            Dictionary.Add("20", new Func<StructDocMess, bool>(case20));
            Dictionary.Add("21", new Func<StructDocMess, bool>(case21));
            Dictionary.Add("22", new Func<StructDocMess, bool>(case22));
            Dictionary.Add("25", new Func<StructDocMess, bool>(case25));
            Dictionary.Add("80", new Func<StructDocMess, bool>(case80));
            Dictionary.Add("85", new Func<StructDocMess, bool>(case85));
            Dictionary.Add("86", new Func<StructDocMess, bool>(case86));
            Dictionary.Add("90", new Func<StructDocMess, bool>(case90));
        }

        public bool case0(StructDocMess mess)
        {
            try
            {
                _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool case1(StructDocMess mess)//log
        {
            try
            {
                dbb _db = new dbb();
                var r = _db.UserNotType.Where(t => t.NameUser == mess.name_user && t.Password == mess.password_user).ToList();
                var d = ServerObject.DictionaryClients.Where(t => t.Value.ClientObject == _clientObject).ToList();
                if (r.Any() && d.Any())
                {
                    int coutline = _db.Database.ExecuteSqlCommand("update UserNotType set index_in_list =" +
                                                                  d.Last().Key + " where Id = " +
                                                                  r.First().Id);
                    //Console.WriteLine("count line: " + coutline);
                    //Console.WriteLine("case 1: " + d.Last().Key);

                    mess.index_user = r.First().Id.ToString();
                    mess.text_message = "True";
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    return true;
                }
                else
                {
                    mess.text_message = "False";
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


        public bool case2(StructDocMess mess)//reg false if there is user treu if reg
        {
            try
            {
                dbb _db = new dbb();
                var r = _db.UserNotType.Where(t => t.NameUser == mess.name_user && t.Password == mess.password_user).ToList();
                if (!r.Any())
                {
                    UserNotType userNotType = new UserNotType();
                    userNotType.NameUser = mess.name_user;
                    userNotType.Password = mess.password_user;
                    userNotType.index_in_list = ser.ServerObject.DictionaryClients.Last(t => t.Value.ClientObject == _clientObject).Key;
                    //userNotType.index_in_list = ser.ServerObject.clients.FindIndex(t => t.ClientObject == _clientObject);
                    _db.UserNotType.Add(userNotType);
                    _db.SaveChanges();

                    r = _db.UserNotType.Where(t => t.NameUser == mess.name_user && t.Password == mess.password_user).ToList();
                    mess.index_user = r.First().Id.ToString();
                    mess.text_message = "True";
                    string fd = XmlParser.XmlParser.struct_to_string(mess);
                    //Console.WriteLine(fd);
                    _clientObject.SendMess(fd);
                    return true;
                }
                else
                {
                    mess.text_message = "False";
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    return false;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                throw;
            }
        }
        public bool case3(StructDocMess mess)
        {
            try
            {
                dbb _db = new dbb();
                var r = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList();
                if (r.Any())
                {
                    r.First().index_in_list = null;
                    _db.SaveChanges();
                    return true;
                }
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case5(StructDocMess mess)
        {
            try
            {
                var xml_str = XmlParser.XmlParser.struct_search_user_to_string(mess, 7);
                _clientObject.SendMess(xml_str);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case10(StructDocMess mess)//create room
        {
            try
            {
                dbb _db = new dbb();
                var user = _db.UserNotType.First(t => t.Id.ToString() == mess.index_user);
                if (user != null)
                {
                    var newroom = new C_Room();
                    newroom.NameRoom = mess.name_room;
                    _db.C_Room.Add(newroom);
                    _db.SaveChanges();

                    var room = _db.C_Room.Where(t => t.NameRoom == mess.name_room).ToList().Last();
                    var userInRoom = new C_User_In_Room();
                    userInRoom.UserNotType = user;
                    userInRoom.C_Room = room;
                    userInRoom.Admin = true;
                    userInRoom.Participant = true;
                    _db.C_User_In_Room.Add(userInRoom);
                    _db.SaveChanges();

                    mess.index_room = room.TableId;
                    mess.count_users_in_room = 1;
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                }
                return true;
            }
            catch (DbEntityValidationException e)
            {
                string message = "";
                foreach (DbEntityValidationResult validationError in e.EntityValidationErrors)
                {
                    message = "Object: " + validationError.Entry.Entity.ToString();

                    foreach (DbValidationError err in validationError.ValidationErrors)
                    {
                        message = message + err.ErrorMessage + "";
                    }
                }
                Console.WriteLine(message);
                throw;
            }

        }

        public bool case13(StructDocMess mess)//get all room
        {
            try
            {
                //string message_string = XmlParser.XmlParser.struct_all_room_to_string(mess);
                string message_string = XmlParser.XmlParser.struct_all_room_not_mess_string(mess);
                _clientObject.SendMess(message_string);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case14(StructDocMess mess)
        {
            try
            {
                //string message_string = XmlParser.XmlParser.struct_all_room_to_string(mess);
                string message_string = XmlParser.XmlParser.struct_all_mess_in_room_to_string(mess);
                _clientObject.SendMess(message_string);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case15(StructDocMess mess)
        {
            try
            {
                string message_string = XmlParser.XmlParser.struct_one_room_list_user_to_string(mess);
                _clientObject.SendMess(message_string);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case16(StructDocMess mess)
        {
            try
            {
                dbb _db = new dbb();
                var nany = _db.C_User_In_Room.Where(t => t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() == mess.index_user && t.Admin && t.Participant).ToList();
                if (nany.Any())
                {
                    var main_q = _db.C_User_In_Room.Where(t =>
                            t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() == mess.text_message)
                        .ToList();
                    if (main_q.Any())
                    {
                        main_q.First().Participant = false;
                        _db.SaveChanges();

                        StructDocMess fDocMess = new StructDocMess();
                        fDocMess.index_command = "26";
                        fDocMess.index_user = "-1";
                        fDocMess.name_user = "Server";
                        fDocMess.index_room = mess.index_room;
                        fDocMess.name_room = mess.name_room;
                        fDocMess.text_message = "Удален пользователь: " + main_q.First().UserNotType.NameUser;
                        fDocMess.time_message = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");

                        var Mess_in_room = new message_on_room();
                        Mess_in_room.C_User_In_Room = main_q.First();
                        Mess_in_room.text_mess = "Удален пользователь: " + main_q.First().UserNotType.NameUser;
                        Mess_in_room.time_mess = DateTime.Now;
                        _db.message_on_room.Add(Mess_in_room);
                        _db.SaveChanges();

                        var all_user_in_room = _db.C_User_In_Room
                            .Where(t => t.C_Room.TableId == mess.index_room && t.Participant).ToList();
                        foreach (var cUserInRoom in all_user_in_room)
                        {
                            int r = cUserInRoom.UserNotType.index_in_list ?? -1;
                            if (r != -1)
                            {
                                if (ServerObject.DictionaryClients.ContainsKey(r))
                                {
                                    ServerObject.DictionaryClients[r].ClientObject
                                        .SendMess(XmlParser.XmlParser.struct_to_string(mess));
                                    ServerObject.DictionaryClients[r].ClientObject
                                        .SendMess(XmlParser.XmlParser.struct_to_string(fDocMess));
                                }
                            }
                        }

                        var all_user_in_room1 = _db.UserNotType
                            .Where(t => t.Id.ToString() == mess.text_message).ToList();
                        fDocMess.text_message = "9E0D14D2-6A42-43F0-BEA6-F75E780EB63B";

                        foreach (var cUserInRoom in all_user_in_room1)
                        {
                            //Console.WriteLine("send");
                            int r = cUserInRoom.index_in_list ?? -1;
                            if (r != -1)
                            {
                                if (ServerObject.DictionaryClients.ContainsKey(r))
                                {
                                    ServerObject.DictionaryClients[r].ClientObject
                                        .SendMess(XmlParser.XmlParser.struct_to_string(fDocMess));
                                }
                            }
                        }

                    }
                }
                else
                {
                    mess.text_message = "AA2B206A-8857-44E0-8190-4F93A9BCC06F";
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case20(StructDocMess mess)//send invait
        {
            try
            {
                dbb _db = new dbb();
                Int32 r = -1;
                var main_user = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList().First();
                var m1 = _db.C_User_In_Room.Where(t => t.Participant && t.C_Room.TableId == mess.index_room && t.UserNotType.NameUser == mess.text_message)
                    .ToList();
                var m = _db.UserNotType.Where(t => t.NameUser == mess.text_message).ToList()
                    .ToList();
                if (!m.Any())
                {
                    if (m.First().index_in_list.HasValue)
                        r = m.First().index_in_list.Value;
                    if (ServerObject.DictionaryClients.ContainsKey(r))
                    {
                        mess.count_users_in_room = _db.C_User_In_Room
                            .Where(t => t.C_Room.TableId == mess.index_room && t.Participant).ToList().Count;
                        ServerObject.DictionaryClients[r].ClientObject
                            .SendMess(XmlParser.XmlParser.struct_to_string(mess));
                        return true;
                    }
                    else
                    {
                        if (main_user.index_in_list.HasValue)
                            r = main_user.index_in_list.Value;
                        if (ServerObject.DictionaryClients.ContainsKey(r))
                            ServerObject.DictionaryClients[r].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                        return false;
                    }
                }
                else
                {
                    if (main_user.index_in_list.HasValue)
                        r = main_user.index_in_list.Value;
                    if (ServerObject.DictionaryClients.ContainsKey(r))
                        ServerObject.DictionaryClients[r].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    return false;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool case21(StructDocMess mess)//new user in Room | send user in room and add
        {
            try
            {
                dbb _db = new dbb();
                var room = _db.C_Room.Where(t => t.TableId == mess.index_room).ToList().First();
                var user = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList().First();

                StructDocMess fDocMess = new StructDocMess();
                if (mess.text_message.Substring(0, 4) == "True")
                {
                    var user_was_in_room = _db.C_User_In_Room.Where(t => t.UserNotType.Id.ToString() == mess.index_user && t.C_Room.TableId == mess.index_room).ToList();
                    if (!user_was_in_room.Any())
                    {
                        var user_in_room = new C_User_In_Room();
                        user_in_room.C_Room = room;
                        user_in_room.UserNotType = user;
                        user_in_room.Participant = true;
                        user_in_room.Admin = false;
                        _db.C_User_In_Room.Add(user_in_room);
                        _db.SaveChanges();
                    }
                    else
                    {
                        user_was_in_room.First().Participant = true;
                        _db.SaveChanges();
                    }

                    var s = _db.C_User_In_Room.Where(t => t.UserNotType.NameUser == user.NameUser).ToList().First();
                    var Mess_in_room = new message_on_room();
                    Mess_in_room.C_User_In_Room = s;
                    Mess_in_room.text_mess = mess.text_message.Substring(5, mess.text_message.Length - 5);
                    Mess_in_room.time_mess = DateTime.Now;
                    _db.message_on_room.Add(Mess_in_room);
                    _db.SaveChanges();

                    fDocMess.index_command = "25";
                    fDocMess.index_user = "-1";
                    fDocMess.name_user = "Server";
                    fDocMess.index_room = mess.index_room;
                    fDocMess.name_room = mess.name_room;
                    fDocMess.text_message = mess.text_message.Substring(5, mess.text_message.Length - 5) + mess.name_user;
                    fDocMess.time_message = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");

                    var all_user = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == room.TableId && t.UserNotType.Id != user.Id).ToList();
                    foreach (var VARIABLE in all_user)
                    {
                        int r = VARIABLE.UserNotType.index_in_list ?? -1;
                        if (r != -1)
                        {
                            if (ServerObject.DictionaryClients.ContainsKey(r))
                            {
                                ServerObject.DictionaryClients[r].ClientObject
                                    .SendMess(XmlParser.XmlParser.struct_to_string(mess));
                                ServerObject.DictionaryClients[r].ClientObject
                                    .SendMess(XmlParser.XmlParser.struct_to_string(fDocMess));
                            }
                        }
                    }
                }
                else if (mess.text_message.Substring(0, 5) == "False")
                {
                    fDocMess.index_command = "25";
                    fDocMess.index_user = "-1";
                    fDocMess.name_user = "Server";
                    fDocMess.index_room = mess.index_room;
                    fDocMess.name_room = mess.name_room;
                    fDocMess.text_message = mess.text_message.Substring(6, mess.text_message.Length - 6) + mess.name_user;
                    fDocMess.time_message = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");

                    var all_user = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == room.TableId && t.UserNotType.Id != user.Id).ToList();
                    foreach (var VARIABLE in all_user)
                    {
                        int r = VARIABLE.UserNotType.index_in_list ?? -1;
                        if (r != -1)
                        {
                            if (ServerObject.DictionaryClients.ContainsKey(r))
                            {
                                ServerObject.DictionaryClients[r].ClientObject
                                    .SendMess(XmlParser.XmlParser.struct_to_string(fDocMess));
                            }
                        }
                    }
                }
                else
                {
                    Console.WriteLine("case 21: Error");
                }



                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case22(StructDocMess mess)
        {
            try
            {
                string message_string = XmlParser.XmlParser.struct_one_room_to_string(mess);
                _clientObject.SendMess(message_string);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public bool case25(StructDocMess mess)//new mess
        {
            try
            {
                dbb _db = new dbb();
                var User = _db.C_User_In_Room.Where(t => t.UserNotType.Id.ToString() == mess.index_user && t.C_Room.TableId == mess.index_room && t.Participant).ToList();
                if (User.Any())
                {
                    message_on_room messageOnRoom = new message_on_room();
                    messageOnRoom.C_User_In_Room = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == mess.index_room &&
                                    t.UserNotType.Id.ToString() == mess.index_user)
                        .ToList().First();
                    messageOnRoom.text_mess = mess.text_message;
                    messageOnRoom.time_mess = DateTime.ParseExact(mess.time_message, "yyyy.MM.dd-HH.mm.ss",
                        System.Globalization.CultureInfo.InvariantCulture);
                    _db.message_on_room.Add(messageOnRoom);
                    _db.SaveChanges();
                    var usersInRoom = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == mess.index_room && t.Participant).ToList();
                    foreach (var VARIABLE in usersInRoom)
                    {
                        int v2 = -1;
                        if (VARIABLE.UserNotType.index_in_list.HasValue)
                            v2 = VARIABLE.UserNotType.index_in_list.Value;
                        if (v2 != -1)
                            if (ServerObject.DictionaryClients.ContainsKey(v2))
                                ServerObject.DictionaryClients[v2].ClientObject
                                    .SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    }
                }
                else
                {
                    mess.text_message = "9E0D14D2-6A42-43F0-BEA6-F75E780EB63B";
                    _clientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case80(StructDocMess mess) //inviting call
        {
            try
            {
                dbb _db = new dbb();
                var usersInRoom = _db.C_User_In_Room
                    .Where(t => t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() != mess.index_user && t.UserNotType.NameUser != mess.name_user && t.Participant).ToList();
                foreach (var VARIABLE in usersInRoom)
                {
                    int v2 = 0;
                    if (VARIABLE.UserNotType.index_in_list.HasValue)
                        v2 = VARIABLE.UserNotType.index_in_list.Value;
                    if (ServerObject.DictionaryClients.ContainsKey(v2))
                        ServerObject.DictionaryClients[v2].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                    //ServerObject.clients[v2].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        public bool case85(StructDocMess mess)
        {
            try
            {
                dbb _db = new dbb();
                if (mess.text_message == "True")
                {
                    var usersInRoom = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == mess.index_room).ToList();
                    int v2 = 0;
                    if (usersInRoom[0].UserNotType.index_in_list.HasValue)
                        v2 = usersInRoom[0].UserNotType.index_in_list.Value;
                    int v4 = 0;
                    if (usersInRoom[1].UserNotType.index_in_list.HasValue)
                        v4 = usersInRoom[1].UserNotType.index_in_list.Value;
                    //Console.WriteLine("v2-{0}:v4-{1}", v2, v4);

                    if (ServerObject.DictionaryClients.ContainsKey(v2) && ServerObject.DictionaryClients.ContainsKey(v4))
                    {
                        mess.index_user = usersInRoom[1].UserNotType.Id.ToString();
                        mess.name_user = usersInRoom[1].UserNotType.NameUser;
                        //Console.WriteLine("v4 port-{0}", ServerObject.DictionaryClients[v4].port_udp);
                        mess.text_message =
                            ((IPEndPoint)(ServerObject.DictionaryClients[v4].ClientObject.client.Client.RemoteEndPoint)).Address
                            .ToString() +
                            ":" + ServerObject.DictionaryClients[v4].port_udp;
                        ServerObject.DictionaryClients[v2].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));

                        mess.index_user = usersInRoom[0].UserNotType.Id.ToString();
                        mess.name_user = usersInRoom[0].UserNotType.NameUser;
                        //Console.WriteLine("v2 port-{0}", ServerObject.DictionaryClients[v2].port_udp);
                        mess.text_message =
                            ((IPEndPoint)(ServerObject.DictionaryClients[v2].ClientObject.client.Client
                                .RemoteEndPoint)).Address
                            .ToString() +
                            ":" + ServerObject.DictionaryClients[v2].port_udp;
                        ServerObject.DictionaryClients[v4].ClientObject
                            .SendMess(XmlParser.XmlParser.struct_to_string(mess));

                        message_on_room me = new message_on_room();
                        me.Room_U = mess.index_room;
                        me.text_mess = "Make call";
                        me.text_mess = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");
                        _db.message_on_room.Add(me);
                        _db.SaveChanges();
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case86(StructDocMess mess)
        {
            try
            {
                dbb _db = new dbb();
                List<C_User_In_Room> LUser = _db.C_User_In_Room
                    .Where(t => t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() != mess.index_user)
                    .ToList();
                int index = -1;
                foreach (var VARIABLE in LUser)
                {
                    if (VARIABLE.UserNotType.index_in_list.HasValue)
                        index = VARIABLE.UserNotType.index_in_list.Value;
                    if (ServerObject.DictionaryClients.ContainsKey(index))
                        ServerObject.DictionaryClients[index].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public bool case90(StructDocMess mess)
        {
            try
            {
                dbb _db = new dbb();
                List<C_User_In_Room> userInRoom = _db.C_User_In_Room
                    .Where(t => t.UserNotType.Id.ToString() == mess.index_user && t.C_Room.TableId == mess.index_room).ToList();
                if (userInRoom.Any())
                {
                    message_on_room sMessageOnRoom = new message_on_room();
                    sMessageOnRoom.Room_U = userInRoom.First().TableId;
                    sMessageOnRoom.text_mess = "Вышел из чата пользователь под именем:" + mess.name_user;
                    sMessageOnRoom.time_mess = DateTime.Now;
                    _db.message_on_room.Add(sMessageOnRoom);
                    _db.SaveChanges();

                    var _user = userInRoom.First();
                    _user.Participant = false;
                    _user.Admin = false;
                    _db.SaveChanges();

                    var adm = _db.C_User_In_Room
                        .Where(t => t.Admin && t.Participant && t.C_Room.TableId == mess.index_room).ToList();
                    if (!adm.Any())
                    {
                        var r5 = _db.C_User_In_Room.Where(t => t.C_Room.TableId == mess.index_room && t.Participant)
                            .ToList();
                        if (r5.Any())
                            r5[0].Admin = true;
                        _db.SaveChanges();
                    }

                    StructDocMess fDocMess = new StructDocMess();
                    fDocMess.index_command = "26";
                    fDocMess.index_user = "-1";
                    fDocMess.name_user = "Server";
                    fDocMess.index_room = mess.index_room;
                    fDocMess.name_room = mess.name_room;
                    fDocMess.text_message = "Вышел из чата пользователь под именем: " + mess.name_user;
                    fDocMess.time_message = DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss");


                    List<C_User_In_Room> LUser = _db.C_User_In_Room
                        .Where(t => t.C_Room.TableId == mess.index_room && t.Participant)
                        .ToList();
                    int index = -1;
                    //Console.WriteLine("Вышел из группы: {0}\tКоличество людей в группе {1}",mess.name_user, LUser.Count);
                    foreach (var VARIABLE in LUser)
                    {
                        if (VARIABLE.UserNotType.index_in_list.HasValue)
                            index = VARIABLE.UserNotType.index_in_list.Value;
                       // Console.WriteLine("case90 номер в масиве: {0}", index);
                        if (ServerObject.DictionaryClients.ContainsKey(index))
                        {
                            ServerObject.DictionaryClients[index].ClientObject
                                .SendMess(XmlParser.XmlParser.struct_to_string(fDocMess));
                            //Console.WriteLine("Сообщения о выходе отправлено: {0}", mess.text_message);
                        }
                    }
                }

                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }
    }
}
