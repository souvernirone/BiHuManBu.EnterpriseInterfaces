using BiHuManBu.ExternalInterfaces.Models.Model;
using System;
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.Dtos
{
    public class SearchOrderDto
    {
        private List<DDOrder> _listOrder = new List<DDOrder>();

        /// <summary>
        /// 
        /// </summary>
        public List<DDOrder> ListOrder { get { return _listOrder; } set { _listOrder = value; } }

        /// <summary>
        /// 
        /// </summary>
        public int TotalCount { get; set; }

    }
}
