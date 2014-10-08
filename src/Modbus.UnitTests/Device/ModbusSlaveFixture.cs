using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using NUnit.Framework;
using Modbus.Data;
using Modbus.Device;
using Modbus.IO;
using Modbus.Message;
using Modbus.UnitTests.Message;
using Rhino.Mocks;
using Modbus.Unme.Common;

namespace Modbus.UnitTests.Device
{
	[TestFixture]
	public class ModbusSlaveFixture : ModbusMessageFixture
	{
		private DataStore _testDataStore;

		[SetUp]
		public void SetUp()
		{
			_testDataStore = DataStoreFactory.CreateTestDataStore();
		}

		[Test]
		public void ReadDiscretesCoils()
		{
			var expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 2, new DiscreteCollection(false, true, false, true, false, true, false, true, false));
            var response = ModbusSlave.ReadDiscretes(new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 1, 9), _testDataStore, _testDataStore.CoilDiscretes);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.ByteCount, response.ByteCount);
		}

		[Test]
		public void ReadDiscretesInputs()
		{
            var expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadInputs, 1, 2, new DiscreteCollection(true, false, true, false, true, false, true, false, true));
            var response = ModbusSlave.ReadDiscretes(new ReadCoilsInputsRequest(Modbus.ReadInputs, 1, 1, 9), _testDataStore, _testDataStore.InputDiscretes);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.ByteCount, response.ByteCount);
		}

		[Test]
		public void ReadRegistersHoldingRegisters()
		{
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 1, new RegisterCollection(1, 2, 3, 4, 5, 6));
            var response = ModbusSlave.ReadRegisters(new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 1, 0, 6), _testDataStore, _testDataStore.HoldingRegisters);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.ByteCount, response.ByteCount);
		}

		[Test]
		public void ReadRegistersInputRegisters()
		{
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadInputRegisters, 1, new RegisterCollection(10, 20, 30, 40, 50, 60));
            var response = ModbusSlave.ReadRegisters(new ReadHoldingInputRegistersRequest(Modbus.ReadInputRegisters, 1, 0, 6), _testDataStore, _testDataStore.InputRegisters);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.ByteCount, response.ByteCount);
		}

		[Test]
		public void WriteSingleCoil()
		{			
			const ushort addressToWrite = 35;
			var valueToWrite = !_testDataStore.CoilDiscretes[addressToWrite + 1];
            var expectedResponse = new WriteSingleCoilRequestResponse(1, addressToWrite, valueToWrite);
            var response = ModbusSlave.WriteSingleCoil(new WriteSingleCoilRequestResponse(1, addressToWrite, valueToWrite), _testDataStore, _testDataStore.CoilDiscretes);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(valueToWrite, _testDataStore.CoilDiscretes[addressToWrite + 1]);
		}

		[Test]
		public void WriteMultipleCoils()
		{
			const ushort startAddress = 35;
			const ushort numberOfPoints = 10;
            var val = !_testDataStore.CoilDiscretes[startAddress + 1];
            var expectedResponse = new WriteMultipleCoilsResponse(1, startAddress, numberOfPoints);
            var response = ModbusSlave.WriteMultipleCoils(new WriteMultipleCoilsRequest(1, startAddress, new DiscreteCollection(val, val, val, val, val, val, val, val, val, val)), _testDataStore, _testDataStore.CoilDiscretes);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(new [] { val, val, val, val, val, val, val, val, val, val }, _testDataStore.CoilDiscretes.Slice(startAddress + 1, numberOfPoints).ToArray());
		}

		[Test]
		public void WriteSingleRegister()
		{
			const ushort startAddress = 35;
			const ushort value = 45;
			Assert.AreNotEqual(value, _testDataStore.HoldingRegisters[startAddress - 1]);
            var expectedResponse = new WriteSingleRegisterRequestResponse(1, startAddress, value);
            var response = ModbusSlave.WriteSingleRegister(new WriteSingleRegisterRequestResponse(1, startAddress, value), _testDataStore, _testDataStore.HoldingRegisters);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
		}

		[Test]
		public void WriteMultipleRegisters()
		{
			const ushort startAddress = 35;
            var valuesToWrite = new ushort[] { 1, 2, 3, 4, 5 };
			Assert.AreNotEqual(valuesToWrite, _testDataStore.HoldingRegisters.Slice(startAddress - 1, valuesToWrite.Length).ToArray());
            var expectedResponse = new WriteMultipleRegistersResponse(1, startAddress, (ushort)valuesToWrite.Length);
            var response = ModbusSlave.WriteMultipleRegisters(new WriteMultipleRegistersRequest(1, startAddress, new RegisterCollection(valuesToWrite)), _testDataStore, _testDataStore.HoldingRegisters);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
		}

		[Test]
		public void ApplyRequest_VerifyModbusRequestReceivedEventIsFired()
		{
            var eventFired = false;
			ModbusSlave slave = ModbusSerialSlave.CreateAscii(1, new SerialPort());
            var request = new WriteSingleRegisterRequestResponse(1, 1, 1);
			slave.ModbusSlaveRequestReceived += (obj, args) => { eventFired = true; Assert.AreEqual(request, args.Message); };
			
			slave.ApplyRequest(request);			
			Assert.IsTrue(eventFired);
		}

		[Test]
		public void WriteMultipCoils_MakeSureWeDoNotWriteRemainder()
		{
			// 0, false initialized data store
			var dataStore = DataStoreFactory.CreateDefaultDataStore();

			var request = new WriteMultipleCoilsRequest(1, 0, new DiscreteCollection(Enumerable.Repeat(true, 8).ToArray())) { NumberOfPoints = 2 };
			ModbusSlave.WriteMultipleCoils(request, dataStore, dataStore.CoilDiscretes);

			Assert.AreEqual(dataStore.CoilDiscretes.Slice(1, 8).ToArray(), new [] { true, true, false, false, false, false, false, false });
		}

		[Test]
		public void RegisterCustomFunction_NullDelegate()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());
			
			Assert.Throws<ArgumentNullException>(() => slave.RegisterCustomFunction<TestMessage>(100, null));
		}

		[Test]
		public void RegisterCustomFunction_FunctionAlreadyExists()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			slave.RegisterCustomFunction<TestMessage>(100, (request, dataStore) => { throw new NotImplementedException(); });
			Assert.Throws<ArgumentException>(() => slave.RegisterCustomFunction<TestMessage>(100, (request, dataStore) => { throw new NotImplementedException(); }));
		}

		[Test]
		public void UnregisterCustomFunction()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			slave.RegisterCustomFunction<TestMessage>(100, (request, dataStore) => { throw new NotImplementedException(); });
			slave.UnregisterCustomFunction(100);

			Assert.Throws<KeyNotFoundException>(() => slave.UnregisterCustomFunction(100));
		}

		[Test]
		public void UnregisterCustomFunction_DoesNotExist()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			Assert.Throws<KeyNotFoundException>(() => slave.UnregisterCustomFunction(100));
		}

		[Test]
		public void TryApplyCustomMessage_ValidateArgs()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			IModbusMessage message;

			Assert.Throws<ArgumentNullException>(() => slave.TryApplyCustomMessage(null, new DataStore(), out message));
			Assert.Throws<ArgumentNullException>(() => slave.TryApplyCustomMessage(new ReadCoilsInputsRequest(), null, out message));
		}

		[Test]
		public void TryApplyCustomMessage()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			IModbusMessage response;
            var applyRequestDelegateExecuted = false;
			slave.RegisterCustomFunction<ReadCoilsInputsRequest>(Modbus.ReadCoils, (request, dataStore) => { applyRequestDelegateExecuted = true; return new ReadCoilsInputsResponse(); });
			Assert.IsTrue(slave.TryApplyCustomMessage(new ReadCoilsInputsRequest(Modbus.ReadCoils, 1, 0, 1), new DataStore(), out response));
			Assert.IsTrue(applyRequestDelegateExecuted);
		}

		[Test]
		public void TryApplyCustomMessage_DoesNotExist()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			IModbusMessage response;
			Assert.IsFalse(slave.TryApplyCustomMessage(new ReadCoilsInputsRequest(), new DataStore(), out response));
		}

		[Test]
		public void TryCreateModbusMessageRequest()
		{
            var mocks = new MockRepository();
			var slave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			slave.RegisterCustomFunction<ReadCoilsInputsRequest>(Modbus.ReadCoils, (r, dataStore) => { throw new NotImplementedException(); });

			IModbusMessage request;
			Assert.IsTrue(slave.TryCreateModbusMessageRequest(Modbus.ReadCoils, new byte[] { 11, Modbus.ReadCoils, 0, 19, 0, 37 }, out request));

			Assert.IsNotNull(request);
		}

		class TestMessage : IModbusMessage
		{
			public byte FunctionCode
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public byte SlaveAddress
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public byte[] MessageFrame
			{
				get { throw new NotImplementedException(); }
			}

			public byte[] ProtocolDataUnit
			{
				get { throw new NotImplementedException(); }
			}

			public ushort TransactionId
			{
				get
				{
					throw new NotImplementedException();
				}
				set
				{
					throw new NotImplementedException();
				}
			}

			public void Initialize(byte[] frame)
			{
				throw new NotImplementedException();
			}
		}
	}
}
