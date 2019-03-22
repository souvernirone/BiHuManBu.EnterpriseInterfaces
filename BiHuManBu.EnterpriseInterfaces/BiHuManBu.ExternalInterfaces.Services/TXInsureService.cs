using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models;
using BiHuManBu.ExternalInterfaces.Models.IRepository;
using BiHuManBu.ExternalInterfaces.Services.Interfaces;

namespace BiHuManBu.ExternalInterfaces.Services
{
   public  class TXInsureService: ITXInsureService
    {
        private ITXInsureRepository _tXInsureRepository;
        

        public TXInsureService(
            ITXInsureRepository tXInsureRepository
            )
        {
            _tXInsureRepository = tXInsureRepository;
        }

        
        public string GetTXInsure(int clueId) {


            tx_clueinsureinfo insure =  _tXInsureRepository.GetTXInsure(clueId);
            if (insure!=null) {
                return insure.InsureInfo;
            }
            return "";
           
        }


        public bool AddTXInsure(int clueId, string insureInfo) {

         return   _tXInsureRepository.AddTXInsure(new Models.tx_clueinsureinfo() {
                 clueid= clueId, InsureInfo= insureInfo, createTime=DateTime.Now, deleted=0, updatetime=DateTime.Now
            });
        
        }
    }
}
