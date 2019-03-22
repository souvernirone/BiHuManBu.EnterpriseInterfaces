using System;
using System.Linq.Expressions;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.NewRenewalInfoService;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.GetRenewalInfoServiceTests
{
    [TestFixture]
    public class GetRenewalInfoTests
    {
        [Test]
        public void GetRenewalInfo_UserIsNull_Return0()
        {
            #region Substitute 配置

            var checkUserService = Substitute.For<ICheckUserService>();
            var renewalInfoService = Substitute.For<IRenewalInfoService>();
            var tempUserService = Substitute.For<ITempUserService>();
            var mapInformationService = Substitute.For<IMapInformationService>();
            var reWriteRelationInfoService = Substitute.For<IReWriteRelationInfoService>();

            checkUserService.CheckUser(Arg.Any<GetRenewalRequest>())
                .Returns(x => new ResultMessage() { BusinessStatus = 0, StatusMessage = "" });

            var getRenewalInfoService = new GetRenewalInfoService(checkUserService, renewalInfoService,
                tempUserService, mapInformationService, reWriteRelationInfoService);

            #endregion

            #region Arg 操作

            var result = getRenewalInfoService.GetRenewalInfo(new GetRenewalRequest());

            #endregion

            #region Assert 断言

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion
        }

        public void GetRenewalInfo_ThrowExcption_ReturnNegavite()
        {
            #region Substitute 配置

            var checkUserService = Substitute.For<ICheckUserService>();
            var renewalInfoService = Substitute.For<IRenewalInfoService>();
            var tempUserService = Substitute.For<ITempUserService>();
            var mapInformationService = Substitute.For<IMapInformationService>();
            var reWriteRelationInfoService = Substitute.For<IReWriteRelationInfoService>();

            checkUserService.When(x => x.CheckUser(Arg.Any<GetRenewalRequest>())).Do(info => { throw new Exception(); });

            var getRenewalInfoService = new GetRenewalInfoService(checkUserService, renewalInfoService,
                tempUserService, mapInformationService, reWriteRelationInfoService);

            #endregion

            #region Arg 操作

            var result = getRenewalInfoService.GetRenewalInfo(new GetRenewalRequest());

            #endregion

            #region Assert 断言

            Assert.AreEqual(-10003, result.BusinessStatus);

            #endregion
        }
    }
}
