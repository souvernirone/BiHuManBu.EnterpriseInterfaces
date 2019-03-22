
using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels.AppViewModels
{
    public class AgentSourceViewModel : BaseViewModel
    {
        public List<int> ComList { get; set; }

    }

    public class AppAgentSourceViewModel : BaseViewModel
    {
        public List<Source> SourceList { get; set; }

        public List<AgentCity> AgentCity { get; set; }
    }

    public class Source
    {
        public int Id { get; set; }
        public long NewId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public class AgentCity
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public List<Source> AgentSource { get; set; }
    }
}
