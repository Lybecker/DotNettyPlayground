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
            if (!IsProtocolPipeline<StringObjHandler>(ctx))
            {
                ctx.Channel.Pipeline.AddLast("StringObj Decoder", new StringObjDecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("StringObj Handler", new StringObjHandler());
            }
        }

        private void EnableBHanderProtocol(IChannelHandlerContext ctx)
        {
            if (!IsProtocolPipeline<NumberObjHandler>(ctx))
            {
                ctx.Channel.Pipeline.AddLast("NumberObj Decoder", new NumberObjDecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("NumberObj Handler", new NumberObjHandler());
            }
        }

        private void DisableAHanderProtocol(IChannelHandlerContext ctx)
        {
            if (IsProtocolPipeline<StringObjHandler>(ctx))
            {
                ctx.Channel.Pipeline.Remove<StringObjDecoder>();
                ctx.Channel.Pipeline.Remove<StringObjHandler>();
            }
        }

        private void DisableBHanderProtocol(IChannelHandlerContext ctx)
        {
            if (IsProtocolPipeline<NumberObjHandler>(ctx))
            {
                ctx.Channel.Pipeline.Remove<NumberObjDecoder>();
                ctx.Channel.Pipeline.Remove<NumberObjHandler>();
            }
        }

        public override bool IsSharable => true;
    }
}