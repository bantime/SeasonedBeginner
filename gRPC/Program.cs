using Grpc.Core;

using GrpcGreeter;

using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


    [GrpcTools.SerializerModel]
    public class RQ
    {

        public string AA;

        public static byte[] Serializer(RQ rQ)
        {
            return Encoding.ASCII.GetBytes(rQ.AA);
        }
        public static RQ Deserializer(byte[] bin)
        {
            return new RQ()
            {
                AA = Encoding.ASCII.GetString(bin)
            };
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
                var client = new GrpcClient("127.0.0.1", 1234, "TestService");
                var r = client.UnarySync<RQ, RQ>("Test3", new RQ() { AA = "Bantime" });
                /*
                var channel = new Channel("127.0.0.1", 1234, ChannelCredentials.Insecure);
                var callInvoker = channel.CreateCallInvoker();

                var response = callInvoker.BlockingUnaryCall(method, null, default, new byte[] { 1, 2, 3, 4 });

                var client = new Greeter.GreeterClient(channel);
                var response = client.SayHello(new HelloRequest() { Name = "Bantime" });
                Console.WriteLine($"Client Receive {response.Message}");*/
            });
            var grpcServer = new GrpcServer("TestService");
            grpcServer.AddMethod<byte[], byte[]>("Test1", (rq, context) =>
            {
                return Task.FromResult(rq.Reverse().ToArray());
            });
            grpcServer.AddMethod<HelloRequest, HelloReply>("Test2", (rq, context) =>
            {
                return Task.FromResult(new HelloReply()
                {
                    Message = $"Hello {rq.Name}"
                });
            });
            grpcServer.AddMethod<RQ, RQ>("Test3", (rq, context) =>
            {
                return Task.FromResult(new RQ() 
                { 
                    AA = $"Hello {rq.AA}"
                });
            });
            /*
            var service = ServerServiceDefinition.CreateBuilder()
          .AddMethod(method, (bin, conteext) =>
          {
              Console.WriteLine(string.Join("-", bin));
              return Task.FromResult(bin);
          }).Build();*/
            var server = new Server
            {
                Services = { grpcServer.CreateServer() },
                Ports = { new ServerPort("localhost", 1234, ServerCredentials.Insecure) }
            };
            server.Start();
            while (true)
                await Task.Delay(1000);
        }
    }
}
