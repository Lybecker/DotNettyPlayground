using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultipleProtocols.Comon
{
    public class ADecoder : MessageToMessageDecoder<IByteBuffer>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<ADecoder>();
        readonly Encoding _encoding;
        readonly bool _logRawMessages;

        public ADecoder(Encoding encoding, bool logRawMessages)
        {
            _encoding = encoding ?? throw new NullReferenceException("encoding");
            _logRawMessages = logRawMessages;
        }

        public ADecoder(bool logRawMessages) : this(Encoding.GetEncoding(0), logRawMessages) { }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(message)}'");

            var text = message.ToString(_encoding);

            output.Add(item: new A() { Message = text });
        }

        public override bool IsSharable => true;
    }

    public class A
    {
        public string Message { get; set; }
    }

    public class B
    {
        public int Number { get; set; }
    }
}