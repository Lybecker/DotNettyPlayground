using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Poco.Comon;

namespace ServerWithPOCO
{
    public class PersonServerHandler : SimpleChannelInboundHandler<Person>
    {
        static readonly IInternalLogger Logger = InternalLoggerFactory.GetInstance<PersonServerHandler>();

        public override void ChannelActive(IChannelHandlerContext ctx)
        {
            ctx.WriteAndFlushAsync("Write your name and age in this format '<name>|<age>': ");

            // Detect when client disconnects
            ctx.Channel.CloseCompletion.ContinueWith((x) => Logger.Info("Channel Closed"));
        }

        protected override void ChannelRead0(IChannelHandlerContext ctx, Person person)
        {
            Logger.Info("Received message: " + person);
            person.Name = person.Name.ToUpper();
            ctx.WriteAndFlushAsync(person);
        }

        public override bool IsSharable => true;
    }
}