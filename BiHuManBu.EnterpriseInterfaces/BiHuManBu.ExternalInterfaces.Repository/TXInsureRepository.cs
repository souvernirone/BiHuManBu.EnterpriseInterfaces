using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Repository
{
   public  class TXInsureRepository: ITXInsureRepository
    {
        private EntityContext db = DataContextFactory.GetDataContext();


      


        public tx_clueinsureinfo GetTXInsure(int clueId)
        {

            return DataContextFactory.GetDataContext().tx_clueinsureinfo.Where(x => x.clueid == clueId).FirstOrDefault<tx_clueinsureinfo>();
            

        }


        public bool AddTXInsure(tx_clueinsureinfo model)
        {
            db.tx_clueinsureinfo.Add(model);
            return db.SaveChanges() > 0;
        }




    }
}
