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
                        List<string> sList = CutXml(message);
                        foreach (var VARIABLE in sList)
                        {
                            string i = XmlParser.XmlParser.GetIndexCommand(VARIABLE);
                            if (i != 0.ToString())
                            {
                                query.Dictionary[i].DynamicInvoke(XmlParser.XmlParser.string_to_struct(VARIABLE));
                            }
                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
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
                byte[] data = Encoding.UTF8.GetBytes(str);
                byte[] countBytes = BitConverter.GetBytes(data.Length);
                byte[] SendArray = countBytes.Concat(data).ToArray();
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

        private List<string> CutXml(string xml)
        {
            try
            {
                string end_str = "</Message>";
                List<string> strList = new List<string>();
                while (xml != "")
                {
                    int ind = xml.IndexOf(end_str);
                    if (ind == -1)
                        break;
                    string g = xml.Substring(0, ind + end_str.Length);
                    strList.Add(g);
                    xml = xml.Remove(0, ind + end_str.Length);
                }

                return strList;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

}
