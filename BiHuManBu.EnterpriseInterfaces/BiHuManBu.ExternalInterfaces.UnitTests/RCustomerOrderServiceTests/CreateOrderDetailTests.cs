using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Achieves;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using NSubstitute;
using NUnit.Framework;
using IOrderRepository = BiHuManBu.ExternalInterfaces.Models.IRepository.IOrderRepository;

namespace BiHuManBu.ExternalInterfaces.UnitTests.RCustomerOrderServiceTests
{
    [TestFixture]
    public class CreateOrderDetailTests
    {
        [Test]
        public void CreateOrderDetail_Check0_Return0()
        {
            #region 配置 Substitute

            var orderCheck = Substitute.For<ICreateOrderCheckService>();
            var mapEntityService = Substitute.For<IMapEntityService>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var carinfoRepository = Substitute.For<IQuoteResultCarinfoRepository>();
            var orderCorrelateService = Substitute.For<IOrderCorrelateService>();

            orderCheck.CreateOrderCheck(Arg.Any<CreateOrderDetailRequest>())
                .Returns(new CheckOrderView() {BusinessStatus = 0});

            var createOrderService = new CreateOrderService(orderCheck, mapEntityService, orderRepository,
                carinfoRepository, orderCorrelateService);

            #endregion

            #region 操作 Arg

            var result = createOrderService.CreateOrderDetail(new CreateOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion
        }

        [Test]
        public void CreateOrderDetail_ThrowExcption_ReturnNegevite()
        {
            #region 配置 Substitute

            var orderCheck = Substitute.For<ICreateOrderCheckService>();
            var mapEntityService = Substitute.For<IMapEntityService>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var carinfoRepository = Substitute.For<IQuoteResultCarinfoRepository>();
            var orderCorrelateService = Substitute.For<IOrderCorrelateService>();

            orderCheck.When(x=> x.CreateOrderCheck(Arg.Any<CreateOrderDetailRequest>()))
                .Do(info => { throw new Exception(); });

            var createOrderService = new CreateOrderService(orderCheck, mapEntityService, orderRepository,
                carinfoRepository, orderCorrelateService);

            #endregion

            #region 操作 Arg

            var result = createOrderService.CreateOrderDetail(new CreateOrderDetailRequest());

            #endregion

            #region 断言 Assert

            Assert.AreEqual(-10003, result.BusinessStatus);

            #endregion
        }
    }
}
