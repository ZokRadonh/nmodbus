using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Modbus.Data;
using Modbus.IO;
using Modbus.Message;
using Modbus.Utility;
using Rhino.Mocks;
using Modbus.Unme.Common;
using Modbus.Device;

namespace Modbus.UnitTests.IO
{
	[TestFixture]
	public class ModbusRtuTransportFixture
	{
		[Test]
		public void BuildMessageFrame()
		{
			byte[] message = { 17, Modbus.ReadCoils, 0, 19, 0, 37, 14, 132 };
            var request = new ReadCoilsInputsRequest(Modbus.ReadCoils, 17, 19, 37);
			Assert.AreEqual(message, new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>()).BuildMessageFrame(request));
		}

		[Test]
		public void ResponseBytesToReadCoils()
		{
			byte[] frameStart = { 0x11, 0x01, 0x05, 0xCD, 0x6B, 0xB2, 0x0E, 0x1B };
			Assert.AreEqual(6, ModbusRtuTransport.ResponseBytesToRead<ReadCoilsInputsResponse>(frameStart, null));
		}

		[Test]
		public void ResponseBytesToReadCoilsNoData()
		{
			byte[] frameStart = { 0x11, 0x01, 0x00, 0x00, 0x00 };
			Assert.AreEqual(1, ModbusRtuTransport.ResponseBytesToRead<ReadCoilsInputsResponse>(frameStart, null));
		}

		[Test]
		public void ResponseBytesToReadWriteCoilsResponse()
		{
			byte[] frameStart = { 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0, 0 };
			Assert.AreEqual(4, ModbusRtuTransport.ResponseBytesToRead<ReadCoilsInputsResponse>(frameStart, null));
		}

		[Test]
		public void ResponseBytesToReadDiagnostics()
		{
			byte[] frameStart = { 0x01, 0x08, 0x00, 0x00 };
			Assert.AreEqual(4, ModbusRtuTransport.ResponseBytesToRead<DiagnosticsRequestResponse>(frameStart, null));
		}

		[Test]
		public void ResponseBytesToReadSlaveException()
		{
			byte[] frameStart = { 0x01, Modbus.ExceptionOffset + 1, 0x01 };
			Assert.AreEqual(1, ModbusRtuTransport.ResponseBytesToRead<ReadCoilsInputsResponse>(frameStart, null));
		}

		[Test, ExpectedException(typeof(NotImplementedException))]
		public void ResponseBytesToReadInvalidFunctionCode()
		{
			byte[] frame = { 0x11, 0x16, 0x00, 0x01, 0x00, 0x02, 0x04 };
			ModbusRtuTransport.ResponseBytesToRead<ReadCoilsInputsResponse>(frame, null);
			Assert.Fail();
		}

