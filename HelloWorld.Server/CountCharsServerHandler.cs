using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;

namespace HelloWorldServer
{
    /// <summary>
    /// Counts chars in the msg
    /// Based on the SimpleChannelInboundHandler (but does not inherit from it)
    /// </summary>
    public class CountCharsServerHandler : ChannelHandlerAdapter
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<CountCharsServerHandler>();
        public bool AcceptInboundMessage(object msg) => msg is string;

        private int MessageLenght;

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            if (this.AcceptInboundMessage(msg))
            {
                var stringMessage = (string)msg;
                ChannelReadImpl(ctx, stringMessage);
                ctx.FireChannelRead(msg); // Triggers the next ChannelInboundHandler.ChannelRead in the ChannelPipeline
            }
        }

        void ChannelReadImpl(IChannelHandlerContext ctx, string msg)
        {
            Logger.Info($"Received message length: {msg.Length}");
            MessageLenght = msg.Length;
        }

        /// <summary>
        /// The ChannelReadComplete executes after the entire ChannelPipeline has been executed.
        /// </summary>
        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.WriteAndFlushAsync($"Your message contained {MessageLenght} chars." + System.Environment.NewLine);
            ctx.FireChannelReadComplete();
        }

        public override bool IsSharable => false; // Not shareable as this handler uses a member variable (MessageLenght)
    }
}