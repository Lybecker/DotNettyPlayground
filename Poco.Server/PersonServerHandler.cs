using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Channels;
using Poco.Comon;

namespace Poco.Server
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

        // The Channel is closed hence the connection is closed
        public override void ChannelInactive(IChannelHandlerContext ctx) => Logger.Info("Client disconnected");

        protected override void ChannelRead0(IChannelHandlerContext ctx, Person person)
        {
            Logger.Info("Received message: " + person);
            person.Name = person.Name.ToUpper();
            ctx.WriteAndFlushAsync(person);
        }

        public override bool IsSharable => true;
    }
}