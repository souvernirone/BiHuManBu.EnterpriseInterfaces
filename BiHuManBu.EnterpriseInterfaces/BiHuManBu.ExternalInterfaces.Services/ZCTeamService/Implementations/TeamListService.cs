using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Messages.ViewModel.ZCTeam;
using BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Interfaces;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;

namespace BiHuManBu.ExternalInterfaces.Services.ZCTeamService.Implementations
{
    public class TeamListService : ITeamListService
    {
        private readonly IAgentRepository _agentRepository;
        private readonly IZCTeamRepository _zcTeamRepository;
        private ILog logError = LogManager.GetLogger("ERROR");
        private ILog logInfo = LogManager.GetLogger("Info");
        public TeamListService(IAgentRepository agentRepository, IZCTeamRepository zcTeamRepository)
        {
            _agentRepository = agentRepository;
            _zcTeamRepository = zcTeamRepository;
        }
        public TeamListViewModel GetTeamList(Messages.Request.ZCTeam.GetTeamManagerRequest request)
        {
            TeamListViewModel viewModel = new TeamListViewModel();
            //1.根据AgentName到表中查询是否存在记录数，如果不存在直接返回
            int recordCount = 0;
            //2.根据请球参数到数据库中查询记录数
            var list = _zcTeamRepository.GetTeamList(request.Agent, request.AgentName, request.Mobile, request.CommissionTimeStart, 
                request.CommissionTimeEnd, request.PageIndex, request.PageSize,out recordCount);
          
            
            //4.返回响应信息
            viewModel.TeamList = list;
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.RecordCount = recordCount;
            viewModel.StatusMessage = "查询成功";
            viewModel.BusinessStatus = 200;
            return viewModel;
        }

        /// <summary>
        /// 获取二级团员保费明细
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TeamChildLevelListViewModel GetTeamChildLevelList(Messages.Request.ZCTeam.GetTeamChildLevelListRequest request)
        {
            TeamChildLevelListViewModel viewModel=new TeamChildLevelListViewModel();
            //1.校验返回值
            //var strUrl = string.Format("ChildAgent={0}&TopAgentId={1}", request.ChildAgent, request.Agent);
            //if (strUrl.GetUrl().GetMd5() != request.SecCode)
            //{
            //    viewModel.BusinessStatus = -10000;
            //    viewModel.StatusMessage = "输入参数错误，请检查您输入的参数是否有空或者长度不符合要求等";
            //    return viewModel;
            //}
            int recordCount = 0;
            //2.根据参数到数据库中查询
            var list = _zcTeamRepository.GetTeamChildLevelList(request.ChildAgent,request.AgentName,request.Mobile ,request.Agent, request.SecCode,
                request.CommissionTimeStart, request.CommissionTimeEnd, request.PageIndex, request.PageSize, out recordCount);
            //4.返回响应信息
            viewModel.TeamChildLevelList = list;
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.RecordCount = recordCount;
            viewModel.StatusMessage = "查询成功";
            viewModel.BusinessStatus = 200;
            return viewModel;
        }

        /// <summary>
        /// 1.当前代理人目前邀请的人数和人员信息
        /// 2.近期邀请的10个人
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public NextLevelAgentListViewModel GetNextLevelAgentList(Messages.Request.ZCTeam.GetNextLevelAgentListRequest request)
        {
            NextLevelAgentListViewModel viewModel = new NextLevelAgentListViewModel();
            int recordCount = 0;

            var list = _zcTeamRepository.GetNextLevelAgentList(request.ChildAgent, request.Agent,
                 request.PageIndex, request.PageSize , request.IsAll, out recordCount); 
            //返回响应信息
            viewModel.NextLevelAgentList = list;
            viewModel.PageIndex = request.PageIndex;
            viewModel.PageSize = request.PageSize;
            viewModel.RecordCount = recordCount;
            viewModel.StatusMessage = "查询成功";
            viewModel.BusinessStatus = 200;
            return viewModel;
        }
    }
}
