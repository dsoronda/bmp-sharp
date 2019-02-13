using System;
using System.IO;

namespace BmpSharp {
	public class BitmapFileHelper {
		public static Bitmap ReadFileAsBitmap( string fileName, bool flipRows = false ) {
			if (string.IsNullOrWhiteSpace( fileName ))
				throw new ArgumentNullException( nameof( fileName ) );
			if (!File.Exists( fileName ))
				throw new Exception( $"File {fileName} not found" );

			var fileInfo = new FileInfo( fileName );
			if (fileInfo.Length <= BitmapHeader.BitmapHeaderSizeInBytes)
				throw new Exception( $"Invalid file format. Size is too small." );

			using (var fileStream = File.OpenRead( fileName )) {
				using (var bReader = new BinaryReader( fileStream )) {
					var headerBytes = bReader.ReadBytes( BitmapHeader.BitmapHeaderSizeInBytes );
					var header = BitmapHeader.GetHeaderFromBytes( headerBytes );

					if (fileInfo.Length < header.FileSize)
						throw new Exception( $"File headerSize [{fileInfo.Length}] is smaller than expected [{header.FileSize}]." );

					var infoHeader = header.infoHeader;
					var width = infoHeader.width;
					var height = infoHeader.height;


					var bytesPerRow = Bitmap.RequiredBytesPerRow( infoHeader.width, (BitsPerPixelEnum) infoHeader.bitsPerPixel );

					var bytesPerPixel = infoHeader.bitsPerPixel / 8;
					var paddingRequired = IsPaddingRequired( infoHeader.width, (BitsPerPixelEnum) infoHeader.bitsPerPixel,
					bytesPerRow );

					byte[] pixelData = new byte[width * height * bytesPerPixel];

					if (paddingRequired) {
						var bytesToCopy = width * bytesPerPixel;
						for (var counter = 0; counter < height; counter++) {
							var rowBuffer = bReader.ReadBytes( bytesPerRow );
							Buffer.BlockCopy( src: rowBuffer, srcOffset: 0, dst: pixelData, dstOffset: counter * bytesToCopy, count: bytesToCopy );
						}
					}

					var bitmap = new Bitmap(
						width, height,
						pixelData: pixelData,
						bitsPerPixel: (BitsPerPixelEnum) infoHeader.bitsPerPixel
						);
					return bitmap;

				}
			}
		}

		public static bool IsPaddingRequired( int width, BitsPerPixelEnum bitsPerPixel, int bytesPerRow ) {
			//var bytesPerRow = Bitmap.RequiredBytesPerRow( width, bitsPerPixel );
			return bytesPerRow != width * (int) bitsPerPixel / 8;
		}

		public static void SaveBitmapToFile( string fileName, Bitmap bitmap ) {
			if (bitmap == null)
				throw new ArgumentNullException( nameof( bitmap ) );
			if (string.IsNullOrWhiteSpace( fileName ))
				throw new ArgumentNullException( nameof( fileName ) );
			var filePath = Path.GetDirectoryName( fileName );
			if (!Directory.Exists( filePath ))
				throw new Exception( $"Destination directory not found." );

			using (var fileStream = File.Create( fileName )) {
				using (MemoryStream bmpStream = bitmap.GetStream()) {
					  bmpStream.CopyTo( fileStream, bufferSize: 16 * 1024 );
				}
			}
		}
	}
}
