using System;
using BmpSharp;
using NUnit.Framework;
using Shouldly;

namespace BmpSharpNUnitTest;

public class BitmapTest {
	[SetUp] public void Setup() { }

	[Test] public void BMP_GetStream_ReturnsData_Success() {
		var data = new byte[] {127, 64, 192};
		var bmp = new Bitmap( 1, 1, data );
		var bufferSize = BitmapFileHeader.BitmapFileHeaderSizeInBytes + BitmapInfoHeader.SizeInBytes + bmp.BytesPerRow;

		var buffer = new byte[bufferSize];
		using (var memoryStream = bmp.GetBmpStream()) {
			var readBytes = memoryStream.Read( buffer, 0, bufferSize );
			readBytes.ShouldBe( bufferSize, "we get less bytes from stream than expected" );
		}

		buffer[0].ShouldBe( BitmapFileHeader.ByteZero, "invalid fileHeader byte[0]" );
		buffer[1].ShouldBe( BitmapFileHeader.ByteOne, "invalid fileHeader byte[1]" );

		if (!BitConverter.IsLittleEndian) Array.Reverse( buffer, index: 2, length: sizeof(uint) );

		var bmpFileSize = System.BitConverter.ToUInt32( buffer, startIndex: 2 );
		bmpFileSize.ShouldBe( (uint) bufferSize, "invalid BMP file headerSize" );
		bmp.PixelData.Length.ShouldBe( data.Length, "invalid pixel data array size" );

		Assert.AreEqual( data, bmp.PixelData, "Generated BMP Pixel data are note valid." );
	}

	[Test] public void DataFlipped_Success() {
		// 2 rows , 3 pixels per row
		var originalData = new byte[] {
			// row 1
			0x00, 0x00, 0xff, // red
			0x00, 0xff, 0x00, // green
			0xff, 0x00, 0x00, // blue
			// row 2
			0xff, 0xff, 0xff, // white
			0x77, 0x76, 0x75, // grayish
			0x10, 0x11, 0x12, // darkish
		};

		var flipedData = new byte[] {
			// row 2
			0xff, 0xff, 0xff, // white
			0x77, 0x76, 0x75, // grayish
			0x10, 0x11, 0x12, // darkish
			// row 1
			0x00, 0x00, 0xff, // red
			0x00, 0xff, 0x00, // green
			0xff, 0x00, 0x00, // blue
		};

		var bmp = new Bitmap( 3, 2, originalData );

		bmp.PixelDataFliped.Length.ShouldBe(flipedData.Length);
		bmp.PixelDataFliped .ShouldBe(flipedData);
	}
}
