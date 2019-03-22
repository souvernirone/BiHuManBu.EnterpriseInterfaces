using Topshelf;

namespace MsgIndexSendService
{
    class Program
    {
        static void Main(string[] args)
        {
            TopShelfExcute();
        }
        private static void TopShelfExcute()
        {
            HostFactory.Run(x =>
            {
                x.Service<MsgIndexSendService>();

                x.SetDescription("消息发送");
                x.SetDisplayName("MsgIndexSendService");
                x.SetServiceName("MsgIndexSendService");

                x.EnablePauseAndContinue();
            });
        }
    }
}
