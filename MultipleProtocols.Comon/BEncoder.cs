using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;

namespace MultipleProtocols.Comon
{
    public class BEncoder : MessageToMessageEncoder<B>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<BEncoder>();
        private readonly bool _logRawMessages;

        public BEncoder(bool logRawMessages)
        {
            _logRawMessages = logRawMessages;
        }

        protected override void Encode(IChannelHandlerContext context, B obj, List<object> output)
        {
            IByteBuffer buffer = context.Allocator.Buffer();

            // Specify the protocol selector
            buffer.WriteByte((byte)'B');

            // BitConverter returns a Little-Endian byte array
            // But DotNetty defaults to Big-Endian - fixed by reversing the array, so the data is Little-Endian
            var data = BitConverter.GetBytes(obj.Number);
            Array.Reverse(data);                              
            buffer.WriteBytes(data);

            // Use Windows style newline to indicate the data frame is complete.
            // As the lenght of this message type is know, then the LineBasedFrameDecoder could be used at the server
            buffer.WriteByte('\r');
            buffer.WriteByte('\n');

            if (_logRawMessages)
                Logger.Debug($"Decoding message: \n\r'{ByteBufferUtil.PrettyHexDump(buffer)}'");

            output.Add(buffer);
        }
    }
}