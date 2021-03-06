﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OfficeOpenXml;
using OfficeOpenXml.ConditionalFormatting;
using OfficeOpenXml.FormulaParsing.Utilities;
using ser.DATA_DB;

namespace ser
{
    class Report
    {
        //create report of the message 
        public string ViewMessage()
        {
            dbb _db = new dbb();
            List<message_on_room> data1 = _db.message_on_room.ToList();
            string app_path_directory = AppDomain.CurrentDomain.BaseDirectory;
            string path_to_xlsx = app_path_directory + "Report.xlsx";
            string name_file = "";
            string str = "";
            ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path_to_xlsx));

            var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here

            myWorksheet.Cells[1, 1].Value = "Группы";
            myWorksheet.Cells[1, 3].Value = "Пользователь";
            myWorksheet.Cells[1, 5].Value = "Сообщения";

            myWorksheet.Cells[1, 1].Style.WrapText = true;
            myWorksheet.Cells[1, 3].Style.WrapText = true;
            myWorksheet.Cells[1, 5].Style.WrapText = true;

            myWorksheet.Cells["A1:B1"].Merge = true;
            myWorksheet.Cells["C1:D1"].Merge = true;
            myWorksheet.Cells["E1:F1"].Merge = true;

            myWorksheet.Cells[2, 1].Value = "Номер";
            myWorksheet.Cells[2, 2].Value = "Имя";
            myWorksheet.Cells[2, 3].Value = "Имя";
            myWorksheet.Cells[2, 4].Value = "Пароль";
            myWorksheet.Cells[2, 5].Value = "Время отправки";
            myWorksheet.Cells[2, 6].Value = "Текст";



            int t = 3;
            foreach (var VARIABLE in data1)
            {
                myWorksheet.Cells[t, 1].Value = VARIABLE.TableId.ToString();
                myWorksheet.Cells[t, 2].Value = VARIABLE.C_User_In_Room.C_Room.NameRoom;
                myWorksheet.Cells[t, 3].Value = VARIABLE.C_User_In_Room.UserNotType.NameUser;
                myWorksheet.Cells[t, 4].Value = VARIABLE.C_User_In_Room.UserNotType.Password;
                myWorksheet.Cells[t, 5].Value = VARIABLE.time_mess.ToString();
                myWorksheet.Cells[t, 6].Value = VARIABLE.text_mess;
                t += 1;
            }

            str = app_path_directory + @"RepotsMessage\";
            Directory.CreateDirectory(str);

            name_file = @"NewReport" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + @".xlsx";
            var file = File.Create(str + name_file);
            file.Close();
            var fi = new FileInfo(str + name_file);
            xlPackage.SaveAs(fi);


            return str + name_file;
        }
        //create report of the group
        public string ViewGroup()
        {
            dbb _db = new dbb();
            List<C_User_In_Room> data1 = _db.C_User_In_Room.Where(t => t.Admin).ToList();
            if (data1.Any())
            {
                string app_path_directory = AppDomain.CurrentDomain.BaseDirectory;
                string path_to_xlsx = app_path_directory + "Report.xlsx";
                string name_file = "";
                string str = "";
                ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path_to_xlsx));

                var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here

                myWorksheet.Cells[1, 1].Value = "Группы";
                myWorksheet.Cells[1, 3].Value = "Администраторы групп";

                myWorksheet.Cells["A1:B1"].Merge = true;
                myWorksheet.Cells["C1:D1"].Merge = true;

                myWorksheet.Cells[2, 1].Value = "Номер";
                myWorksheet.Cells[2, 2].Value = "Имя";
                myWorksheet.Cells[2, 3].Value = "Имя";
                myWorksheet.Cells[2, 4].Value = "Пароль";

                int t = 3;
                foreach (var VARIABLE in data1)
                {
                    myWorksheet.Cells[t, 1].Value = VARIABLE.TableId.ToString();
                    myWorksheet.Cells[t, 2].Value = VARIABLE.C_Room.NameRoom;
                    myWorksheet.Cells[t, 3].Value = VARIABLE.UserNotType.NameUser;
                    myWorksheet.Cells[t, 4].Value = VARIABLE.UserNotType.Password;
                    t += 1;
                }

                str = app_path_directory + @"RepotsGroup\";
                Directory.CreateDirectory(str);


                name_file = @"NewReport" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + @".xlsx";
                var file = File.Create(str + name_file);
                file.Close();
                var fi = new FileInfo(str + name_file);
                xlPackage.SaveAs(fi);
                return str + name_file;
            }

            return "Нет групп";
        }
        //create report of the Users
        public string ViewUsers()
        {
            dbb _db = new dbb();
            
            string app_path_directory = AppDomain.CurrentDomain.BaseDirectory;
            string path_to_xlsx = app_path_directory + "Report.xlsx";
            string name_file = "";
            string str = "";
            ExcelPackage xlPackage = new ExcelPackage(new FileInfo(path_to_xlsx));

            var myWorksheet = xlPackage.Workbook.Worksheets.First(); //select sheet here

            myWorksheet.Cells[1, 1].Value = "Пользователи";

            myWorksheet.Cells["A1:C1"].Merge = true;

            myWorksheet.Cells[2, 1].Value = "Номер";
            myWorksheet.Cells[2, 2].Value = "Имя";
            myWorksheet.Cells[2, 3].Value = "Пароль";

            int t = 3;
            List<UserNotType> data1 = _db.UserNotType.ToList();
            foreach (var VARIABLE in data1)
            {
                myWorksheet.Cells[t, 1].Value = VARIABLE.Id.ToString();
                myWorksheet.Cells[t, 2].Value = VARIABLE.NameUser;
                myWorksheet.Cells[t, 3].Value = VARIABLE.Password;
                t += 1;
            }

            str = app_path_directory + @"RepotsUsers\";
            Directory.CreateDirectory(str);

            name_file = @"NewReport" + DateTime.Now.ToString("yyyy.MM.dd-HH.mm.ss") + @".xlsx";
            var file = File.Create(str + name_file);
            file.Close();
            var fi = new FileInfo(str + name_file);
            xlPackage.SaveAs(fi);
            return str + name_file;
        }
    }
}
