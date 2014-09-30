using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Net;
using Modbus.Data;
using Modbus.Unme.Common;

namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class DiagnosticsRequestResponse : ModbusMessageWithData<RegisterCollection>, IModbusMessage
	{		
		/// <summary>
		/// 
		/// </summary>
		public DiagnosticsRequestResponse()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="subFunctionCode"></param>
		/// <param name="slaveAddress"></param>
		/// <param name="data"></param>
		public DiagnosticsRequestResponse(ushort subFunctionCode, byte slaveAddress, RegisterCollection data)
			: base(slaveAddress, Modbus.Diagnostics)
		{
			SubFunctionCode = subFunctionCode;
			Data = data;
		}

        /// <summary>
        /// 
        /// </summary>
		public override int MinimumFrameSize
		{
			get { return 6; }
		}

		/// <summary>
		/// 
		/// </summary>
		[SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "May implement addtional sub function codes in the future.")]
		public ushort SubFunctionCode
		{
			get { return MessageImpl.SubFunctionCode.Value; }
			set { MessageImpl.SubFunctionCode = value; }
		}

        /// <summary>
        /// 
        /// </summary>
		public override string ToString()
		{
			Debug.Assert(SubFunctionCode == Modbus.DiagnosticsReturnQueryData, "Need to add support for additional sub-function.");

			return String.Format(CultureInfo.InvariantCulture, "Diagnostics message, sub-function return query data - {0}.", Data);
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
		protected override void InitializeUnique(byte[] frame)
		{
			SubFunctionCode = (ushort) IPAddress.NetworkToHostOrder(BitConverter.ToInt16(frame, 2));
			Data = new RegisterCollection(frame.Slice(4, 2).ToArray());
		}
	}
}
