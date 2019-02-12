using System;
using System.IO;

namespace BmpSharp {
	public class BitmapFileReader {
		public static Bitmap ReadFileAsBitmap( string fileName ) {
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
					var infoHeader = header.infoHeader;
					var bitmap = new Bitmap(
						infoHeader.width, infoHeader.height,
						pixelData: bReader.ReadBytes( infoHeader.width * infoHeader.height * ( infoHeader.bitsPerPixel / 8 ) ),
						bitsPerPixel: (BitsPerPixelEnum) infoHeader.bitsPerPixel
						);
					return bitmap;

				}
			}



		}
	}
}
