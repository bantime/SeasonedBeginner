using Google.Protobuf;

using Grpc.Core;

using GrpcGreeter;

using System;
using System.ComponentModel;
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

    public class GrpcClient
    {
        public readonly string ServiceName;
        private readonly CallInvoker CallInvoker;
        public GrpcClient(string ip, int port, string serviceName)
        {
            var channel = new Channel(ip, port, ChannelCredentials.Insecure);
            CallInvoker = channel.CreateCallInvoker();
        }

        public TResponse CallUnary<TRequest, TResponse>(TRequest request, string apiName)
            where TRequest : class
            where TResponse : class
        {
            var requestMarshaller = CreateMarshaller<TRequest>();
            var responseMarshaller = CreateMarshaller<TResponse>();
            var method = new Method<TRequest, TResponse>(MethodType.Unary, ServiceName, apiName, requestMarshaller, responseMarshaller);
            return CallInvoker.BlockingUnaryCall(method, null, default, request);
        }

        private Marshaller<T> CreateMarshaller<T>()
        {
            if(typeof(T) is IMessage)
            {
               return CreateIMessageMarshaller<T>();
            }
        }

        private Marshaller<T> CreateIMessageMarshaller<T>()
            where T : IMessage<T>, new()
        {
            var parse = new MessageParser<T>(() => new T());
            return Marshallers.Create<T>(instance =>
            {
                return instance.ToByteArray();
            },parse.ParseFrom);
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
                var channel = new Channel("127.0.0.1", 1234, ChannelCredentials.Insecure);
                var callInvoker = channel.CreateCallInvoker();

                var response = callInvoker.BlockingUnaryCall(method, null, default, new byte[] { 1, 2, 3, 4 });
                
                var client = new Greeter.GreeterClient(channel);
                var response = client.SayHello(new HelloRequest() { Name = "Bantime" });
                Console.WriteLine($"Client Receive {response.Message}");
            });
            var service = ServerServiceDefinition.CreateBuilder()
          .AddMethod(method, (bin, conteext) =>
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
