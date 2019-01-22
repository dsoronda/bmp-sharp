using System;
using BmpSharp;
using NUnit.Framework;

namespace Tests {
	public class BitmapTest {
		[SetUp]
		public void Setup() {
		}

		[Test]
		public void BMP_GetStream_ReturnsData_Success() {

			var data = new byte[] { 127, 64, 192 };
			var bmp = new Bitmap( 1, 1, data );
			var bufferSize = BitmapHeader.BitmapHeaderSizeInBytes + data.Length;

			var buffer = new byte[bufferSize];
			using (var memoryStream = bmp.GetStream()) {
				var readedBytes = memoryStream.Read( buffer, 0, bufferSize );
				Assert.AreEqual( bufferSize, readedBytes, "we get less bytes from stream than expected" );
			}

			Assert.AreEqual( BitmapHeader.ByteZero, buffer[0], "invalid header byte[0]" );
			Assert.AreEqual( BitmapHeader.ByteOne, buffer[1], "invalid header byte[1]" );

			if (!BitConverter.IsLittleEndian)
				Array.Reverse( buffer, 2, sizeof( uint ) );

			var bmpFileSize = System.BitConverter.ToUInt32( buffer, 2 );
			Assert.AreEqual( bufferSize, bmpFileSize, "invalid BMP file size" );
		}
	}
}
