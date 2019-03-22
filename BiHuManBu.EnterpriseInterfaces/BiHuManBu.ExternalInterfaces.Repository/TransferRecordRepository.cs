using BiHuManBu.ExternalInterfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using log4net;

namespace BiHuManBu.ExternalInterfaces.Repository
{
    public class TransferRecordRepository : ITransferRecordRepository
    {
        private ILog logError;
        public TransferRecordRepository()
        {
            logError = LogManager.GetLogger("ERROR");
        }
        public long Add(bx_transferrecord record)
        {
            long recordid = 0;
            try
            {
                var rcd = DataContextFactory.GetDataContext().bx_transferrecord.Add(record);
                DataContextFactory.GetDataContext().SaveChanges();
                recordid = rcd.Id;
            }
            catch (Exception ex)
            {
                logError.Info("发生异常:" + ex.Source + "\n" + ex.StackTrace + "\n" + ex.Message + "\n" + ex.InnerException);
            }
            return recordid;
        }

        public List<bx_transferrecord> FindListByBuidList(List<long> buids)
        {
            return DataContextFactory.GetDataContext().bx_transferrecord.Where(x => buids.Contains(x.BuId) && x.ToAgentId == 0).ToList();
        }

        public List<bx_transferrecord> FindByBuid(long buid)
        {
            return DataContextFactory.GetDataContext().bx_transferrecord.OrderByDescending(o => o.Id).Where(x => x.BuId == buid).ToList();
        }

        /// <summary>
        /// 取sa的第一条记录
        /// </summary>
        /// <param name="buid"></param>
        /// <returns></returns>
        public bx_transferrecord FindFirstSaByBuid(long buid)
        {
            return DataContextFactory.GetDataContext().bx_transferrecord.OrderBy(o => o.Id).FirstOrDefault(x => x.BuId == buid && x.StepType == 1);
        }

        public bool SaveTransferRecord(long buId, int fromAgentId, int stepType, int? ToAgentId)
        {
            bool isSuccess = false;
            using (var _dbContext = new EntityContext())
            {
                var transferRecord = _dbContext.bx_transferrecord.SingleOrDefault(x => !x.Deleted && x.BuId == buId && x.StepType == stepType);
                if (transferRecord == null)
                {
                    PrepareContext(buId, fromAgentId, stepType, ToAgentId, _dbContext);
                    isSuccess = _dbContext.SaveChanges() > 0 ? true : false;
                }
                else
                {
                   
                    transferRecord.Deleted = true;
                    PrepareContext(buId, fromAgentId, stepType, ToAgentId,  _dbContext);
                    isSuccess = _dbContext.SaveChanges() > 0 ? true : false;

                }
            }
            return isSuccess;


        }

        private void PrepareContext(long buId, int fromAgentId, int stepType, int? ToAgentId,  EntityContext _dbContext)
        {
            _dbContext.bx_transferrecord.Add(new bx_transferrecord()
            {
                BuId = buId,
                CreateAgentId = fromAgentId,
                CreateTime = DateTime.Now,
                Deleted = false,
                FromAgentId = fromAgentId,
                StepType = stepType,
                ToAgentId = ToAgentId,
                UpdateTime = DateTime.Now
            });
        }

    }
}
