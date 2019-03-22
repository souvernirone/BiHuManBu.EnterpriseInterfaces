using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.UploadImg;
using ServiceStack.Text;
using System.Linq;

namespace BiHuManBu.ExternalInterfaces.API.Controllers
{
    public class UploadImgController:ApiController
    {
        string imageUpload = System.Configuration.ConfigurationManager.AppSettings["ImageUpload"];

        public HttpResponseMessage Upload(string  baseContent)
        {
            string source = baseContent;
            string base64 = source.Substring(source.IndexOf(',') + 1);
            byte[] data = Convert.FromBase64String(base64);
            FileUploadModel fileUploadModel = new FileUploadModel();
            fileUploadModel.VersionKey = new Dictionary<string, string>();
            fileUploadModel.BusinessType = UploadBusinessType.IdCard;
            fileUploadModel.FileName = "xxxxx.jpg";
            //上传图片至服务器 
            DynamicWebService DW = new DynamicWebService();
            object[] postArg = new object[2];
            postArg[0] = fileUploadModel.ToJson();
            postArg[1] = data;
            var ret = DW.InvokeWebservice(
                imageUpload + "/fileuploadcenter.asmx", "BiHuManBu.ServerCenter.FileUploadCenter", "FileUploadCenter", "ImageUpload", postArg);
            var tt = ret.ToString().FromJson<UploadFileResult>();
            return tt.ResponseToJson();
        }

        [HttpPost]
        public HttpResponseMessage UploadImg([FromBody]Services.Messages.Request.AppRequest.UploadImgRequest request)
        {
            UploadFileResult viewModel = new UploadFileResult();
            if (!ModelState.IsValid)
            {
                viewModel.ResultCode = -100;
                string msg = ModelState.Values.Where(a => a.Errors.Count == 1).Aggregate(string.Empty, (current, a) => current + (a.Errors[0].ErrorMessage + ";"));
                viewModel.Message = "输入参数错误，" + msg;
                return viewModel.ResponseToJson();
            }
            try
            {
                string source = request.baseContent;
                string base64 = source.Substring(source.IndexOf(',') + 1);
                byte[] data = Convert.FromBase64String(base64);
                var versions = new Dictionary<string, string>();
                versions.Add("_small", "maxwidth=50&maxheight=50&format=jpg");
                versions.Add("_medium", "maxwidth=200&maxheight=200&format=jpg");
                versions.Add("_large", "maxwidth=800&maxheight=660&format=jpg");
                var fileUploadModel = new FileUploadModel
                {
                    FileName = "xxxxx.jpg",
                    VersionKey = versions
                };
                //上传图片至服务器 
                var dw = new DynamicWebService();
                object[] postArg = new object[2];
                postArg[0] = fileUploadModel.ToJson();
                postArg[1] = data;
                var ret = dw.InvokeWebservice(
                    imageUpload + "/fileuploadcenter.asmx", "BiHuManBu.ServerCenter.FileUploadCenter", "FileUploadCenter", "ImageUpload", postArg);
                viewModel = ret.ToString().FromJson<UploadFileResult>();
            }
            catch
            {
                viewModel.ResultCode = -100;
                viewModel.Message = "图片上传异常";
            }
            return viewModel.ResponseToJson();
        }
    }
}