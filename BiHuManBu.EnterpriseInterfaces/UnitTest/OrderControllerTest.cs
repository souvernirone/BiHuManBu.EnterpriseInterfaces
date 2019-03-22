using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Hosting;
using BiHuManBu.ExternalInterfaces.API.Controllers;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.Messages.Response;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NUnit.Framework;

namespace UnitTest
{
    [TestClass]
    public  class OrderControllerTest
    {
        private Mock<IOrderService> _mockOrderService;

        [SetUp]
        public void SetUp()
        {
            _mockOrderService = new Mock<IOrderService>();
        }
        [Test]
        public async void PostOrder_WithProvidedInput_ExpectProperTootip()
        {
            CreateOrderRequest request = new CreateOrderRequest
            {
                AgentId = 103,
                Buid = 1,
                CarriagePrice = 100,
                DistributionTime = DateTime.Now,
                DistributionType = 1,
                IdNum = "12312121212",
                IdType = 1,
                InsurancePrice = 12,
                InsuredName = "张三",
                OrderNum = "213331323123123",
                PayType = 1,
                Receipt = "张三",
                SecCode = "234234343423",
                TotalPrice = 1000,
                UserId = 12
            };
            OrderController controller = new OrderController(_mockOrderService.Object)
            {
                Request = new HttpRequestMessage
                {
                    Properties = { { HttpPropertyKeys.HttpConfigurationKey, new HttpConfiguration() } }
                }
            };
            _mockOrderService.Setup(e => e.Create(request, controller.Request.GetQueryNameValuePairs()))
                .Returns(CreateOrderResponse);

           var tt= await controller.PostOrder(request);
            NUnit.Framework.Assert.AreEqual(1,tt.Content.ReadAsAsync<CreateOrderResponse>().Result.OrderId);
        }


        private async Task<CreateOrderResponse> CreateOrderResponse()
        {
            return new CreateOrderResponse()
            {
                OrderId = 1,
                Status = HttpStatusCode.OK
            };
        }
        
    }
}
