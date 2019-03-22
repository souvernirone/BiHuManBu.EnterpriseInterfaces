using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
   public class UpdateBatchRenewalItemModel
    {
        private long _id;
       /// <summary>
       /// 批次编号
       /// </summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private int _itemStatus;
       /// <summary>
       /// 执行状态
       /// </summary>
        public int ItemStatus
        {
            get { return _itemStatus; }
            set { _itemStatus = value; }
        }

        private DateTime? _updateTime;
       /// <summary>
       /// 更新时间
       /// </summary>
        public DateTime? UpdateTime
        {
            get {
                return _updateTime.HasValue?_updateTime.Value:DateTime.Now; 
            }
            set { _updateTime = value; }
        }
    }
}
