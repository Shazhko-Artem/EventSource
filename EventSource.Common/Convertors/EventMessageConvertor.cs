using System;
using System.Net.Mime;
using System.Text;
using EventSource.Common.Models;
using EventSource.Common.Models.Messages;
using Newtonsoft.Json;

namespace EventSource.Common.Convertors
{
    public class EventMessageConvertor
    {
        public static EventMessage GetEventMessageFor<TObject>(string eventName, TObject obj)
        {
            var dataInBytes = Serialize(obj);
            return new BytesEventMessage(eventName, dataInBytes, MediaTypeNames.Application.Json);
        }

        public static byte[] GetBytesForSocket<TObject>(TObject obj)
        {
            var dataInBytes = Serialize(obj);

            var lengthInBytes = BitConverter.GetBytes(dataInBytes.Length);
            var dataInBytesWithSize = new byte[dataInBytes.Length + lengthInBytes.Length];
            lengthInBytes.CopyTo(dataInBytesWithSize, 0);
            dataInBytes.CopyTo(dataInBytesWithSize, lengthInBytes.Length);

            return dataInBytesWithSize;
        }

        public static TObject GetObjectFromMessage<TObject>(EventMessage message)
        {
            if (message.ContentType != MediaTypeNames.Application.Json)
            {
                throw new ArgumentException($"Could not convert message data. Expected content type '{MediaTypeNames.Application.Json}' but received '{message.ContentType}'.");
            }

            return Deserialize<TObject>(message.Body);
        }

        public static string GetMessageDataAsString(EventMessage message)
        {
            return GetEncoding().GetString(message.Body);
        }

        public static byte[] Serialize<TObject>(TObject obj)
        {
            return GetEncoding().GetBytes(JsonConvert.SerializeObject(obj));
        }

        public static TObject Deserialize<TObject>(byte[] data)
        {
            var json = GetEncoding().GetString(data);
            return JsonConvert.DeserializeObject<TObject>(json);
        }

        public static Encoding GetEncoding() => Encoding.UTF8;
    }
}