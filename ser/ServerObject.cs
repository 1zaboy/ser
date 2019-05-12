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
        static TcpListener tcpListener; // сервер для прослушивания
        static public List<MainListUser> clients = new List<MainListUser>(); // все подключения
        dbb M = new dbb();

        //CLEAR ITEM LIST
        protected internal void RemoveConnection(string id)
        {
            try
            {
                // получаем по id закрытое подключение

                MainListUser client = clients.FirstOrDefault(c => c.ClientObject.Id == id);
                var _index = clients.FindIndex(a => a == client);
                // и удаляем его из списка подключений
                if (client != null)
                {
                    clients.Remove(client);
                    var sqlObj = M.UserNotType.Where(t => t.index_in_list == _index).ToList().First();
                    sqlObj.index_in_list = null;
                    M.SaveChanges();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        // прослушивание входящих подключений
        public void Listen()
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Any, 3487);
                tcpListener.Start();
                Console.WriteLine("Сервер запущен. Ожидание подключений...");

                while (true)
                {
                    TcpClient tcpClient = tcpListener.AcceptTcpClient();
                    
                    MainListUser listUser = new MainListUser();
                    listUser.ClientObject = new ClientObject(tcpClient, this);

                    clients.Add(listUser);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Disconnect();
            }
        }

        // отключение всех клиентов
        protected internal void Disconnect()
        {
            tcpListener.Stop(); //остановка сервера

            for (int i = 0; i < clients.Count; i++)
            {
                clients[i].ClientObject.Close(); //отключение клиента
            }
            Environment.Exit(0); //завершение процесса
        }
    }
}
