using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.MessageCenter;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using Moq;
using NUnit.Framework;

namespace UnitTest.ServicesLayers
{
    [TestFixture]
    public class CarInsuranceServiceTest
    {
        private Mock<ISaveQuoteRepository> _stubSaveQuoteRepository;
        private Mock<IUserInfoRepository> _stubUserInfoRepository;
        private Mock<ILoginService> _logService;
        private Mock<ILastInfoRepository> _stubLastInfoRepository;
        private Mock<ISubmitInfoRepository> _stubSubmitInfoRepository;
        private Mock<IQuoteResultRepository> _stubQuoteResultRepository;
        private Mock<IAgentRepository> _sutbAgentRepository;
        private Mock<IMessageCenter> _messageCenter;
        [SetUp]
        public void SetUp()
        {
            _stubLastInfoRepository = new Mock<ILastInfoRepository>();
            _stubQuoteResultRepository = new Mock<IQuoteResultRepository>();
            _stubSaveQuoteRepository = new Mock<ISaveQuoteRepository>();
            _stubSubmitInfoRepository = new Mock<ISubmitInfoRepository>();
            _stubUserInfoRepository = new Mock<IUserInfoRepository>();
            _logService = new Mock<ILoginService>();
            _sutbAgentRepository = new Mock<IAgentRepository>();
            _messageCenter = new Mock<IMessageCenter>();

        }
        [TestCase(1,"京P8WB50",1,"XXXXXXS")]
        public async void GetReInfo_WithNotExistMobile_ExpectHttpStatusCode_Forbidden(int agent,string licenseno,int citycode,string seccode)
        {
            GetReInfoRequest reInfoRequest = new GetReInfoRequest
            {
                AgentId = agent,
                LicenseNo = licenseno,
                CityCode = citycode,
                SecCode = seccode
            };

            var pars= CommonHelper.EachProperties(reInfoRequest);
           

            _sutbAgentRepository.Setup(x => x.GetAgent(It.IsAny<int>())).Returns<bx_agent>(null);
            ICarInsuranceService carInsuranceService = new CarInsuranceService(_stubSaveQuoteRepository.Object,_stubUserInfoRepository.Object,
                _logService.Object, _stubSubmitInfoRepository.Object, _stubQuoteResultRepository.Object, _stubLastInfoRepository.Object, _sutbAgentRepository.Object, _messageCenter.Object);
            var response =await carInsuranceService.GetReInfo(reInfoRequest, pars);
            Assert.AreEqual(HttpStatusCode.Forbidden,response.Status);
        }

