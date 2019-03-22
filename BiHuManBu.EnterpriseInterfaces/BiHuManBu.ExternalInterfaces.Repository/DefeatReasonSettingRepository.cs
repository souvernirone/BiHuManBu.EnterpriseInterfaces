using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class DefeatReasonSettingRepository : IDefeatReasonSettingRepository
    {
        public dynamic AddDefeatReason(bx_defeatreasonsetting defeatReasonSetting)
        {
            DataContextFactory.GetDataContext().bx_defeatreasonsetting.Add(defeatReasonSetting);
            dynamic result = new System.Dynamic.ExpandoObject();
            result.IsSuccess = DataContextFactory.GetDataContext().SaveChanges() > 0;
            result.Id = defeatReasonSetting.Id;
            return result;
        }

        public string DeleteDefeatReason(bx_defeatreasonsetting defeatReasonSetting)
        {
            var message = string.Empty;
            var defeatReasonSettingModel = DataContextFactory.GetDataContext().bx_defeatreasonsetting.SingleOrDefault(x => x.Id == defeatReasonSetting.Id);

            message = CheckDefeatReasonIsCanChange(defeatReasonSettingModel);
            if (message == string.Empty)
            {

                defeatReasonSettingModel.Deleted = defeatReasonSetting.Deleted;

                if (DataContextFactory.GetDataContext().SaveChanges() > 0)
                {
                    message = "删除成功";
                }
            }
            return message;
        }

        public string EditDefeatReason(bx_defeatreasonsetting defeatReasonSetting)
        {
            string message = string.Empty;
            var defeatReasonSettingModel = DataContextFactory.GetDataContext().bx_defeatreasonsetting.SingleOrDefault(x => x.Id == defeatReasonSetting.Id);

            message = CheckDefeatReasonIsCanChange(defeatReasonSettingModel);
            if (message == string.Empty)
            {
                defeatReasonSettingModel.DefeatReason = defeatReasonSetting.DefeatReason;
                defeatReasonSettingModel.UpdateTime = defeatReasonSetting.UpdateTime;
                if (DataContextFactory.GetDataContext().SaveChanges() >= 0)
                {
                    message = "更新成功";
                }
            }
            return message;
        }

        public List<DefeatReasonSettingViewModel> GetDefeatReasonSetting(int agentId)
        {
            return DataContextFactory.GetDataContext().bx_defeatreasonsetting.Where(x => x.AgentId == agentId && x.Deleted==false).Select(x => new DefeatReasonSettingViewModel { AgentId = x.AgentId, DefeatReason = x.DefeatReason, DefeatReasonId = x.Id, IsChange = x.IsChange }).ToList();
        }
        private string CheckDefeatReasonIsCanChange(bx_defeatreasonsetting defeatReasonSetting)
        {
            var message = string.Empty;
            if (defeatReasonSetting == null)
            {
                message = "无此战败原因";
            }
            else if (!defeatReasonSetting.IsChange) { message = "此战败原因不可操作"; }
            return message;
        }
    }
}
