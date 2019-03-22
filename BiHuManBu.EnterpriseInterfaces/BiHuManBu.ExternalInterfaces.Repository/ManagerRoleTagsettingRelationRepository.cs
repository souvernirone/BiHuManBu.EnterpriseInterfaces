using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class ManagerRoleTagsettingRelationRepository : IManagerRoleTagsettingRelationRepository
    {
        private ILog logError;
        public ManagerRoleTagsettingRelationRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }

        /// <summary>
        /// 添加角色跟组 的关系
        /// </summary>
        /// <param name="roleId"></param>
        /// <param name="tagId"></param>
        //public void AddRoleTagsetting(List<manager_role_db> roleItem, List<bx_tagsetting> tagsettingItem)
        //{
        //    try
        //    {
        //        foreach (var role in roleItem)
        //        {
        //            var item = new manager_role_tagsetting_relation()
        //            {
        //                role_id = role.id,
        //                //tagId = (tagsettingItem.FirstOrDefault(x => x.Type == role.role_type) ?? new bx_tagsetting()).Id,
        //                creator_time = DateTime.Now
        //            };
        //            _context.manager_role_tagsetting_relation.Add(item);
        //            _context.SaveChanges();
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
        //    }
        //}
    }
}
