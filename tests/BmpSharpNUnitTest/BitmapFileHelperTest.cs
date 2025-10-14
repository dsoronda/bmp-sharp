using System;
using System.IO;
using BmpSharp;
using NUnit.Framework;
using Shouldly;

namespace BmpSharpNUnitTest;

public class BitmapFileHelperTest {
	private static readonly string TestFileName_RGB = "rgb_ycm_colors.bmp";
	private static readonly string RGBA_TestFileName = "ARGB_colors.bmp";


	private static readonly string ImagesFolder = @"../../../../../images";

	private string TestImageFullPath => Path.Combine( Path.GetRelativePath( ".", ImagesFolder ), TestFileName_RGB );

	private string RGBA_TestImageFullPath => Path.Combine( Path.GetRelativePath( ".", ImagesFolder ), RGBA_TestFileName );

	[SetUp] public void Setup() { }

	[Test] public void TestFileExists_Success() => File.Exists( TestImageFullPath ).ShouldBeTrue( $"Missing file {TestImageFullPath}" );

	[Test] public void ReadFileAsBitmap_Success() {
		var bitmap = BitmapFileHelper.ReadFileAsBitmap( TestImageFullPath );
		bitmap.ShouldNotBeNull();

		bitmap.Width.ShouldBe( 10);
		bitmap.Height.ShouldBe( 2 );
		bitmap.BytesPerPixel.ShouldBe(3);
		bitmap.PixelData.Length .ShouldBe(10 * 2 * 3);

		bitmap.BitsPerPixelEnum .ShouldBe(BitsPerPixelEnum.RGB24);
	}

	[Test] public void WriteFileAsBitmap_Success() {
		var bitmap = BitmapFileHelper.ReadFileAsBitmap( TestImageFullPath );
		var tempFolder = System.IO.Path.GetTempPath();
		var targetFile = Path.Combine( tempFolder, TestFileName_RGB );
		BitmapFileHelper.SaveBitmapToFile( targetFile, bitmap );

		File.Exists( targetFile ).ShouldBeTrue($"The file {targetFile} does not exist");
		var fileInfo = new FileInfo( targetFile );
		bitmap.GetBmpBytes().Length.ShouldBeEquivalentTo((int)fileInfo.Length );
	}

	[Test] public void RGBA_ReadFileAsBitmap_Success() {
		var bitmap = BitmapFileHelper.ReadFileAsBitmap( RGBA_TestImageFullPath );
		Assert.NotNull( bitmap );

		Assert.AreEqual( 10, bitmap.Width );
		Assert.AreEqual( 2, bitmap.Height );
		const int bytesPerPixel = (int) BitsPerPixelEnum.RGBA32 / 8;
		Assert.AreEqual( bytesPerPixel, bitmap.BytesPerPixel );
		Assert.AreEqual( 10 * 2 * bytesPerPixel, bitmap.PixelData.Length );
		Assert.AreEqual( BitsPerPixelEnum.RGBA32, bitmap.BitsPerPixelEnum );
	}

	[Test] public void RGBA_WriteFileAsBitmap_Success() {
		var bitmap = BitmapFileHelper.ReadFileAsBitmap( RGBA_TestImageFullPath );
		var tempFolder = System.IO.Path.GetTempPath();
		var targetFile = Path.Combine( tempFolder, RGBA_TestFileName );
		BitmapFileHelper.SaveBitmapToFile( targetFile, bitmap );

		Assert.IsTrue( File.Exists( targetFile ) );
		var fileInfo = new FileInfo( targetFile );
		Assert.AreEqual( bitmap.GetBmpBytes().Length, fileInfo.Length );
	}
}
