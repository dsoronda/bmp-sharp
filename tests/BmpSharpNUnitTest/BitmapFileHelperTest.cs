using System.IO;
using BmpSharp;
using NUnit.Framework;

namespace Tests {
	public class BitmapFileHelperTest {
		public static readonly string TestFileName = "rgb_ycm_colors.bmp";

		public static readonly string ImagesFolder = @"..\..\..\..\..\images\";

		private string TestImageFullPath => Path.Combine( Path.GetRelativePath( ".", ImagesFolder ), TestFileName );

		[SetUp]
		public void Setup() {
		}

		[Test]
		public void TestFileExists_Success() => Assert.IsTrue( File.Exists( TestImageFullPath ), $"Missing file {TestImageFullPath}" );

		[Test]
		public void ReadFileAsBitmap_Success() {
			Bitmap bitmap = BitmapFileHelper.ReadFileAsBitmap( TestImageFullPath );
			Assert.NotNull( bitmap );

			Assert.AreEqual( 10, bitmap.Width );
			Assert.AreEqual( 2, bitmap.Height );
			Assert.AreEqual( 3, bitmap.BytesPerPixel );
			Assert.AreEqual( 10 * 2 * 3, bitmap.PixelData.Length );
			Assert.AreEqual( BitsPerPixelEnum.RGB24, bitmap.BitsPerPixelEnum );


			//Assert.Fail("not implemented");
		}

		[Test]
		public void WriteFileAsBitmap_Success() {
			Bitmap bitmap = BitmapFileHelper.ReadFileAsBitmap( TestImageFullPath );
			var tempFolder = System.IO.Path.GetTempPath();
			var targetFile = Path.Combine( tempFolder, TestFileName );
			BitmapFileHelper.SaveBitmapToFile( targetFile, bitmap );

			Assert.IsTrue( File.Exists( targetFile ) );
			var fileInfo = new FileInfo( targetFile );
			Assert.AreEqual( bitmap.GenerateBmpBytes().Length, fileInfo.Length );
		}
	}
}
