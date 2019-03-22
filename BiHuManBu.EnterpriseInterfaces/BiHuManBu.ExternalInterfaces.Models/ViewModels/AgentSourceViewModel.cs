using System.Collections.Generic;

namespace BiHuManBu.ExternalInterfaces.Models.ViewModels
{
    public class AgentSourceViewModel:BaseViewModel
    {
        public List<Source> SourceList { get; set; }

        public List<AgentCity> AgentCity { get; set; }
    }


    public class Source
    {
        public int Id { get; set; }
        public long NewId { get; set; }
        public string Name { get; set; }
        //public string ImageUrl { get; set; }
    }

    public class NewAgentSourceViewModel : BaseViewModel
    {
        public List<NewSource> SourceList { get; set; }

        public List<NewAgentCity> AgentCity { get; set; }
    }

    public class NewSource
    {
        public int Id { get; set; }
        public long NewId { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
    }

    public class NewAgentCity
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public List<NewSource> AgentSource { get; set; }
    }

    public class AgentCity
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public List<Source> AgentSource { get; set; }
    }
}
