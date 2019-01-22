using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace BmpSharp {
	public enum BitsPerPixelEnum : int {
		Monochrome = 1,
		Four = 4,
		Eight = 8,
		RBG16 = 16,
		RGB24 = 24,
		RGBA32 = 32
	}

	public class Bitmap {
		public int Width { get; }
		public int Height { get; }
		public BitsPerPixelEnum BitsPerPixelEnum { get; }
		public byte[] PixelData { get; }
		public BitmapHeader Header { get; }

		public Bitmap( int width, int height, byte[] pixelData, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24 ) {
			this.Width = width;
			this.Height = height;
			this.PixelData = pixelData ?? throw new ArgumentNullException( nameof( pixelData ) );
			this.BitsPerPixelEnum = bitsPerPixel;
			this.Header = new BitmapHeader( width, height, bitsPerPixel, (uint) pixelData.Length );
		}

		public byte[] GetBytes() {
			var b = new byte[BitmapHeader.BitmapHeaderSizeInBytes + PixelData.Length];
			Buffer.BlockCopy( this.Header.HeaderBytes, 0, b, 0, BitmapHeader.BitmapHeaderSizeInBytes );
			Buffer.BlockCopy( this.PixelData, 0, b, BitmapHeader.BitmapHeaderSizeInBytes, PixelData.Length );
			return b;
		}

		public System.IO.Stream GetStream() {
			var stream = new System.IO.MemoryStream( BitmapHeader.BitmapHeaderSizeInBytes + PixelData.Length );
			stream.Write( this.Header.HeaderBytes, 0, BitmapHeader.BitmapHeaderSizeInBytes );
			stream.Write( this.PixelData, 0, PixelData.Length );
			stream.Position = 0;
			return stream;
		}

	}

	[StructLayout( LayoutKind.Sequential )]
	public class BitmapHeader {
		/// <summary>
		/// Header size in bytes (fist 14 bytes from start)
		/// </summary>
		public const int BitmapHeaderSizeInBytes = 54; // 14 + 40
		public uint FileSize { get; private set; }

		/// <summary>
		/// The offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found from the beggining of the file
		/// </summary>
		private uint pixelDataOffset;

		private BitmapInfoHeader infoHeader;

		public BitmapHeader( int width = 1, int height = 1, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, uint sizeOfPixelDataInBytes = 0 ) {
			//if (System.BitConverter.IsLittleEndian)

			//fileSize = (uint)(width * height * (int)bitsPerPixel) / 8;
			FileSize = BitmapHeaderSizeInBytes + sizeOfPixelDataInBytes;

			pixelDataOffset = BitmapHeaderSizeInBytes + (uint) BitmapInfoHeader.SizeInBytes;

			infoHeader = new BitmapInfoHeader( width, height, bitsPerPixel: bitsPerPixel );
		}

		public byte[] HeaderBytes {
			get {
				var byteArray = new byte[BitmapHeaderSizeInBytes];//{ 0x42, 0x4d }
				byteArray[0] = 0x42;
				byteArray[1] = 0x4D;
				var sizeBytes = BitConverter.GetBytes( this.FileSize );
				var offset = BitConverter.GetBytes( BitmapHeaderSizeInBytes );

				// BMP byte order is little endian so i have to take care on byte ordering
				if (!System.BitConverter.IsLittleEndian) {
					Array.Reverse( sizeBytes );
					Array.Reverse( offset );
				}
				// everything is ok
				sizeBytes.CopyTo( byteArray, 2 );//02 	2 	4 bytes 	The size of the BMP file in bytes
				offset.CopyTo( byteArray, 10 );//0A 	10 	4 bytes 	The offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found.


				var infoHeaderBytes = this.infoHeader.HeaderInfoBytes;
				Buffer.BlockCopy( infoHeaderBytes, 0, byteArray, 14, infoHeaderBytes.Length );

				// TODO : handle little endian for infoHeaderBytes
				return byteArray;
			}
		}

		public static BitmapHeader GetHeaderFromBytes( byte[] headerBytes ) {
			if (headerBytes == null)
				throw new ArgumentNullException( nameof( headerBytes ) );
			if (headerBytes.Length != BitmapHeader.BitmapHeaderSizeInBytes)
				throw new ArgumentOutOfRangeException( $"{nameof( headerBytes )} should be {BitmapHeader.BitmapHeaderSizeInBytes} bytes in size" );

			//var width = BitConverter.ToUInt32(headerBytes, 2);

			if (!System.BitConverter.IsLittleEndian) {
				Array.Reverse( headerBytes, 2, 4 );
				Array.Reverse( headerBytes, 10, 4 );
			}

			var sizeBytes = BitConverter.ToUInt32( headerBytes, 2 );
			var offset = BitConverter.ToUInt32( headerBytes, 10 );

			var header = new BitmapHeader() {
				pixelDataOffset = offset,
				FileSize = sizeBytes
			};

			var infoHeader = BinarySerializationExtensions.Deserialize<BitmapInfoHeader>( headerBytes.Skip( 14 ).Take( 40 ).ToArray() );
			header.infoHeader = infoHeader;

			return header;
		}
	}

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
