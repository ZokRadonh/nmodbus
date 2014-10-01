using System;
using System.Collections.Generic;
using System.Globalization;

namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    public class SlaveExceptionResponse : ModbusMessage, IModbusMessage
	{
		private static readonly Dictionary<byte, string> _exceptionMessages = CreateExceptionMessages();		

		/// <summary>
		/// 
		/// </summary>
		public SlaveExceptionResponse()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="slaveAddress"></param>
		/// <param name="functionCode"></param>
		/// <param name="exceptionCode"></param>
		public SlaveExceptionResponse(byte slaveAddress, byte functionCode, byte exceptionCode)
			: base(slaveAddress, functionCode)	
		{
			SlaveExceptionCode = exceptionCode;
		}

        /// <summary>
        /// 
        /// </summary>
		public override int MinimumFrameSize
		{
			get { return 3; }
		}

		/// <summary>
		/// 
		/// </summary>
		public byte SlaveExceptionCode
		{
			get { return MessageImpl.ExceptionCode.Value; }
			set { MessageImpl.ExceptionCode = value; }
		}

		/// <summary>
		/// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </summary>
		/// <returns>
		/// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
		/// </returns>
		public override string ToString()
		{
		    var message = _exceptionMessages.ContainsKey(SlaveExceptionCode)
		        ? _exceptionMessages[SlaveExceptionCode]
		        : Resources.Unknown;

		    return String.Format(
		        CultureInfo.InvariantCulture, Resources.SlaveExceptionResponseFormat,
		        Environment.NewLine, FunctionCode, SlaveExceptionCode, message);
		}

		internal static Dictionary<byte, string> CreateExceptionMessages()
		{
			var messages = new Dictionary<byte, string>(9)
			{
			    {1, Resources.IllegalFunction},
			    {2, Resources.IllegalDataAddress},
			    {3, Resources.IllegalDataValue},
			    {4, Resources.SlaveDeviceFailure},
			    {5, Resources.Acknowlege},
			    {6, Resources.SlaveDeviceBusy},
			    {8, Resources.MemoryParityError},
			    {10, Resources.GatewayPathUnavailable},
			    {11, Resources.GatewayTargetDeviceFailedToRespond}
			};

		    return messages;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="frame"></param>
		protected override void InitializeUnique(byte[] frame)
        {
            if (FunctionCode <= Modbus.ExceptionOffset)
                throw new FormatException(Resources.SlaveExceptionResponseInvalidFunctionCode);

			SlaveExceptionCode = frame[2];
		}
	}
}
