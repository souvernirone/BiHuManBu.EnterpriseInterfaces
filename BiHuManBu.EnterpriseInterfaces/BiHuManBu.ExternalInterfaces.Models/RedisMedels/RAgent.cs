using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.RedisMedels
{
   public enum EAgent
    {
        pid,
        tid,
        level


    }

    public class RAgent
    {

        public RAgent(int id, int pid, int tid, int level) {
            this.id = id;
            this.pid = pid;
            this.tid = tid;
            this.level = level;

        }
        public int id { get; set; }
       public int pid { get; set; }
        public int tid { get; set; }
        public int level { get; set; }
       


    }


}
