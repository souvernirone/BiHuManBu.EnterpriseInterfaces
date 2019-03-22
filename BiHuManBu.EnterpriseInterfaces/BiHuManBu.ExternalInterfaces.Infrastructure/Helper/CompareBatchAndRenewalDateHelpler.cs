namespace BiHuManBu.ExternalInterfaces.Infrastructure.Helper
{
    /// <summary>
    /// 续保和批续时间比较工具
    /// </summary>
    public static class CompareBatchAndRenewalDateHelpler
    {
        /// <summary>
        /// 获取最新商业险时间
        /// </summary>
        /// <returns></returns>
        public static string GetLastBizEndDate()
        {
            return CompareTemplate("cr.LastBizEndDate", "bx_batchrenewal_item.BizEndDate");
        }
        public static string GetLastBizEndMonth()
        {
            return CompareTemplate("Month(cr.LastBizEndDate)", "Month(bx_batchrenewal_item.BizEndDate)");
        }

        /// <summary>
        /// 获取最新交强险时间
        /// </summary>
        /// <returns></returns>
        public static string GetLastForceEndDate()
        {
            return CompareTemplate("cr.LastForceEndDate", "bx_batchrenewal_item.ForceEndDate");
        }
        /// <summary>
        /// 获取月份交强险到期时间
        /// </summary>
        /// <returns></returns>
        public static string GetLastForceEndMonth()
        {
            return CompareTemplate("Month(cr.LastForceEndDate)", "Month(bx_batchrenewal_item.ForceEndDate)");
        }
        /// <summary>
        /// 判断是采用批续的
        /// </summary>
        /// <returns></returns>
        public static string GetIsBatch()
        {
            return CompareTemplate("0", "1");
        }       
        /// <summary>
        /// 品牌型号
        /// </summary>
        /// <returns></returns>
        public static string GetMoldName()
        {
            return CompareTemplateBatchFirst("ui.MoldName", "bx_batchrenewal_item.MoldName");
        }

        /// <summary>
        /// 上年投保公司
        /// </summary>
        /// <returns></returns>
        public static string GetLastYearSource()
        {
            return CompareTemplate("ui.LastYearSource", "bx_batchrenewal_item.LastYearSource");
        }

        /// <summary>
        /// 判断是否有交强或商业到期时间
        /// </summary>
        /// <param name="type">0：交强 1：商业</param>
        /// <returns></returns>
        public static string GetValueLastEndDate(int type)
        {
            var template = @"
                IF(
                    YEAR({0})>1970,
                    1,
                    0
                )
                ";
            var endDate = string.Empty;
            switch (type)
            {
                case 0:
                    endDate = GetLastForceEndDate();
                    break;
                case 1:
                    endDate = GetLastBizEndDate();
                    break;
                default:
                    endDate = GetLastBizEndDate();
                    break;
            }

            return string.Format(template, endDate);
        }

        /// <summary>
        /// 续保和批续比较的模板
        /// </summary>
        /// <param name="renewalValue">续保的值</param>
        /// <param name="batchValue">批续的值</param>
        /// <returns></returns>
        private static string CompareTemplate(string renewalValue, string batchValue)
        {
            // 先判断续保是否成功，成功：比较时间。失败：使用批续的
            // 比较时间，先判断批续的年份儿是否<=1970，是：使用续保的，否：比较续保年份儿和批续年份儿，那个大用那个
            var template = @"
            IF(
                ui.renewalstatus=1,
                IF(
                    YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                    {1},
                    IF(
                        {0},
                        {2},
                        {1}
                    )
                ),
                {2}
            )
            ";
            return string.Format(template, GetLastBizEndDateKernel(), renewalValue, batchValue);
        }
     
        /// <summary>
        /// 先判断是否是批续的
        /// 目前是用来获取车辆品牌型号时使用
        /// </summary>
        /// <param name="renewalValue"></param>
        /// <param name="batchValue"></param>
        /// <returns></returns>
        private static string CompareTemplateBatchFirst(string renewalValue, string batchValue)
        {
            var template = @"
            IF(
                YEAR(bx_batchrenewal_item.BizEndDate)<=1970,
                {1},
                IF(
                        {0},
                        {2},
                        {1}
                    )
            )
            ";
            return string.Format(template, GetLastBizEndDateKernel(), renewalValue, batchValue);
        }

        private static string GetLastBizEndDateKernel()
        {
            var bizEndDateKernel = @"
                YEAR(bx_batchrenewal_item.BizEndDate)
                > (
	                IF(
		                ISNULL(cr.LastBizEndDate),
		                0,
		                YEAR(cr.LastBizEndDate)
	                )
                )
                ";
            return bizEndDateKernel;
        }
    }
}
