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
    class Program
    {
        static async Task RunServerAsync()
        {
            var logLevel = LogLevel.INFO;
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));

            var serverPort = 8080;

            var bossGroup = new MultithreadEventLoopGroup(1); //  accepts an incoming connection
            var workerGroup = new MultithreadEventLoopGroup(); // handles the traffic of the accepted connection once the boss accepts the connection and registers the accepted connection to the worker

            var encoder = new StringEncoder();
            var decoder = new StringDecoder();
            var serverHandler = new EchoConnectionHandler();

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

                        pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter())); // Do not allow requests longer than n chars
                        pipeline.AddLast(new IdleStateHandler(5, 5, 0));
                        pipeline.AddLast(encoder); // Encoder has to be before every handler in the pipeline that writes data e.g. TerminateIdleConnectionHandler. 
                        pipeline.AddLast(new TerminateIdleConnectionHandler());

                        // It is possible to use multiple encoders, but the client cannot parse the StringEncoder. Telnet to the server works.
                        // Without the below StringEncoder, the PersonServerHandler.ChannelActive output will never be transmitted over the wire.
                        // If the server needs to decode into multiple POCOs then take a look at the Port Unification sample: https://github.com/netty/netty/blob/4.0/example/src/main/java/io/netty/example/portunification/PortUnificationServerHandler.java
                        //pipeline.AddLast(new StringEncoder()); // Multiple encoders allowed. PersonEncoder encodes Person objects to IByteBuffer

                        pipeline.AddLast(decoder, serverHandler);
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
