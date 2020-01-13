using System;
using System.Linq;
using BmpSharp;
using NUnit.Framework;

namespace Tests {
	public class BitmapTest {
		[SetUp]
		public void Setup() { }

		[Test]
		public void BMP_GetStream_ReturnsData_Success() {
			var data = new byte[] { 127, 64, 192 };
			var bmp = new Bitmap( 1, 1, data );
			var bufferSize = BitmapFileHeader.BitmapFileHeaderSizeInBytes + BitmapInfoHeader.SizeInBytes + bmp.BytesPerRow;

			var buffer = new byte[bufferSize];
			var bitmapBytes = bmp.GetBmpBytes();
			using (var memoryStream = bmp.GetBmpStream()) {
				var readedBytes = memoryStream.Read( buffer, 0, bufferSize );
				Assert.AreEqual( bufferSize, readedBytes, "we get less bytes from stream than expected" );
			}

			Assert.AreEqual( BitmapFileHeader.ByteZero, buffer[0], "invalid fileHeader byte[0]" );
			Assert.AreEqual( BitmapFileHeader.ByteOne, buffer[1], "invalid fileHeader byte[1]" );

			if (!BitConverter.IsLittleEndian)
				Array.Reverse( buffer, index: 2, length: sizeof( uint ) );

			var bmpFileSize = System.BitConverter.ToUInt32( buffer, startIndex: 2 );
			Assert.AreEqual( bufferSize, bmpFileSize, "invalid BMP file headerSize" );
			Assert.AreEqual( data.Length, bmp.PixelData.Length, "invalid pixel data array size" );

			Assert.AreEqual( data, bmp.PixelData , "Generated BMP Pixel data are note valid.");
		}

		[Test]
		public void DataFliped__Success() {
			// 2 rows , 3 pixels per row
			var originalData = new byte[] {
				// row 1
				0x00, 0x00, 0xff,	// red
				0x00, 0xff, 0x00,	// green
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
				0x00, 0x00, 0xff,	// red
				0x00, 0xff, 0x00,	// green
				0xff, 0x00, 0x00,	// blue
			};

			var bmp = new Bitmap( 3, 2, originalData );

			Assert.AreEqual( flipedData.Length, bmp.PixelDataFliped.Length );
			Assert.AreEqual( flipedData, bmp.PixelDataFliped );

		}
	}

}
