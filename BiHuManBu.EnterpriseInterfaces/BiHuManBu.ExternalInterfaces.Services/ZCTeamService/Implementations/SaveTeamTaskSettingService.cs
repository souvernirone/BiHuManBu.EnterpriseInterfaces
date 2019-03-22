using System;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Repository;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class SaveTeamTaskSettingService: ISaveTeamTaskSettingService
    {
        private readonly ITeamTaskSettingRepository _teamTaskSettingRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        public SaveTeamTaskSettingService(ITeamTaskSettingRepository teamTaskSettingRepository)
        {
            _teamTaskSettingRepository = teamTaskSettingRepository;
        }
        public BaseViewModel SaveTeamTaskSetting(int agentCount, int orderCount)
        {
            BaseViewModel viewModel = new BaseViewModel();
            try
            {
                bx_zc_team_task_setting model = new bx_zc_team_task_setting();
                model.agent_count = agentCount;
                model.order_count = orderCount;
                model.create_time = DateTime.Now;
                model.is_delete = 0;
                //先删除
                int del = _teamTaskSettingRepository.Del();
                //再创建
                int add = _teamTaskSettingRepository.Add(model);
                if (add > 0)
                {
                    viewModel.BusinessStatus = 1;
                    viewModel.StatusMessage = "创建成功";
                }
                else
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "创建失败";
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
                viewModel.BusinessStatus = -100003;
                viewModel.StatusMessage = "创建模型异常，请重试";
            }
            return viewModel;
        }

    }
}
