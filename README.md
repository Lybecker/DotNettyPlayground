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
Yes, used by [Akka.Net](https://getakka.net/), [Azure IoT Hub](https://azure.microsoft.com/en-us/services/iot-hub/) and the [IoT Hub Edge Runtime](https://github.com/Azure/iotedge). [Microsoft has tested it with 100,000 connections](https://github.com/Azure/DotNetty/issues/135).