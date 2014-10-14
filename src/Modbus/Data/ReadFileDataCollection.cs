using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Modbus.Data
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadFileDataCollection : Collection<FileRecord>, IModbusMessageDataCollection
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bytes"></param>
        public ReadFileDataCollection(byte[] bytes)
        {
            ByteCount = (byte) bytes.Length;

            var recordsCount = ByteCount / 7;

            for (var i = 0; i < recordsCount; i++)
            {
                if (bytes[7 * i] != 0x06) continue;

                Add(new FileRecord(
                    (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 1 + 7 * i)),
                    (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 3 + 7 * i)),
                    (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(bytes, 5 + 7 * i))));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] NetworkBytes
        {
            get
            {
                var bytes = new List<byte>();

                foreach (var record in this)
                {
                    bytes.Add(0x06);
                    bytes.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)record.FileNumber)));
                    bytes.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)record.RecordNumber)));
                    bytes.AddRange(BitConverter.GetBytes((ushort)IPAddress.HostToNetworkOrder((short)record.RecordLength)));
                }

                return bytes.ToArray();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte ByteCount { get; private set; }
    }
}
