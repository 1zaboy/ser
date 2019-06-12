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
                var r = _mmm.UserNotType.ToList();
                foreach (var VARIABLE in r)
                {
                    VARIABLE.index_in_list = null;
                }
                _mmm.SaveChanges();

                Init();
                while (true)
                {
                    Console.WriteLine("1\tЗапуск TCP");
                    Console.WriteLine("2\tЗапуск UDP");
                    Console.WriteLine("3\tОтображения в консоль пользователей");
                    Console.WriteLine("4\tСоздание отчётов о отправленных сообщениях");
                    Console.WriteLine("5\tСоздание отчётов о группах");
                    Console.WriteLine("6\tСоздание отчётов о пользователях");
                    string com = Console.ReadLine();
                    if (Dictionary.ContainsKey(com))
                    {
                        Dictionary[com].DynamicInvoke();
                    }
                    else
                    {
                        Console.WriteLine("Нет такого варианта");
                    }
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
            Dictionary.Add("4", new Action(case4));
            Dictionary.Add("5", new Action(case5));
            Dictionary.Add("6", new Action(case6));
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
                dbb ddDbb = new dbb();
                var t = ddDbb.UserNotType.ToList();
                if (t.Any())
                {
                    foreach (var VARIABLE in t)
                    {
                        Console.WriteLine("{0}:{1}:{2}:{3}", VARIABLE.Id, VARIABLE.NameUser, VARIABLE.Password, VARIABLE.index_in_list);
                    }
                }
                else
                {
                    Console.WriteLine("Пользователи отсутствуют");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        static private void case4()
        {
            try
            {
                Report r = new Report();
                var str = r.ViewMessage();
                Console.WriteLine("Отчёт создан, он находится по адресу: {0}", str);
                //r.sss();
                
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        static private void case5()
        {
            try
            {
                Report r = new Report();
                var str = r.ViewGroup();
                Console.WriteLine("Отчёт создан, он находится по адресу: {0}", str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
        static private void case6()
        {
            try
            {
                Report r = new Report();
                var str = r.ViewUsers();
                Console.WriteLine("Отчёт создан, он находится по адресу: {0}", str);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}
