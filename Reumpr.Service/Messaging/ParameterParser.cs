using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace tetryds.Reumpr.Service
{
    public class ParameterParser
    {
        readonly Func<byte[], Type, object> objectParser;
        readonly Func<object, byte[]> objectSerializer;

        ConcurrentDictionary<Type, Func<byte[], object>> parserMap;
        ConcurrentDictionary<Type, Func<object, byte[]>> serializerMap;

        public ParameterParser(Func<byte[], Type, object> objectParser, Func<object, byte[]> objectSerializer)
        {
            this.objectParser = objectParser;
            this.objectSerializer = objectSerializer;

            parserMap = new ConcurrentDictionary<Type, Func<byte[], object>>
            {
                [typeof(byte[])] = obj => (byte[])obj.Clone(),
                [typeof(bool)] = data => BitConverter.ToBoolean(data, 0),
                [typeof(char)] = data => BitConverter.ToChar(data, 0),
                [typeof(double)] = data => BitConverter.ToDouble(data, 0),
                [typeof(short)] = data => BitConverter.ToInt16(data, 0),
                [typeof(int)] = data => BitConverter.ToInt32(data, 0),
                [typeof(long)] = data => BitConverter.ToInt64(data, 0),
                [typeof(float)] = data => BitConverter.ToSingle(data, 0),
                [typeof(ushort)] = data => BitConverter.ToUInt16(data, 0),
                [typeof(uint)] = data => BitConverter.ToUInt32(data, 0),
                [typeof(ulong)] = data => BitConverter.ToUInt64(data, 0),
                [typeof(string)] = data => Encoding.UTF8.GetString(data),
            };

            serializerMap = new ConcurrentDictionary<Type, Func<object, byte[]>>
            {
                [typeof(void)] = obj => new byte[0],
                [typeof(byte[])] = obj => (byte[])((byte[])obj).Clone(),
                [typeof(bool)] = obj => BitConverter.GetBytes((bool)obj),
                [typeof(char)] = obj => BitConverter.GetBytes((char)obj),
                [typeof(double)] = obj => BitConverter.GetBytes((double)obj),
                [typeof(short)] = obj => BitConverter.GetBytes((short)obj),
                [typeof(int)] = obj => BitConverter.GetBytes((int)obj),
                [typeof(long)] = obj => BitConverter.GetBytes((long)obj),
                [typeof(float)] = obj => BitConverter.GetBytes((float)obj),
                [typeof(ushort)] = obj => BitConverter.GetBytes((ushort)obj),
                [typeof(uint)] = obj => BitConverter.GetBytes((uint)obj),
                [typeof(ulong)] = obj => BitConverter.GetBytes((ulong)obj),
                [typeof(string)] = obj => Encoding.UTF8.GetBytes((string)obj),
                [typeof(Exception)] = obj => Encoding.UTF8.GetBytes(obj.ToString()),
            };
        }

        public object Parse(byte[] data, Type type)
        {
            if (parserMap.TryGetValue(type, out Func<byte[], object> parser))
                return parser(data);

            if (objectParser == null)
                throw new SerializeException($"Cannot parse object, no custom parser for type {type.GetType()}");
            return objectParser(data, type);
        }

        public byte[] Serialize(object obj)
        {
            if (obj == null) return new byte[0];
            if (serializerMap.TryGetValue(obj.GetType(), out Func<object, byte[]> serializer))
                return serializer(obj);

            if (objectSerializer == null)
                throw new SerializeException($"Cannot serialize object, no custom serializer for type {obj.GetType()}");
            return objectSerializer(obj);
        }
    }
}
