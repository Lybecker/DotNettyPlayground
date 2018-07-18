using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Transport.Channels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Poco.Comon
{
    /// <summary>
    /// Custom Person object decoder
    /// // Format: <Name>|<Age>
    /// </summary>
    public class PersonDecoder : MessageToMessageDecoder<IByteBuffer>
    {
        readonly Encoding _encoding;

        public PersonDecoder() : this(Encoding.GetEncoding(0))
        {}

        public PersonDecoder(Encoding encoding)
        {
            _encoding = encoding ?? throw new NullReferenceException("encoding");
        }

        protected override void Decode(IChannelHandlerContext ctx, IByteBuffer message, List<object> output)
        {
            var text = message.ToString(_encoding);

            Match match = Regex.Match(text, @"^(?<Name>\w+)\|(?<Age>\d{1,3})[\r\n]?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (match.Success)
            {
                string name = match.Groups["Name"].Value;
                int age = int.Parse(match.Groups["Age"].Value);

                output.Add(new Person() { Name = name, Age = age });
            }
        }

        public override bool IsSharable => true;
    }
}