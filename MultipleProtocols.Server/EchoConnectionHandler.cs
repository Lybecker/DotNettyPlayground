using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;

namespace MultipleProtocols.Server
{
    public class EchoConnectionHandler : SimpleChannelInboundHandler<string>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<EchoConnectionHandler>();

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