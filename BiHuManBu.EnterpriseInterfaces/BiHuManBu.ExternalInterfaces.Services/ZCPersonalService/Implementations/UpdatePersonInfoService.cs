﻿using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Services.Messages.Request;
using BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Interfaces;
using System;
using System.Transactions;

namespace BiHuManBu.ExternalInterfaces.Services.ZCPersonalService.Implementations
{
    public class UpdatePersonInfoService : IUpdatePersonInfoService
    {
        private readonly IZCPersonalRepository _zcPersonalRepository;
        private readonly IGroupAuthenRepository _groupAuthenRepository;

        public UpdatePersonInfoService(IZCPersonalRepository zcPersonalRepository, IGroupAuthenRepository groupAuthenRepository)
        {
            _zcPersonalRepository = zcPersonalRepository;
            _groupAuthenRepository = groupAuthenRepository;
        }

        /// <summary>
        /// 修改用户信息 sjy 2018-2-4
        /// </summary>
        /// <param name="person"></param>
        /// <returns></returns>
        public BaseViewModel UpdatePerson(ZCPersonRequest person)
        {
            bool result = false;

            //验证身份证是否已经绑定
            if (!string.IsNullOrEmpty(person.CardId))
            {
                var groupAuthen = _groupAuthenRepository.GetModel(person.CardId);
                if (groupAuthen.BusinessStatus == 0)
                {
                    if (groupAuthen.Data != null && (groupAuthen.Data as bx_group_authen).agentId!=person.AgentId)
                    {
                        return BaseViewModel.GetBaseViewModel(-10001, "身份证号已经存在");
                    }
                }
                else
                {
                    return BaseViewModel.GetBaseViewModel(groupAuthen.BusinessStatus, groupAuthen.StatusMessage);
                }
            }

            //验证银行卡否已经绑定
            if (!string.IsNullOrEmpty(person.BankCardNo))
            {
                var groupAuthen = _groupAuthenRepository.GetModel("", person.BankCardNo);
                if (groupAuthen.BusinessStatus == 0)
                {
                    if (groupAuthen.Data != null && (groupAuthen.Data as bx_group_authen).agentId != person.AgentId)
                    {
                        return BaseViewModel.GetBaseViewModel(-10002, "银行卡号已经存在");
                    }
                }
                else
                {
                    return BaseViewModel.GetBaseViewModel(groupAuthen.BusinessStatus, groupAuthen.StatusMessage);
                }
            }

            var personModel = _groupAuthenRepository.Get(person.Id);
            if (personModel != null)
            {
                personModel.agentId = person.AgentId;
                personModel.cardholder = person.CardHolder;
                //2018-09-20 张克亮 小V盟项目时 改动如下：认证状态取传入的参，默认值为0
                personModel.authen_state = person.AuthenState;
                personModel.card_id = person.CardId;
                personModel.card_face_url = person.CardFace;
                personModel.card_reverse_url = person.CardReverse;
                personModel.bank_id =string.IsNullOrEmpty(person.BankId)?0:int.Parse(person.BankId);
                personModel.bank_card_number = person.BankCardNo;
                personModel.bankcard_face_url = person.BankCardFront;
                personModel.bankcard_reverse_url = person.BankCardBack;
                _groupAuthenRepository.UpdateModel(personModel);
                result = true;
            }

            if (result)
            {
                return BaseViewModel.GetBaseViewModel(1, "OK");
            }
            return BaseViewModel.GetBaseViewModel(-10007, "操作失败");
        }

    }
}