using Grpc.Core;

namespace gRPC
{
    public class GrpcServer
    {
        public readonly string ServiceName;
        private ServerServiceDefinition.Builder Builder;
        public GrpcServer(string serviceName)
        {
            ServiceName = serviceName;
            Builder = ServerServiceDefinition.CreateBuilder();
        }

        public void AddMethod<TRQ,TRP>(string apiName, UnaryServerMethod<TRQ,TRP> del)
            where TRQ : class
            where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.Unary, ServiceName, apiName, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            Builder.AddMethod(method, del);
        }

        public void AddMethod<TRQ, TRP>(string apiName, ClientStreamingServerMethod<TRQ, TRP> del)
    where TRQ : class
    where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.ClientStreaming, ServiceName, apiName, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            Builder.AddMethod(method, del);
        }

        public void AddMethod<TRQ, TRP>(string apiName, ServerStreamingServerMethod<TRQ, TRP> del)
where TRQ : class
where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.ServerStreaming, ServiceName, apiName, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            Builder.AddMethod(method, del);
        }
        public void AddMethod<TRQ, TRP>(string apiName, DuplexStreamingServerMethod<TRQ, TRP> del)
where TRQ : class
where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.DuplexStreaming, ServiceName, apiName, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            Builder.AddMethod(method, del);
        }
        public ServerServiceDefinition CreateServer()
        {
            return Builder.Build();
        }
    }
}
