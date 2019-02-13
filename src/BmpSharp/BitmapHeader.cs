using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public class BitmapHeader {
		/// <summary>
		/// Header headerSize in bytes (fist 14 bytes from start)
		/// </summary>
		public const int BitmapHeaderSizeInBytes = 54; // 14 + 40
		public const byte ByteZero = 0x42;
		public const byte ByteOne = 0x4D;
		public uint FileSize { get; private set; }

		/// <summary>
		/// The offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found from the beggining of the file
		/// </summary>
		private uint pixelDataOffset;

		public BitmapInfoHeader infoHeader;

		public BitmapHeader( int width = 1, int height = 1, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0 ) {
			//if (System.BitConverter.IsLittleEndian)

			//fileSize = (uint)(width * height * (int)bitsPerPixel) / 8;
			FileSize = (uint) ( BitmapHeaderSizeInBytes + rawImageSize );

			pixelDataOffset = BitmapHeaderSizeInBytes + (uint) BitmapInfoHeader.SizeInBytes;

			infoHeader = new BitmapInfoHeader( width, height, bitsPerPixel: bitsPerPixel, rawImageSize: rawImageSize );
		}

		public byte[] HeaderBytes {
			get {
				var byteArray = new byte[BitmapHeaderSizeInBytes];//{ 0x42, 0x4d }
				byteArray[0] = ByteZero;
				byteArray[1] = ByteOne;
				var sizeBytes = BitConverter.GetBytes( this.FileSize );
				var offset = BitConverter.GetBytes( BitmapHeaderSizeInBytes );

				// BMP byte order is little endian so we have to take care on byte ordering
				if (!System.BitConverter.IsLittleEndian) {
					// we are on BigEndian system so we have to revers byte order
					Array.Reverse( sizeBytes );
					Array.Reverse( offset );
				}
				// everything is ok
				sizeBytes.CopyTo( byteArray, 2 );//02 	2 	4 bytes 	The headerSize of the BMP file in bytes
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
				throw new ArgumentOutOfRangeException( $"{nameof( headerBytes )} should be {BitmapHeader.BitmapHeaderSizeInBytes} bytes in headerSize" );

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


}
