using System;
using System.IO;

namespace PumaCode.SvnDotNet.AprSharp {
    /// <summary>
    /// The AprStream class can be used to create an in-memory pipe through which
    /// output of various commands can be read. It implements the System.IO.Stream
    /// abstract class, and exposes two AprFile properties, the AprFileInput and
    /// AprFileOutput.
    /// </summary>
    public class AprStream : Stream {

        private IntPtr _aprFileInput = IntPtr.Zero;
        private IntPtr _aprFileOutput = IntPtr.Zero;

        public AprStream(AprPool pool)
        {
            int res = Apr.apr_file_pipe_create(out _aprFileOutput,
                out _aprFileInput, pool);

            if(res != 0)
                throw new AprException(res);

        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {
            AprFileFlush(_aprFileInput);
            AprFileFlush(_aprFileOutput);
        }

        private void AprFileFlush(AprFile aprFile)
        {
            if(aprFile.IsNull)
                throw new ObjectDisposedException("aprFile",
                    "The AprFile object for this stream has been disposed.");

            int res = Apr.apr_file_flush(aprFile);

            if(res != 0)
                throw new AprStreamException(res);


        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if(buffer == null)
                throw new ArgumentNullException("buffer cannot be null.");

            if(offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("Neither offset nor count may be negative.");

            if(offset + count > buffer.Length)
                throw new ArgumentException(String.Format("Attempt to read past end of buffer "
                    + "(Buffer length is {0} but requested read of {1} bytes at offset {2})",
                    buffer.Length, count, offset));

            if(((AprFile) _aprFileOutput).IsNull)
                throw new ObjectDisposedException("aprFileOutput",
                    "The AprFile object for this stream has been disposed.");

            if(Apr.apr_file_eof(_aprFileOutput) != 0)
                return 0;

            int size = count;

            // size is buffer size on entry, actual bytes read on exit
            int res = Apr.apr_file_read(_aprFileOutput, buffer, ref size);

            // apr_file_read() and apr_file_eof will both == APR_EOF at end of stream;
            // any other nonzero return svnStatus is an error
            if(size == 0 && res != 0 && Apr.apr_file_eof(_aprFileOutput) == res)
                return 0;
            else if(res != 0)
                throw new AprStreamException(res);

            return size;

        }

        /// <summary>
        /// A convenience method which closes the AprFileInput object and reads all data
        /// in the stream into a string.
        /// </summary>
        /// <returns>A string containing all data from the current stream location
        /// (usually the beginning) to the end.</returns>
        public string ReadToEnd()
        {
            AprFileInput.Close();
            string contents;

            using(StreamReader reader = new StreamReader(this)) {
                contents = reader.ReadToEnd();
            }

            return contents;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if(buffer == null)
                throw new ArgumentNullException("buffer cannot be null.");

            if(offset < 0 || count < 0)
                throw new ArgumentOutOfRangeException("Neither offset nor count may be negative.");

            if(offset + count > buffer.Length)
                throw new ArgumentException(String.Format("Attempt to write past end of buffer "
                    + "(Buffer length is {0} but requested write of {1} bytes at offset {2})",
                    buffer.Length, count, offset));

            if(((AprFile) _aprFileInput).IsNull)
                throw new ObjectDisposedException("aprFileOutput",
                    "The AprFile object for this stream has been disposed.");

            int size = count;

            // size is buffer size on entry, actual bytes read on exit
            int res = Apr.apr_file_write(_aprFileInput, buffer, ref size);

            if(res != 0)
                throw new AprStreamException(res);

        }

        /// <summary>
        /// An AprFile object which represents the "input" end of the pipe. This
        /// can be passed to any method which expects an AprFile for writing.
        /// </summary>
        public AprFile AprFileInput
        {
            get { return (AprFile) _aprFileInput; }
        }

        /// <summary>
        /// An AprFile object which represents the "output" end of the pipe. This
        /// </summary>
        public AprFile AprFileOutput
        {
            get { return (AprFile) _aprFileOutput; }
        }

        #region Unsupported System.IO.Stream Members
        // The System.IO.Stream abstract class allows implemtations to not support
        // methods involved with seeking for streams (such as this one) based on pipes
        // where seeking doesn't make sense.

        public override long Length
        {
            get { throw new NotSupportedException("AprStream does not support seeking."); }
        }

        public override long Position
        {
            get { throw new NotSupportedException("AprStream does not support seeking."); }
            set { throw new NotSupportedException("AprStream does not support seeking."); }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("AprStream does not support seeking.");
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException("AprStream does not support seeking.");
        }

        #endregion

    }

    public class AprStreamException : IOException {

        public AprStreamException(int res)
            : base("An I/O exception has occurred; see InnerException for details.",
            new AprException(res))
        {
        }
    }
}
