using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using MultipleProtocols.Comon;
using System.Collections.Generic;
using System.Text;

namespace MultipleProtocols.Server
{
    /// <summary>
    /// The initial message will determing the protocol. The session with the server will keep using the same protocol.
    /// 
    /// 1st message
    ///     A | B
    /// 
    /// Sequencial messages has to map to the expected handler
    /// 
    /// The MultipleProtocols.Client has to be modified to work with this decoder, as it sends the protocol selector with every message.
    /// </summary>
    public class InitialProtocolSelectorDecoder : MessageToMessageDecoder<IByteBuffer> 
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<InitialProtocolSelectorDecoder>();
        readonly Encoding Encoding = Encoding.ASCII;
        readonly bool _logRawMessages;

        public InitialProtocolSelectorDecoder(bool logRawMessages)
        {
            _logRawMessages = logRawMessages;
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer message, List<object> output)
        {
            message.MarkReaderIndex();

            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(message)}'");

            var text = message.ToString(Encoding);
            message.ResetReaderIndex();

            if (text.StartsWith('A'))
            {
                ctx.Channel.Pipeline.AddLast("StringObj Decoder", new StringObjDecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("StringObj Handler", new StringObjHandler());
                //output.Add(message.Retain());
            }
            else if (text.StartsWith('B'))
            {
                ctx.Channel.Pipeline.AddLast("NumberObj Decoder", new NumberObjDecoder(logRawMessages: _logRawMessages));
                ctx.Channel.Pipeline.AddLast("NumberObj Handler", new NumberObjHandler());
            }
            else
            {
                // Unknown protocol; discard and close the connection.
                message.Clear();
                ctx.CloseAsync();
            }

            ctx.Channel.Pipeline.Remove(this);
        }

        public override bool IsSharable => true;
    }
}