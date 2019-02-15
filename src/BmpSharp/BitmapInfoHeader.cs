using System;
using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public class BitmapInfoHeader {
		// NOTE : do not reorder fields !!! we use this layout for direct binary de/serialization!!

		// Warning CS0414  The field 'BitmapInfoHeader.HorizontalPixelPerMeter' is assigned but its value is never used
		// disable error warning , we dont need values in those fields !!
#pragma warning disable CS0414
		//public readonly uint BitmapInfoHeaderSize;

		/// <summary>
		/// the bitmap Width in pixels (signed integer)
		/// </summary>
		public int Width { get; protected set; }

		/// <summary>
		/// the bitmap Height in pixels (signed integer)
		/// </summary>
		public int Height { get; protected set; }

		/// <summary>
		/// the number of color planes (must be 1)
		/// </summary>
		public short ColorPlanes => 1;

		/// <summary>
		/// the number of bits per pixel, which is the color depth of the image. Typical values are 1, 4, 8, 16, 24 and 32.
		/// </summary>
		public BitsPerPixelEnum BitsPerPixel { get; protected set; }

		/// <summary>
		/// 0 	BI_RGB (UNCOMPRESSED)
		/// </summary>
		public CompressionMethod CompressionMethod { get; protected set; } = CompressionMethod.BI_RGB;

		/// <summary>
		/// the image size. This is the size of the raw bitmap data; a dummy 0 can be given for BI_RGB bitmaps.
		/// </summary>
		public int ImageSize { get; protected set; }

		/// <summary>
		/// the horizontal resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		public int HorizontalPixelPerMeter { get; protected set; }

		/// <summary>
		/// the vertical resolution of the image. (pixel per metre, signed integer)
		/// </summary>
		public int VerticalPixelPerMeter { get; protected set; }

		/// <summary>
		/// the number of colors in the color palette, or 0 to default to 2n (not used)
		/// </summary>
		public uint nuberOfColorsInPallete;
		/// <summary>
		/// numberOfImportantColorsUsed (not used)
		/// </summary>
		public uint numberOfImportantColorsUsed;
#pragma warning restore CS0414

		/// <summary>
		/// DIB header (bitmap information header)
		/// This is standard Windows BITMAPINFOHEADER as described here https://en.wikipedia.org/wiki/BMP_file_format#Bitmap_file_header
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bitsPerPixel"></param>
		/// <param name="rawImageSize"></param>
		public BitmapInfoHeader( int width, int height, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0, int horizontalPixelPerMeter = 3780, int verticalPixelPerMeter = 3780, CompressionMethod compressionMethod = CompressionMethod.BI_RGB ) {
			this.Width = width;
			this.Height = height;
			this.BitsPerPixel = bitsPerPixel;
			this.CompressionMethod = compressionMethod;  // CompressionMethod.BI_RGB;
			this.ImageSize = rawImageSize;
			this.HorizontalPixelPerMeter = horizontalPixelPerMeter; // 96 DPI
			this.VerticalPixelPerMeter = verticalPixelPerMeter;   // 96 DPI
			this.nuberOfColorsInPallete = 0; // ignored
			this.numberOfImportantColorsUsed = 0;    // ignored
		}

		//public static int SizeInBytes => System.Runtime.InteropServices.Marshal.SizeOf(typeof(BitmapInfoHeader));
		public static int SizeInBytes => 56;


		public byte[] HeaderInfoBytes {
			get {
				throw new NotImplementedException();
				//return BinarySerializationExtensions.Serialize<BitmapInfoHeader>( this );
			}
		}

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
				Array.Reverse( bytes, 4, 4 ); // size of Width
				Array.Reverse( bytes, 8, 4 ); // size of Height
				Array.Reverse( bytes, BITS_PER_PIXEL_OFFSET, 2 ); // BitsPerPixelEnum
				Array.Reverse( bytes, COMPRESSION_METHOD_OFFSET, 4 ); // CompressionMethod
				Array.Reverse( bytes, 0X20, 4 ); // the image size. This is the size of the raw bitmap data; a dummy 0 can be given for BI_RGB bitmaps.
				Array.Reverse( bytes, HORIZONTAL_RESOLUTION_OFFSET, 4 ); // the horizontal resolution of the image. (pixel per metre, signed integer) 
				Array.Reverse( bytes, VERTICAL_RESOLUTION_OFFSET, 4 ); // the vertical resolution of the image. (pixel per metre, signed integer) 
				Array.Reverse( bytes, 0x2C, 4 ); // the number of colors in the color palette, or 0 to default to 2n (ignored)
				Array.Reverse( bytes, 0x32, 4 ); // the number of important colors used, or 0 when every color is important; generally ignored 
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
				verticalPixelPerMeter: verticalPixelPerMeter,
				compressionMethod: compression
				);
			return infoHeader;
		}
	}

}
