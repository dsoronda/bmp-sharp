using System;
using System.Runtime.InteropServices;

namespace BmpSharp
{
	public static class BinarySerializationExtensions
	{
		/// <summary>
		/// Convert struct to byte[]
		/// </summary>
		public static byte[] Serialize<T>(T data) where T : struct
		{
			var size = Marshal.SizeOf(data);    // how much bytes we need ?
			var bufferArray = new byte[size];   // init buffer

			var pointer = Marshal.AllocHGlobal(size);    // alocate memory for buffer and get pointer
			Marshal.StructureToPtr(data, pointer, true);    // copy data from struct to alocated memory
			Marshal.Copy(pointer, bufferArray, 0, size);    // copy data from alocated memory to buffer array
			Marshal.FreeHGlobal(pointer);           // free alocated memory
			return bufferArray;                     // return bufferArray
		}

		/// <summary>
		/// Convert byte[] to struct
		/// </summary>
		public static T Deserialize<T>(byte[] array) where T : struct
		{
			var structure = new T();

			var size = Marshal.SizeOf(structure);   // how much bytes we need ?
			var pointer = Marshal.AllocHGlobal(size);    // mem alloc.

			Marshal.Copy(array, 0, pointer, size);      // copy bytes to alloc. mem

			structure = (T)Marshal.PtrToStructure(pointer, structure.GetType());    // conver aloc. mem to structure
			Marshal.FreeHGlobal(pointer);   // free memory

			return structure;   // return new structure
		}

		public static Span<int> ToInt( this byte[] bytes ) => MemoryMarshal.Cast<byte, int>( bytes );
	}
}
