using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System.Collections.Generic;

namespace MultipleProtocols.Comon
{
    public class NumberObjDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<NumberObjDecoder>();
        readonly bool _logRawMessages;

        public NumberObjDecoder(bool logRawMessages)
        {
            _logRawMessages = logRawMessages;
        }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer input, List<object> output)
        {
            if (_logRawMessages)
                Logger.Debug($"Decoding message: \n\r'{ByteBufferUtil.PrettyHexDump(input)}'");

            if (input.ReadableBytes != 4)
                return;

            var number = input.ReadInt();  // Could have used .ReadIntLE() if the message was transfered as Little-Endian

            output.Add(item: new NumberObj() { Number = number });
        }

        public override void ChannelReadComplete(IChannelHandlerContext ctx)
        {
            ctx.Flush();
        }

        public override bool IsSharable => true;
    }
}