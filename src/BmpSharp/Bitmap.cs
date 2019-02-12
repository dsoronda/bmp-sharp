using System;
using System.Collections.Generic;
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

	public class Bitmap {
		public int Width { get; }
		public int Height { get; }
		public BitsPerPixelEnum BitsPerPixelEnum { get; }
		public int BytesPerPixel => (int) BitsPerPixelEnum / 8;
		public byte[] PixelData { get; }

		/// <summary>
		/// Get reversed order or rows.
		/// For Bitmap image, pixel rows are stored from bottom to top.
		/// So first row in bitmap file is lowest row in Image.
		/// </summary>
		/// <returns>Pixel data with reversed (fliped) rows</returns>
		private byte[] PixelDataFliped {
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
		public BitmapHeader Header { get; }

		public Bitmap( int width, int height, byte[] pixelData, BitsPerPixelEnum bitsPerPixel = BitsPerPixelEnum.RGB24 ) {
			this.Width = width;
			this.Height = height;
			this.PixelData = pixelData ?? throw new ArgumentNullException( nameof( pixelData ) );
			this.BitsPerPixelEnum = bitsPerPixel;
			this.Header = new BitmapHeader( width, height, bitsPerPixel, (uint) pixelData.Length );
		}

		/// <summary>
		/// Get bitmap as bytes for saving to file
		/// </summary>
		/// <param name="flipped">Flip (reverse order of) rows. Bitmap pixel rows are stored from bottom to up as shown in image</param>
		/// <returns></returns>
		public byte[] GetBytes( bool flipped = false ) {
			var buffer = new byte[BitmapHeader.BitmapHeaderSizeInBytes + PixelData.Length];
			Buffer.BlockCopy( this.Header.HeaderBytes, 0, buffer, 0, BitmapHeader.BitmapHeaderSizeInBytes );

			if (flipped) {
				Buffer.BlockCopy( this.PixelDataFliped, 0, buffer, BitmapHeader.BitmapHeaderSizeInBytes, PixelData.Length );
			} else {
				Buffer.BlockCopy( this.PixelData, 0, buffer, BitmapHeader.BitmapHeaderSizeInBytes, PixelData.Length );
			}
			return buffer;
		}

		public System.IO.Stream GetStream( bool fliped = false ) {
			var stream = new System.IO.MemoryStream( BitmapHeader.BitmapHeaderSizeInBytes + PixelData.Length );
			var bytes = GetBytes( fliped );
			stream.Write( bytes, 0, bytes.Length );
			stream.Position = 0;
			return stream;
		}
	}
}
