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
        public dbb _db = new dbb();
        private ClientObject _clientObject;
        public query(ClientObject clientObject)
        {
            _clientObject = clientObject;
            Dictionary.Add("1", new Func<StructDocMess, bool>(case1));
            Dictionary.Add("2", new Func<StructDocMess, bool>(case2));
            Dictionary.Add("10", new Func<StructDocMess, bool>(case10));
            Dictionary.Add("13", new Func<StructDocMess, bool>(case13));
            Dictionary.Add("14", new Func<StructDocMess, bool>(case14));
            Dictionary.Add("15", new Func<StructDocMess, bool>(case15));
            Dictionary.Add("20", new Func<StructDocMess, bool>(case20));
            Dictionary.Add("21", new Func<StructDocMess, bool>(case21));
            Dictionary.Add("22", new Func<StructDocMess, bool>(case22));
            Dictionary.Add("25", new Func<StructDocMess, bool>(case25));
            Dictionary.Add("80", new Func<StructDocMess, bool>(case80));
            Dictionary.Add("85", new Func<StructDocMess, bool>(case85));
            Dictionary.Add("90", new Func<StructDocMess, bool>(case90));
        }

        public bool case1(StructDocMess mess)//log
        {
            try
            {
                var r = _db.UserNotType.Where(t => t.NameUser == mess.name_user && t.Password == mess.password_user).ToList();
                var d = ServerObject.DictionaryClients.Where(t => t.Value.ClientObject == _clientObject).ToList();
                if (r.Any() && d.Any())
                {
                    var r1 = r.First();
                    //r.First().index_in_list = ser.ServerObject.clients.FindIndex(t => t.ClientObject == _clientObject);
                    r1.index_in_list = d.Last().Key;
                    _db.SaveChanges();
                    Console.WriteLine("case 1: " + d.Last().Key);
                    

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
                    Console.WriteLine(fd);
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

        public bool case10(StructDocMess mess)//create room
        {
            try
            {
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

        public bool case20(StructDocMess mess)//send invait
        {
            try
            {
                var main_user = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList().First();
                var user = _db.UserNotType.Where(t => t.NameUser == mess.text_message).ToList();
                if (user.Any())
                {
                    int r = user.First().index_in_list + 1 ?? default(int);
                    Console.WriteLine("invite index: {0}", r);
                    if (r != default(int) && main_user.Id != user.First().Id)
                    {
                        //ServerObject.clients[r - 1].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                        if (ServerObject.DictionaryClients.ContainsKey(r - 1))
                        {
                            mess.count_users_in_room = _db.C_User_In_Room
                                .Where(t => t.C_Room.TableId == mess.index_room).ToList().Count;
                            ServerObject.DictionaryClients[r - 1].ClientObject
                                .SendMess(XmlParser.XmlParser.struct_to_string(mess));
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        r = main_user.index_in_list ?? default(int);
                        ServerObject.DictionaryClients[r].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                        return false;
                    }
                }
                else
                {
                    int r1 = main_user.index_in_list ?? default(int);
                    ServerObject.DictionaryClients[r1].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
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
                var room = _db.C_Room.Where(t => t.TableId == mess.index_room).ToList().First();
                var user = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList().First();

                var user_in_room = new C_User_In_Room();
                user_in_room.C_Room = room;
                user_in_room.UserNotType = user;
                user_in_room.Participant = true;
                _db.C_User_In_Room.Add(user_in_room);
                _db.SaveChanges();

                var all_user = _db.C_User_In_Room.Where(t => t.C_Room.TableId == room.TableId && t.UserNotType.Id != user.Id).ToList();
                foreach (var VARIABLE in all_user)
                {
                    int r = VARIABLE.UserNotType.index_in_list ?? default(int);
                    if (r != default(int))
                    {
                        //ServerObject.clients[r].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
                        if (ServerObject.DictionaryClients.ContainsKey(r))
                            ServerObject.DictionaryClients[r].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));
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
                var Room = _db.C_Room.Where(t => t.TableId == mess.index_room).ToList().First();
                var User = _db.UserNotType.Where(t => t.Id.ToString() == mess.index_user).ToList().First();
                message_on_room messageOnRoom = new message_on_room();
                messageOnRoom.C_User_In_Room = _db.C_User_In_Room
                    .Where(t => t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() == mess.index_user)
                    .ToList().First();
                messageOnRoom.text_mess = mess.text_message;
                messageOnRoom.time_mess = DateTime.ParseExact(mess.time_message, "yyyy.MM.dd-HH.mm.ss", System.Globalization.CultureInfo.InvariantCulture);
                _db.message_on_room.Add(messageOnRoom);
                _db.SaveChanges();
                var usersInRoom = _db.C_User_In_Room
                    .Where(t => t.C_Room.TableId == mess.index_room).ToList();
                foreach (var VARIABLE in usersInRoom)
                {
                    int v2 = -1;
                    if (VARIABLE.UserNotType.index_in_list.HasValue)
                        v2 = VARIABLE.UserNotType.index_in_list.Value;
                    if (v2 != -1)
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

        public bool case80(StructDocMess mess) //inviting call
        {
            try
            {
                var usersInRoom = _db.C_User_In_Room
                    .Where(t => t.C_Room.TableId == mess.index_room && t.UserNotType.Id.ToString() != mess.index_user).ToList();
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
                    Console.WriteLine("v2-{0}:v4-{1}", v2, v4);

                    Console.WriteLine("all key\n");
                    foreach (var VARIABLE in ServerObject.DictionaryClients)
                    {
                        Console.WriteLine(VARIABLE.Value.port_udp);
                    }

                    if (ServerObject.DictionaryClients.ContainsKey(v2) && ServerObject.DictionaryClients.ContainsKey(v4))
                    {
                        mess.index_user = usersInRoom[1].UserNotType.Id.ToString();
                        mess.name_user = usersInRoom[1].UserNotType.NameUser;
                        Console.WriteLine("v4 port-{0}", ServerObject.DictionaryClients[v4].port_udp);
                        mess.text_message =
                            ((IPEndPoint)(ServerObject.DictionaryClients[v4].ClientObject.client.Client.RemoteEndPoint)).Address
                            .ToString() +
                            ":" + ServerObject.DictionaryClients[v4].port_udp;
                        ServerObject.DictionaryClients[v2].ClientObject.SendMess(XmlParser.XmlParser.struct_to_string(mess));

                        mess.index_user = usersInRoom[0].UserNotType.Id.ToString();
                        mess.name_user = usersInRoom[0].UserNotType.NameUser;
                        Console.WriteLine("v2 port-{0}", ServerObject.DictionaryClients[v2].port_udp);
                        mess.text_message =
                            ((IPEndPoint)(ServerObject.DictionaryClients[v2].ClientObject.client.Client
                                .RemoteEndPoint)).Address
                            .ToString() +
                            ":" + ServerObject.DictionaryClients[v2].port_udp;
                        ServerObject.DictionaryClients[v4].ClientObject
                            .SendMess(XmlParser.XmlParser.struct_to_string(mess));
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

        public bool case90(StructDocMess mess)
        {
            try
            {
                C_User_In_Room userInRoom = _db.C_User_In_Room
                    .Where(t => t.UserNotType.Id.ToString() == mess.index_user && t.C_Room.TableId == mess.index_room).ToList().First();
                userInRoom.Participant = false;
                //_db.C_User_In_Room.Remove(userInRoom);
                _db.SaveChanges();
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
