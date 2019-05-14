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
        dbb dataBaceDbb = new dbb();
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
                    Console.WriteLine("info user:{0} :: {1}", remoteIp.Address, remoteIp.Port);
                    Console.WriteLine("UDP String:{0}", str);
                    string[] arrayStr = str.Split(':');
                    if (arrayStr.Length > 1)
                    {
                        Console.WriteLine("{0}:{1}", arrayStr[0], arrayStr[1]);
                        int f = 0;
                        if (arrayStr[0] == "1" && Int32.TryParse(arrayStr[1], out f))
                        {
                            string str_with_array = arrayStr[1];
                            var user = dataBaceDbb.UserNotType.Where(t => t.Id.ToString() == str_with_array).ToList();
                            if (user.Any())
                            {
                                int r = user.First().index_in_list + 1 ?? default(int);
                                if (r != default(int))
                                    ServerObject.clients[r - 1].port_udp = remoteIp.Port;
                            }

                            //for (int i = 0; i < ServerObject.clients.Count; i++)
                            //{
                            //    IPAddress address =
                            //        ((IPEndPoint) (ServerObject.clients[i].ClientObject.client.Client.RemoteEndPoint))
                            //        .Address;
                            //    if (address.MapToIPv4().ToString() == remoteIp.Address.MapToIPv4().ToString())
                            //    {
                            //        ServerObject.clients[i].port_udp = remoteIp.Port;
                            //        Console.WriteLine("Udp: " + remoteIp.Address + ":" + remoteIp.Port);
                            //        break;
                            //    }
                            //}
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
