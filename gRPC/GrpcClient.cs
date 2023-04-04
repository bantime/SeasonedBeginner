using Grpc.Core;

namespace gRPC
{
    public class GrpcClient
    {
        private readonly CallInvoker CallInvoker;
        public readonly string ServiceName;
        public GrpcClient(string host, int port, string serviceName)
        {
            var channel = new Channel(host, port, ChannelCredentials.Insecure);
            CallInvoker = channel.CreateCallInvoker();
            ServiceName = serviceName;
        }

        public TRP UnarySync<TRQ, TRP>(string apiname, TRQ rQ) 
            where TRQ : class
            where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.Unary, ServiceName, apiname, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            return CallInvoker.BlockingUnaryCall(method, null, default, rQ);
        }

        public AsyncUnaryCall<TRP> UnaryASync<TRQ, TRP>(string apiname, TRQ rQ)
    where TRQ : class
    where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.Unary, ServiceName, apiname, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            return CallInvoker.AsyncUnaryCall(method, null, default, rQ);
        }
        public AsyncClientStreamingCall<TRQ, TRP> AsyncClientStreamingCall<TRQ, TRP>(string apiname)
where TRQ : class
where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.ClientStreaming, ServiceName, apiname, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            return CallInvoker.AsyncClientStreamingCall(method, null, default);
        }
        public AsyncServerStreamingCall<TRP> AsyncServerStreamingCall<TRQ, TRP>(string apiname,TRQ rQ)
where TRQ : class
where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.ServerStreaming, ServiceName, apiname, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            return CallInvoker.AsyncServerStreamingCall(method, null, default, rQ);
        }
        public AsyncDuplexStreamingCall<TRQ, TRP> AsyncDuplexStreamingCall<TRQ, TRP>(string apiname)
where TRQ : class
where TRP : class
        {
            var method = new Method<TRQ, TRP>(MethodType.DuplexStreaming, ServiceName, apiname, GrpcTools.CreateMarshallers<TRQ>(), GrpcTools.CreateMarshallers<TRP>());
            return CallInvoker.AsyncDuplexStreamingCall(method, null, default);
        }
    }
}
