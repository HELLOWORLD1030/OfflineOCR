using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace offlineOCR
{
    public class ParmeterToSql
    {
        private  int Flag;
        // private SelectInfo par;
        public  ParmeterToSql(SelectInfo list)
        {
            if (list.StudentName == null)
            {
               Flag= parToFlag(list);
            }
            else
            {
                Flag = 9;
            }
            
        }

        public string GetSQLStr(SelectInfo par)
        {
            string Result = "";
            switch (Flag)
            {
                case 4:
                Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE province='{par.Province}' and level='{par.Level}' ";
                break;
                case 2:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE year='{par.Year}' and level='{par.Level}' ";
                    break;
                case 1:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE province='{par.Province}' and year='{par.Year}' ";
                    break;
                case 6:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE  level='{par.Level}' ";
                    break;
                case 5:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE  province='{par.Province}' ";
                    break;
                case 3:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE  year='{par.Year}' ";
                    
                    break;
                case 7:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE 1 ";
                    break;
                case 0 :
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE year='{par.Year}' and level='{par.Level}' and province='{par.Province}' ";
                    break;
                case 9:
                    Result =$"SELECT `id`, `year`, `province`, `level`, `imagelink`, `createtime` FROM `offline` WHERE imagetext like'%{par.StudentName}%' ";
                    break;
            }
        
            return Result;
        }

        public  int getFlag()
        {
            return Flag;
        }

        public static bool CheckPar(SelectInfo select)
        {
            return parToFlag(select)==0;
        }
        public static bool ObjectIsNullOrEmpty<T>(T t) 
        {
            foreach (var item in t.GetType().GetProperties()) 
            {
                if (item.GetValue(t) == null)
                {
                    return true;
                }
                if (item.GetValue(t).ToString() == "")
                {
                    return true;
                }
            }
            return false;
        }

        public static int parToFlag(SelectInfo t)
        {
            List<string> property = new List<string>();
            foreach (var item in t.GetType().GetProperties()) 
            {
                if (item.GetValue(t) == null)
                {
                    property.Add(item.Name);
                }
                if (item.GetValue(t)?.ToString() == "")
                {
                    property.Add(item.Name);
                }
            }
            
            Console.WriteLine(property);
            int Flag = 0;
            foreach (string po in property)
            {
                switch (po)
                {
                    case "Year":
                        Flag += 4;
                        break;
                    case "Province":
                        Flag += 2;
                        break;
                    case "Level":
                        Flag += 1;
                        break;
                    default:
                        Flag += 0;
                        break;
                }
            }

            return Flag;
        }

       
        
    }
}