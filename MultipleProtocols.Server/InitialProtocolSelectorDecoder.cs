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

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            message.MarkReaderIndex();

            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(message)}'");

            var text = message.ToString(Encoding);
            message.ResetReaderIndex();

            if (text.StartsWith('A'))
            {
                context.Channel.Pipeline.AddLast("A Decoder", new ADecoder(logRawMessages: _logRawMessages));
                context.Channel.Pipeline.AddLast("A Handler", new AHandler());
                //output.Add(message.Retain());
            }
            else if (text.StartsWith('B'))
            {
                context.Channel.Pipeline.AddLast("B Decoder", new BDecoder(logRawMessages: _logRawMessages));
                context.Channel.Pipeline.AddLast("B Handler", new BHandler());
            }
            else
            {
                context.Channel.Pipeline.AddLast("String Decoder", new StringDecoder());
                context.Channel.Pipeline.AddLast("Echo Handler", new EchoConnectionHandler());
            }

            context.Channel.Pipeline.Remove(this);
        }

        public override bool IsSharable => true;
    }
}