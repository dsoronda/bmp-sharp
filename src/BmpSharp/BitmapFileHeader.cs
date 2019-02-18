using System;
using System.Runtime.InteropServices;

namespace BmpSharp {
	[StructLayout( LayoutKind.Sequential, Pack = 1 )]
	public class BitmapFileHeader {
		/// <summary>
		/// FileHeader headerSize in bytes (fist 14 bytes from start)
		/// </summary>
		public const int BitmapFileHeaderSizeInBytes = 14; // 14 + 40 // THIS IS NOT TRUE

		public const byte ByteZero = 0x42;
		public const byte ByteOne = 0x4D;

		/// <summary>
		/// Explicitly set file in size
		/// </summary>
		/// <param name="fileSize"></param>
		public BitmapFileHeader( uint fileSize ) {
			this.FileSize = fileSize;
		}

		public uint FileSize { get; private set; }

		/// <summary>
		/// The offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found from the beggining of the file
		/// </summary>
		public uint pixelDataOffset;

		/// <summary>
		/// Create header and calculate file size depending on input data
		/// </summary>
		/// <param name="width"></param>
		/// <param name="height"></param>
		/// <param name="bitsPerPixel"></param>
		/// <param name="rawImageSize">Depends on row padding and number of rows</param>
		public BitmapFileHeader( int width = 1, int height = 1, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24, int rawImageSize = 0 ) {
			//if (System.BitConverter.IsLittleEndian)
			var infoHeaderSize = bitsPerPixel == BitsPerPixelEnum.RGB24 ? BitmapInfoHeader.SizeInBytes : BitmapInfoHeaderRGBA.SizeInBytes;

			FileSize = (uint) ( BitmapFileHeader.BitmapFileHeaderSizeInBytes + infoHeaderSize + rawImageSize );

			pixelDataOffset = (uint) ( BitmapFileHeader.BitmapFileHeaderSizeInBytes + infoHeaderSize );

			//infoHeader = new BitmapInfoHeader( Width, Height, BitsPerPixel: BitsPerPixel, rawImageSize: rawImageSize );
		}

		public byte[] HeaderBytes {
			get {
				var byteArray = new byte[BitmapFileHeader.BitmapFileHeaderSizeInBytes]; // 14
																						//{ 0x42, 0x4d } BM string
				byteArray[0] = ByteZero; // B
				byteArray[1] = ByteOne;  // M
				byte[] sizeBytes = BitConverter.GetBytes( this.FileSize );
				byte[] offset = BitConverter.GetBytes( this.pixelDataOffset );

				// BMP byte order is little endian so we have to take care on byte ordering
				if (!System.BitConverter.IsLittleEndian) {
					// we are on BigEndian system so we have to revers byte order
					Array.Reverse( sizeBytes );
					Array.Reverse( offset );
				}

				// everything is ok
				sizeBytes.CopyTo( byteArray, 2 );//02 	2 	4 bytes 	The headerSize of the BMP file in bytes
				offset.CopyTo( byteArray, 10 );//0A 	10 	4 bytes 	The offset, i.e. starting address, of the byte where the bitmap image data (pixel array) can be found.

				return byteArray;
			}
		}

		public static BitmapFileHeader GetHeaderFromBytes( byte[] headerBytes ) {
			if (headerBytes == null)
				throw new ArgumentNullException( nameof( headerBytes ) );
			if (headerBytes.Length != BitmapFileHeader.BitmapFileHeaderSizeInBytes)
				throw new ArgumentOutOfRangeException( $"{nameof( headerBytes )} should be {BitmapFileHeader.BitmapFileHeaderSizeInBytes} bytes in headerSize" );

			//var Width = BitConverter.ToUInt32(headerBytes, 2);

			if (!System.BitConverter.IsLittleEndian) {
				Array.Reverse( headerBytes, 2, 4 );
				Array.Reverse( headerBytes, 10, 4 );
			}

			var sizeBytes = BitConverter.ToUInt32( headerBytes, 2 );
			var offset = BitConverter.ToUInt32( headerBytes, 10 );

			var header = new BitmapFileHeader() {
				pixelDataOffset = offset,
				FileSize = sizeBytes
			};

			return header;
		}
	}


}
