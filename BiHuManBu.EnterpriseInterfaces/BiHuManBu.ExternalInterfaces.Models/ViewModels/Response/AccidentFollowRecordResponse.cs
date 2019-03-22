using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
 public  class AccidentFollowRecordResponse
    {

        public AccidentFollowRecordModel Data { get; set; }
        

        public int Code { get; set; }

        public string Message { get; set; }

    }

    public class AccidentFollowRecordResponseV2
    {

        public AccidentFollowRecordModelV2 Data { get; set; }


        public int Code { get; set; }

        public string Message { get; set; }

    }



    public class AccidentFollowRecordModelV2
    {
        public List<AccidentFollowRecordVM> list { get; set; }
    }

    public class AccidentFollowRecordModel
    {
        public List<AccidentFollowRecord> list { get; set; }
    }


    public class AccidentFollowRecordVM
    {
        public string CreateDate { get; set; }
        public List<AccidentFollowRecord> Data { get; set; }
    }


    public class AccidentFollowRecord {
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 短信内容
        /// </summary>

        public string SmsContent { get; set; }

        /// <summary>
        /// 跟进状态：-1：未跟进、1:跟进中、2:上门接车、3:车辆到店、4:流失、5:发短信（跟进中）、6:打电话（跟进中）
        /// </summary>

        public int State { get; set; }


        public string StateDesc { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>

        public string CreateTime { get; set; }

        /// <summary>
        /// 下次跟进时间
        /// </summary>

        public string NextFollowUpTime { get; set; }

        /// <summary>
        /// 接车地点
        /// </summary>
        public string ReciveCarAea { get; set; }

        /// <summary>
        /// 到店方式：1：接车到店、2：客户自行到店
        /// </summary>
        public int ArrivalType { get; set; }
        /// <summary>
        /// 接车人
        /// </summary>
        public string ReciveCarAgent { get; set; }
        /// <summary>
        /// 流失原因
        /// </summary>
        public string LossReason { get; set; }
        /// <summary>
        /// 转接人
        /// </summary>
        public string AgentName { get; set; }
        /// <summary>
        /// 事故备注
        /// </summary>
        public string Remark { get; set; }

        public int ReciveCarAgentId { get; set; }

        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get; set; }

        public string ExpectArrivalTime { get; set; }

        public string ExpectedFinishedTime { get; set; }
    }
}
