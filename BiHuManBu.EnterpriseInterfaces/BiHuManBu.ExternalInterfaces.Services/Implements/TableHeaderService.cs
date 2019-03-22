using BiHuManBu.ExternalInterfaces.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BiHuManBu.ExternalInterfaces.Models.ViewModels;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Request;
using BiHuManBu.ExternalInterfaces.Models.ViewModels.Response;
using BiHuManBu.ExternalInterfaces.Models.IRepository;

namespace BiHuManBu.ExternalInterfaces.Services.Implements
{
    public class TableHeaderService : ITableHeaderService
    {
        private readonly ITableHeaderRepository _tableHeaderRepository;

        public TableHeaderService(ITableHeaderRepository tableHeaderRepository)
        {
            _tableHeaderRepository = tableHeaderRepository;
        }

        public async  Task<GetTableHeaderViewModel> GetTableHeaderAsync(GetTableHeaderRequest request)
        {
            var data= await _tableHeaderRepository.FirstOrDefaultAsync(o => o.agent_id == request.ChildAgent && o.table_name == request.TableName);
            var result= GetTableHeaderViewModel.GetModel(Models.BusinessStatusType.OK);
            result.Json = data == null ? "" : data.json;
            return result;
        }

        public async  Task<BaseViewModel> SetTableHeaderAsync(SetTableHeaderRequest request)
        {
            var tableHeader=await _tableHeaderRepository.FirstOrDefaultAsync(o => o.agent_id == request.ChildAgent && o.table_name == request.TableName);
            
            if(tableHeader==null)
            {
                tableHeader = new Models.bx_table_header
                {
                    agent_id=request.ChildAgent,
                    json=request.Json,
                    create_time=DateTime.Now,
                    table_name=request.TableName
                };
                _tableHeaderRepository.Insert(tableHeader);
            }
            else
            {
                tableHeader.json = request.Json;
                _tableHeaderRepository.Update(tableHeader);
            }
            await _tableHeaderRepository.SaveChangesAsync();
            return BaseViewModel.GetBaseViewModel(Models.BusinessStatusType.OK);
        }
    }
}
