using DotNetty.Codecs;
using DotNetty.Common.Internal.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Microsoft.Extensions.Logging.Console;
using Poco.Comon;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Poco.Client
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
                        
                        pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter())); // Do not allow requests longer than n chars
                        pipeline.AddLast(new PersonEncoder(), new PersonDecoder(), new PersonClientHandler());
                    }));

                IChannel bootstrapChannel = await bootstrap.ConnectAsync(new IPEndPoint(serverIP, serverPort));

                for (; ; )
                {
                    Console.WriteLine("Enter your name:");
                    string name = Console.ReadLine();
                    if (string.IsNullOrEmpty(name))
                        continue;

                    Console.WriteLine("Enter your age:");
                    string ageString = Console.ReadLine();
                    int age;
                    if (!Int32.TryParse(ageString, out age))
                        continue;

                    await bootstrapChannel.WriteAndFlushAsync(new Person() { Name = name, Age = age });
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