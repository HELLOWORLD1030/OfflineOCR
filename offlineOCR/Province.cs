using System;
using System.Collections.Generic;
using System.Text;

namespace offlineOCR
{
    class Province
    {
        public string Key { get; set; }
        public string Value { get; set; }
        public  static List<Province> GetProvinceList()
        {
            List<Province> Result = new List<Province>();
            Result.Add(new Province(){Key = "",Value = ""});
            Result.Add(new Province() { Key = "北京市", Value = "北京市" });
            Result.Add(new Province() { Key = "天津市", Value = "天津市" });
            Result.Add(new Province() { Key = "上海市", Value = "上海市" });
            Result.Add(new Province() { Key = "重庆市", Value = "重庆市" });
            Result.Add(new Province() { Key = "内蒙古", Value = "内蒙古" });
            Result.Add(new Province() { Key = "广西", Value = "广西" });
            Result.Add(new Province() { Key = "西藏", Value = "西藏" });
            Result.Add(new Province() { Key = "宁夏", Value = "宁夏" });
            Result.Add(new Province() { Key = "新疆", Value = "新疆" });
            Result.Add(new Province() { Key = "河北", Value = "河北" });
            Result.Add(new Province() { Key = "山西", Value = "山西" });
            Result.Add(new Province() { Key = "辽宁", Value = "辽宁" });
            Result.Add(new Province() { Key = "吉林", Value = "吉林" });
            Result.Add(new Province() { Key = "黑龙江", Value = "黑龙江" });
            Result.Add(new Province() { Key = "江苏", Value = "江苏" });
            Result.Add(new Province() { Key = "浙江", Value = "浙江" });
            Result.Add(new Province() { Key = "安徽", Value = "安徽" });
            Result.Add(new Province() { Key = "福建", Value = "福建" });
            Result.Add(new Province() { Key = "江西", Value = "江西" });
            Result.Add(new Province() { Key = "山东", Value = "山东" });
            Result.Add(new Province() { Key = "河南", Value = "河南" });
            Result.Add(new Province() { Key = "湖北", Value = "湖北" });
            Result.Add(new Province() { Key = "湖南", Value = "湖南" });
            Result.Add(new Province() { Key = "广东", Value = "广东" });
            Result.Add(new Province() { Key = "海南", Value = "海南" });
            Result.Add(new Province() { Key = "四川", Value = "四川" });
            Result.Add(new Province() { Key = "贵州", Value = "贵州" });
            Result.Add(new Province() { Key = "云南", Value = "云南" });
            Result.Add(new Province() { Key = "陕西", Value = "陕西" });
            Result.Add(new Province() { Key = "甘肃", Value = "甘肃" });
            Result.Add(new Province() { Key = "青海", Value = "青海" });
            return Result;
        }
    }
}
