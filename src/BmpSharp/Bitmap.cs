using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BmpSharp {
	public enum BitsPerPixelEnum : int {
		Monochrome = 1,
		Four = 4,
		Eight = 8,
		RBG16 = 16,
		RGB24 = 24,
		RGBA32 = 32
	}

	public enum CompressionMethod : int {
		BI_RGB = 0, // none
		BI_RLE8 = 1,
		BI_RLE4 = 2,
		BI_BITFIELDS = 3,
		BI_JPEG = 4,
		BI_PNG = 5,
		BI_ALPHABITFIELDS = 6,
		BI_CMYK = 11,
		BI_CMYKRLE8 = 12,
		BI_CMYKRLE4 = 13
	}

	public class Bitmap {
		public int Width { get; } = 0;
		public int Height { get; } = 0;
		public BitsPerPixelEnum BitsPerPixelEnum { get; }

		/// <summary>
		/// BMP file must be aligned at 4 butes at the end of row
		/// </summary>
		/// <param name="BitsPerPixelEnum"></param>
		/// <returns></returns>
		public int BytesPerRow => RequiredBytesPerRow( Width, BitsPerPixelEnum );

		/// <summary>
		/// NOTE: we don't care for images that are less than 24 bits
		/// </summary>
		/// <returns></returns>
		public int BytesPerPixel => (int) BitsPerPixelEnum / 8;
		public byte[] PixelData { get; }

		/// <summary>
		/// Get reversed order or rows.
		/// For Bitmap image, pixel rows are stored from bottom to top.
		/// So first row in bitmap file is lowest row in Image.
		/// </summary>
		/// <returns>Pixel data with reversed (fliped) rows</returns>
		public byte[] PixelDataFliped {
			get {
				var rowListData = new List<byte[]>();
				var totalRows = Height;
				var pixelsInRow = Width;

				for (var row = totalRows - 1; row >= 0; row--) {
					// NOTE: this only works on images that are 8/24/32 bits per pixel
					byte[] one_row = PixelData.Skip( row * Width * BytesPerPixel ).Take( Width * BytesPerPixel ).ToArray();
					rowListData.Add( one_row );
				}
				var reversedBytes = rowListData.SelectMany( row => row ).ToArray();
				return reversedBytes;
			}
		}

		public BitmapFileHeader FileHeader { get; }
		public byte[] InfoHeaderBytes { get; }

		public Bitmap( int width, int height, byte[] pixelData, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24 ) {
			this.Width = width;
			this.Height = height;
			this.PixelData = pixelData ?? throw new ArgumentNullException( nameof( pixelData ) );
			this.BitsPerPixelEnum = bitsPerPixel;
			var rawImageSize = BytesPerRow * height;

			if (bitsPerPixel == BitsPerPixelEnum.RGB24)
				this.InfoHeaderBytes = new BitmapInfoHeader( width, height, bitsPerPixel, rawImageSize ).HeaderInfoBytes;
			if (bitsPerPixel == BitsPerPixelEnum.RGBA32) {
				this.InfoHeaderBytes = new BitmapInfoHeaderRGBA( width, height, bitsPerPixel, rawImageSize ).HeaderInfoBytes;
			}

			this.FileHeader = new BitmapFileHeader( width, height, bitsPerPixel, rawImageSize );
		}

		/// <summary>
		/// Get bitmap as byte aray for saving to file
		/// </summary>
		/// <param name="flipped">Flip (reverse order of) rows. Bitmap pixel rows are stored from bottom to up as shown in image</param>
		/// <returns></returns>
		public byte[] GenerateBmpBytes( bool flipped = false ) {
			//var rawImageSize = BytesPerRow * Height;
			//var buffer = new byte[BitmapFileHeader.BitmapFileHeaderSizeInBytes + rawImageSize];
			//Buffer.BlockCopy( this.FileHeader.HeaderBytes, 0, buffer, 0, BitmapFileHeader.BitmapFileHeaderSizeInBytes );

			//if (flipped) {
			//	Buffer.BlockCopy( this.PixelDataFliped, 0, buffer, BitmapFileHeader.BitmapFileHeaderSizeInBytes, PixelData.Length );
			//} else {
			//	Buffer.BlockCopy( this.PixelData, 0, buffer, BitmapFileHeader.BitmapFileHeaderSizeInBytes, PixelData.Length );
			//}
			//return buffer;

			using (var stream = GetStream( flipped )) {
				return stream.ToArray();
			}
		}

		public MemoryStream GetStream( bool fliped = false ) {
			var rawImageSize = BytesPerRow * Height;

			//var stream = new System.IO.MemoryStream( BitmapFileHeader.BitmapFileHeaderSizeInBytes + (int) rawImageSize );
			var stream = new MemoryStream( rawImageSize );

			//using (var writer = new BinaryWriter( stream )) {
			var writer = new BinaryWriter( stream );
			writer.Write( this.FileHeader.HeaderBytes );
			writer.Write( this.InfoHeaderBytes );
			writer.Flush();
			stream.Flush();

			var paddingRequired = BytesPerRow != ( Width * BytesPerPixel );
			var bytesToCopy = Width * BytesPerPixel;
			var pixData = fliped ? PixelDataFliped : PixelData;

			if (paddingRequired) {
				for (var counter = 0; counter < Height; counter++) {
					var rowBuffer = new byte[this.BytesPerRow];
					Buffer.BlockCopy( src: pixData, srcOffset: counter * bytesToCopy, dst: rowBuffer, dstOffset: 0, count: bytesToCopy );
					writer.Write( rowBuffer );
				}
			} else {
				writer.Write( pixData );
			}

			stream.Position = 0;

			return stream;
		}

		/// <summary>
		/// BMP file must be aligned at 4 butes at the end of row
		/// </summary>
		/// <param name="width">Image Width</param>
		/// <param name="bitsPerPixel">Bits per pixel</param>
		/// <returns>How many bytes BMP requires per row</returns>
		public static int RequiredBytesPerRow( int width, BitsPerPixelEnum bitsPerPixel ) => (int) Math.Ceiling( (decimal) ( width * (int) bitsPerPixel ) / 32 ) * 4;

		/// <summary>
		/// Check if padding is required (extra bytes for a row).
		/// </summary>
		/// <param name="width">Width of image</param>
		/// <param name="bitsPerPixel">Bits per pixels to calculate actual byte requirement</param>
		/// <param name="bytesPerRow">BMP required bytes per row</param>
		/// <returns>True/false if we need to allocate extra bytes (for BMP savign) for padding</returns>
		public static bool IsPaddingRequired( int width, BitsPerPixelEnum bitsPerPixel, int bytesPerRow ) =>
			bytesPerRow != width * (int) bitsPerPixel / 8;

	}
}
