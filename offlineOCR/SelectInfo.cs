using Community.CsharpSqlite;

namespace offlineOCR
{
    public class SelectInfo
    {
        public string Year { get; set; }
        public string Province { get; set; }
        public string Level { get; set; }
        public string StudentName{ get; set; }
        public SelectInfo GetAll()
        {
            return this;
        }

        public string ToString()
        {
            return this.Year + " " + this.Province + " " + this.Level;
        }
    }
}