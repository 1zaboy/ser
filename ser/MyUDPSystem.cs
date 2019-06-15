using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations.Builders;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using ser.DATA_DB;

namespace ser
{
    class MyUDPSystem
    {
        bool alive = false; // будет ли работать поток для приема
        UdpClient client;
        int LOCALPORT = 8001; // порт для приема сообщений
        string HOST; // хост для групповой рассылки
        IPAddress groupAddress; // адрес для групповой рассылки

        string userName; // имя пользователя в чате
        public MyUDPSystem(string ip = "127.0.0.1", int port = 3488)
        {
            HOST = IPAddress.Any.ToString();
            LOCALPORT = port;
        }
        public void Start()
        {
            try
            {
                groupAddress = IPAddress.Parse(HOST);
                client = new UdpClient(LOCALPORT);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public void Process()
        {
            alive = true;
            try
            {
                while (alive)
                {
                    IPEndPoint remoteIp = null;
                    byte[] data = client.Receive(ref remoteIp);
                    string str = Encoding.UTF8.GetString(data, 0, data.Length);
                    sendInfo(str, remoteIp);
                    string[] arrayStr = str.Split(':');
                    Console.WriteLine("{0}:{1}", arrayStr[0], arrayStr[1]);
                    if (arrayStr.Length > 1)
                    {
                        int f = 0;
                        if (arrayStr[0] == "1" && Int32.TryParse(arrayStr[1], out f))
                        {
                            string str_with_array = arrayStr[1];
                            dbb dataBaceDbb = new dbb();
                            var user = dataBaceDbb.UserNotType.Where(t => t.Id.ToString() == str_with_array).ToList();
                            if (user.Any())
                            {
                                var user1 = user.First();
                                int r = -1;
                                if (user1.index_in_list.HasValue)
                                    r = user1.index_in_list.Value;
                                if (r != -1 && ServerObject.DictionaryClients.ContainsKey(r))
                                {
                                    ServerObject.DictionaryClients[r].port_udp = remoteIp.Port;
                                    Console.WriteLine("Enter port in dic: {0}", ServerObject.DictionaryClients[r].port_udp);
                                }
                            }
                        }
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                if (!alive)
                    return;
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int sendInfo(string str, IPEndPoint endPoint)
        {
            byte[] data = Encoding.UTF8.GetBytes(str.ToCharArray(), 0, str.Length);
            return client.Send(data, data.Length, endPoint);
        }

        public void ExitChat()
        {
            try
            {
                string message = userName + " покидает чат";
                byte[] data = Encoding.UTF8.GetBytes(message);
                client.Send(data, data.Length, HOST, LOCALPORT);
                client.DropMulticastGroup(groupAddress);

                alive = false;
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
