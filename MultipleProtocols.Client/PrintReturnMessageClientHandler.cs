using DotNetty.Transport.Channels;
using System;

namespace MultipleProtocols.Client
{
    public class PrintReturnMessageClientHandler : ChannelHandlerAdapter
    {
        public override void ChannelRead(IChannelHandlerContext ctx, object obj)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Message returned from Server:");
            Console.WriteLine(obj.ToString());
            Console.ResetColor();
        }

        public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("{0}", e.StackTrace);
            contex.CloseAsync();
            Console.ResetColor();
        }
    }
}