using Google.Protobuf;

using Grpc.Core;

using System;
using System.Reflection;

namespace gRPC
{
    public class GrpcTools
    {
        /// <summary>
        /// 冠上此Attribute 需實作兩個public static的方法 預設名稱為Serializer/Deserializer
        /// </summary>
        [AttributeUsage(AttributeTargets.Class)]
        public class SerializerModelAttribute : Attribute
        {
            public string SerializerName = "Serializer";
            public string DeserializerName = "Deserializer";
        }
        private readonly static MethodInfo CreateIMessageMarshallersMethod =
           typeof(GrpcTools).GetMethod("CreateIMessageMarshallers", BindingFlags.Static | BindingFlags.NonPublic);

        public static Marshaller<T> CreateMarshallers<T>()
        {
            var type = typeof(T);
            if (type.GetInterface(nameof(IMessage)) == typeof(IMessage))
            {
                return CreateIMessageMarshallersMethod.MakeGenericMethod(type).Invoke(null, null) as Marshaller<T>;
            }
            else if (type.IsArray && type.GetElementType() == typeof(byte))
            {
                return Marshallers.Create(bin => bin, bin => bin) as Marshaller<T>;
            }
            else
            {
                var att = type.GetCustomAttribute<SerializerModelAttribute>();
                if (att != null)
                {
                    var serFunc = type.GetMethod(att.SerializerName, BindingFlags.Public | BindingFlags.Static);
                    var serDelegate = serFunc.CreateDelegate(typeof(Func<T, byte[]>)) as Func<T, byte[]>;
                    var deSerFunc = type.GetMethod(att.DeserializerName, BindingFlags.Public | BindingFlags.Static);
                    var deSerDelegate = deSerFunc.CreateDelegate(typeof(Func<byte[], T>)) as Func<byte[], T>;
                    return Marshallers.Create(serDelegate, deSerDelegate);
                }
            }
            throw new NotImplementedException();
        }


        private static Marshaller<T> CreateIMessageMarshallers<T>()
            where T : IMessage<T>, new()
        {
            var parser = new MessageParser<T>(() => new T());
            return Marshallers.Create(instance => MessageExtensions.ToByteArray(instance), parser.ParseFrom);
        }
    }
}
