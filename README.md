# DotNetty Playground
This is a learning experince - Getting to know DotNetty with .Net Core.

# DotNetty
[DotNetty](https://github.com/Azure/DotNetty) is a port of [Netty](http://netty.io/), asynchronous event-driven network application framework for rapid development of maintainable high performance protocol servers & clients.

* [Docs](http://netty.io/wiki/user-guide.html) - The docs are for Netty (the Java version) and requires only small changes to apply to DotNetty
* [Terminology](https://github.com/Azure/azure-iot-protocol-gateway/blob/master/docs/DeveloperGuide.md#component-structure)

## Great tutorials & blogs
* http://netty.io/wiki/user-guide-for-4.x.html
* http://shengwangi.blogspot.com/2016/03/netty-tutorial-hello-world-example.html
* http://seeallhearall.blogspot.com/2012/06/netty-tutorial-part-15-on-channel.html
* [Netty in Action](https://www.manning.com/books/netty-in-action) book

## Is it production ready?
Yes, used by [Akka.Net](https://getakka.net/), [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/) and the [IoT Hub Edge Runtime](https://github.com/Azure/iotedge). [Microsoft has tested it with 100,000 connections](https://github.com/Azure/DotNetty/issues/135). Lots of [big companies are using Netty](https://github.com/netty/netty/wiki/Adopters).

# This repository - Coding Examples
Besides the [official DotNetty examples](https://github.com/Azure/DotNetty/tree/dev/examples) this repo contains a couple of enhanced examples exploring features not covered by the official repo.

## Hello World
- Encode and decode byte[] into strings via the build-in StringEncoder and StringDecoder
- Chaining ChannelHandlers into a ChannelPipeline
- HelloWorldServerHandler implements SimpleChannelInboundHandler, which encapsulated DotNetty specific stuff like reference counting 
- HasUpperCharsServerHandler implements raw ChannelHandlerAdapter and usages message forwarding to next Handler in the ChannelPipeline via FireChannelRead
- CountCharsServerHandler uses ChannelReadComplete to write to the output stream


## POCO (Plain Old C# Objects)
Shows how to transform data into C# objects and back

- Encode and decode byte[] into POCO (Plain Old C# Objects) via the custom PersonEncoder and PersonDecoder
- PersonServerHandler implements SimpleChannelInboundHandler and works with Person objects
- Using multiple output encoders - one for Person objects and one for strings

## Multiple Protocols
Using multiple protocols at the same endpoint/port

- Two object types (StringObj and NumberObj) each decoded differently
    - The first char is used as the protocol selector
        - A = StringObj
        - B = NumberObj
- There are multiple ways of handling multiple protocols
    - The initial message sets the protocol - see InitialProtocolSelectorDecoder
    - Every message forward the protocol selector char - See ProtocolSelectorDecoder
- The channel pipeline is modified dynamically via one of the to ProtocolSelectorDecoders, so the pipeline in Program.cs does not have a full functional pipeline.
- This sample also shows how to use the IdleStateHandler fireing UserEvents, catched by TerminateIdleConnectionHandler
