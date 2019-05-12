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
            HOST = ip;
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
                    if (data[0] == 1)
                    {
                        for (int i = 0; i < ServerObject.clients.Count; i++)
                        {
                            IPAddress address =
                                ((IPEndPoint) (ServerObject.clients[i].ClientObject.client.Client.RemoteEndPoint))
                                .Address;
                            if (address.MapToIPv4().ToString() == remoteIp.Address.MapToIPv4().ToString())
                            {
                                ServerObject.clients[i].port_udp = remoteIp.Port;
                                Console.WriteLine("Udp: " + remoteIp.Address + ":" + remoteIp.Port);
                                break;
                            }
                        }
                        //ServerObject.clients.Find(t =>
                        //        ((IPEndPoint) (t.ClientObject.client.Client.RemoteEndPoint)).Address ==
                        //        remoteIp.Address)
                        //    .port_udp = remoteIp.Port;
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
