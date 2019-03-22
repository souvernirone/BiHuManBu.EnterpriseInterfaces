using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{

    public class TempUserViewModel
    {
        public List<TempUser> tempuser { get; set; }
        public List<RelationDetailInfo> relationDetail { get; set; }

    }

    /// <summary>
    /// 临时关系人模型
    /// </summary>
    public class TempUser : IValidatableObject
    {
        //定义数组
        static List<int> temyGH = new List<int> { 2, 9 };
        //定义数组
        static List<int> temyGR = new List<int> { 1, 5, 14 };
        /// <summary>
        /// 主键编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// bx_agent.Id 代理编号
        /// </summary>
        public int AgentId { get; set; }
        ///// <summary>
        ///// userinfo.id 
        ///// </summary>
        //public int BuId { get; set; }
        /// <summary>
        /// 临时关系人类型 0:个人  1：公户
        /// </summary>
        private bool _tempUserType = false;
        public bool TempUserType
        {
            get { return _tempUserType; }
            set { _tempUserType = value; }
        }
        /// <summary>
        /// 临时关系人姓名
        /// </summary>
        public string TempUserName { get; set; }
        /// <summary>
        /// 临时证件类型
        /// </summary>
        public int TempIdCardType { get; set; }
        /// <summary>
        /// 临时证件号码
        /// </summary>
        public string TempIdCard { get; set; }
        /// <summary>
        /// 临时关系人电话
        /// </summary>
        public string TempUserMobile { get; set; }
        /// <summary>
        /// 临时关系人邮箱
        /// </summary>
        public string TempUserEmail { get; set; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public Nullable<System.DateTime> UpdateTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public Nullable<System.DateTime> CreateTime { get; set; }
        /// <summary>
        /// 逻辑删除标示列  0:未删除  1：已删除
        /// </summary>
        private bool _deleted = false;
        public bool Deleted
        {
            get { return _deleted; }
            set { _deleted = value; }
        }
        /// <summary>
        /// 临时关系人性别:1男,2女
        /// </summary>
        public int? TempUserSex { get; set; }
        /// <summary>
        /// 临时关系人出生日期
        /// </summary>
        public string TempUserBirthday { get; set; }
        /// <summary>
        /// 标签类型：0：去年投保，1：临时车主，2：正常值
        /// </summary>
        public int TagTypeTempUser { get; set; }
        /// <summary>
        /// 标签类型：0：去年投保，1：临时被保险人，2：正常值
        /// </summary>
        public int TagTypeTempInsured { get; set; }

        /// <summary>
        /// 标签类型：0：去年投保，1：临时投保人，2：正常值
        /// </summary>
        public int TagTypeCover { get; set; }
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            List<ValidationResult> validationResult = new List<ValidationResult>();

            if (!TempUserType)
            {
                if (!temyGR.Contains(TempIdCardType))
                {

                    validationResult.Add(new ValidationResult("选择临时类型为个人时,证件类型输入不合理!"));
                }
                if (temyGR.Contains(TempIdCardType))
                {
                    if (TempIdCardType == 5 || TempIdCardType == 14)
                    {
                        if (TempIdCard != "" & (TempIdCard.Length < 1 || TempIdCard.Length > 32))
                        {
                            validationResult.Add(new ValidationResult("港澳通行证或港澳身份证, 最少1个字符 最多32个"));
                        }
                    }

                }

            }
            else
            {
                if (!temyGH.Contains(TempIdCardType))
                {
                    validationResult.Add(new ValidationResult("选择临时类型为公户时,证件类型输入不合理!"));
                }
            }

            return validationResult;
        }

    }
    /// <summary>
    /// 中间表模型
    /// </summary>
    public class RelationDetailInfo
    {
        public int Id { get; set; }
        public int TuId { get; set; }
        public int BuId { get; set; }
        public bool Deleted { get; set; }
        //1：临时车主 2临时被保险人
        public int TempType { get; set; }
    }
}
