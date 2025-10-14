using System;
using System.IO;

namespace BmpSharp;

public static class BitmapFileHelper {
	public static Bitmap ReadFileAsBitmap( string fileName, bool flipRows = false ) {
		if (string.IsNullOrWhiteSpace( fileName ))
			throw new ArgumentNullException( nameof(fileName) );
		if (!File.Exists( fileName ))
			throw new Exception( $"File {fileName} not found" );

		var fileInfo = new FileInfo( fileName );
		if (fileInfo.Length <= BitmapFileHeader.BitmapFileHeaderSizeInBytes)
			throw new Exception( $"Invalid file format. Size is too small." );

		using (var fileStream = File.OpenRead( fileName )) {
			using (var bReader = new SafeBinaryReader( fileStream )) {
				var headerBytes = bReader.ReadBytes( BitmapFileHeader.BitmapFileHeaderSizeInBytes );
				var fileHeader = BitmapFileHeader.GetHeaderFromBytes( headerBytes );

				if (fileInfo.Length != fileHeader.FileSize)
					throw new Exception( $"File size is different than in header." );

				var dibHeaderSize = bReader.ReadInt32();
				fileStream.Seek( -4, SeekOrigin.Current );

				var infoHeader = BitmapInfoHeader.GetHeaderFromBytes( bReader.ReadBytes( dibHeaderSize ) );

				var width = infoHeader.Width;
				var height = infoHeader.Height;

				var bytesPerRow = Bitmap.RequiredBytesPerRow( infoHeader.Width, infoHeader.BitsPerPixel );

				var bytesPerPixel = (int) infoHeader.BitsPerPixel / 8;
				var paddingRequired = Bitmap.IsPaddingRequired( infoHeader.Width, infoHeader.BitsPerPixel,
					bytesPerRow );
				var pixelData = new byte[width * height * bytesPerPixel];
				// seek to location where pixel data is
				fileStream.Seek( fileHeader.PixelDataOffset, SeekOrigin.Begin );

				if (paddingRequired) {
					var bytesToCopy = width * bytesPerPixel;
					for (var counter = 0; counter < height; counter++) {
						var rowBuffer = bReader.ReadBytes( bytesPerRow );
						Buffer.BlockCopy( src: rowBuffer, srcOffset: 0, dst: pixelData, dstOffset: counter * bytesToCopy, count: bytesToCopy );
					}
				} else {
					var rowBuffer = bReader.ReadBytes( pixelData.Length );
					rowBuffer.CopyTo( pixelData, 0 );
				}

				var bitmap = new Bitmap(
					width, height,
					pixelData: pixelData,
					bitsPerPixel: infoHeader.BitsPerPixel
				);
				return bitmap;
			}
		}
	}

	public static void SaveBitmapToFile( string fileName, Bitmap bitmap ) {
		if (bitmap == null)
			throw new ArgumentNullException( nameof(bitmap) );
		if (string.IsNullOrWhiteSpace( fileName ))
			throw new ArgumentNullException( nameof(fileName) );
		var filePath = Path.GetDirectoryName( fileName );
		if (!Directory.Exists( filePath ))
			throw new Exception( $"Destination directory not found." );

		using (var fileStream = File.Create( fileName )) {
			using (var bmpStream = bitmap.GetBmpStream()) {
				bmpStream.CopyTo( fileStream, bufferSize: 16 * 1024 );
			}
		}
	}
}
