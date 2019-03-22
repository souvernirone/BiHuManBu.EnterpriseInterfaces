using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;

namespace BiHuManBu.ExternalInterfaces.Services
{
    public class QuoteRecordService : IQuoteRecordService
    {
        private IQuoteRecordRepository _quoteRecordRepository;
        private IUserinfoExpandRepository _userInfoExpandRepository;
        private IPushSpecialMessageService _pushSpecialMessageService;
        private IUserInfoRepository _userInfoRepository;
        private IAgentRepository _agentRepository;
        private IQuotehistoryRelatedRepository _quotehistoryRelatedRepository;
        private IRenewalInfoRepository _renewalInfoRepository;
        public QuoteRecordService(IQuoteRecordRepository quoteRecordRepository, IUserinfoExpandRepository userInfoExpandRepository,
            IPushSpecialMessageService pushSpecialMessageService, IUserInfoRepository userInfoRepository, IAgentRepository agentRepository,
            IQuotehistoryRelatedRepository quotehistoryRelatedRepository,
            IRenewalInfoRepository renewalInfoRepository
            )
        {
            _userInfoExpandRepository = userInfoExpandRepository;
            _quoteRecordRepository = quoteRecordRepository;
            _pushSpecialMessageService = pushSpecialMessageService;
            _userInfoRepository = userInfoRepository;
            _agentRepository = agentRepository;
            _quotehistoryRelatedRepository = quotehistoryRelatedRepository;
            _renewalInfoRepository = renewalInfoRepository;
        }

        public async Task<BaseViewModel> AddQuoteRecord(AddQuoteRecordRequest request)
        {
            BaseViewModel viewModel = new BaseViewModel();
            bx_quote_record record = new bx_quote_record()
            {
                Buid = request.Buid,
                AgentId = request.ReQuoteAgent,
                LotNo = request.LotNO,
                ValidFlag = 1,
                CreateTime = DateTime.Now,
                UpdateTime = DateTime.Now
            };
            bx_userinfo userinfo = _userInfoRepository.GetUserInfo(request.Buid);
            if (userinfo == null)
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "添加失败，未能找到userinfo信息";
                return viewModel;
            }

            //根据buid取当前记录的agent，再取其顶级
            request.ChildAgent = int.Parse(userinfo.Agent);
            bx_agent agent = _agentRepository.GetAgent(request.ChildAgent);
            if (agent != null)
            {
                request.Agent = agent.TopAgentId;
            }
            var carOwnerTopAgentId = ConfigurationManager.AppSettings["CarOwnerTopAgentId"].Split(',').Select(t => Convert.ToInt32(t)).ToList();
            if (!carOwnerTopAgentId.Contains(request.Agent))
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "添加失败，该条记录不支持车主报价";
                return viewModel;
            }

