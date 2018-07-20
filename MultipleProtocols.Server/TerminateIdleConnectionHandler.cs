using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Channels;

namespace MultipleProtocols.Server
{
    public class TerminateIdleConnectionHandler : ChannelHandlerAdapter
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<TerminateIdleConnectionHandler>();

        public override void UserEventTriggered(IChannelHandlerContext ctx, object evt)
        {
            var e = evt as IdleStateEvent;

            if (e == null)
                return;

            Logger.Info($"Idle event triggered: {e.State}");

            //if (e.State == IdleState.ReaderIdle)
            //    ctx.CloseAsync();
            //else if (e.State == IdleState.WriterIdle)

            ctx.WriteAndFlushAsync("What are your doing? Not sending me any data...\r\n");
        }

        public override bool IsSharable => true;
    }
}