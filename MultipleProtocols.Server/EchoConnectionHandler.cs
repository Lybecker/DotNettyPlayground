using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;

namespace MultipleProtocols.Server
{
    public class EchoConnectionHandler : SimpleChannelInboundHandler<string>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<EchoConnectionHandler>();

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            ctx.WriteAsync("Hello,\r\n");
            ctx.WriteAndFlushAsync("Write anything and the server will return the message\r\n");
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, string msg)
        {
            ctx.WriteAsync($"You send me: '{msg}'\r\n");
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }
    }
}