using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using MultipleProtocols.Comon;
using System.Collections.Generic;

namespace MultipleProtocols.Server
{
    /// <summary>
    /// The first byte of every message decised the protokol and therefore the decoder and handler used. At any time the client can change protocol just by specifying a different value in the first byte
    /// </summary>
    public class ProtocolSelectorDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<ProtocolSelectorDecoder>();
        readonly bool _logRawMessages;

        public ProtocolSelectorDecoder(bool logRawMessages)
        {
            _logRawMessages = logRawMessages;
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer message, List<object> output)
        {
            message.MarkReaderIndex();

            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(message)}'");

            if (message.ReadableBytes < 1)
                return;

            var protocolSelector = message.ReadByte();

            if (protocolSelector == 'A')
            {
                EnableAHanderProtocol(ctx);
                DisableBHanderProtocol(ctx);

                output.Add(message.Retain());
            }
            else if (protocolSelector == 'B')
            {
                EnableBHanderProtocol(ctx);
                DisableAHanderProtocol(ctx);

                output.Add(message.Retain());
            }
            else
            {
                // Unknown protocol; discard and close the connection.
                message.Clear();
                ctx.CloseAsync();
            }
        }

        private bool IsProtocolPipeline<T>(IChannelHandlerContext ctx) where T : class, IChannelHandler
        {
            if (ctx.Channel.Pipeline.Get<T>() == null)
                return false;

            return true;
        }

        private void EnableAHanderProtocol(IChannelHandlerContext ctx)
        {
            if (!IsProtocolPipeline<AHandler>(ctx))
            {
                ctx.Channel.Pipeline.AddLast("A Decoder", new ADecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("A Handler", new AHandler());
            }
        }

        private void EnableBHanderProtocol(IChannelHandlerContext ctx)
        {
            if (!IsProtocolPipeline<BHandler>(ctx))
            {
                ctx.Channel.Pipeline.AddLast("B Decoder", new BDecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("B Handler", new BHandler());
            }
        }

        private void DisableAHanderProtocol(IChannelHandlerContext ctx)
        {
            if (IsProtocolPipeline<AHandler>(ctx))
            {
                ctx.Channel.Pipeline.Remove<ADecoder>();
                ctx.Channel.Pipeline.Remove<AHandler>();
            }
        }

        private void DisableBHanderProtocol(IChannelHandlerContext ctx)
        {
            if (IsProtocolPipeline<BHandler>(ctx))
            {
                ctx.Channel.Pipeline.Remove<BDecoder>();
                ctx.Channel.Pipeline.Remove<BHandler>();
            }
        }

        public override bool IsSharable => true;
    }
}