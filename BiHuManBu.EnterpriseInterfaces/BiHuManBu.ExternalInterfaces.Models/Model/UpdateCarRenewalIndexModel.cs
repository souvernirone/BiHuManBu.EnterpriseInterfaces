using System;

namespace BiHuManBu.ExternalInterfaces.Models.Model
{
    public class UpdateCarRenewalIndexModel
    {
        public long BuId { get; set; }
        public long CarRenewalId { get; set; }

        public DateTime CreateTime
        {
            get
            {
                return _createTime;
            }

            set
            {
                _createTime = value;
            }
        }

        private DateTime _createTime = Convert.ToDateTime("2017-3-31 11:59:59");
    }
}
