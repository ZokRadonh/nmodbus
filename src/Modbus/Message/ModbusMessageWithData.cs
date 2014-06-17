using Modbus.Data;

namespace Modbus.Message
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class ModbusMessageWithData<TData> : ModbusMessage where TData : IModbusMessageDataCollection
	{
		/// <summary>
		/// 
		/// </summary>
		public ModbusMessageWithData()
		{
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="slaveAddress"></param>
		/// <param name="functionCode"></param>
		public ModbusMessageWithData(byte slaveAddress, byte functionCode)
			: base(slaveAddress, functionCode)
		{
		}

		/// <summary>
		/// 
		/// </summary>
		public TData Data
		{
			get { return (TData) MessageImpl.Data; }
			set { MessageImpl.Data = value; }
		}
	}
}
