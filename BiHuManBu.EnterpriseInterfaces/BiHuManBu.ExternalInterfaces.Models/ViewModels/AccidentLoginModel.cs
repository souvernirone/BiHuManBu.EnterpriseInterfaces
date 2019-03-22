using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AccidentLoginModel
    {
        public int AgentId { get; set; }
        private string _name;
        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get { return _name != null ? _name : ""; } set { _name = value; } }
       
        private string _account;
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get { return _account != null ? _account : ""; } set { _account = value; } }
     
        /// <summary>
        /// 上级id
        /// </summary>
        public int TopAgentId { get; set; }
        /// <summary>
        /// 角色id
        /// </summary>
        public int RoleId { get; set; }
        /// <summary>
        /// 角色类型
        /// </summary>
        public int RoleType { get; set; }

        
        private string _roleName;
        /// <summary>
        /// 角色名称
        /// </summary>
        public string RoleName { get { return _roleName != null ? _roleName : ""; } set { _roleName = value; } }
        /// <summary>
        /// 部门id
        /// </summary>
        public int DepartmentId { get; set; }
        private string _secretKey;
        public string SecretKey { get { return _secretKey != null ? _secretKey : ""; } set { _secretKey = value; } }

        /// <summary>
        /// 顶级账号所在城市
        /// </summary>
        public int CityId { get; set; }
        
        private string _token;
        /// <summary>
        /// token 秘钥
        /// </summary>
        public string Token { get { return _token != null ? _token : ""; } set { _token = value; } }

        /// <summary>
        /// FunctionCode
        /// </summary>
        public List<FunctionCodeModel> FunctionCode { get; set; }
    }

    public class FunctionCodeModel
    {
        public int AgentId { get; set; }

        public string FunctionCode { get; set; }
    }
}
