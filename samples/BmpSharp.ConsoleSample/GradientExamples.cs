using BmpSharp;

public static class GradientExamples {
	public static byte[] GenerateRedGreen24BitGradient() {
		var width = 256;
		var height = 256;
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

	public static byte[] GenerateRedBlueArgb32GradientWithAlpha() {
		var width = 256;
		var height = 256;
		var bytesPerPixel = (int) BmpSharp.BytesPerPixelEnum.RGBA32;
		var framebuffer = new byte[width * height * bytesPerPixel];

		for (var row = 0; row < height; row++) {
			for (var column = 0; column < width; column++) {
				long offset = ( row * height * bytesPerPixel ) + ( column * bytesPerPixel );
				// NOTE
				// Colors in integer are in ARGB order, but BMP expect bytes in little endian order
				// So we have to store bytes array in BGRA order
				framebuffer[offset] = (byte) column; // blue
				framebuffer[offset + 1] = (byte) 0; // green
				framebuffer[offset + 2] = (byte) row; // red
				framebuffer[offset + 3] = (byte) ( ( row + column ) / 2 ); // alpha
			}
		}

		return framebuffer;
	}

	public static void RunSamples() {
		var redGreenGradient = GenerateRedGreen24BitGradient();
		var redGreenGradientBitmap = new Bitmap( 256, 256, redGreenGradient, BitsPerPixelEnum.RGB24 );
		System.IO.File.WriteAllBytes( $"{nameof(redGreenGradient)}.bmp", redGreenGradientBitmap.GetBmpBytes() );

		// store BMP as Red Blue gradient with Alpha channel
		// NOTE : Paint and Paint.NET doesn't support Alpha channel properly !!! Use XnView or similar app

		var redblueGradient = GenerateRedBlueArgb32GradientWithAlpha();
		var redblueGradientBitmap = new Bitmap( 256, 256, redblueGradient, BitsPerPixelEnum.RGBA32 );
		System.IO.File.WriteAllBytes( $"{nameof(redblueGradient)}.bmp", redblueGradientBitmap.GetBmpBytes() );
	}
}
