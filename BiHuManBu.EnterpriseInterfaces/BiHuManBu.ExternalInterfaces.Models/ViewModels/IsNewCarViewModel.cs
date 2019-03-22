using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    /// <summary>
    /// 是否是新车模型
    /// </summary>
    public class IsNewCarViewModel
    {
        /// <summary>
        /// 
        /// </summary>
        public long Buid { get; set; }


        private int _isNewCar = 2;

        /// <summary>
        /// 是否新车（1：新车2：旧车（默认）
        /// 如果库里是0或者null，返回2
        /// </summary>
        public int? IsNewCar
        {
            get
            {
                return _isNewCar;
            }
            set
            {
                if (value.HasValue && value.Value == 1)
                {
                    _isNewCar = 1;
                }
                else
                {
                    _isNewCar = 2;
                }
            }
        }
    }
}
