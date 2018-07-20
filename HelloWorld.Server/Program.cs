using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;

namespace HelloWorldServer
{
    /*
     * This solution shows how to build a TCP server
     *  - Encode and decode byte[] into strings via the build-in StringEncoder and StringDecoder
     *  - Chaining ChannelHandlers into a ChannelPipeline
     *  - HelloWorldServerHandler implements SimpleChannelInboundHandler, which encapsulated DotNetty specific stuff like reference counting 
     *  - HasUpperCharsServerHandler implements raw ChannelHandlerAdapter and usages message forwarding to next Handler in the ChannelPipeline via FireChannelRead
     *  - CountCharsServerHandler uses ChannelReadComplete to write to the output stream
     */
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
            var helloWorldServerHandler = new HelloWorldServerHandler();

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

                        // handler evaluation order is 1, 2, 3, 4, 5 for inbound data and 5, 4, 3, 2, 1 for outbound

                        pipeline.AddLast("1", new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter())); // Do not allow requests longer than n chars
                        pipeline.AddLast("2", encoder);
                        pipeline.AddLast("3", decoder);
                        pipeline.AddLast("4", new CountCharsServerHandler());
                        //pipeline.AddLast("4½", new HasUpperCharsServerHandler());
                        pipeline.AddLast("5", helloWorldServerHandler);
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