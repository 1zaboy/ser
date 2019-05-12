using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Data.Entity;
using System.Data.Entity.Validation;
using ser.DATA_DB;
using ser.XmlParser;

namespace ser
{
    class Program
    {
        static ServerObject server; // сервер
        static MyUDPSystem udp_server; // сервер
        static Thread listenThread; // потока для прослушивания
        static Thread LThread2;
        static public Dictionary<string, Delegate> Dictionary = new Dictionary<string, Delegate>();
        static void Main()
        {
           
            try
            {
                Init();
                while (true)
                {
                    Console.WriteLine("Enter command");
                    string com = Console.ReadLine();
                    Dictionary[com].DynamicInvoke();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        static public void Init()
        {
            Dictionary.Add("1", new Action(case1));
            Dictionary.Add("2", new Action(case2));
            Dictionary.Add("3", new Action(case3));
        }

        static private void case1()
        {
            try
            {
                server = new ServerObject();
                listenThread = new Thread(new ThreadStart(server.Listen));
                listenThread.Start();
            }
            catch (Exception e)
            {
                server.Disconnect();
                Console.WriteLine(e);
                throw;
            }
        }
        static private void case2()
        {
            udp_server = new MyUDPSystem();
            udp_server.Start();
            LThread2 = new Thread(new ThreadStart(udp_server.Process));
            LThread2.Start();
        }
        static dbb _mmm = new dbb();
        static private void case3()
        {
            try
            {
                var t = _mmm.UserNotType.ToList();
                if (t.Any())
                {
                    foreach (var VARIABLE in t)
                    {
                        Console.WriteLine("{0}:{1}", VARIABLE.NameUser, VARIABLE.Password);
                    }
                }
                else
                {
                    Console.WriteLine("not have users");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
