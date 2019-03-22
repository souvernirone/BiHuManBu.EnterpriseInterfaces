using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.GetRenewalInfoServiceTests
{
    [TestFixture]
    public class CheckUserTests
    {
        [Test]
        public void CheckUser_UserIsNull_ReturnNegavite()
        {
            //#region Substitute 配置

            //var renewalInfoService = Substitute.For<IRenewalInfoService>();
            //var agentService = Substitute.For<IAgentService>();

            //renewalInfoService.GetUserInfo(Arg.Any<long>(), Arg.Any<List<string>>()).Returns(x => null);

            //var checkUserService = new CheckUserService(renewalInfoService, agentService);

            //#endregion

            //#region Arg 操作

            //var result = checkUserService.CheckUser(new GetRenewalRequest());

            //#endregion

            //#region Assert 断言

            //Assert.AreEqual(-10014, result.BusinessStatus);

            //#endregion
        }

        [Test]
        public void CheckUser_UserNotNull_Return1()
        {
            //#region Substitute 配置

            //var renewalInfoService = Substitute.For<IRenewalInfoService>();
            //var agentService = Substitute.For<IAgentService>();

            //renewalInfoService.GetUserInfo(Arg.Any<long>(), Arg.Any<List<string>>()).Returns(x => new bx_userinfo());

            //var checkUserService = new CheckUserService(renewalInfoService, agentService);

            //#endregion

            //#region Arg 操作

            //var result = checkUserService.CheckUser(new GetRenewalRequest());

            //#endregion

            //#region Assert 断言

            //Assert.AreEqual(1, result.BusinessStatus);

            //#endregion
        }

        [Test]
        public void CheckUser_TrowExcption_Return0()
        {
            //#region Substitute 配置

            //var renewalInfoService = Substitute.For<IRenewalInfoService>();
            //var agentService = Substitute.For<IAgentService>();

            //renewalInfoService.When(x => x.GetUserInfo(Arg.Any<long>(), Arg.Any<List<string>>())).Do(info => { throw new Exception(); });

            //var checkUserService = new CheckUserService(renewalInfoService, agentService);

            //#endregion

            //#region Arg 操作

            //var result = checkUserService.CheckUser(new GetRenewalRequest());

            //#endregion

            //#region Assert 断言

            //Assert.AreEqual(-1, result.BusinessStatus);

            //#endregion
        }
    }
}
