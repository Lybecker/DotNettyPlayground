using DotNetty.Buffers;
using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using MultipleProtocols.Comon;
using System;
using System.Net;
using System.Threading.Tasks;

namespace MultipleProtocols.Client
{
    class Program
    {
        static async Task RunClientAsync()
        {
            InternalLoggerFactory.DefaultFactory.AddProvider(new ConsoleLoggerProvider((s, level) => true, false));

            var group = new MultithreadEventLoopGroup();

            var serverIP = IPAddress.Parse("127.0.0.1");
            int serverPort = 8080;

            try
            {
                var bootstrap = new Bootstrap();
                bootstrap
                    .Group(group)
                    .Channel<TcpSocketChannel>()
                    .Option(ChannelOption.TcpNodelay, true) // Do not buffer and send packages right away
                    .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                    {
                        IChannelPipeline pipeline = channel.Pipeline;

                        pipeline.AddLast(new BEncoder(logRawMessages: true), new StringDecoder(), new PrintReturnMessageClientHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(serverIP, serverPort));

                for (; ; )
                {
                    Console.WriteLine("Enter a number (int):");
                    string stringNumber = Console.ReadLine();
                    if (!Int32.TryParse(stringNumber, out int number))
                        continue;

                    await bootstrapChannel.WriteAndFlushAsync(new B() { Number = number });
                }

                //await bootstrapChannel.CloseAsync();
            }
            finally
            {
                group.ShutdownGracefullyAsync().Wait(1000);
            }
        }

        static void Main() => RunClientAsync().Wait();
    }
}