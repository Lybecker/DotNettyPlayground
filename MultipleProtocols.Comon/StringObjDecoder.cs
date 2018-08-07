using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultipleProtocols.Comon
{
    public class StringObjDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<StringObjDecoder>();
        readonly Encoding _encoding;
        readonly bool _logRawMessages;

        public StringObjDecoder(Encoding encoding, bool logRawMessages)
        {
            _encoding = encoding ?? throw new NullReferenceException("encoding");
            _logRawMessages = logRawMessages;
        }

        public StringObjDecoder(bool logRawMessages) : this(Encoding.GetEncoding(0), logRawMessages) { }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer input, List<object> output)
        {
            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(input)}'");

            var text = input.ToString(_encoding);

            output.Add(item: new MessageObj() { Message = text });
        }

        public override bool IsSharable => true;
    }
}