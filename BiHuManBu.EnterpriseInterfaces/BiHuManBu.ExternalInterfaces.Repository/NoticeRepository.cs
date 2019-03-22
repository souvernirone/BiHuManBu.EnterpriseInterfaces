
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using ServiceStack.Text;
using BiHuManBu.ExternalInterfaces.Infrastructure.MySqlDbHelper;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class NoticeRepository : INoticeRepository
    {
        private static readonly string Dbconfig = ConfigurationManager.ConnectionStrings["zb"].ConnectionString;
        private static readonly MySqlHelper _dbHelper = new MySqlHelper(Dbconfig);
        private ILog logInfo = LogManager.GetLogger("INFO");
        private ILog logError = LogManager.GetLogger("ERROR");
        public int ShareBatchRenewal(string buids)
        {
            try
            {
                GetNinetyDaysXuBao(buids);
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                return -1;
            }
            return 1;
        }
        /// <summary>
        /// </summary>
        /// <param name="type"> 1,90--今天,2，60-今天，3，30-今天</param>
        /// <returns></returns>
        public void GetNinetyDaysXuBao(string buids)
        {
            try
            {

                string sqlUpdate =
                    string.Format(
                        @"
UPDATE bx_notice_xb SET day_num=(TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE())),
days= CASE WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>=0 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=30 THEN 3 
                             WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>30 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=60 THEN 2 
                            WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>60 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=90 THEN 1 END   WHERE b_uid IN ({0});",
                        buids);
                int num = _dbHelper.ExecuteNonQuery(sqlUpdate);
                string sqlSelect = string.Format(@"select GROUP_CONCAT(b_uid)as buids from bx_notice_xb where b_uid   in({0})", buids);
                var existBuids = _dbHelper.ExecuteScalar<string>(sqlSelect);
                string[] b = buids.Split(',');
                var buidsList = new List<string>(b);
                string[] existBuidsArray = existBuids.Split(',');
                var existBuidsList = new List<string>(existBuidsArray);
                buidsList.AddRange(existBuidsList);
                List<string> result = buidsList.Concat(existBuidsList).Distinct().ToList();
                result.Remove("");
                string resultBuid = result.Join(",");
                string sqlInsert =
                    string.Format(
                        @"INSERT INTO bx_notice_xb(license_no, create_time, stauts,last_force_end_date, last_biz_end_date, next_force_start_date, next_biz_start_date, source, agent_id, 
                            days, b_uid,day_num) 
                            SELECT a.license_no,a.create_time,a.stauts,a.last_force_end_date,a.Last_biz_end_date,a.next_force_start_date,a.next_biz_start_date ,a.source,a.Agent AS agent_id,a.days,a.b_uid  ,day_num
                            FROM (
                            SELECT bx_userinfo.LicenseNo AS license_no,NOW() AS create_time,0 AS stauts,bx_userinfo.Agent,LastForceEndDate AS last_force_end_date,lastBizEndDate
                            AS last_biz_end_date,NextForceStartDate AS next_force_start_date,NextBizStartDate AS next_biz_start_date,
                            bx_car_renewal.LastYearSource AS source,
                             CASE WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>=0 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=30 THEN 3 
                             WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>30 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=60 THEN 2 
                            WHEN (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))>60 AND 
                            (TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))<=90 THEN 1 END   AS days,
                            bx_userinfo.id AS b_uid  ,(TO_DAYS(bx_notice_xb.last_force_end_date) - TO_DAYS(CURDATE()))AS day_num
                            FROM bx_userinfo 
                            LEFT JOIN bx_userinfo_renewal_index ON bx_userinfo.id=bx_userinfo_renewal_index.b_uid  
                            LEFT JOIN bx_car_renewal ON bx_userinfo_renewal_index.car_renewal_id=bx_car_renewal.Id 
                            where bx_userinfo.QuoteStatus=-1
                            AND bx_userinfo.id in ({0}) AND bx_car_renewal.LastForceEndDate<=DATE_ADD(NOW(), INTERVAL 90 DAY) AND bx_userinfo.Agent IS NOT NULL AND bx_car_renewal.LastForceEndDate>=DATE_ADD(NOW(), INTERVAL 0 DAY)
                            ) as a ", resultBuid);

                int insertnum = _dbHelper.ExecuteNonQuery(sqlInsert);

                logInfo.Info("更新条数：" + num + "buids:" + buids + "插入条数：" + insertnum + "buids:" + resultBuid);
                //                int days = 0;
                //                switch (type)
                //                {
                //                    case 1:
                //                        days = 90;
                //                        break;
                //                    case 2:
                //                        days = 60;
                //                        break;
                //                    case 3:
                //                        days = 30;
                //                        break;
                //                }




                //                string sqlDeleteWhere = string.Format(@" where 1=1 and  a.b_uid in ({0})", buids);
                //                if (days == 2)
                //                {
                //                    sqlDeleteWhere += " and days=1";

                //                }
                //                else if (days == 3)
                //                {
                //                    sqlDeleteWhere += " and days in(2,1)";

                //                }
                //                logInfo.Info("批量续保通知:" + days + "buids" + buids);
                //                string sqlDelete = string.Format(@"DELETE  FROM bx_notice_xb WHERE b_uid IN (
                //SELECT a.b_uid 
                //FROM (
                //SELECT 
                //bx_userinfo.id AS b_uid
                //FROM bx_userinfo 
                //INNER JOIN bx_car_renewal ON bx_userinfo.LicenseNo=bx_car_renewal.LicenseNo AND bx_car_renewal.Id=
                //(SELECT id FROM bx_car_renewal WHERE bx_car_renewal.LicenseNo=bx_userinfo.LicenseNo  ORDER BY  RenewalYear DESC LIMIT 0,1) 
                //AND bx_userinfo.QuoteStatus=-1
                //AND bx_car_renewal.LastForceEndDate<=DATE_ADD(NOW(), INTERVAL {1} DAY) AND bx_userinfo.Agent IS NOT NULL AND bx_car_renewal.LastForceEndDate>=DATE_ADD(NOW(), INTERVAL {2} DAY)
                //LEFT JOIN bx_notice_xb ON bx_notice_xb.b_uid=bx_userinfo.Id AND bx_notice_xb.days={0} 
                //where bx_notice_xb.id IS NULL) AS a  {3} ) ", type, days, days - 30, sqlDeleteWhere);
                //                if (days != 1)
                //                {
                //                    _dbHelper.ExecuteNonQuery(sqlDelete);
                //                }
                //                string sql = string.Format(@"
                //                      INSERT INTO bx_notice_xb(license_no, create_time, stauts,last_force_end_date, last_biz_end_date, next_force_start_date, next_biz_start_date, source, agent_id, 
                //                days, b_uid,day_num) 
                //                SELECT a.license_no,a.create_time,a.stauts,a.last_force_end_date,a.Last_biz_end_date,a.next_force_start_date,a.next_biz_start_date ,a.source,a.Agent AS agent_id,a.days,a.b_uid  ,day_num
                //                FROM (
                //                SELECT bx_userinfo.LicenseNo AS license_no,NOW() AS create_time,0 AS stauts,bx_userinfo.Agent,LastForceEndDate AS last_force_end_date,lastBizEndDate
                //                AS last_biz_end_date,NextForceStartDate AS next_force_start_date,NextBizStartDate AS next_biz_start_date,
                //                bx_car_renewal.LastYearSource AS source,{0} AS days,
                //                bx_userinfo.id AS b_uid  ,((UNIX_TIMESTAMP(bx_car_renewal.LastForceEndDate) - UNIX_TIMESTAMP(CURDATE()))/(60*60*24))AS day_num
                //                FROM bx_userinfo 
                //                INNER JOIN bx_car_renewal ON bx_userinfo.LicenseNo=bx_car_renewal.LicenseNo AND bx_car_renewal.Id=
                //(SELECT id FROM bx_car_renewal WHERE bx_car_renewal.LicenseNo=bx_userinfo.LicenseNo  ORDER BY  RenewalYear DESC LIMIT 0,1)
                //                     and bx_userinfo.QuoteStatus=-1
                //                AND bx_car_renewal.LastForceEndDate<=DATE_ADD(NOW(), INTERVAL {1} DAY) AND bx_userinfo.Agent IS NOT NULL AND bx_car_renewal.LastForceEndDate>=DATE_ADD(NOW(), INTERVAL {2} DAY)
                //                LEFT JOIN bx_notice_xb ON bx_notice_xb.b_uid=bx_userinfo.Id AND bx_notice_xb.days={0} 
                // 
                //                where bx_notice_xb.id IS NULL) AS a  {3} 
                //                 ", type, days, days - 30, sqlDeleteWhere);


                //                int num = _dbHelper.ExecuteNonQuery(sql);
                //int numMessage = _dbHelper.ExecuteNonQuery(sqlBxmessage);

            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);

            }
        }



        public string GetBuidsForBatchId(int batchId)
        {
            try
            {
                string sql = String.Format(@"
                                    SELECT 	 
                                    GROUP_CONCAT(BUId)
                                    FROM 
                                    bx_batchrenewal_item where BatchId ={0}", batchId);
                return _dbHelper.ExecuteScalar(sql).ToString();
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return "-1";

        }
    }
}
