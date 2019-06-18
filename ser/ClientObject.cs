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
                string start_str = "<Message>";
                string end_str = "</Message>";
                List<string> strList = new List<string>();
                while (xml != "")
                {
                    if(xml.Length < start_str.Length + end_str.Length)
                        break;
                    int ind1 = xml.IndexOf(start_str);
                    if(ind1==-1)
                        break;
                    int ind2 = xml.IndexOf(end_str, ind1);
                    Console.WriteLine("xml string: {0}", xml);
                    Console.WriteLine("ind1: {0}", ind1);
                    Console.WriteLine("ind2: {0}", ind2);
                    Console.WriteLine('\n');
                    if (ind1 == -1 || ind2 == -1)
                        break;
                    if (ind1 + start_str.Length < ind2 && ind2 + end_str.Length < xml.Length)
                    {
                        string g = xml.Substring(ind1, (ind2 + end_str.Length) - ind1);
                        strList.Add(g);
                        xml = xml.Remove(ind1, (ind2 + end_str.Length) - ind1);
                        xml = xml.Replace(" ", "");
                    }
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
