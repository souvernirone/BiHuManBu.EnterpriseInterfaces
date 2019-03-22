using BiHuManBu.ExternalInterfaces.Infrastructure.Helper;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class GroupAuthenRepository : IGroupAuthenRepository
    {
        private readonly EntityContext db = DataContextFactory.GetDataContext();

        /// <summary>
        /// 根据id查询实体模型 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public GroupAuthenModel GetModel(int id)
        {
            GroupAuthenModel model = new GroupAuthenModel();
            try
            {
                model = db.bx_group_authen.Where(t => t.id == id).Select(t => new GroupAuthenModel()
                {
                    Id = t.id,
                    AgentId = t.agentId,
                    AuthenState = t.authen_state.Value,
                    CardHolder = t.cardholder ?? "",
                    CardId = t.card_id ?? "",
                    BankCardNum = t.bank_card_number ?? "",
                    BankName = t.bank_name ?? "",
                    ChildBankName = t.bank_son_name ?? "",
                    BusinessLicenceUrl = t.business_licence_url ?? "",
                    CardFaceUrl = t.card_face_url ?? "",
                    CardReverseUrl = t.card_reverse_url ?? "",
                    FieldUrl = t.field_url ?? "",
                    BankcardFaceUrl = t.bankcard_face_url ?? "",
                    BankcardReverseUrl = t.bankcard_reverse_url,
                    BankId = t.bank_id ?? 0,
                    //2018-09-18 张克亮 加入头像信息
                    HeadPortrait = t.head_portrait ?? "",
                    Nickname = t.nickname ?? ""
                }).FirstOrDefault();
            }
            catch (Exception ex)
            {
            }

            return model;
        }

        /// <summary>
        /// 2018-09-25 张克亮
        /// 根据身份证号或银行卡号查询实体模型
        /// </summary>
        /// <param name="idcard">身份证号</param>
        /// <param name="bankCardNumber">银行卡号</param>
        /// <returns></returns>
        public BaseViewModel GetModel(string idcard="",string bankCardNumber="")
        {
            BaseViewModel baseModel = new BaseViewModel();
            try
            {
                if (!string.IsNullOrEmpty(idcard))
                {
                    var model = db.bx_group_authen.Where(t => t.card_id == idcard).FirstOrDefault();
                    baseModel.Data = model;

                }
                if (!string.IsNullOrEmpty(bankCardNumber))
                {
                    var model = db.bx_group_authen.Where(t => t.bank_card_number == bankCardNumber).FirstOrDefault();
                    baseModel.Data = model;
                }
            }
            catch 
            {
                baseModel.StatusMessage = "根据身份证号或银行卡号查询绑定情况出现异常";
                baseModel.BusinessStatus = -999;
            }
            return baseModel;
        }

        /// <summary>
        /// 更新实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="model"></param>
        /// <param name="save"></param>
        /// <returns></returns>
        public bool UpdateModel(bx_group_authen model, bool save = true)
        {
            bool result = false;
            try
            {
                db.bx_group_authen.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                if (save)
                {
                    result = db.SaveChanges() > 0;
                }
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                    }
                }

            }
            return result;
        }

        /// <summary>
        /// 添加实体 zky 2017-11-14 /crm
        /// </summary>
        /// <param name="model"></param>
        /// <param name="save"></param>
        /// <returns></returns>
        public bool AddModel(bx_group_authen model, bool save = true)
        {
            bool result = false;
            db.bx_group_authen.Attach(model);
            db.Entry(model).State = EntityState.Added;
            if (save)
            {
                result = db.SaveChanges() > 0;
            }
            return result;
        }

        /// <summary>
        /// 执行保存 zky 2017-11-14 /crm
        /// </summary>
        /// <returns></returns>
        public int SaveChange()
        {
            return db.SaveChanges();
        }

        /// <summary>
        /// 根据条件查询列表 zky 2017-11-14
        /// </summary>
        /// <param name="whereLamda"></param>
        /// <returns></returns>
        public IQueryable<bx_group_authen> GetList(Expression<Func<bx_group_authen, bool>> whereLamda)
        {
            return db.bx_group_authen.Where(whereLamda);
        }

        /// <summary>
        /// 根据id查询 zky 2017-11-14
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bx_group_authen Get(int id)
        {
            return db.bx_group_authen.Where(t => t.id == id).FirstOrDefault();
        }
        /// <summary>
        /// 根据ahentId查询 sjy 2018-2-4
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bx_group_authen GetByAgentId(int id)
        {
            return db.bx_group_authen.Where(t => t.agentId == id).FirstOrDefault();
        }
        /// <summary>
        /// 查询所有的银行 zky 2018-1-2/crm
        /// </summary>
        /// <returns></returns>
        public IList<bx_bank> GetAllBank()
        {
            return db.bx_bank.ToList();
        }

        /// <summary>
        /// 工号管理列表 zky 2018-1-2/crm
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="ukeyName"></param>
        /// <param name="cityName"></param>
        /// <param name="orgName"></param>
        /// <returns></returns>
        public IList<JobNumberDto> JobNumberList(int agentId, string ukeyName, string cityName, string orgName, int pageIndex, int pageSize, int cityId, int ukeyId, int groupId, out int total)
        {
            string sqlWhere = string.Empty;
            if (agentId > 0)
            {
                sqlWhere += " and agent.id=?agentId";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(orgName))
                {
                    sqlWhere += " and agent.agentName like concat('%',?orgName,'%')";
                }
            }

            if (cityId > 0)
            {
                sqlWhere += " and city.id=?cityId";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(cityName))
                {
                    sqlWhere += " and city.city_name like concat('%',?cityName,'%')";
                }
            }
            if (ukeyId > 0)
            {
                sqlWhere += " and ukey.id=?ukeyId";
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(ukeyName))
                {
                    sqlWhere += " and ukey.NAME like concat('%',?ukeyName,'%')";
                }
            }

            string sql = @"SELECT
                            agent.id AS OrgId,
	                        agent.AgentName as OrgName,
	                        ukey.NAME AS UkeyName,
                            ukey.id as UkeyId,
	                        city.city_name AS CityName,
                            city.id as CityId,
	                        company.name as CompanyName
                        FROM
                            bx_agent_ukey ukey
                        INNER JOIN bx_agent agent ON ukey.agent_id = agent.Id
                        INNER JOIN bx_companyrelation company ON company.source = ukey.source
                        INNER JOIN bx_city city ON ukey.city_id = city.id
                        WHERE
                            agent.group_id =?groupId ";
            MySqlParameter[] param = new MySqlParameter[]
            {
                new MySqlParameter{ParameterName="ukeyName",Value=ukeyName, MySqlDbType = MySqlDbType.VarChar},
                new MySqlParameter{ParameterName="ukeyId",Value=ukeyId, MySqlDbType = MySqlDbType.Int32},
                new MySqlParameter{ParameterName="orgName",Value=orgName, MySqlDbType = MySqlDbType.VarChar},
                new MySqlParameter{ParameterName="agentId",Value=agentId, MySqlDbType = MySqlDbType.Int32},
                new MySqlParameter{ParameterName="groupId",Value=groupId, MySqlDbType = MySqlDbType.Int32},
                new MySqlParameter{ParameterName="cityName",Value=cityName, MySqlDbType = MySqlDbType.VarChar},
                new MySqlParameter{ParameterName="cityId",Value=cityId, MySqlDbType = MySqlDbType.Int32},

            };

            var query = db.Database.SqlQuery<JobNumberDto>(sql + sqlWhere, param);
            total = query.Count();
            return query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
        }

        public IList<JobNumberDto> CityInfoAndUkeyInfo(int agentId)
        {
            string sql = @"SELECT
                            agent.id AS OrgId,
	                        agent.AgentName AS OrgName,
	                        ukey.NAME AS UkeyName,
                            ukey.id as UkeyId,
	                        city.city_name AS CityName,
                            city.id as CityId,
	                        company.name as CompanyName
                        FROM
                            bx_agent_ukey ukey
                        INNER JOIN bx_agent agent ON ukey.agent_id = agent.Id
                        INNER JOIN bx_companyrelation company ON company.source = ukey.source
                        INNER JOIN bx_city city ON ukey.city_id = city.id
                        WHERE
                            agent.group_id =?agentId ";

            MySqlParameter[] param = new MySqlParameter[]
           {
                new MySqlParameter{ParameterName="agentId",Value=agentId, MySqlDbType = MySqlDbType.Int32},
           };

            return db.Database.SqlQuery<JobNumberDto>(sql, param).ToList();
        }
        /// <summary>
        /// 更新团队等级
        /// </summary>
        /// <param name="agentId"></param>
        /// <param name="levelId"></param>
        /// <returns></returns>
        public int UpdateGroupLevel(List<TeamLevelViewModel> list) {
            StringBuilder sql = new StringBuilder();
            sql.Append("select 1;");
            list.ForEach(x => 
            {
                sql.Append("UPDATE bx_group_authen SET level_id="+x.LevelId+ ",last_balance_accounts_datetime='"+x.LastBalance.ToShortDateString()+ "' WHERE agentId="+x.AgentId+";");
            });
            return db.Database.ExecuteSqlCommand(sql.ToString());
        }
        public bx_zc_team_incoming_setting GetTeamIncomingSetByLevelId(int levelId)
        {
            bx_zc_team_incoming_setting model = new bx_zc_team_incoming_setting();
            try
            {
                model = DataContextFactory.GetDataContext().bx_zc_team_incoming_setting.Where(a => a.level_id == levelId && a.setting_status == 1).FirstOrDefault();
                return model;
            }
            catch (Exception ex)
            {
                LogHelper.Error("发生异常：" + ex);
            }
            return model;
        }
    }
}
