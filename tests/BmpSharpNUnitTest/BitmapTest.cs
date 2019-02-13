using System;
using System.Linq;
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
			var bufferSize = BitmapHeader.BitmapHeaderSizeInBytes + bmp.BytesPerRow;

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
			Assert.AreEqual( bufferSize, bmpFileSize, "invalid BMP file headerSize" );
		}

		[Test]
		public void DataFliped__Success() {
			// 2 rows , 3 pixels per row
			var originalData = new byte[] {
				// row 1
				0x00, 0xff ,0xff,	// red
				0x00, 0xff ,0x00,	// green
				0xff, 0x00, 0x00,	// blue
				// row 2
				0xff, 0xff, 0xff,	// white
				0x77, 0x76, 0x75,	// grayish
				0x10, 0x11, 0x12,	// darkish
			};

			var flipedData = new byte[] {
				// row 2
				0xff, 0xff, 0xff,	// white
				0x77, 0x76, 0x75,	// grayish
				0x10, 0x11, 0x12,	// darkish
				// row 1
				0x00, 0xff ,0xff,	// red
				0x00, 0xff ,0x00,	// green
				0xff, 0x00, 0x00,	// blue
			};

			var bmp = new Bitmap( 3, 2, originalData );

			Assert.AreEqual( flipedData.Length, bmp.PixelDataFliped.Length );
			Assert.AreEqual( flipedData, bmp.PixelDataFliped);

		}
	}
}
