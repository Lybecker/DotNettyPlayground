using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace MultipleProtocols.Comon
{
    public class BDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<BDecoder>();
        readonly bool _logRawMessages;

        public BDecoder(bool logRawMessages)
        {
            _logRawMessages = logRawMessages;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            if (_logRawMessages)
                Logger.Debug($"Decoding message: \n\r'{ByteBufferUtil.PrettyHexDump(message)}'");

            var number = message.ReadInt();  // Could have used .ReadIntLE() if the message was transfered as Little-Endian

            output.Add(item: new B() { Number = number });
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }

        public override bool IsSharable => true;
    }
}