            var result = await _quoteRecordRepository.Add(record);
            if (result > 0)
            {
                #region userinfo扩展表增加字段
                bx_userinfo_expand expand = await _userInfoExpandRepository.GetAsync(request.Buid);
                if (expand == null)
                {
                    expand = new bx_userinfo_expand()
                    {
                        b_uid = request.Buid,
                        is_temp_email = 0,
                        is_temp_mobile = 0,
                        delete_type = -1,
                        update_time = DateTime.Now,
                        delete_time = new DateTime(1970, 1, 1),
                    };
                }
                expand.CarOwnerStatus = 1;
                await _userInfoExpandRepository.Update(expand);
                #endregion

                #region 添加关系人到历史表
                bx_quotehistory_related relation = new bx_quotehistory_related()
                {
                    b_uid = request.Buid,
                    create_time = DateTime.Now,
                    update_time = DateTime.Now,
                    groupspan = request.LotNO,
                    ower_name = userinfo.LicenseOwner,
                    ower_id_type = userinfo.OwnerIdCardType,
                    ower_id_card = userinfo.IdCard,
                    ower_sex = userinfo.OwnerSex,
                    ower_nation = userinfo.OwnerNation,
                    ower_birthday = !string.IsNullOrWhiteSpace(userinfo.OwnerBirthday) ? DateTime.Parse(userinfo.OwnerBirthday) : (DateTime?)null,
                    ower_address = userinfo.OwnerCertiAddress,
                    ower_certi_start_date = !string.IsNullOrWhiteSpace(userinfo.OwnerCertiStartdate) ? DateTime.Parse(userinfo.OwnerCertiStartdate) : (DateTime?)null,
                    ower_certi_end_date = !string.IsNullOrWhiteSpace(userinfo.OwnerCertiEnddate) ? DateTime.Parse(userinfo.OwnerCertiEnddate) : (DateTime?)null,
                    ower_authority = userinfo.OwnerIssuer,
                    ower_mobile = userinfo.Mobile,
                    ower_email = userinfo.Email,
                    holder_name = userinfo.HolderName,
                    holder_id_type = userinfo.HolderIdType,
                    holder_id_card = userinfo.HolderIdCard,
                    holder_sex = userinfo.HolderSex,
                    holder_nation = userinfo.HolderNation,
                    holder_birthday = !string.IsNullOrWhiteSpace(userinfo.HolderBirthday) ? DateTime.Parse(userinfo.HolderBirthday) : (DateTime?)null,
                    holder_address = userinfo.HolderAddress,
                    holder_certi_start_date = !string.IsNullOrWhiteSpace(userinfo.HolderCertiStartdate) ? DateTime.Parse(userinfo.HolderCertiStartdate) : (DateTime?)null,
                    holder_certi_end_date = !string.IsNullOrWhiteSpace(userinfo.HolderCertiEnddate) ? DateTime.Parse(userinfo.HolderCertiEnddate) : (DateTime?)null,
                    holder_authority = userinfo.HolderIssuer,
                    holder_mobile = userinfo.HolderMobile,
                    holder_email = userinfo.HolderEmail,
                    insured_name = userinfo.InsuredName,
                    insured_id_type = userinfo.InsuredIdType,
                    insured_id_card = userinfo.InsuredIdCard,
                    insured_sex = userinfo.InsuredSex,
                    insured_nation = userinfo.InsuredNation,
                    insured_birthday = !string.IsNullOrWhiteSpace(userinfo.InsuredBirthday) ? DateTime.Parse(userinfo.InsuredBirthday) : (DateTime?)null,
                    insured_address = userinfo.InsuredAddress,
                    insured_certi_start_date = !string.IsNullOrWhiteSpace(userinfo.InsuredCertiStartdate) ? DateTime.Parse(userinfo.InsuredCertiStartdate) : (DateTime?)null,
                    insured_certi_end_date = !string.IsNullOrWhiteSpace(userinfo.InsuredCertiEnddate) ? DateTime.Parse(userinfo.InsuredCertiEnddate) : (DateTime?)null,
                    insured_authority = userinfo.InsuredIssuer,
                    insured_mobile = userinfo.InsuredMobile,
                    insured_email = userinfo.InsuredEmail
                };
                _quotehistoryRelatedRepository.Insert(relation);
                _quotehistoryRelatedRepository.SaveChanges();
                #endregion

                #region 添加客户记录
                CustomerInfo customerInfo = await _renewalInfoRepository.GetCustomerInfoAsync(request.Buid);
                agent = _agentRepository.GetAgent(request.ReQuoteAgent);
                if (agent != null && agent.Id > 0)
                {
                    if (string.IsNullOrEmpty(agent.Mobile) && !string.IsNullOrEmpty(agent.AgentName) && agent.AgentName.Length == 11)
                    {//如果没有手机号，就改成用户名
                        agent.Mobile = agent.AgentName;
                    }
                    if (customerInfo != null)
                    {
                        //如果buid有电话了
                        if (!string.IsNullOrWhiteSpace(customerInfo.CustomerMobile))
                        {
                            customerInfo.ClientMobileOther = agent.Mobile;
                        }
                        else
                        {
                            //buid没电话
                            customerInfo.CustomerMobile = agent.Mobile;
                        }
                    }
                    else
                    {
                        customerInfo = new CustomerInfo()
                        {
                            BuId = request.Buid,
                            CustomerMobile = agent.Mobile,
                        };
                    }
                    await _renewalInfoRepository.SaveCustomerInfoAsync(customerInfo);
                }
                #endregion

                viewModel.BusinessStatus = 1;
                viewModel.StatusMessage = "添加成功";
                List<int> agents = new List<int>();
                //取顶级下所有的管理员
                agents = _agentRepository.GetManagerId(request.Agent);
                if (agents.Any())
                {//如果当前记录是管理员，就排除掉。后文会给排除掉的推消息
                    agents.Remove(request.ChildAgent);
                }
                ////推送消息
                _pushSpecialMessageService.PushMsg(request.Buid, request.ChildAgent, request.Agent, agents);
            }
            else
            {
                viewModel.BusinessStatus = 0;
                viewModel.StatusMessage = "添加失败";
            }
            return viewModel;
        }
    }
}
