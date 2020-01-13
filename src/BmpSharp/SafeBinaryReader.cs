using System;
using System.IO;

namespace BmpSharp {
	public class SafeBinaryReader : BinaryReader {
		public SafeBinaryReader( System.IO.Stream stream ) : base( stream ) { }

		public override short ReadInt16() {
			var data = base.ReadBytes( 2 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt16( data, 0 );
		}

		public override ushort ReadUInt16() => (ushort) ReadInt16();

		public override int ReadInt32() {
			var data = base.ReadBytes( 4 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt32( data, 0 );
		}

		public override uint ReadUInt32() => (uint) ReadUInt32();

		public override long ReadInt64() {
			var data = base.ReadBytes( 8 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt64( data, 0 );
		}

		public override ulong ReadUInt64() => (ulong) ReadInt64();
	}
}
