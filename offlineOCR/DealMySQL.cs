using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
namespace offlineOCR
{
    public class DealMySQL
    {
        private string Config;
        private MySqlConnection conn;
        private Dictionary<string,dynamic> Result=new Dictionary<string, dynamic>()
        {
            {"res",100},{"mess","ok"},{"data",null}
        };
        public DealMySQL()
        {
            JsonCon jsonCon = new JsonCon();
            JsonData jsonData = jsonCon.JsonToObject();
            string user=Base64.DecodeBase64(jsonData.dbname);
            string password = Base64.DecodeBase64(jsonData.dbpass);
            string database = jsonData.db;
            Config = $"server=127.0.0.1;port=3306;user={user};password={password}; database={database};";
             conn = new MySqlConnection(Config);
             
             try
             {
                 conn.Open(); //打开通道，建立连接，可能出现异常,使用try catch语句

             }
             catch (MySqlException ex)
             {
                 Result["res"] = 0;
                 Result["mess"] = ex.Message;
                 
             }
        }
        public Dictionary<string,dynamic> Command(string sql)
        {
            if (Result["res"] != 100) return Result;
            
            MySqlCommand cmd = new MySqlCommand(sql,conn);
            MySqlDataReader reader =cmd.ExecuteReader();//执行ExecuteReader()返回一个MySqlDataReader对象
            try
            {
                int i = 0;
                
                int s1 = reader.FieldCount;
                DataTable Dt=new DataTable();
                string[] configColumn=new string[]{"序号","年份","省份","本专科","图片链接","插入时间"};
                for(int p=0;p<s1;p++)
                {
                    Dt.Columns.Add(configColumn[p],typeof(string));
                }
                while (reader.Read())//Read()函数设计的时候是按行查询，查完一行换下一行。
                {
                   
                    Dt.Rows.Add();
                    for (int j = 0; j < s1; j++)
                    {
                       
                        Dt.Rows[i][j] = reader[j].ToString();
                    }
                   
                    i++;
                }
                Result["data"] = Dt;
            }catch(Exception ex)
            {
                Result["res"] = 10;
                Result["mess"] = ex.Message;
            }
            reader.Close();
            return Result;
        }

        public Dictionary<string,dynamic> insertDate(string sql)
        {
            if (Result["res"] != 100) return Result;
            MySqlCommand cmd = new MySqlCommand(sql,conn);
            cmd.ExecuteNonQuery();
            return Result;
        }
        
    }
}