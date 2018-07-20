using DotNetty.Common.Internal.Logging;
using DotNetty.Common.Utilities;
using DotNetty.Transport.Channels;

namespace HelloWorld.Server
{
    /// <summary>
    /// Detects if a message has upper chars in the string
    /// Based on the SimpleChannelInboundHandler (but does not inherit from it)
    /// </summary>
    public class HasUpperCharsServerHandler : ChannelHandlerAdapter
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<HasUpperCharsServerHandler>();
        public bool AcceptInboundMessage(object msg) => msg is string;

        public override void ChannelRead(IChannelHandlerContext ctx, object msg)
        {
            try
            {
                if (this.AcceptInboundMessage(msg))
                {
                    var stringMessage = (string)msg;
                    ChannelReadImpl(ctx, stringMessage);
                    ctx.FireChannelRead(msg); // Triggers the next ChannelInboundHandler.ChannelRead in the ChannelPipeline
                }
            }
            finally
            {
                // As the msg is passed to the next ChannelHandler via the FireChannelRead, reference counting is not required. See https://github.com/netty/netty/wiki/Reference-counted-objects#reference-counting-in-channelhandler
                ReferenceCountUtil.Release(msg);
            }
        }

        void ChannelReadImpl(IChannelHandlerContext ctx, string msg)
        {
            if (msg.HasUpperChar())
            {
                Logger.Info("The message has upper chars.");
                ctx.WriteAsync("Your message has upper chars." + System.Environment.NewLine);
            }
        }

        public override bool IsSharable => true;
    }

    static class StringExtensions
    {
        public static bool HasUpperChar(this string value)
        {
            for (int i = 0; i < value.Length; i++)
            {
                if (char.IsUpper(value[i]))
                {
                    return true;
                }
            }
            return false;
        }
    }
}