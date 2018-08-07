using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using MultipleProtocols.Comon;

namespace MultipleProtocols.Server
{
    public class NumberObjHandler : SimpleChannelInboundHandler<NumberObj>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<NumberObjHandler>();

        protected override void ChannelRead0(IChannelHandlerContext ctx, NumberObj obj)
        {
            ctx.WriteAsync($"You send me a NumberObj with number: '{obj.Number}'\r\n");
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }
        public override bool IsSharable => true;
    }
}