using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Services;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.SaveUserExpandTests
{
    [TestFixture]
    public class CreateUpdateUserExpandTests
    {
        [Test]
        public void SaveUserExpandAsync_ThrowExecption_ReturnFalse()
        {
            #region Substitute 配置
            ITempInsuredRepository _tempInsuredRepository = Substitute.For<ITempInsuredRepository>();
            IUserInfoRepository _userInfoRepository = Substitute.For<IUserInfoRepository>();
            IRenewalInfoRepository _renewalInfoRepository = Substitute.For<IRenewalInfoRepository>();
            
            _tempInsuredRepository.When(x => x.AddUserExpandAsync(new bx_userinfo_expand())).Do(x => { throw new Exception(); });
           

            TempInsuredService tempInsuredService = new TempInsuredService(_tempInsuredRepository, _userInfoRepository,
                _renewalInfoRepository);

            #endregion

            #region Arg 操作

            var result = tempInsuredService.SaveUserExpandAsync(new UserExpandRequest(){IsEdit = false}).Result;

            #endregion

            #region Assert 断言

            Assert.AreEqual(false, result);

            #endregion
        }

        /// <summary>
        /// 此次测试意义在于异步返回值的时候配置怎么处理
        /// </summary>
        [Test]
        public void SaveUserExpandAsync_Add_ReturnTrue()
        {
            #region Substitute 配置
            ITempInsuredRepository _tempInsuredRepository = Substitute.For<ITempInsuredRepository>();
            IUserInfoRepository _userInfoRepository = Substitute.For<IUserInfoRepository>();
            IRenewalInfoRepository _renewalInfoRepository = Substitute.For<IRenewalInfoRepository>();

            _tempInsuredRepository.AddUserExpandAsync(Arg.Any<bx_userinfo_expand>())
                .Returns(x => Task.FromResult(false));

            TempInsuredService tempInsuredService = new TempInsuredService(_tempInsuredRepository, _userInfoRepository,
                _renewalInfoRepository);

            #endregion

            #region Arg 操作

            var result = tempInsuredService.SaveUserExpandAsync(new UserExpandRequest() { IsEdit = false }).Result;

            #endregion

            #region Assert 断言

            Assert.AreEqual(false, result);

            #endregion
        }
    }
}
