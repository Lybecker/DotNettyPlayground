using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;

namespace MultipleProtocols.Server
{
    public class BHandler : SimpleChannelInboundHandler<B>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<BHandler>();

        protected override void ChannelRead0(IChannelHandlerContext ctx, B obj)
        {
            ctx.WriteAsync($"You send me an B object with message: '{obj.Number}'\r\n");
        }

        public override bool IsSharable => true;
    }
}