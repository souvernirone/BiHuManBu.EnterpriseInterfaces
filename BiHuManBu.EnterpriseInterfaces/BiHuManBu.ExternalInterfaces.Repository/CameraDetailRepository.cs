using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class CameraDetailRepository : EfRepositoryBase<bx_camera_detail>, ICameraDetailRepository
    {
        /// <summary>
        /// 默认字段
        /// </summary>
        private const string DEFAULT_FIELDS = " detail.id,detail.car_plate,detail.camera_id,detail.createtime ";

        /// <summary>
        /// 默认模板
        /// </summary>
        private const string DEFAULT_FORMAT = @"
        SELECT 
         {0}
        FROM
        bx_camera_detail AS detail
        WHERE
         detail.camera_id IN (SELECT 
            camera_id
        FROM
            bx_camera_config
        WHERE
            park_id = '{1}')
        AND 
        ";
        private const string CAMERA_FORMAT = @"
        SELECT 
         {0}
        FROM
        bx_camera_detail  AS detail
        WHERE
            detail.park_id = '{1}'
        AND 
        ";

        public CameraDetailRepository(DbContext context)
            : base(context)
        {
        }

        public int GetCount(int agent, DateTime createStartTime, DateTime createEndTime)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format(CAMERA_FORMAT, " COUNT(1) ", agent));
            GenerateSql(ref builder, createStartTime, createEndTime);
            var sql = builder.ToString();
            return Context.Database.SqlQuery<int>(sql).FirstOrDefault();
        }

        public List<CameraDetailModel> GetList(int agent, DateTime createStartTime, DateTime createEndTime, List<int> listId)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format(CAMERA_FORMAT, DEFAULT_FIELDS, agent));
            GenerateSql(ref builder, createStartTime, createEndTime, listId);
            var sql = builder.ToString();
            return Context.Database.SqlQuery<CameraDetailModel>(sql).ToList();
        }

        public List<CameraDetailModel> GetPageList(int pageIndex, int pageSize, int agent, DateTime createStartTime, DateTime createEndTime)
        {
            var builder = new StringBuilder();
            builder.Append(string.Format(CAMERA_FORMAT, DEFAULT_FIELDS, agent));

            GenerateSql(ref builder, createStartTime, createEndTime);
            builder.Append(" ORDER BY detail.createtime DESC  ");
            builder.Append(string.Format(" LIMIT {0},{1} ", (pageIndex - 1) * pageSize, pageSize));

            var sql = builder.ToString();
            return Context.Database.SqlQuery<CameraDetailModel>(sql).ToList();
        }


        private void GenerateSql(ref StringBuilder builder, DateTime createStartTime, DateTime createEndTime, List<int> listId = null)
        {
            builder.Append(" 1=1 ");
            if (listId != null && listId.Any())
            {
                builder.Append(string.Format(" AND detail.id in ({0}) ", string.Join(",", listId)));
            }
            builder.Append(string.Format(" AND detail.createtime BETWEEN '{0}' AND '{1}' ", createStartTime.ToString(), createEndTime.ToString()));
        }
    }
}
