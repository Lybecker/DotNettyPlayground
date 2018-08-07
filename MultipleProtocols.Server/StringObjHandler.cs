using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using MultipleProtocols.Comon;

namespace MultipleProtocols.Server
{
    public class StringObjHandler : SimpleChannelInboundHandler<MessageObj>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<StringObjHandler>();

        protected override void ChannelRead0(IChannelHandlerContext ctx, MessageObj obj)
        {
            ctx.WriteAsync($"You send me a StringObj with message: '{obj.Message}'\r\n");
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }

        public override bool IsSharable => true;
    }
}