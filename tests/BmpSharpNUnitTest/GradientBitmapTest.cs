using BmpSharp;
using NUnit;
using NUnit.Framework;
using System;
using System.IO;

namespace Tests {
	[TestFixture]

	public class GradientBitmapTest {
		private byte[] GenerateRedGreenGradient( int width, int height ) {
			var bytesPerPixel = (int) BmpSharp.BytesPerPixelEnum.RGB24;
			var framebuffer = new byte[width * height * bytesPerPixel];

			for (var row = 0; row < height; row++) {
				for (var column = 0; column < width; column++) {
					long offset = ( row * height * bytesPerPixel ) + ( column * bytesPerPixel );
					framebuffer[offset] = 0; // blue
					framebuffer[offset + 1] = (byte) column; // green
					framebuffer[offset + 2] = (byte) row; // red
				}
			}
			return framebuffer;
		}

		private byte[] GenerateRedBlueGradientWithAlpha( int width, int height ) {
			var bytesPerPixel = (int) BmpSharp.BytesPerPixelEnum.RGBA32;
			var framebuffer = new byte[width * height * bytesPerPixel];

			for (var row = 0; row < height; row++) {
				for (var column = 0; column < width; column++) {
					long offset = ( row * height * bytesPerPixel ) + ( column * bytesPerPixel );
					framebuffer[offset] = (byte) column; // blue
					framebuffer[offset + 1] = (byte) 0; // green
					framebuffer[offset + 2] = (byte) row; // red
					framebuffer[offset + 3] = (byte) ((row + column)/2); // alpha
				}
			}
			return framebuffer;
		}

		byte[] RedGreenGradient = null;

		[OneTimeSetUp]
		public void Setup() {
			RedGreenGradient = GenerateRedGreenGradient( 256, 256 );
		}

		[Test]
		public void GenerateRedGradient_Success() {
			var width = 256;
			var height = 256;
			var bitmap = new Bitmap( width, height, RedGreenGradient );
			var bytes = bitmap.GetBmpBytes();

			var expectedPixelSize = width * height * (int) BmpSharp.BytesPerPixelEnum.RGB24;
			var expectedFileSize = BmpSharp.BitmapFileHeader.BitmapFileHeaderSizeInBytes +
				BmpSharp.BitmapInfoHeader.SizeInBytes + expectedPixelSize;
			Assert.AreEqual( bitmap.FileHeader.FileSize, expectedFileSize, "File size is not same" );

			// NOTE: this is just to verify that gradient file is properly generated.
			//var tempFolder = System.IO.Path.GetTempPath();
			//var targetFile = Path.Combine( tempFolder, "BmpSharpredGreenGradient.bmp" );
			//BitmapFileHelper.SaveBitmapToFile( targetFile, bitmap );

			//Assert.IsTrue( File.Exists( targetFile ) );
		}
	}
}
