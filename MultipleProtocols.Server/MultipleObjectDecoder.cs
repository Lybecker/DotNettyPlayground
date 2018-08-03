using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultipleProtocols.Server
{
    public class MultiObjectDecoder : MessageToMessageDecoder<IByteBuffer> 
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<MultiObjectDecoder>();
        readonly Encoding _encoding;
        readonly bool _logRawMessages;

        public MultiObjectDecoder(Encoding encoding, bool logRawMessages)
        {
            _encoding = encoding ?? throw new NullReferenceException("encoding");
            _logRawMessages = logRawMessages;
        }

        public MultiObjectDecoder(bool logRawMessages) : this(Encoding.GetEncoding(0), logRawMessages) { }

        protected override void Decode(IChannelHandlerContext context, IByteBuffer message, List<object> output)
        {
            if (_logRawMessages)
                Logger.Debug($"Decoding message: '{ByteBufferUtil.HexDump(message)}'");

            var text = message.ToString(_encoding);

            if (text.StartsWith('A'))
            {
                output.Add(item: new A() { Message = text.Substring(1) });
            }
            else if (text.StartsWith('B'))
            {
                if (int.TryParse(text.Substring(1), out int val))
                {
                    output.Add(item: new B() { Number = val });
                }
            }
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