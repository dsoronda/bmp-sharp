using System;
using System.IO;

namespace BmpSharp {
	public class SafeBinaryReader : BinaryReader {
		public SafeBinaryReader( System.IO.Stream stream ) : base( stream ) { }

		public Int16 ReadInt16() {
			var data = base.ReadBytes( 2 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt16( data, 0 );
		}

		public ushort ReadUInt16() => (ushort) ReadInt16();

		public override Int32 ReadInt32() {
			var data = base.ReadBytes( 4 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt32( data, 0 );
		}

		public override UInt32 ReadUInt32() => (UInt32) ReadUInt32();

		public Int64 ReadInt64() {
			var data = base.ReadBytes( 8 );
			if (!System.BitConverter.IsLittleEndian)
				Array.Reverse( data );
			return BitConverter.ToInt64( data, 0 );
		}

		public UInt64 ReadUInt64() => (UInt64) ReadInt64();
	}
}