		[Test]
		public void RequestBytesToReadDiagnostics()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			byte[] frame = { 0x01, 0x08, 0x00, 0x00, 0xA5, 0x37, 0, 0 };
			Assert.AreEqual(1, ModbusRtuTransport.RequestBytesToRead(frame, mockSlave));
		}

		[Test]
		public void RequestBytesToReadCoils()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			byte[] frameStart = { 0x11, 0x01, 0x00, 0x13, 0x00, 0x25 };
			Assert.AreEqual(1, ModbusRtuTransport.RequestBytesToRead(frameStart, mockSlave));
		}

		[Test]
		public void RequestBytesToReadWriteCoilsRequest()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			byte[] frameStart = { 0x11, 0x0F, 0x00, 0x13, 0x00, 0x0A, 0x02, 0xCD, 0x01 };
			Assert.AreEqual(4, ModbusRtuTransport.RequestBytesToRead(frameStart, mockSlave));
		}

		[Test]
		public void RequestBytesToReadWriteMultipleHoldingRegisters()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			byte[] frameStart = { 0x11, 0x10, 0x00, 0x01, 0x00, 0x02, 0x04 };
			Assert.AreEqual(6, ModbusRtuTransport.RequestBytesToRead(frameStart, mockSlave));
		}

		[Test, ExpectedException(typeof(NotImplementedException))]
		public void RequestBytesToReadInvalidFunctionCode()
		{
            var mocks = new MockRepository();
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			byte[] frame = { 0x11, 0xFF, 0x00, 0x01, 0x00, 0x02, 0x04 };
			ModbusRtuTransport.RequestBytesToRead(frame, mockSlave);
			Assert.Fail();
		}

		[Test]
		public void ChecksumsMatchSucceed()
		{
            var transport = new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>());
            var message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 17, 19, 37);
			byte[] frame = { 17, Modbus.ReadCoils, 0, 19, 0, 37, 14, 132 };
			Assert.IsTrue(transport.ChecksumsMatch(message, frame));
		}

		[Test]
		public void ChecksumsMatchFail()
		{
            var transport = new ModbusRtuTransport(MockRepository.GenerateStub<IStreamResource>());
            var message = new ReadCoilsInputsRequest(Modbus.ReadCoils, 17, 19, 38);
			byte[] frame = { 17, Modbus.ReadCoils, 0, 19, 0, 37, 14, 132 };
			Assert.IsFalse(transport.ChecksumsMatch(message, frame));
		}

		[Test]
		public void ReadResponse()
		{
            var mocks = new MockRepository();
            var transport = mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

			Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
				.Return(new byte[] { 1, 1, 1, 0 });

			Expect.Call(transport.Read(2))
				.Return(new byte[] { 81, 136 });

			mocks.ReplayAll();

            var response = transport.ReadResponse<ReadCoilsInputsResponse>() as ReadCoilsInputsResponse;
			Assert.IsNotNull(response);
            var expectedResponse = new ReadCoilsInputsResponse(Modbus.ReadCoils, 1, 1, new DiscreteCollection(false));
			Assert.AreEqual(response.MessageFrame, expectedResponse.MessageFrame);

			mocks.VerifyAll();
		}

		[Test]
		public void ReadResponseSlaveException()
		{
            var mocks = new MockRepository();
            var transport = mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

			byte[] messageFrame = { 0x01, 0x81, 0x02 };
			byte[] crc = ModbusUtility.CalculateCrc(messageFrame);

			Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
				.Return(messageFrame.Concat(SequenceUtility.ToSequence(crc[0])).ToArray());

			Expect.Call(transport.Read(1))
				.Return(new byte[] { crc[1] });

			mocks.ReplayAll();

			Assert.IsTrue(transport.ReadResponse<ReadCoilsInputsResponse>() is SlaveExceptionResponse);

			mocks.VerifyAll();
		}

		/// <summary>
		/// We want to throw an IOException for any message w/ an invalid checksum, 
		/// this must preceed throwing a SlaveException based on function code > 127
		/// </summary>
		[Test, ExpectedException(typeof(IOException))]
		public void ReadResponseSlaveExceptionWithErroneousLrc()
		{
            var mocks = new MockRepository();
            var transport = mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());

			byte[] messageFrame = { 0x01, 0x81, 0x02 };
			// invalid crc
			byte[] crc = { 0x9, 0x9 };

			Expect.Call(transport.Read(ModbusRtuTransport.ResponseFrameStartLength))
				.Return(messageFrame.Concat(crc[0].ToSequence()).ToArray());

			Expect.Call(transport.Read(1))
				.Return(new [] { crc[1] });

			mocks.ReplayAll();

			transport.ReadResponse<ReadCoilsInputsResponse>();

			mocks.VerifyAll();
		}

		[Test]
		public void ReadRequest()
		{
            var mocks = new MockRepository();
            var transport = mocks.PartialMock<ModbusRtuTransport>(MockRepository.GenerateStub<IStreamResource>());
			var mockSlave = mocks.PartialMock<ModbusSlave>((byte) 1, new EmptyTransport());

			Expect.Call(transport.Read(ModbusRtuTransport.RequestFrameStartLength))
				.Return(new byte[] { 1, 1, 1, 0, 1, 0, 0 });

			Expect.Call(transport.Read(1))
				.Return(new byte[] { 5 });

			mocks.ReplayAll();

			Assert.AreEqual(new byte[] { 1, 1, 1, 0, 1, 0, 0, 5 }, transport.ReadRequest(mockSlave));
			mocks.VerifyAll();
		}

		[Test]
		public void Read()
		{
            var mocks = new MockRepository();
            var mockSerialResource = mocks.StrictMock<IStreamResource>();

			Expect.Call(mockSerialResource.Read(new byte[5], 0, 5)).Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
			{
				Array.Copy(new byte[] { 2, 2, 2 }, buf, 3);
				return 3;
			}));

			Expect.Call(mockSerialResource.Read(new byte[] { 2, 2, 2, 0, 0 }, 3, 2)).Do(((Func<byte[], int, int, int>) delegate(byte[] buf, int offset, int count)
			{
				Array.Copy(new byte[] { 3, 3 }, 0, buf, 3, 2);
				return 2;
			}));

			mocks.ReplayAll();

            var transport = new ModbusRtuTransport(mockSerialResource);
			Assert.AreEqual(new byte[] { 2, 2, 2, 3, 3 }, transport.Read(5));

			mocks.VerifyAll();
		}
	}
}
