using DotNetty.Transport.Channels;
using Poco.Comon;
using System;

namespace Poco.Client
{
    public class PersonClientHandler : SimpleChannelInboundHandler<Person>
    {
        protected override void ChannelRead0(IChannelHandlerContext contex, Person person)
        {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("Data returned from Server:");
            Console.WriteLine(person.ToString());
            Console.ResetColor();
        }

        public override void ExceptionCaught(IChannelHandlerContext contex, Exception e)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(DateTime.Now.Millisecond);
            Console.WriteLine("{0}", e.StackTrace);
            contex.CloseAsync();
            Console.ResetColor();
        }
    }
}