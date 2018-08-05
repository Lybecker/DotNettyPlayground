using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using MultipleProtocols.Comon;

namespace MultipleProtocols.Server
{
    public class BHandler : SimpleChannelInboundHandler<B>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<BHandler>();

        protected override void ChannelRead0(IChannelHandlerContext ctx, B obj)
        {
            ctx.WriteAsync($"You send me an B object with number: '{obj.Number}'\r\n");
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }
        public override bool IsSharable => true;
    }
}