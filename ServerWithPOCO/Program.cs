using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using System;
using System.Threading.Tasks;

namespace ServerWithPOCO
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

            var encoder = new PersonEncoder();
            var decoder = new PersonDecoder();
            var serverHandler = new PersonServerHandler();

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

                        pipeline.AddLast(new StringEncoder()); // Multiple encoders allowed. PersonEncoder encodes Person objects to IByteBuffer
                        pipeline.AddLast(encoder, decoder, serverHandler);
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