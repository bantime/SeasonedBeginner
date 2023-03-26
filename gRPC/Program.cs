using Google.Protobuf.WellKnownTypes;

using Grpc.Core;

using GrpcGreeter;

using System;
using System.Net.WebSockets;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace gRPC
{
    public class GreeterService : Greeter.GreeterBase
    {
        public override Task<HelloReply> SayHello(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine($"Server Receive {request.Name}");
            return Task.FromResult(new HelloReply()
            {
                Message = $"Hello {request.Name}"
            });
        }
    }
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var ser = Marshallers.Create(bin => bin, bin => bin);
            var method = new Method<byte[], byte[]>(MethodType.Unary, "ServiceName", "APIName", ser, ser);
            _ = Task.Run(async () =>
            {
                await Task.Delay(1000);//等服務建立好
                var channel = new Channel("127.0.0.1:1234", ChannelCredentials.Insecure);
                var callInvoker = channel.CreateCallInvoker();

                var response = callInvoker.BlockingUnaryCall(method, null, default, new byte[] { 1,2,3,4});
                /*
                var client = new Greeter.GreeterClient(channel);
                var response = client.SayHello(new HelloRequest() { Name = "Bantime" });
                Console.WriteLine($"Client Receive {response.Message}");*/
            });
            var service = ServerServiceDefinition.CreateBuilder()
          .AddMethod(method, (bin,conteext) =>
          {
              Console.WriteLine(string.Join("-", bin));
              return Task.FromResult(bin);
          }).Build();
            var server = new Server
            {
                Services = { service },
                Ports = { new ServerPort("localhost", 1234, ServerCredentials.Insecure) }
            };
            server.Start();
            while (true)
                await Task.Delay(1000);
        }
    }
}
