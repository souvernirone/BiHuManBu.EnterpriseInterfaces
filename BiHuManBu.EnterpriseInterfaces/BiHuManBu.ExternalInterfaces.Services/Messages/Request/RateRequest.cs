namespace BiHuManBu.ExternalInterfaces.Services.Messages.Request
{
    public  class RateRequest
    {
      
       public long Buid { get; set; }
       public int ChannelId { get; set; }
    
       public int Source { get; set; }
       public string CarLicense { get; set; }
    }
}
