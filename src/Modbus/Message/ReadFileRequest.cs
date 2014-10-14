using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Modbus.Data;
using Modbus.Unme.Common;

namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class ReadFileRequest : ModbusMessage, IModbusRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public byte? ByteCount
        {
            get { return MessageImpl.ByteCount; }
            private set { MessageImpl.ByteCount = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ReadFileDataCollection Data
        {
            get { return (ReadFileDataCollection) MessageImpl.Data; }
        }

        /// <summary>
        /// 
        /// </summary>
        public override int MinimumFrameSize
        {
            get { return 0x08; }
        }

        /// <summary>
        /// 
        /// </summary>
        protected override void InitializeUnique(byte[] frame)
        {
            ByteCount = frame[2];
            MessageImpl.Data = new ReadFileDataCollection(frame.Slice(3, frame.Length - 3).ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"></param>
        /// <exception cref="IOException"></exception>
        public void ValidateResponse(IModbusMessage response)
        {
            // var typedResponse = (ReadFileRequest)response;
        }

        /// <summary>
        /// 
        /// </summary>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Read file");
        }
    }
}
