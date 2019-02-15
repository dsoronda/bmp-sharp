using System;
using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public struct BitmapInfoHeaderRGBA {
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

		public const uint RedChannelBitMask = 0x0FF0000;
		public const uint GreenChannelBitMask = 0x0000FF00;
		public const uint BlueChannelBitMask = 0x000000FF;
		public const uint AlphaChannelBitMask = 0xFF000000;
#pragma warning restore CS0414

		/// <summary>
		/// DIB header (bitmap information header)
		/// This is standard Windows BITMAPINFOHEADER as described here https://en.wikipedia.org/wiki/BMP_file_format#Bitmap_file_header
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bitsPerPixel"></param>
		/// <param name="rawImageSize"></param>
		public BitmapInfoHeaderRGBA( int width, int height, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0, int horizontalPixelPerMeter = 3780, int verticalPixelPerMeter = 3780 ) {
			bitmapInfoHeaderSize = SizeInBytes;
			this.width = width;
			this.height = height;
			colorPlanes = 1;
			this.bitsPerPixel = (short) bitsPerPixel;
			compressionMethod = (uint) CompressionMethod.BI_RGB;
			imageSize = rawImageSize;
			this.horizontalPixelPerMeter = horizontalPixelPerMeter; // 96 DPI
			this.verticalPixelPerMeter = verticalPixelPerMeter;   // 96 DPI
			nuberOfColorsInPallete = 0; // ignored
			numberOfImportantColorsUsed = 0;    // ignored
		}

		//public static int SizeInBytes => System.Runtime.InteropServices.Marshal.SizeOf(typeof(BitmapInfoHeader));
		public const int SizeInBytes = 56;
		
		public byte[] HeaderInfoBytes => BinarySerializationExtensions.Serialize<BitmapInfoHeaderRGBA>( this );

		public static BitmapInfoHeader GetHeaderFromBytes( byte[] bytes ) {

			if (bytes.Length < BitmapInfoHeader.SizeInBytes)
				throw new ArgumentOutOfRangeException( $"Info header should be at least 40 bytes. Smaller versions are not supported." );

			// NOTE offses are 0 based for current byteArray (different than in wiki)
			const int BITS_PER_PIXEL_OFFSET = 0x0E;
			const int COMPRESSION_METHOD_OFFSET = 0x10;

			const int HORIZONTAL_RESOLUTION_OFFSET = 0x18;
			const int VERTICAL_RESOLUTION_OFFSET = 0x1C;
			if (!BitConverter.IsLittleEndian) {
				// BMP file is in little endian, we have to reverse bytes for parsing on Big-endian platform
				Array.Reverse( bytes, 0, 4 ); // size of header
				Array.Reverse( bytes, 4, 4 ); // size of width
				Array.Reverse( bytes, 8, 4 ); // size of height
				Array.Reverse( bytes, BITS_PER_PIXEL_OFFSET, 2 ); // BitsPerPixelEnum
				Array.Reverse( bytes, COMPRESSION_METHOD_OFFSET, 4 ); // CompressionMethod
				Array.Reverse( bytes, 0X20, 4 ); // the image size. This is the size of the raw bitmap data; a dummy 0 can be given for BI_RGB bitmaps.
				Array.Reverse( bytes, HORIZONTAL_RESOLUTION_OFFSET, 4 ); // the horizontal resolution of the image. (pixel per metre, signed integer) 
				Array.Reverse( bytes, VERTICAL_RESOLUTION_OFFSET, 4 ); // the vertical resolution of the image. (pixel per metre, signed integer) 
				//Array.Reverse( bytes, 0x2C, 4 ); // the number of colors in the color palette, or 0 to default to 2n (ignored)
				//Array.Reverse( bytes, 0x32, 4 ); // the number of important colors used, or 0 when every color is important; generally ignored 
			}

			var headerSize = BitConverter.ToInt32( bytes, 0 );
			var width = BitConverter.ToInt32( bytes, 4 );
			var height = BitConverter.ToInt32( bytes, 8 );
			var bitsPerPixel = (BitsPerPixelEnum) BitConverter.ToInt16( bytes, BITS_PER_PIXEL_OFFSET );

			var compression = (CompressionMethod) BitConverter.ToInt16( bytes, COMPRESSION_METHOD_OFFSET );
			if (!( ( compression == CompressionMethod.BI_RGB ) || ( compression == CompressionMethod.BI_BITFIELDS ) ))
				throw new Exception( $"This {Enum.GetName( compression.GetType(), compression )} is not supported." );

			var horizontalPixelPerMeter = BitConverter.ToInt32( bytes, HORIZONTAL_RESOLUTION_OFFSET );
			var verticalPixelPerMeter = BitConverter.ToInt32( bytes, VERTICAL_RESOLUTION_OFFSET );

			var infoHeader = new BitmapInfoHeader(
				width, height, bitsPerPixel, rawImageSize: 0,
				horizontalPixelPerMeter: horizontalPixelPerMeter,
				verticalPixelPerMeter: verticalPixelPerMeter
				);
			return infoHeader;
		}
	}

}