        [TestCase(103, "京P8WB50", 1, "24360d7b99aa19e98b5d4cda135e830f")]
        public async void GetReInfo_WithInValidInput_ExpectHttpStatusCode_Forbidden(int agent, string licenseno, int citycode, string seccode)
        {
            GetReInfoRequest reInfoRequest = new GetReInfoRequest
            {
                AgentId = agent,
                LicenseNo = licenseno,
                CityCode = citycode,
                SecCode = seccode
            };

            var pars = CommonHelper.EachProperties(reInfoRequest);


            _sutbAgentRepository.Setup(x => x.GetAgent(It.IsAny<int>())).Returns(new bx_agent{Id = 103,SecretKey = "1233"});
            ICarInsuranceService carInsuranceService = new CarInsuranceService(_stubSaveQuoteRepository.Object, _stubUserInfoRepository.Object,
                _logService.Object, _stubSubmitInfoRepository.Object, _stubQuoteResultRepository.Object, _stubLastInfoRepository.Object, _sutbAgentRepository.Object, _messageCenter.Object);
            var response = await carInsuranceService.GetReInfo(reInfoRequest, pars);
            Assert.AreEqual(HttpStatusCode.BadRequest, response.Status);
        }
         [TestCase(103, "京P8WB50", 1, "24360d7b99aa19e98b5d4cda135e830f")]
        public async void GetReInfo_WithNewLicense_ExpectCorrectResult(int agent, string licenseno, int citycode, string seccode)
        {
            GetReInfoRequest reInfoRequest = new GetReInfoRequest
            {
                AgentId = agent,
                LicenseNo = licenseno,
                CityCode = citycode,
                SecCode = seccode
            };

            var pars = CommonHelper.EachProperties(reInfoRequest);

            _sutbAgentRepository.Setup(x => x.GetAgent(It.IsAny<int>())).Returns(new bx_agent { Id = 103, SecretKey = "1233",Mobile = "18310825788"});
             _stubUserInfoRepository.Setup(x => x.FindByOpenIdAndLicense(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(default(bx_userinfo));
            
            _logService.Setup(x => x.LoginAccount(It.IsAny<string>())).Returns(GetAccount);
             
             _stubUserInfoRepository.Setup(x => x.Add(It.IsAny<bx_userinfo>())).Returns(1);
            ICarInsuranceService carInsuranceService = new CarInsuranceService(_stubSaveQuoteRepository.Object, _stubUserInfoRepository.Object,
                _logService.Object, _stubSubmitInfoRepository.Object, _stubQuoteResultRepository.Object, _stubLastInfoRepository.Object, _sutbAgentRepository.Object, _messageCenter.Object);
            var response = await carInsuranceService.GetReInfo(reInfoRequest, pars);
            Assert.AreEqual(HttpStatusCode.OK, response.Status);
        }

         [TestCase(103, "京P8WB50", 1, "24360d7b99aa19e98b5d4cda135e830f")]
         public async void GetReInfo_WithOldLicense_ExpectCorrectResult(int agent, string licenseno, int citycode, string seccode)
         {
             GetReInfoRequest reInfoRequest = new GetReInfoRequest
             {
                 AgentId = agent,
                 LicenseNo = licenseno,
                 CityCode = citycode,
                 SecCode = seccode
             };

             var pars = CommonHelper.EachProperties(reInfoRequest);

             _sutbAgentRepository.Setup(x => x.GetAgent(It.IsAny<int>())).Returns(new bx_agent { Id = 103, SecretKey = "1233", Mobile = "18310825788" });
             _stubUserInfoRepository.Setup(x => x.FindByOpenIdAndLicense(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new bx_userinfo{Id = 1});

             _logService.Setup(x => x.LoginAccount(It.IsAny<string>())).Returns(GetAccount);
             _messageCenter.Setup(x => x.SendToMessageCenter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new MessageResult());
             _stubUserInfoRepository.Setup(x => x.Update(It.IsAny<bx_userinfo>())).Returns(1);
             ICarInsuranceService carInsuranceService = new CarInsuranceService(_stubSaveQuoteRepository.Object, _stubUserInfoRepository.Object,
                 _logService.Object, _stubSubmitInfoRepository.Object, _stubQuoteResultRepository.Object, _stubLastInfoRepository.Object, _sutbAgentRepository.Object, _messageCenter.Object);
             var response = await carInsuranceService.GetReInfo(reInfoRequest, pars);
             Assert.AreEqual(HttpStatusCode.OK, response.Status);
         }

         [TestCase(103, "京P8WB50", 1, "24360d7b99aa19e98b5d4cda135e830f")]
         public async void GetReInfo_WithOldLicense_ExpectCorrectResultWithXuBaoFault(int agent, string licenseno, int citycode, string seccode)
         {
             GetReInfoRequest reInfoRequest = new GetReInfoRequest
             {
                 AgentId = agent,
                 LicenseNo = licenseno,
                 CityCode = citycode,
                 SecCode = seccode
             };

             var pars = CommonHelper.EachProperties(reInfoRequest);

             _sutbAgentRepository.Setup(x => x.GetAgent(It.IsAny<int>())).Returns(new bx_agent { Id = 103, SecretKey = "1233", Mobile = "18310825788" });
             _stubUserInfoRepository.Setup(x => x.FindByOpenIdAndLicense(It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new bx_userinfo { Id = 1 });

             _logService.Setup(x => x.LoginAccount(It.IsAny<string>())).Returns(GetAccount);
             _messageCenter.Setup(x => x.SendToMessageCenter(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
                 .Returns(new MessageResult());
             _stubUserInfoRepository.Setup(x => x.Update(It.IsAny<bx_userinfo>())).Returns(1);
             _stubUserInfoRepository.Setup(x => x.FindByBuid(It.IsAny<long>()))
                 .Returns(new bx_userinfo {NeedEngineNo = 1});
             ICarInsuranceService carInsuranceService = new CarInsuranceService(_stubSaveQuoteRepository.Object, _stubUserInfoRepository.Object,
                 _logService.Object, _stubSubmitInfoRepository.Object, _stubQuoteResultRepository.Object, _stubLastInfoRepository.Object, _sutbAgentRepository.Object, _messageCenter.Object);
             var response = await carInsuranceService.GetReInfo(reInfoRequest, pars);
             Assert.AreEqual(HttpStatusCode.OK, response.Status);
             Assert.AreEqual(1, response.UserInfo.NeedEngineNo);
         }

        private async Task<Account> GetAccount()
        {
            return new Account
            {
                Mobile = "12134564",
                UserId = 1111
            };
        }

        private async Task<Account> GetNullAccount()
        {
            return default(Account);
        }
    }
}
