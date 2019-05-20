using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Xml.Linq;
using ser.XmlParser;

namespace ser
{
    class ClientObject
    {
        private string _ip;
        private int _port;
        //
        protected internal string Id { get; private set; }
        protected internal NetworkStream Stream { get; private set; }
        string userName;
        public TcpClient client;
        ServerObject server;

        Thread LThrerad;
        
        public ClientObject(string ip, int port)
        {
            _ip = ip;
            _port = port;
        }
        public ClientObject(TcpClient tcpClient, ServerObject serverObject)
        {
            Id = Guid.NewGuid().ToString();
            client = tcpClient;
            server = serverObject;
            

            LThrerad = new Thread(new ThreadStart(this.Process));
            LThrerad.Start();
        }
       
        public void Process()
        {
            try
            {
                Stream = client.GetStream();
                string message = "";
                query query = new query(this);
                while (true)
                {
                    try
                    {
                        message = GetMessage();
                        //Console.WriteLine(message);
                        string i = XmlParser.XmlParser.GetIndexCommand(message);
                        query.Dictionary[i].DynamicInvoke(XmlParser.XmlParser.string_to_struct(message));
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        message = String.Format("покинул чат");
                        Console.WriteLine(message);
                        break;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                // в случае выхода из цикла закрываем ресурсы
                server.RemoveConnection(this.Id);
                Close();
            }
        }

        public bool SendMess(string str)
        {
            try
            {
                Console.WriteLine(str);
                byte[] data = Encoding.UTF8.GetBytes(str);
                byte[] countBytes = BitConverter.GetBytes(data.Length);
                Console.WriteLine("Send count bytes: {0}", data.Length);
                byte[] SendArray = countBytes.Union(data).ToArray();
                Stream.Write(SendArray, 0, SendArray.Length);
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }
        // чтение входящего сообщения и преобразование в строку
        private string GetMessage()
        {
            byte[] data = new byte[2048]; // буфер для получаемых данных
            StringBuilder builder = new StringBuilder();
            int bytes = 0;
            do
            {
                bytes = Stream.Read(data, 0, data.Length);
                builder.Append(Encoding.UTF8.GetString(data, 0, bytes));
            }
            while (Stream.DataAvailable);

            return builder.ToString();
        }

        // закрытие подключения
        protected internal void Close()
        {
            if (Stream != null)
                Stream.Close();
            if (client != null)
                client.Close();
        }
    }

}
