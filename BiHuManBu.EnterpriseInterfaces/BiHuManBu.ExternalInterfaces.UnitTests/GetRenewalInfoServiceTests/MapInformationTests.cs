using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.GetRenewalInfoServiceTests
{
    [TestFixture]
    public class MapInformationTests
    {
        [Test]
        [TestCase(-1, 0, 1, 4)]
        [TestCase(0, -2, 1, 2)]
        [TestCase(0, 0, 0, 1)]
        [TestCase(0, -2, 0, 3)]
        public void SetRenewalStatus_UserRenewalStatusNegavite_ReturnThis(int renewalStatus, int lastYearSource, int needEngineNo, int status)
        {
            #region Substitute 配置

            var renewalInfoService = Substitute.For<IRenewalInfoService>();
            var renewalInfoRepository = Substitute.For<IRenewalInfoRepository>();
            var information = new MapInformationService(renewalInfoService, renewalInfoRepository);

            #endregion

            #region Arg 操作

            var result =
                information.SetRenewalStatus(
                    new bx_userinfo() { RenewalStatus = renewalStatus, LastYearSource = lastYearSource, NeedEngineNo = needEngineNo }, status);

            #endregion

            #region Assert 断言

            Assert.AreEqual(status, result);

            #endregion
        }
    }
}
