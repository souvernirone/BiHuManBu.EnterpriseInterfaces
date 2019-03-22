using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.AppIRepository;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request.Order;
using BiHuManBu.ExternalInterfaces.Services.RCustomerOrderService.Extends;
using NSubstitute;
using NSubstitute.Core;
using NUnit.Framework;
using IOrderRepository = BiHuManBu.ExternalInterfaces.Models.IRepository.IOrderRepository;
using IUserInfoRepository = BiHuManBu.ExternalInterfaces.Models.IUserInfoRepository;

namespace BiHuManBu.ExternalInterfaces.UnitTests.RCustomerOrderServiceTests
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class CreateOrderCheckServiceTests
    {
        /// <summary>
        /// fromMethod：默认四个值1，2，4，8其他数值暂时无效
        /// </summary>
        /// <param name="fromMethod"></param>
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        [TestCase(8)]
        public void CreateOrderCheck_CheckFromMethod_Return(int fromMethod)
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => new bx_agent_config() { is_used  = 1});
            submitInfoRepository.GetSubmitInfo(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_submit_info() { id = 1, submit_status  = 1});
            saveQuoteRepository.GetSavequoteByBuid(Arg.Any<long>()).Returns(x => new bx_savequote(){Id = 1});
            quoteResultRepository.GetQuoteResultByBuid(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_quoteresult() {Id = 1});

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);


            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = fromMethod, OrderType = 41, GetOrderMethod=1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(1, result.BusinessStatus);

            #endregion

        }


        /// <summary>
        /// orderType：暂时为0，41时候通过
        /// </summary>
        /// <param name="orderType"></param>
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(41)]
        [TestCase(42)]
        public void CreateOrderCheck_CheckOrderType_Return(int orderType)
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => new bx_agent_config() { is_used = 1 });
            submitInfoRepository.GetSubmitInfo(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_submit_info() { id = 1, submit_status = 1 });
            saveQuoteRepository.GetSavequoteByBuid(Arg.Any<long>()).Returns(x => new bx_savequote() { Id = 1 });
            quoteResultRepository.GetQuoteResultByBuid(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_quoteresult() { Id = 1 });

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);


            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = orderType, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(1, result.BusinessStatus);

            #endregion

        }
        
        [Test]
        public void CreateOrderCheck_UserinfoIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => null);

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }

        [Test]
        public void CreateOrderCheck_ListOrderIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(x => null);

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }
        
        [Test]
        public void CreateOrderCheck_SubmitInfoIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            bx_submit_info bb = null;

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(x => new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => new bx_agent_config() { is_used = 1 });
            submitInfoRepository.GetSubmitInfo(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => null);

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }

        [Test]
        public void CreateOrderCheck_AgentConfigIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            bx_submit_info bb = null;

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(x => new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => null);

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }
        
        [Test]
        public void CreateOrderCheck_SaveQuoteIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            bx_submit_info bb = null;

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(x => new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => new bx_agent_config());
            submitInfoRepository.GetSubmitInfo(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_submit_info() { id = 1, submit_status = 1 });
            saveQuoteRepository.GetSavequoteByBuid(Arg.Any<long>()).Returns(x => null);
            quoteResultRepository.GetQuoteResultByBuid(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_quoteresult() { Id = 1 });

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }


        [Test]
        public void CreateOrderCheck_QuoteResultIsNull_Return0()
        {
            #region 配置 Substitute

            var userInfoRepository = Substitute.For<IUserInfoRepository>();
            var orderRepository = Substitute.For<IOrderRepository>();
            var submitInfoRepository = Substitute.For<ISubmitInfoRepository>();
            var agentConfigRepository = Substitute.For<Models.IAgentConfigRepository>();
            var saveQuoteRepository = Substitute.For<ISaveQuoteRepository>();
            var quoteResultRepository = Substitute.For<IQuoteResultRepository>();

            bx_submit_info bb = null;

            userInfoRepository.FindByBuid(Arg.Any<long>()).Returns(x => new bx_userinfo());
            orderRepository.FindOrderListByBuid(Arg.Any<long>()).Returns(x => new List<dd_order>());
            submitInfoRepository.GetSubmitInfoAsync(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => Task.FromResult(new bx_submit_info()));
            agentConfigRepository.GetAgentConfigById(Arg.Any<long>()).Returns(x => new bx_agent_config());
            submitInfoRepository.GetSubmitInfo(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => new bx_submit_info() { id = 1, submit_status = 1 });
            saveQuoteRepository.GetSavequoteByBuid(Arg.Any<long>()).Returns(x => new bx_savequote());
            quoteResultRepository.GetQuoteResultByBuid(Arg.Any<long>(), Arg.Any<int>())
                .Returns(x => null);

            var createOrderCheckService = new CreateOrderCheckService(userInfoRepository,
                orderRepository, submitInfoRepository, agentConfigRepository, saveQuoteRepository, quoteResultRepository);

            #endregion

            #region 操作 Arg

            var result = createOrderCheckService.CreateOrderCheck(new CreateOrderDetailRequest() { FromMethod = 1, OrderType = 41, GetOrderMethod = 1 });

            #endregion

            #region 断言 Assert

            Assert.AreEqual(0, result.BusinessStatus);

            #endregion

        }
    }
}
