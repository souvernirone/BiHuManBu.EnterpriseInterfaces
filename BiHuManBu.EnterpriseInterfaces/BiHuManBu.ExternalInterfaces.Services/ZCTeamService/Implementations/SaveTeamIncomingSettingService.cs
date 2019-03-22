using System;
using System.Collections.Generic;
using System.Text;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class SaveTeamIncomingSettingService : ISaveTeamIncomingSettingService
    {
        private readonly ITeamIncomingSettingRepository _teamIncomingSettingRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        public SaveTeamIncomingSettingService(ITeamIncomingSettingRepository teamIncomingSettingRepository)
        {
            _teamIncomingSettingRepository = teamIncomingSettingRepository;
        }
        public BaseViewModel SaveTeamIncomingSetting(List<TeamIncomingSetting> teamIncomingSetting)
        {
            BaseViewModel viewModel = new BaseViewModel();
            try
            {
                //先删除
                int del = _teamIncomingSettingRepository.Del();
                //再创建，拼接个sql
                StringBuilder sb = new StringBuilder();
                foreach (var i in teamIncomingSetting)
                {
                    sb.Append(GetSql(i.LevelId, i.PreMiumFrom, i.PreMiumTo, i.RewardTwoLevel, i.RewardThreeLevel));
                }
                int add = _teamIncomingSettingRepository.Add(sb.ToString());
                //if (add > 0)
                //{
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "创建成功";
                //}
                //else
                //{
                //    viewModel.BusinessStatus = 0;
                //    viewModel.StatusMessage = "创建失败";
                //}
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -100003;
                viewModel.StatusMessage = "创建模型异常，请重试";
            }
            return viewModel;
        }

        private string GetSql(int level_id, double premium_from, double premium_to, double reward_two_level, double reward_three_level)
        {
            string sql = string.Format("insert into bx_zc_team_incoming_setting (level_id,premium_from,premium_to,reward_two_level,reward_three_level,setting_status,create_time,update_time) values ({0},{1},{2},{3},{4},{5},'{6}','{7}');",
                level_id, premium_from, premium_to, reward_two_level, reward_three_level, 1, DateTime.Now, DateTime.Now);
            return sql;
        }
    }
}
