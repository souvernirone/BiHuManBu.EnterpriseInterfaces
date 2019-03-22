using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.RCustomerOrderServiceTests
{
    [TestFixture]
    public class CheckEntityServiceTests
    {
        [Test]
        public void CheckGetOrder_OrderIsNull_Return0()
        {
            #region 配置 Substitute

            var orderRepository = Substitute.For<IOrderRepository>();
            var userInfoRepository = Substitute.For<IUserInfoRepository>();

            orderRepository.FindOrder(Arg.Any<string>()).Returns(info => null);

            var checkEntityService = new CheckEntityService(orderRepository, userInfoRepository);

            #endregion

            #region 操作 Arg

            var result = checkEntityService.CheckGetOrder(new GetOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }

        [Test]
        public void CheckGetOrder_UserinfoIsNull_Return0()
        {
            #region 配置 Substitute

            var orderRepository = Substitute.For<IOrderRepository>();
            var userInfoRepository = Substitute.For<IUserInfoRepository>();

            orderRepository.FindOrder(Arg.Any<string>()).Returns(info => new dd_order());
            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(info => null);

            var checkEntityService = new CheckEntityService(orderRepository, userInfoRepository);

            #endregion

            #region 操作 Arg

            var result = checkEntityService.CheckGetOrder(new GetOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }

        [Test]
        public void CheckGetOrder_ThrowException_ReturnNegavite()
        {
            #region 配置 Substitute

            var orderRepository = Substitute.For<IOrderRepository>();
            var userInfoRepository = Substitute.For<IUserInfoRepository>();

            orderRepository.FindOrder(Arg.Any<string>()).Returns(info => new dd_order());
            userInfoRepository.When(x => x.FindByBuid(Arg.Any<long>())).Do(info => { throw new Exception(); });

            var checkEntityService = new CheckEntityService(orderRepository, userInfoRepository);

            #endregion

            #region 操作 Arg

            var result = checkEntityService.CheckGetOrder(new GetOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(-10003, result.BusinessStatus);

            #endregion

        }
    }
}
