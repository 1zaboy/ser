using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ser.DATA_DB;

namespace ser
{
    class ServerObject
    {
        static TcpListener tcpListener;
        static public Dictionary<int, MainListUser> DictionaryClients = new Dictionary<int, MainListUser>();
        static public int IndexUser = 0;
        //CLEAR ITEM LIST
        //remove user from list connection users
        protected internal void RemoveConnection(string id)
        {
            try
            {
                dbb M = new dbb();
                // получаем по id закрытое подключение
                var client = DictionaryClients.FirstOrDefault(c => c.Value.ClientObject.Id == id);
                if (client.Value != null)
                {
                    DictionaryClients.Remove(client.Key);
                    //clients.Remove(client.);
                    //var sqlObj = M.UserNotType.Where(t => t.index_in_list == _index).ToList();
                    var sqlObj = M.UserNotType.Where(t => t.index_in_list == client.Key).ToList();
                    if (sqlObj.Any())
                    {
                        sqlObj.First().index_in_list = null;
                        M.SaveChanges();
                    }
                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        // listen new connection
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 3487);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен.");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    
                    MainListUser listUser = new MainListUser();
                    listUser.ClientObject = new ClientObject(tcpClient, this);

                    //clients.Add(listUser);
                    DictionaryClients.Add(IndexUser, listUser);
                    IndexUser += 1;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // disconnection all users
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера
            foreach (var t in DictionaryClients)
            {
                t.Value.ClientObject.Close();
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
