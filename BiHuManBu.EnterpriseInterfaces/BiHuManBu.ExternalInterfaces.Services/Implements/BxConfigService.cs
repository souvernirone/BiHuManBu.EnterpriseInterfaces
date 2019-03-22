using System;
using System.Collections.Generic;
using System.Linq;
using BiHuManBu.ExternalInterfaces.Infrastructure;
using BiHuManBu.ExternalInterfaces.Infrastructure.Caches;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels;
using BiHuManBu.ExternalInterfaces.Services.Interfaces.AppInterfaces;
using ServiceStack.Logging;
using IMessageRepository = BiHuManBu.ExternalInterfaces.Models.IRepository.IMessageRepository;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class BxConfigService : IBxConfigService
    {
        private ILog logError = LogManager.GetLogger("ERROR");

        private IBxConfigRepository _bxConfigRepository;
        private IMessageRepository _messageRepository;

        public BxConfigService(IBxConfigRepository bxConfigRepository, IMessageRepository IMessageRepository)
        {
            _messageRepository = IMessageRepository;
            _bxConfigRepository = bxConfigRepository;
        }


        public BxConfigViewModel CompareVersion(RequestCompareConfig request)
        {
            var viewModel = new BxConfigViewModel();
            //取app的版本号
            string cachKey_version = "app_version_key";
            var cachValue_version = CacheProvider.Get<string>(cachKey_version);
            bx_config configObj = new bx_config();//初始化bx_config对象，方便下文使用
            //缓存里的app版本号没值，取数据库值
            if (cachValue_version == null)
            {
                configObj = _bxConfigRepository.FindByConfigKey("app_version").FirstOrDefault();
                if (configObj == null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "请求获取版本信息失败";
                    return viewModel;
                }
                cachValue_version = configObj.config_value;
                CacheProvider.Set(cachKey_version, cachValue_version, 10800);//缓存版本号
            }

            //app的版本是否要更新
            //缓存里的isupload是否升级版本检查判断
            string cachKey_isupload = "app_upload_key";
            var cachValue_isupload = CacheProvider.Get<string>(cachKey_isupload);
            bx_config configObj1 = new bx_config();//初始化bx_config对象，方便下文使用
            if (cachValue_isupload == null)
            {
                configObj1 = _bxConfigRepository.FindByConfigKey("app_isupload").FirstOrDefault();
                if (configObj1 == null)
                {
                    viewModel.BusinessStatus = 0;
                    viewModel.StatusMessage = "请求获取是否需要升级信息失败";
                    return viewModel;
                }
                cachValue_isupload = configObj1.config_value;
                CacheProvider.Set(cachKey_isupload, cachValue_isupload, 10800);//缓存版本号
            }

            //解析json对象，取版本号内容
            //需要完善
            List<ConfigViewModel> lst = CommonHelper.ToListT<ConfigViewModel>(cachValue_version);
            ConfigViewModel mo = lst.FirstOrDefault(n => n.Type == request.Type);

            //bengin 20170912 新增来源判断
            //只有苹果使用版本升级的开关，鉴于ios app审核的机制不允许在线更新,安卓在升级判断上都需要做教研
            if (request.Type == 6 && cachValue_isupload == "0")
            {
                viewModel.CompareResult = 0;// 
                viewModel.Version = mo != null ? mo.Ver : "版本信息解析失败";
            }
            else if ((request.Type == 6 && cachValue_isupload == "1") || request.Type == 7)
            {
                //如果取出来消息
                if (mo != null)
                {
                    if (mo.Ver == request.Version)
                    {//如果版本号一致，返回0
                        viewModel.CompareResult = 0;
                        viewModel.Version = mo.Ver;
                    }
                    else
                    {//版本号不一致，返回消息内容 
                        var oldVersion = mo.Ver.Split('.').Select(i => i.PadLeft(2, '0')).ToArray().Select(i => Convert.ToInt64(i)).ToArray();
                        var newVersion = request.Version.Split('.').Select(i => i.PadLeft(2, '0')).ToArray().Select(i => Convert.ToInt64(i)).ToArray();
                        var length = oldVersion.Count() < newVersion.Count() ? oldVersion.Count() : newVersion.Count();
                        var oldBig = 0;
                        for (int i = 0; i < length; i++)
                        {
                            if (oldVersion[i] > newVersion[i])
                            {
                                oldBig = 1;
                                break;
                            }
                            if (oldVersion[i] < newVersion[i])
                            {
                                oldBig = 0;
                                break;
                            }
                            if (oldVersion[i] == newVersion[i])
                            {
                                continue;
                            }
                            if ((i == length - 1) && oldVersion.Count() != newVersion.Count())
                            {
                                if (oldVersion.Count() == length)
                                {
                                    oldBig = 0;
                                    break;
                                }
                                else
                                {
                                    oldBig = 1;
                                    break;
                                }
                            }
                            if ((i == length - 1) && oldVersion.Count() == newVersion.Count())
                            {
                                oldBig = 0;
                                break;
                            }
                        }


                        //当数据库版本小于新的版本号  更新
                        if (oldBig == 1)
                        {
                            int msgid = mo.MsgId;
                            if (msgid != 0)
                            {//取消息，弹框处理
                                bx_message message = _messageRepository.FindById(msgid);
                                viewModel.UpContent = message.Body;
                                viewModel.CompareResult = 1;
                                viewModel.Version = mo.Ver;
                            }
                            else
                            {//如果取出来的消息为0，则表示版本一致
                                viewModel.CompareResult = 0;
                                viewModel.Version = mo.Ver;
                            }
                        }
                        else
                        {
                            viewModel.CompareResult = 0;
                            viewModel.Version = mo.Ver;
                        }

                    }
                }
                //取不出来消息
                else
                {
                    viewModel.CompareResult = 0;
                    viewModel.Version = request.Version;
                }
            }
            else
            {
                viewModel.CompareResult = 0;
            }

            return viewModel;
        }

        public int EditVersion(RequestEditConfig request)
        {
            int result = 0;
            try
            {
                var configObj = _bxConfigRepository.FindByConfigKey("app_version").FirstOrDefault();

                ////bengin 20170912 新增来源判断
                string oldConfigValue = configObj.config_value;
                List<ConfigViewModel> lst = CommonHelper.ToListT<ConfigViewModel>(oldConfigValue);
                ConfigViewModel mo = lst.Where(n => n.Type == request.Type).FirstOrDefault();
                ////end 

                //1.修复了手机系统9.3版本闪退问题
                //2.修复了一系列已知的bug
                //3.其他性能优化

                request.UpContent = request.UpContent.Replace("\\n", "\r\n");

                if (mo.Ver != request.Version)
                {
                    bx_message bxMessage = new bx_message()
                    {
                        Title = "升级通知",
                        Body = request.UpContent,
                        Msg_Type = 8,
                        Create_Time = DateTime.Now,
                        Update_Time = DateTime.Now,
                        Msg_Status = 1,
                        Msg_Level = 1,
                        Send_Time = DateTime.Now,
                        Create_Agent_Id = 2668,
                        Agent_Id = 2668,
                        MsgStatus = "1"
                    };

                    dynamic d = 7;
                    int a = d;

                    int msgId = _messageRepository.Add(bxMessage);

                    List<ConfigViewModel> listConfig = new List<ConfigViewModel>();
                    foreach (var item in lst)
                    {
                        ConfigViewModel mm = new ConfigViewModel();
                        mm.Type = item.Type;
                        mm.Ver = item.Type == request.Type ? request.Version : item.Ver;
                        mm.MsgId = item.Type == request.Type ? msgId : item.MsgId;
                        listConfig.Add(mm);
                    }
                    configObj.config_value = CommonHelper.TToString<List<ConfigViewModel>>(listConfig);
                    result = _bxConfigRepository.Update(configObj);

                    if (result > 0)
                    {
                        var cachKey = "app_version_key";
                        CacheProvider.Remove(cachKey);
                    }
                }
                else
                {
                    result = 1;
                }
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }

        /// <summary>
        /// 修改  是否验证版本号
        /// </summary>
        /// <param name="configKey"></param>
        /// <returns></returns>
        public bool EditIsuploadByKey(RequestKeyConfig request)
        {
            bool result = false;
            try
            {
                result = _bxConfigRepository.UpdateByConfigKey_Isupload(request);
                CacheProvider.Remove("app_upload_key");
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return result;
        }
    }
};
