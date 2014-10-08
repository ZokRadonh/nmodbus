using System;
using System.Linq;
using NUnit.Framework;
using Modbus.Data;
using Modbus.Message;
using Modbus.Unme.Common;
using Modbus.Device;
using Rhino.Mocks;
using Modbus.IO;

namespace Modbus.UnitTests.Message
{
	[TestFixture]
	public class ModbusMessageFactoryFixture : ModbusMessageFixture
	{
		[Test]
		public void CreateModbusMessageReadCoilsRequest()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsRequest>(new byte[] { 11, Modbus.ReadCoils, 0, 19, 0, 37 });
            var expectedRequest = new ReadCoilsInputsRequest(Modbus.ReadCoils, 11, 19, 37);
			AssertModbusMessagePropertiesAreEqual(request, expectedRequest);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.NumberOfPoints, request.NumberOfPoints);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadCoilsRequestWithInvalidFrameSize()
		{
			byte[] frame = { 11, Modbus.ReadCoils, 4, 1, 2 };
            var request = ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsRequest>(frame);
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageReadCoilsResponse()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(new byte[] { 11, Modbus.ReadCoils, 1, 1 });
            var expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 11, 1, new DiscreteCollection(true, false, false, false));
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.Data.NetworkBytes, response.Data.NetworkBytes);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadCoilsResponseWithNoByteCount()
		{
			byte[] frame = { 11, Modbus.ReadCoils };
            var response = ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(frame);
			Assert.Fail();
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadCoilsResponseWithInvalidDataSize()
		{
			byte[] frame = { 11, Modbus.ReadCoils, 4, 1, 2, 3 };
            var response = ModbusMessageFactory.CreateModbusMessage<ReadCoilsInputsResponse>(frame);
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageReadHoldingRegistersRequest()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(new byte[] { 17, Modbus.ReadHoldingRegisters, 0, 107, 0, 3 });
            var expectedRequest = new ReadHoldingInputRegistersRequest(Modbus.ReadHoldingRegisters, 17, 107, 3);
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.NumberOfPoints, request.NumberOfPoints);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadHoldingRegistersRequestWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersRequest>(new byte[] { 11, Modbus.ReadHoldingRegisters, 0, 0, 5 });
		}

		[Test]
		public void CreateModbusMessageReadHoldingRegistersResponse()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersResponse>(new byte[] { 11, Modbus.ReadHoldingRegisters, 4, 0, 3, 0, 4 });
            var expectedResponse = new ReadHoldingInputRegistersResponse(Modbus.ReadHoldingRegisters, 11, new RegisterCollection(3, 4));
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadHoldingRegistersResponseWithInvalidFrameSize()
		{
			ModbusMessageFactory.CreateModbusMessage<ReadHoldingInputRegistersResponse>(new byte[] { 11, Modbus.ReadHoldingRegisters });
		}

		[Test]
		public void CreateModbusMessageSlaveExceptionResponse()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 129, 2 });
            var expectedException = new SlaveExceptionResponse(11, Modbus.ReadCoils + Modbus.ExceptionOffset, 2);
			Assert.AreEqual(expectedException.FunctionCode, response.FunctionCode);
			Assert.AreEqual(expectedException.SlaveAddress, response.SlaveAddress);
			Assert.AreEqual(expectedException.MessageFrame, response.MessageFrame);
			Assert.AreEqual(expectedException.ProtocolDataUnit, response.ProtocolDataUnit);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageSlaveExceptionResponseWithInvalidFunctionCode()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 128, 2 });
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageSlaveExceptionResponseWithInvalidFrameSize()
		{
			ModbusMessageFactory.CreateModbusMessage<SlaveExceptionResponse>(new byte[] { 11, 128 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageWriteSingleCoilRequestResponse()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteSingleCoilRequestResponse>(new byte[] { 17, Modbus.WriteSingleCoil, 0, 172, byte.MaxValue, 0 });
            var expectedRequest = new WriteSingleCoilRequestResponse(17, 172, true);
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageWriteSingleCoilRequestResponseWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteSingleCoilRequestResponse>(new byte[] { 11, Modbus.WriteSingleCoil, 0, 105, byte.MaxValue });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageWriteSingleRegisterRequestResponse()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteSingleRegisterRequestResponse>(new byte[] { 17, Modbus.WriteSingleRegister, 0, 1, 0, 3 });
            var expectedRequest = new WriteSingleRegisterRequestResponse(17, 1, 3);
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageWriteSingleRegisterRequestResponseWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteSingleRegisterRequestResponse>(new byte[] { 11, Modbus.WriteSingleRegister, 0, 1, 0 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageWriteMultipleRegistersRequest()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(new byte[] { 11, Modbus.WriteMultipleRegisters, 0, 5, 0, 1, 2, 255, 255 });
            var expectedRequest = new WriteMultipleRegistersRequest(11, 5, new RegisterCollection(ushort.MaxValue));
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.NumberOfPoints, request.NumberOfPoints);
			Assert.AreEqual(expectedRequest.ByteCount, request.ByteCount);
			Assert.AreEqual(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageWriteMultipleRegistersRequestWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersRequest>(new byte[] { 11, Modbus.WriteMultipleRegisters, 0, 5, 0, 1, 2 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageWriteMultipleRegistersResponse()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<WriteMultipleRegistersResponse>(new byte[] { 17, Modbus.WriteMultipleRegisters, 0, 1, 0, 2 });
            var expectedResponse = new WriteMultipleRegistersResponse(17, 1, 2);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.StartAddress, response.StartAddress);
			Assert.AreEqual(expectedResponse.NumberOfPoints, response.NumberOfPoints);
		}

		[Test]
		public void CreateModbusMessageWriteMultipleCoilsRequest()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsRequest>(new byte[] { 17, Modbus.WriteMultipleCoils, 0, 19, 0, 10, 2, 205, 1 });
            var expectedRequest = new WriteMultipleCoilsRequest(17, 19, new DiscreteCollection(true, false, true, true, false, false, true, true, true, false));
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
			Assert.AreEqual(expectedRequest.StartAddress, request.StartAddress);
			Assert.AreEqual(expectedRequest.NumberOfPoints, request.NumberOfPoints);
			Assert.AreEqual(expectedRequest.ByteCount, request.ByteCount);
			Assert.AreEqual(expectedRequest.Data.NetworkBytes, request.Data.NetworkBytes);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageWriteMultipleCoilsRequestWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsRequest>(new byte[] { 17, Modbus.WriteMultipleCoils, 0, 19, 0, 10, 2 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageWriteMultipleCoilsResponse()
		{
            var response = ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsResponse>(new byte[] { 17, Modbus.WriteMultipleCoils, 0, 19, 0, 10 });
            var expectedResponse = new WriteMultipleCoilsResponse(17, 19, 10);
			AssertModbusMessagePropertiesAreEqual(expectedResponse, response);
			Assert.AreEqual(expectedResponse.StartAddress, response.StartAddress);
			Assert.AreEqual(expectedResponse.NumberOfPoints, response.NumberOfPoints);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageWriteMultipleCoilsResponseWithInvalidFrameSize()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<WriteMultipleCoilsResponse>(new byte[] { 17, Modbus.WriteMultipleCoils, 0, 19, 0 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageReadWriteMultipleRegistersRequest()
		{
            var request = ModbusMessageFactory.CreateModbusMessage<ReadWriteMultipleRegistersRequest>(new byte[] { 0x05, 0x17, 0x00, 0x03, 0x00, 0x06, 0x00, 0x0e, 0x00, 0x03, 0x06, 0x00, 0xff, 0x00, 0xff, 0x00, 0xff });
            var writeCollection = new RegisterCollection(255, 255, 255);
            var expectedRequest = new ReadWriteMultipleRegistersRequest(5, 3, 6, 14, writeCollection);
			AssertModbusMessagePropertiesAreEqual(expectedRequest, request);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReadWriteMultipleRegistersRequestWithInvalidFrameSize()
		{
			byte[] frame = { 17, Modbus.ReadWriteMultipleRegisters, 1, 2, 3 };
            var request = ModbusMessageFactory.CreateModbusMessage<ReadWriteMultipleRegistersRequest>(frame);
			Assert.Fail();
		}

		[Test]
		public void CreateModbusMessageReturnQueryDataRequestResponse()
		{
			const byte slaveAddress = 5;
            var data = new RegisterCollection(50);
			var frame = SequenceUtility.ToSequence<byte>(slaveAddress, 8, 0, 0).Concat(data.NetworkBytes).ToArray();
            var message = ModbusMessageFactory.CreateModbusMessage<DiagnosticsRequestResponse>(frame);
            var expectedMessage = new DiagnosticsRequestResponse(Modbus.DiagnosticsReturnQueryData, slaveAddress, data);

			Assert.AreEqual(expectedMessage.SubFunctionCode, message.SubFunctionCode);
			AssertModbusMessagePropertiesAreEqual(expectedMessage, message);
		}

		[Test]
		[ExpectedException(typeof(FormatException))]
		public void CreateModbusMessageReturnQueryDataRequestResponseTooSmall()
		{
            var frame = new byte[] { 5, 8, 0, 0, 5 };
            var message = ModbusMessageFactory.CreateModbusMessage<DiagnosticsRequestResponse>(frame);
		}

		[Test, ExpectedException(typeof(FormatException))]
		public void CreateModbusRequestWithInvalidMessageFrame()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			ModbusMessageFactory.CreateModbusRequest(mockSlave, new byte[] { 0, 1 });
			Assert.Fail();
		}

		[Test, ExpectedException(typeof(ArgumentException))]
		public void CreateModbusRequestWithInvalidFunctionCode()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			ModbusMessageFactory.CreateModbusRequest(mockSlave, new byte[] { 1, 99, 0, 0, 0, 1, 23 });
			Assert.Fail();
		}

		[Test]
		public void CreateModbusRequestForReadCoils()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

            var req = new ReadCoilsInputsRequest(1, 2, 1, 10);
			IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(mockSlave, req.MessageFrame);
			Assert.AreEqual(typeof(ReadCoilsInputsRequest), request.GetType());
		}

		[Test]
		public void CreateModbusRequestForDiagnostics()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

            var diagnosticsRequest = new DiagnosticsRequestResponse(0, 2, new RegisterCollection(45));
			IModbusMessage request = ModbusMessageFactory.CreateModbusRequest(mockSlave, diagnosticsRequest.MessageFrame);
			Assert.AreEqual(typeof(DiagnosticsRequestResponse), request.GetType());
		}
	}
}
