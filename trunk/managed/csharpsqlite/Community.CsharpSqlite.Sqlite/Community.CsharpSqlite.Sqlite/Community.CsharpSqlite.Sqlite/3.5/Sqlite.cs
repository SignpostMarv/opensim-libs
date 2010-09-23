//
// Community.CsharpSqlite.Sqlite.Sqlite.cs
//
// Provides C# bindings to the library sqlite.dll
//
//            	Everaldo Canuto  <everaldo_canuto@yahoo.com.br>
//			Chris Turchin <chris@turchin.net>
//			Jeroen Zwartepoorte <jeroen@xs4all.nl>
//			Thomas Zoechling <thomas.zoechling@gmx.at>
//
// Copyright (C) 2004  Everaldo Canuto
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//
#if !NET_2_0
using System;
using System.Security;
using System.Runtime.InteropServices;
using System.Text;

namespace Community.CsharpSqlite.Sqlite
{
	/// <summary>
	/// Represents the return values for sqlite_exec() and sqlite_step()
	/// </summary>
	internal enum SqliteError : int {
		/// <value>Successful result</value>
		OK        =  0,
		/// <value>SQL error or missing database</value>
		ERROR     =  1,
		/// <value>An internal logic error in SQLite</value>
		INTERNAL  =  2,
		/// <value>Access permission denied</value>
		PERM      =  3,
		/// <value>Callback routine requested an abort</value>
		ABORT     =  4,
		/// <value>The database file is locked</value>
		BUSY      =  5,
		/// <value>A table in the database is locked</value>
		LOCKED    =  6,
		/// <value>A malloc() failed</value>
		NOMEM     =  7,
		/// <value>Attempt to write a readonly database</value>
		READONLY  =  8,
		/// <value>Operation terminated by public const int interrupt()</value>
		INTERRUPT =  9,
		/// <value>Some kind of disk I/O error occurred</value>
		IOERR     = 10,
		/// <value>The database disk image is malformed</value>
		CORRUPT   = 11,
		/// <value>(Internal Only) Table or record not found</value>
		NOTFOUND  = 12,
		/// <value>Insertion failed because database is full</value>
		FULL      = 13,
		/// <value>Unable to open the database file</value>
		CANTOPEN  = 14,
		/// <value>Database lock protocol error</value>
		PROTOCOL  = 15,
		/// <value>(Internal Only) Database table is empty</value>
		EMPTY     = 16,
		/// <value>The database schema changed</value>
		SCHEMA    = 17,
		/// <value>Too much data for one row of a table</value>
		TOOBIG    = 18,
		/// <value>Abort due to contraint violation</value>
		CONSTRAINT= 19,
		/// <value>Data type mismatch</value>
		MISMATCH  = 20,
		/// <value>Library used incorrectly</value>
		MISUSE    = 21,
		/// <value>Uses OS features not supported on host</value>
		NOLFS     = 22,
		/// <value>Authorization denied</value>
		AUTH      = 23,
		/// <value>Auxiliary database format error</value>
		FORMAT    = 24,
		/// <value>2nd parameter to sqlite_bind out of range</value>
		RANGE     = 25,
		/// <value>File opened that is not a database file</value>
		NOTADB    = 26,
		/// <value>sqlite_step() has another row ready</value>
		ROW       = 100,
		/// <value>sqlite_step() has finished executing</value>
		DONE      = 101
	}

	/// <summary>
	/// Provides the core of C# bindings to the library sqlite.dll
	/// </summary>
	internal sealed class SqliteUtil {

		// These are adapted from Mono.Unix.  When encoding is null,
		// use Ansi encoding, which is a superset of the default
		// expected encoding (ISO-8859-1).

		public static IntPtr StringToHeap (string s, Encoding encoding)
		{
			if (encoding == null)
				return Marshal.StringToHGlobalAnsi (s);
				
			int min_byte_count = encoding.GetMaxByteCount(1);
			char[] copy = s.ToCharArray ();
			byte[] marshal = new byte [encoding.GetByteCount (copy) + min_byte_count];

			int bytes_copied = encoding.GetBytes (copy, 0, copy.Length, marshal, 0);

			if (bytes_copied != (marshal.Length-min_byte_count))
				throw new NotSupportedException ("encoding.GetBytes() doesn't equal encoding.GetByteCount()!");

			IntPtr mem = Marshal.AllocHGlobal (marshal.Length);
			if (mem == IntPtr.Zero)
				throw new OutOfMemoryException ();

			bool copied = false;
			try {
				Marshal.Copy (marshal, 0, mem, marshal.Length);
				copied = true;
			}
			finally {
				if (!copied)
					Marshal.FreeHGlobal (mem);
			}

			return mem;
		}

		public static unsafe string HeapToString (IntPtr p, Encoding encoding)
		{
			if (encoding == null)
				return Marshal.PtrToStringAnsi (p);
		
			if (p == IntPtr.Zero)
				return null;
				
			// This assumes a single byte terminates the string.

			int len = 0;
			while (Marshal.ReadByte (p, len) != 0)
				checked {++len;}

			string s = new string ((sbyte*) p, 0, len, encoding);
			len = s.Length;
			while (len > 0 && s [len-1] == 0)
				--len;
			if (len == s.Length) 
				return s;
			return s.Substring (0, len);
		}



	}
}
#endif
