using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using NSubstitute;
using NUnit.Framework;

namespace BiHuManBu.ExternalInterfaces.UnitTests.RCustomerOrderServiceTests
{
    [TestFixture]
    public class GetOrderDetailServiceTests
    {
        [Test]
        public void GetOrderDetail_Check0_Return0()
        {
            #region 配置 Substitute

            var checkEntityService = Substitute.For<ICheckEntityService>();
            var customerOrderService = Substitute.For<ICustomerOrderService>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var mapEntityService = Substitute.For<IMapEntityService>();

            checkEntityService.CheckGetOrder(Arg.Any<GetOrderDetailRequest>()).Returns(info=>new CheckOrderView(){BusinessStatus = 0,StatusMessage = ""});

            var orderDetailService = new GetOrderDetailService(checkEntityService,
                customerOrderService, orderRepository, mapEntityService);

            #endregion

            #region 操作 Arg

            var result = orderDetailService.GetOrderDetail(new GetOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion
        }

        [Test]
        public void GetOrderDetail_ShrowException_ReturnNegavite()
        {
            #region 配置 Substitute

            var checkEntityService = Substitute.For<ICheckEntityService>();
            var customerOrderService = Substitute.For<ICustomerOrderService>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var mapEntityService = Substitute.For<IMapEntityService>();

            checkEntityService.When(x=> x.CheckGetOrder(Arg.Any<GetOrderDetailRequest>())).Do(info =>
            {
                throw new Exception();
            });

            var orderDetailService = new GetOrderDetailService(checkEntityService,
                customerOrderService, orderRepository, mapEntityService);

            #endregion

            #region 操作 Arg

            var result = orderDetailService.GetOrderDetail(new GetOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(-10003, result.BusinessStatus);

            #endregion
        }
    }
}
