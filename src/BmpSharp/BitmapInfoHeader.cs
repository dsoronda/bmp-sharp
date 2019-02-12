using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public struct BitmapInfoHeader {
		// Warning CS0414  The field 'BitmapInfoHeader.horizontalPixelPerMeter' is assigned but its value is never used
		// disable error warning , we dont need values in those fields !!
#pragma warning disable CS0414
		private readonly uint size;

		/// <summary>
		/// the bitmap width in pixels (signed integer)
		/// </summary>
		private readonly int width;

		/// <summary>
		/// the bitmap height in pixels (signed integer)
		/// </summary>
		private readonly int height;

		/// <summary>
		/// the number of color planes (must be 1)
		/// </summary>
		private readonly short colorPlanes;

		/// <summary>
		/// the number of bits per pixel, which is the color depth of the image. Typical values are 1, 4, 8, 16, 24 and 32.
		/// </summary>
		private readonly short bitsPerPixel;

		/// <summary>
		/// 0 	BI_RGB (UNCOMPRESSED)
		/// </summary>
		private readonly uint compressionMethod;

		private readonly uint imageSize;

		/// <summary>
		/// the horizontal resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		private readonly int horizontalPixelPerMeter;

		/// <summary>
		/// the vertical resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		private readonly int verticalPixelPerMeter;

		/// <summary>
		/// the number of colors in the color palette, or 0 to default to 2n
		/// </summary>
		private readonly uint nuberOfColorsInPallete;

		private readonly uint numberOfImportantColorsUsed;
#pragma warning restore CS0414

		public BitmapInfoHeader( int width, int height, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24 ) {
			size = SizeInBytes;
			this.width = width;
			this.height = height;
			colorPlanes = 1;
			this.bitsPerPixel = (short) bitsPerPixel;
			compressionMethod = 0;
			imageSize = 0;
			horizontalPixelPerMeter = 3780; // 96 DPI
			verticalPixelPerMeter = 3780;   // 96 DPI
			nuberOfColorsInPallete = 0;
			numberOfImportantColorsUsed = 0;
		}

		//public static int SizeInBytes => System.Runtime.InteropServices.Marshal.SizeOf(typeof(BitmapInfoHeader));
		public const int SizeInBytes = 40;

		public byte[] HeaderInfoBytes => BinarySerializationExtensions.Serialize<BitmapInfoHeader>( this );
	}


}
