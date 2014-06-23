using System;
using Modbus.Device;
using NUnit.Framework;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusUsbAsciiMasterFixture
	{
		[Test, ExpectedException(typeof(TimeoutException))]
		public void NModbusUsbAsciiMaster_ReadTimeout()
		{
			var port = ModbusMasterFixture.CreateAndOpenUsbPort(ModbusMasterFixture.DefaultMasterUsbPortId);
			using (var master = ModbusSerialMaster.CreateAscii(port))
			{
				master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;
				master.ReadCoils(100, 1, 1);
			}
		}
	}
}
