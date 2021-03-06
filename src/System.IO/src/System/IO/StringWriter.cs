// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using System.Globalization;
using System.Threading.Tasks;

namespace System.IO
{
    // This class implements a text writer that writes to a string buffer and allows
    // the resulting sequence of characters to be presented as a string.
    //
    public class StringWriter : TextWriter
    {
        private static volatile UnicodeEncoding s_encoding = null;

        private StringBuilder _sb;
        private bool _isOpen;

        // Constructs a new StringWriter. A new StringBuilder is automatically
        // created and associated with the new StringWriter.
        public StringWriter()
            : this(new StringBuilder(), CultureInfo.CurrentCulture)
        {
        }

        public StringWriter(IFormatProvider formatProvider)
            : this(new StringBuilder(), formatProvider)
        {
        }

        // Constructs a new StringWriter that writes to the given StringBuilder.
        // 
        public StringWriter(StringBuilder sb) : this(sb, CultureInfo.CurrentCulture)
        {
        }

        public StringWriter(StringBuilder sb, IFormatProvider formatProvider) : base(formatProvider)
        {
            if (sb == null)
            {
                throw new ArgumentNullException("sb", SR.ArgumentNull_Buffer);
            }

            _sb = sb;
            _isOpen = true;
        }

        protected override void Dispose(bool disposing)
        {
            // Do not destroy _sb, so that we can extract this after we are
            // done writing (similar to MemoryStream's GetBuffer & ToArray methods)
            _isOpen = false;
            base.Dispose(disposing);
        }


        public override Encoding Encoding
        {
            get
            {
                if (s_encoding == null)
                {
                    s_encoding = new UnicodeEncoding(false, false);
                }
                return s_encoding;
            }
        }

        // Returns the underlying StringBuilder. This is either the StringBuilder
        // that was passed to the constructor, or the StringBuilder that was
        // automatically created.
        //
        public virtual StringBuilder GetStringBuilder()
        {
            return _sb;
        }

        // Writes a character to the underlying string buffer.
        //
        public override void Write(char value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            _sb.Append(value);
        }

        // Writes a range of a character array to the underlying string buffer.
        // This method will write count characters of data into this
        // StringWriter from the buffer character array starting at position
        // index.
        //
        public override void Write(char[] buffer, int index, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer", SR.ArgumentNull_Buffer);
            }
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("index", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (count < 0)
            {
                throw new ArgumentOutOfRangeException("count", SR.ArgumentOutOfRange_NeedNonNegNum);
            }
            if (buffer.Length - index < count)
            {
                throw new ArgumentException(SR.Argument_InvalidOffLen);
            }
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            _sb.Append(buffer, index, count);
        }

        // Writes a string to the underlying string buffer. If the given string is
        // null, nothing is written.
        //
        public override void Write(string value)
        {
            if (!_isOpen)
            {
                throw new ObjectDisposedException(null, SR.ObjectDisposed_WriterClosed);
            }

            if (value != null)
            {
                _sb.Append(value);
            }
        }


        #region Task based Async APIs
#pragma warning disable 1998 // async method with no await
        private async Task MakeCompletedTask()
        {
            // do nothing.  We're taking advantage of the async infrastructure's optimizations, one of which is to
            // return a cached already-completed Task when possible.
        }
#pragma warning restore 1998

        public override Task WriteAsync(char value)
        {
            Write(value);
            return MakeCompletedTask();
        }

        public override Task WriteAsync(string value)
        {
            Write(value);
            return MakeCompletedTask();
        }

        public override Task WriteAsync(char[] buffer, int index, int count)
        {
            Write(buffer, index, count);
            return MakeCompletedTask();
        }

        public override Task WriteLineAsync(char value)
        {
            WriteLine(value);
            return MakeCompletedTask();
        }

        public override Task WriteLineAsync(string value)
        {
            WriteLine(value);
            return MakeCompletedTask();
        }

        public override Task WriteLineAsync(char[] buffer, int index, int count)
        {
            WriteLine(buffer, index, count);
            return MakeCompletedTask();
        }

        public override Task FlushAsync()
        {
            return MakeCompletedTask();
        }
        #endregion

        // Returns a string containing the characters written to this TextWriter
        // so far.
        //
        public override string ToString()
        {
            return _sb.ToString();
        }
    }
}
