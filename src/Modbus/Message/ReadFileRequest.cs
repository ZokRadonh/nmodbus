using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadFileRequest : IModbusRequest
    {
        private readonly ModbusMessageImpl _messageImpl;

        /// <summary>
        /// 
        /// </summary>
        public byte ByteCount { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public List<ReadFileSubRequest> SubRequest { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public ReadFileRequest()
        {
            _messageImpl = new ModbusMessageImpl();
            SubRequest = new List<ReadFileSubRequest>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slaveAddress"></param>
        /// <param name="functionCode"></param>
        public ReadFileRequest(byte slaveAddress, byte functionCode)
        {
            _messageImpl = new ModbusMessageImpl(slaveAddress, functionCode);
            SubRequest = new List<ReadFileSubRequest>();
        }

        /// <summary>
        /// 
        /// </summary>
        public byte FunctionCode
        {
            get { return _messageImpl.FunctionCode; }
            set { _messageImpl.FunctionCode = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte SlaveAddress
        {
            get { return _messageImpl.SlaveAddress; }
            set { _messageImpl.SlaveAddress = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] MessageFrame
        {
            get { return _messageImpl.MessageFrame; }
        }

        /// <summary>
        /// 
        /// </summary>
        public byte[] ProtocolDataUnit
        {
            get { return _messageImpl.ProtocolDataUnit; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ushort TransactionId
        {
            get { return _messageImpl.TransactionId; }
            set { _messageImpl.TransactionId = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
        public void Initialize(byte[] frame)
        {
            _messageImpl.Initialize(frame);

            ByteCount = frame[2];

            var subRequestCount = (frame.Length - 3) / 7;
            for (var i = 0; i < subRequestCount; i++)
            {
                SubRequest.Add(new ReadFileSubRequest
                {
                    ReferenceType = frame[3 + 7 * i],
                    FuleNumber = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 4 + 7 * i)),
                    RecordNumber = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 6 + 7 * i)),
                    RecordLength = (ushort)IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 8 + 7 * i))
                });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="IOException"></exception>
        public void ValidateResponse(IModbusMessage response)
        {
            var typedResponse = (ReadFileRequest)response;

            if (ByteCount > 0xF5 || ByteCount < 0x07)
            {
                throw new IOException(String.Format(
                    "Unexpected byte count. Received {0}.", typedResponse.ByteCount));
            }

            // best effort validation - the same response for a request for 1 vs 6 coils (same byte count) will pass validation.
            //            var expectedByteCount = (NumberOfPoints + 7) / 8;
            //            if (expectedByteCount != typedResponse.ByteCount)
            //            {
            //                throw new IOException(String.Format(CultureInfo.InvariantCulture,
            //                    "Unexpected byte count. Expected {0}, received {1}.",
            //                    expectedByteCount,
            //                    typedResponse.ByteCount));
            //            }
        }
    }
}
