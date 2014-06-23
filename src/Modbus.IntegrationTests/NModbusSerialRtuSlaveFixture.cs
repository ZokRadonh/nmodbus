using NUnit.Framework;
using Modbus.Device;
using Modbus.Data;
using System.Threading;

namespace Modbus.IntegrationTests
{
	[TestFixture]
	public class NModbusSerialRtuSlaveFixture
	{
		[TestFixtureSetUp]
		public void SetUp()
		{
			
		}

		[Test]
		public void NModbusSerialRtuSlave_BonusCharacter_VerifyTimeout()
		{
			var masterPort = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultMasterSerialPortName);
			var slavePort = ModbusMasterFixture.CreateAndOpenSerialPort(ModbusMasterFixture.DefaultSlaveSerialPortName);

			using (var master = ModbusSerialMaster.CreateRtu(masterPort))
			using (var slave = ModbusSerialSlave.CreateRtu(1, slavePort))
			{
				master.Transport.ReadTimeout = master.Transport.WriteTimeout = 1000;				
				slave.DataStore = DataStoreFactory.CreateTestDataStore();

                var slaveThread = new Thread(slave.Listen) {IsBackground = true};
			    slaveThread.Start();

				// assert successful communication
				Assert.AreEqual(new[] { false, true }, master.ReadCoils(1, 1, 2));

				// write "bonus" character
				masterPort.Write("*");

				// assert successful communication
				Assert.AreEqual(new[] { false, true }, master.ReadCoils(1, 1, 2));
			}
		}
	}
}
