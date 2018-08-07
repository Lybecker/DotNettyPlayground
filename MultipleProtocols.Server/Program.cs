using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Handlers.Timeout;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;

namespace MultipleProtocols.Server
{
    /*
     * This solution shows how to handle multiple protocols with the same endpoint/port
     *  - Two object types (StringObj and NumberObj) each decoded differently
     *   - The first char is used as the protocol selector
     *    - A = StringObj
     *    - B = NumberObj
     *  - There are multiple ways of handling multiple protocols
     *   - The initial message sets the protocol - see InitialProtocolSelectorDecoder
     *   - Every message forward the protocol selector char - See ProtocolSelectorDecoder
     *  - The channel pipeline is modified dynamically via one of the to ProtocolSelectorDecoders, so the pipeline in Program.cs does not have a full functional pipeline.
     *  - This sample also shows how to use the IdleStateHandler fireing UserEvents, catched by TerminateIdleConnectionHandler
     *  
     *  Try the server via the MultipleProtocols.Client (only sends NumberObj) or via telnet
     */
    class Program
    {
        static async Task RunServerAsync()
        {
            var logLevel = LogLevel.DEBUG;
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));

            var serverPort = 8080;

            var bossGroup = new MultithreadEventLoopGroup(1); //  accepts an incoming connection
            var workerGroup = new MultithreadEventLoopGroup(); // handles the traffic of the accepted connection once the boss accepts the connection and registers the accepted connection to the worker

            var encoder = new StringEncoder();
            var decoder = new ProtocolSelectorDecoder(logRawMessages: true);

            try
            {
                var bootstrap = new ServerBootstrap();

                bootstrap
                    .Group(bossGroup, workerGroup)
                    .Channel<TcpServerSocketChannel>()
                    .Option(ChannelOption.SoBacklog, 100) // maximum queue length for incoming connection
                    .Handler(new LoggingHandler(logLevel))
                    .ChildHandler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new LineBasedFrameDecoder(80));
                        pipeline.AddLast(new IdleStateHandler(5, 5, 0));
                        pipeline.AddLast(encoder); // Encoder has to be before every handler in the pipeline that writes data e.g. TerminateIdleConnectionHandler. 
                        pipeline.AddLast(new TerminateIdleConnectionHandler());

                        pipeline.AddLast(decoder);
                    }));

                IChannel bootstrapChannel = await bootstrap.BindAsync(serverPort);

                Console.WriteLine("Let us test the server in a command prompt");
                Console.WriteLine($"\n telnet localhost {serverPort}");
                Console.ReadLine();

                await bootstrapChannel.CloseAsync();
            }
            finally
            {
                Task.WaitAll(bossGroup.ShutdownGracefullyAsync(), workerGroup.ShutdownGracefullyAsync());
            }
        }

        static void Main(string[] args) => RunServerAsync().Wait();
    }
}