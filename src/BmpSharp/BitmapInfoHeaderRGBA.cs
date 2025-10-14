using System;
using System.Runtime.InteropServices;

namespace BmpSharp;

[StructLayout( LayoutKind.Sequential, Pack = 1 )]
public class BitmapInfoHeaderRGBA : BitmapInfoHeader {
	public const uint RedChannelBitMask = 0x00FF0000;
	public const uint GreenChannelBitMask = 0x0000FF00;
	public const uint BlueChannelBitMask = 0x000000FF;
	public const uint AlphaChannelBitMask = 0xFF000000;
#pragma warning restore CS0414

	/// <summary>
	/// DIB header (bitmap information header)
	/// This is standard Windows BITMAPINFOHEADER as described here https://en.wikipedia.org/wiki/BMP_file_format#Bitmap_file_header
	/// </summary>
	public BitmapInfoHeaderRGBA( int width, int height, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0, int horizontalPixelPerMeter = 3780, int verticalPixelPerMeter = 3780, CompressionMethod compressionMethod = CompressionMethod.BI_BITFIELDS ) :
		base( width, height, bitsPerPixel, rawImageSize, horizontalPixelPerMeter, verticalPixelPerMeter, compressionMethod ) {
		//this.Width = width;
		//this.Height = height;

		//this.BitsPerPixel = bitsPerPixel;
		//this.CompressionMethod = (uint) CompressionMethod.BI_RGB;
		//this.ImageSize = rawImageSize;
		//this.HorizontalPixelPerMeter = horizontalPixelPerMeter; // 96 DPI
		//this.VerticalPixelPerMeter = verticalPixelPerMeter;   // 96 DPI
	}

	//public static int SizeInBytes => System.Runtime.InteropServices.Marshal.SizeOf(typeof(BitmapInfoHeader));
	public static int SizeInBytes => 56;

	/// <summary>
	/// This is BitmapInfoHeader for ARGB32 as described here https://en.wikipedia.org/wiki/BMP_file_format#Example_2
	/// </summary>
	public new byte[] HeaderInfoBytes //=> BinarySerializationExtensions.Serialize<BitmapInfoHeaderRGBA>( this );
	{
		get {
			var byteArray = new byte[BitmapInfoHeaderRGBA.SizeInBytes]; // 56

			// get base array
			base.HeaderInfoBytes.CopyTo( byteArray, 0 );


			// chage header size
			var size = BitConverter.GetBytes( BitmapInfoHeaderRGBA.SizeInBytes );

			var redChannelBitMaskBytes = BitConverter.GetBytes( RedChannelBitMask );
			var greenChannelBitMaskBytes = BitConverter.GetBytes( GreenChannelBitMask );
			var blueChannelBitMaskBytes = BitConverter.GetBytes( BlueChannelBitMask );
			var alphaChannelBitMaskBytes = BitConverter.GetBytes( AlphaChannelBitMask );

			// BMP byte order is little endian so we have to take care on byte ordering
			if (!System.BitConverter.IsLittleEndian) {
				// we are on BigEndian system so we have to revers byte order
				Array.Reverse( size );
				Array.Reverse( redChannelBitMaskBytes );
				Array.Reverse( greenChannelBitMaskBytes );
				Array.Reverse( blueChannelBitMaskBytes );
				Array.Reverse( alphaChannelBitMaskBytes );
			}

			size.CopyTo( byteArray, 0 ); //	0, 4 bytes 	The headerSize of the BMP file in bytes
			redChannelBitMaskBytes.CopyTo( byteArray, 0x28 ); //	0, 4 bytes 	The headerSize of the BMP file in bytes
			greenChannelBitMaskBytes.CopyTo( byteArray, 0x2C ); //	0, 4 bytes 	The headerSize of the BMP file in bytes
			blueChannelBitMaskBytes.CopyTo( byteArray, 0x30 ); //	0, 4 bytes 	The headerSize of the BMP file in bytes
			alphaChannelBitMaskBytes.CopyTo( byteArray, 0x34 ); //	0, 4 bytes 	The headerSize of the BMP file in bytes

			return byteArray;
		}
	}

	public new static BitmapInfoHeaderRGBA GetHeaderFromBytes( byte[] bytes ) {
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

		var infoHeader = new BitmapInfoHeaderRGBA(
			width, height, bitsPerPixel, rawImageSize: 0,
			horizontalPixelPerMeter: horizontalPixelPerMeter,
			verticalPixelPerMeter: verticalPixelPerMeter
		);
		return infoHeader;
	}
}
