using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ReportModel
{
    public class GSCClaimsListInfo
    {
        public string AccidentPlace { get; set; }    //出险地点
        public string AccidentPsss { get; set; } //出险经过
        public string DriverName { get; set; }   //驾驶员姓名
        public int IsCommerce { get; set; }  //是否商业
        public int IsOwners { get; set; }    //是否车主
        public int IsThreecCar { get; set; }  //是否三者
        public string ReportDate { get; set; }   //报案时间 
    } 
}
