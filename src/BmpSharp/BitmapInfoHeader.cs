using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public struct BitmapInfoHeader {
		// NOTE : do not reorder fields !!! we use this layout for direct binary de/serialization!!

		// Warning CS0414  The field 'BitmapInfoHeader.horizontalPixelPerMeter' is assigned but its value is never used
		// disable error warning , we dont need values in those fields !!
#pragma warning disable CS0414
		public readonly uint bitmapInfoHeaderSize;

		/// <summary>
		/// the bitmap width in pixels (signed integer)
		/// </summary>
		public readonly int width;

		/// <summary>
		/// the bitmap height in pixels (signed integer)
		/// </summary>
		public readonly int height;

		/// <summary>
		/// the number of color planes (must be 1)
		/// </summary>
		public readonly short colorPlanes;

		/// <summary>
		/// the number of bits per pixel, which is the color depth of the image. Typical values are 1, 4, 8, 16, 24 and 32.
		/// </summary>
		public readonly short bitsPerPixel;

		/// <summary>
		/// 0 	BI_RGB (UNCOMPRESSED)
		/// </summary>
		public readonly uint compressionMethod;

		/// <summary>
		/// the image size. This is the size of the raw bitmap data; a dummy 0 can be given for BI_RGB bitmaps.
		/// </summary>
		public readonly int imageSize;

		/// <summary>
		/// the horizontal resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		public readonly int horizontalPixelPerMeter;

		/// <summary>
		/// the vertical resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		public readonly int verticalPixelPerMeter;

		/// <summary>
		/// the number of colors in the color palette, or 0 to default to 2n
		/// </summary>
		public readonly uint nuberOfColorsInPallete;

		public readonly uint numberOfImportantColorsUsed;
#pragma warning restore CS0414

		public BitmapInfoHeader( int width, int height, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0 ) {
			bitmapInfoHeaderSize = SizeInBytes;
			this.width = width;
			this.height = height;
			colorPlanes = 1;
			this.bitsPerPixel = (short) bitsPerPixel;
			compressionMethod = 0;
			imageSize = rawImageSize;
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
