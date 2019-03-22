using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class RenewalInformationViewModel
    {
        public bx_userinfo_renewal_info UserinfoRenewalInfo { get; set; }

        public bx_consumer_review ConsumerReview { get; set; }

        public bx_quotereq_carinfo QuotereqCarinfo { get; set; }

        public bx_car_renewal CarRenewal { get; set; }

        public bx_carmodel Carmodel { get; set; }

        public bx_carinfo Carinfo { get; set; }

        public bx_agent Agent { get; set; }

        public bx_batchrenewal_item BatchrenewalItem { get; set; }

        public bx_car_renewal_premium CarRenewalPremium { get; set; }
    }
}
