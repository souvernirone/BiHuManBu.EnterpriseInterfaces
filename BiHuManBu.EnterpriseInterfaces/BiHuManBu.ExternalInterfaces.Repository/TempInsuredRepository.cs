using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TempInsuredRepository : ITempInsuredRepository
    {
        public async Task<bx_tempinsuredinfo> GetTempInsuredInfoAsync(int agentId, long buId)
        {

            using (var _dbContext = new EntityContext())
            {
                return await Task.Run(() =>
                {
                    if (agentId != -1)
                    {
                        return _dbContext.bx_tempinsuredinfo.Where(x => x.AgentId == agentId && !x.Deleted).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    }
                    else
                    {
                        return _dbContext.bx_tempinsuredinfo.Where(x => !x.Deleted && x.BuId == buId).OrderByDescending(x => x.UpdateTime).FirstOrDefault();
                    }
                });
            }
        }

        public async Task<bool> SaveTempInsuredInfoAsync(bx_tempinsuredinfo tempInsuredInfo, bool isEdit)
        {
            using (var _dbContext = new EntityContext())
            {
                if (!isEdit)
                {
                    _dbContext.bx_tempinsuredinfo.Add(tempInsuredInfo);
                }
                else
                {
                    _dbContext.Set<bx_tempinsuredinfo>().Attach(tempInsuredInfo);
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("DetailInfo").IsModified = true;
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("UpdateTime").IsModified = true;
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("InsuredName").IsModified = true;
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("LicenseNo").IsModified = true;
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("Deleted").IsModified = true;
                    _dbContext.Entry<bx_tempinsuredinfo>(tempInsuredInfo).Property("BuId").IsModified = true;
                }
                return await Task.Run(() =>
                {
                    return _dbContext.SaveChanges() >= 0;
                });
            }

        }

        public bool AddUserExpand(bx_userinfo_expand userinfoExpand)
        {
            using (var _dbContext = new EntityContext())
            {
                _dbContext.bx_userinfo_expand.Add(userinfoExpand);
                return _dbContext.SaveChanges() >= 0;
            }
        }

        public bool UpdateUserExpand(bx_userinfo_expand userinfoExpand)
        {
            using (var _dbContext = new EntityContext())
            {
                _dbContext.Set<bx_userinfo_expand>().Attach(userinfoExpand);
                _dbContext.Entry<bx_userinfo_expand>(userinfoExpand).Property("b_uid").IsModified = true;
                _dbContext.Entry<bx_userinfo_expand>(userinfoExpand).Property("is_temp_email").IsModified =
                    userinfoExpand.is_temp_email != -1;
                _dbContext.Entry<bx_userinfo_expand>(userinfoExpand).Property("is_temp_mobile").IsModified =
                    userinfoExpand.is_temp_mobile != -1;
                return _dbContext.SaveChanges() >= 0;
            }

        }

        public bx_userinfo_expand GetUserExpand(long buid)
        {
            return
                DataContextFactory.GetDataContext()
                    .bx_userinfo_expand.Where(i => i.b_uid == buid)
                    .OrderByDescending(x => x.id)
                    .FirstOrDefault();
        }

    }
}
