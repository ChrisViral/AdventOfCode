using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdventOfCode.Tools
{
    /// <inheritdoc />
    /// <summary>
    /// Fast Console IO helper
    /// </summary>
    [DebuggerStepThrough]
    public sealed class ConsoleHelper : IDisposable
    {
        #region Constants
        /// <summary>
        /// The standard input and output buffers size (2^20, 1Mb)
        /// </summary>
        private const int baseSize = 1048576;
        /// <summary>
        /// Integer out string conversion buffer
        /// </summary>
        private static readonly char[] numBuffer = new char[20];
        #endregion
 
        #region Fields
        private readonly BufferedStream inStream; //Buffered console input stream
        private readonly StreamWriter outStream;  //Buffered console output stream
 
        private readonly byte[] inBuffer; //Input buffer 
        private int inputIndex; //Input buffer current index
        private int bufferEnd;  //Input buffer ending index
        private bool writeToConsole;
        #endregion
 
        #region Properties
        /// <summary>
        /// The buffer size, in bytes
        /// </summary>
        public int BufferSize { get; }
 
        /// <summary>
        /// If this memory stream is open and available to read/write
        /// </summary>
        public bool Open { get; private set; }
        #endregion
 
        #region Constructors
        /// <summary>
        /// Creates a new console IO helper reading from the standard Console input and output
        /// </summary>
        /// <param name="input">Input file path</param>
        /// <param name="output">Output file path</param>
        /// <param name="writeToConsole">If this helper must also write to the C# Console</param>
        public ConsoleHelper(string input = "input.txt", string output = "output.txt", bool writeToConsole = true) : this(baseSize, input, output, writeToConsole) { }

        /// <summary>
        /// Creates a new console IO helper reading from the standard Console input and output with the specified buffer size
        /// </summary>
        /// <param name="bufferSize">Size of the buffer to use in bytes</param>
        /// <param name="input">Input file path</param>
        /// <param name="output">Output file path</param>
        /// <param name="writeToConsole">If this helper must also write to the C# Console</param>
        public ConsoleHelper(int bufferSize, string input, string output, bool writeToConsole)
        {
            //Input and output streams
            this.inStream = new BufferedStream(File.OpenRead(input), bufferSize);
            this.outStream = new StreamWriter(File.Create(output, bufferSize), Encoding.ASCII, bufferSize);
            this.writeToConsole = writeToConsole;
 
            //Set fields
            this.inBuffer = new byte[bufferSize];
            this.inputIndex = this.bufferEnd = 0;
            this.BufferSize = bufferSize;
            this.Open = true;
        }
        #endregion
 
        #region Static methods
        /// <summary>
        /// Verifies that the passed character is a non-special ASCII character
        /// </summary>
        /// <param name="i">Character to validate</param>
        /// <returns>True if the character is not a special character</returns>
        public static bool ValidateChar(int i) => i >= ' ';
 
        /// <summary>
        /// Verifies that the passed character is a non-special ASCII character or a whitespace
        /// </summary>
        /// <param name="i">Character to validate</param>
        /// <returns>True if the character is not a whitespace or a special character</returns>
        public static bool ValidateCharNoSpace(int i) => i > ' ';
 
        /// <summary>
        /// Verifies that the passed character is a numerical character (0-9)
        /// </summary>
        /// <param name="i">Character to validate</param>
        /// <returns>True if the character is a numerical character, false otherwise</returns>
        public static bool ValidateNumber(int i) => i >= '0' && i <= '9';
 
        /// <summary>
        /// Verifies if a character is an Endline character
        /// </summary>
        /// <param name="i">Character to check</param>
        /// <returns>True if it is an Endline character, false otherwise</returns>
        public static bool IsEndline(int i) => i == '\n' || i == '\0';
 
        /// <summary>
        /// Takes a signed int value and copies it's characters at the end of the integer char buffer
        /// </summary>
        /// <param name="n">Int to write to the buffer</param>
        /// <returns>Head index at which the buffer's writing starts</returns>
        private static int GetIntBuffer(int n)
        {
            int head = 20;
            bool neg;
            if (n < 0)
            {
                neg = true;
                n = -n;
            }
            else { neg = false; }
 
            do
            {
                numBuffer[--head] = (char)((n % 10) + 48);
                n /= 10;
            }
            while (n > 0);
 
            if (neg) { numBuffer[--head] = '-'; }
            return head;
        }
 
        /// <summary>
        /// Takes a signed long value and copies it's characters at the end of the integer char buffer
        /// </summary>
        /// <param name="n">Long to write to the buffer</param>
        /// <returns>Head index at which the buffer's writing starts</returns>
        private static int GetLongBuffer(long n)
        {
            int head = 20;
            bool neg;
            if (n < 0L)
            {
                neg = true;
                n = -n;
            }
            else { neg = false; }
 
            do
            {
                numBuffer[--head] = (char)((n % 10L) + 48L);
                n /= 10L;
            }
            while (n > 0L);
 
            if (neg) { numBuffer[--head] = '-'; }
            return head;
        }
        #endregion
 
        #region Methods
        /// <summary>
        /// Returns the next byte data in the console input stream
        /// </summary>
        /// <returns>Next data byte from the console</returns>
        public byte Read()
        {
            CheckBuffer();
            return this.inBuffer[this.inputIndex++];
        }
 
        /// <summary>
        /// Returns the next byte data in the console input stream without consuming it
        /// </summary>
        /// <returns>Next data byte from the console</returns>
        public byte Peek()
        {
            CheckBuffer();
            return this.inBuffer[this.inputIndex];
        }
 
        /// <summary>
        /// Skips a number of characters in the input stream
        /// </summary>
        /// <param name="n">Amount of chars to skip, defaults to 1</param>
        public void Skip(int n = 1) => this.inputIndex += n;
 
        /// <summary>
        /// Assures we have data available in the input buffer
        /// </summary>
        private void CheckBuffer()
        {
            // If we reach the end of the buffer, load more data
            if (this.inputIndex >= this.bufferEnd)
            {
                this.inputIndex = this.inputIndex - this.bufferEnd;
                this.bufferEnd = this.inStream.Read(this.inBuffer, 0, this.BufferSize);
 
                // If nothing was added, add a null char at the start
                if (this.bufferEnd < 1) { this.inBuffer[this.bufferEnd++] = 0; }
            }
        }

        /// <summary>
        /// Checks if there's more data on the next line of the stream
        /// </summary>
        /// <returns>True if there is more data, false otherwise</returns>
        public bool HasNextLine()
        {
            byte b = Peek();
            while (!ValidateCharNoSpace(b) && !IsEndline(b))
            {
                Skip();
                b = Peek();
            }

            if (b == '\n') { Skip(); }
            return b != 0;
        }
 
        /// <summary>
        /// Returns the next character in the console input stream
        /// </summary>
        /// <returns>Next character in the input stream</returns>
        public char NextChar() => (char)Read();
 
        /// <summary>
        /// Returns the next string token from the console input
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <returns>Parsed string, separated by spaces or special characters such as line feeds</returns>
        public string Next()
        {
            byte b = SkipInvalid();
            ValidateEndline(b);
 
            // Append all characters
            StringBuilder sb = new StringBuilder().Append((char)b);
            b = Peek();
            while (ValidateCharNoSpace(b))
            {
                // Peek to not consume terminator
                sb.Append((char)b);
                Skip();
                b = Peek();
            }
 
            return sb.ToString();
        }
 
        /// <summary>
        /// Returns the next int value in the console input, this is a fast parse
        /// </summary>
        /// <exception cref="FormatException">If the text is not a valid integer</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for integer</exception>
        /// <returns>Parsed int value from the input</returns>
        public int NextInt()
        {
            // Skip invalids
            byte b = SkipInvalid();
            ValidateEndline(b);
 
            // Verify for negative
            bool neg = false;
            if (b == '-')
            {
                neg = true;
                b = Read();
            }
 
            // Get first digit
            if (!ValidateNumber(b)) { throw new FormatException("Integer parsing has failed because the string contained invalid characters"); }
 
            int n = b - '0';
            b = Peek();
            while (ValidateNumber(b))
            {
                // Peek to not consume terminator, and check for overflow
                n = checked((n << 3) + (n << 1) + (b - '0'));
                Skip();
                b = Peek();
            }
            // If the character causing the exit is a valid ASCII character, the integer isn't correct formatted
            if (ValidateCharNoSpace(b)) { throw new FormatException("Integer parsing has failed because the string contained invalid characters"); }
 
            return neg ? -n : n;
        }
 
        /// <summary>
        /// Returns the next long value in the console input, this is a fast parse
        /// </summary>
        /// <exception cref="FormatException">If the text is not a valid long</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for long</exception>
        /// <returns>Parsed long value from the input</returns>
        public long NextLong()
        {
            byte b = SkipInvalid();
            ValidateEndline(b);
 
            // Verify negative
            bool neg = false;
            if (b == '-')
            {
                neg = true;
                b = Read();
            }
 
            // Get first digit
            if (!ValidateNumber(b)) { throw new FormatException("Integer parsing has failed because the string contained invalid characters"); }
 
            long n = b - '0';
            b = Peek();
            while (ValidateNumber(b))
            {
                // Peek to not consume terminator, and check for overflow
                n = checked((n << 3) + (n << 1) + (b - '0'));
                Skip();
                b = Peek();
            }
            // If the character causing the exit is a valid ASCII character, the long isn't correct formatted
            if (ValidateCharNoSpace(b)) { throw new FormatException("Long parsing has failed because the string contained invalid characters"); }
 
            return neg ? -n : n;
        }
 
        /// <summary>
        /// Returns the next double value in the console input
        /// Note: fast double parsing is slightly harder, I'll implement it if I need to
        /// </summary>
        /// <exception cref="FormatException">If the text is not a valid double</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for double</exception>
        /// <returns>Parsed double value from the input</returns>
        public double NextDouble() => double.Parse(Next());
 
        /// <summary>
        /// Returns the next n int values on the same line in an array
        /// </summary>
        /// <param name="n">Number of values to seek</param>
        /// <exception cref="FormatException">If the text is not a valid integer</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for integer</exception>
        /// <exception cref="OutOfMemoryException">If the created array is too large for the system memory</exception>
        /// <returns>The n integer values in an array</returns>
        public int[] NextInts(int n)
        {
            int[] array = new int[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = NextInt();
            }
 
            SkipNextLine();
            return array;
        }
 
        /// <summary>
        /// Returns the next n long values on the same line in an array
        /// </summary>
        /// <param name="n">Number of values to seek</param>
        /// <exception cref="FormatException">If the text is not a valid long</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for long</exception>
        /// <exception cref="OutOfMemoryException">If the created array is too large for the system memory</exception>
        /// <returns>The n long values in an array</returns>
        public long[] NextLongs(int n)
        {
            long[] array = new long[n];
            for (int i = 0; i < n; i++)
            {
                array[i] = NextLong();
            }
 
            SkipNextLine();
            return array;
        }
 
        /// <summary>
        /// Returns the next n int values on the next m lines in the output stream under the form of an NxM matrix
        /// </summary>
        /// <param name="n">Number of rows to the matrix</param>
        /// <param name="m">Number of columns to the matrix</param>
        /// <exception cref="FormatException">If the text is not a valid integer</exception>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <exception cref="OverflowException">If the value is too large for integer</exception>
        /// <exception cref="OutOfMemoryException">If the created 2D array is too large for the system memory</exception>
        /// <returns>The NxM matrix of integers</returns>
        public int[,] NextMatrix(int n, int m)
        {
            int[,] matrix = new int[n, m];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < m; j++)
                {
                    matrix[i, j] = NextInt();
                }
 
                SkipNextLine();
            }
 
            return matrix;
        }
 
        /// <summary>
        /// Returns the next line of text in the console
        /// </summary>
        /// <returns>Next string line from the console</returns>
        public string NextLine()
        {
            byte b = SkipInvalid();
            if (b == 0)
            {
                // Consume newline and return empty string
                Skip();
                return string.Empty;
            }
 
            // Read all the characters until the next linefeed
            StringBuilder sb = new StringBuilder().Append((char)b);
            b = Read();
            while (!IsEndline(b))
            {
                // Don't append special characters, but don't exit
                if (ValidateChar(b))
                {
                    sb.Append((char)b);
                }
                b = Read();
            }
 
            return sb.ToString();
        }
 
        /// <summary>
        /// Skips the next token in input
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        public void SkipNext()
        {
            byte b = SkipInvalid();
            ValidateEndline(b);
 
            for (b = Peek(); ValidateCharNoSpace(b); b = Peek())
            {
                Skip();
            }
        }
 
        /// <summary>
        /// Skips all the text on the current line and jump to the next
        /// </summary>
        public void SkipNextLine()
        {
            for (byte b = Read(); !IsEndline(b); b = Read()) { }
        }
 
        /// <summary>
        /// Enumerates the given number of integers on the current line
        /// </summary>
        /// <param name="n">Number of integers on the current line</param>
        /// <returns>Enumerable of the integers</returns>
        public IEnumerable<int> EnumerateInts(int n)
        {
            while (n-- > 0)
            {
                yield return NextInt();
            }
 
            SkipNextLine();
        }

        /// <summary>
        /// Enumerates the next token on the console line
        /// </summary>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        /// <returns>Enumerable of the characters in the token</returns>
        public IEnumerable<char> EnumerateNext()
        {
            byte b = SkipInvalid();
            ValidateEndline(b);

            for (char c = (char)b; ValidateCharNoSpace(c); c = NextChar())
            {
                yield return c;
            }
        }
 
        /// <summary>
        /// Enumerates all the characters in the next line until the feed is exhausted or an endline character is met
        /// </summary>
        /// <returns>Enumerable of all the characters in the current line</returns>
        public IEnumerable<char> EnumerateLine()
        {
            for (char c = NextChar(); !IsEndline(c); c = NextChar())
            {
                if (ValidateChar(c))
                {
                    yield return c;
                }
            }
        }

        /// <summary>
        /// Return all the lines in the stream as long as there are more
        /// </summary>
        /// <returns>Enumerable of all the lines in the stream</returns>
        public IEnumerable<string> EnumerateLines()
        {
            while (HasNextLine())
            {
                yield return NextLine();
            }
        }
 
        /// <summary>
        /// Assures we are not trying to read more data on the line that there exists
        /// </summary>
        /// <param name="b">Byte data to verify</param>
        /// <exception cref="InvalidOperationException">If there is no more data on the line being read</exception>
        private void ValidateEndline(byte b)
        {
            // If empty char
            if (b == 0)
            {
                // Go back a char and throw
                this.inputIndex--;
                throw new InvalidOperationException("No values left on line");
            }
        }
 
        /// <summary>
        /// Skips all invalid character bytes then returns the first valid byte found, spaces are considered invalid
        /// </summary>
        /// <returns>The next valid character byte, cannot be a whitespace</returns>
        private byte SkipInvalid()
        {
            byte b = Peek();
            if (IsEndline(b)) { return 0; }
 
            while (!ValidateCharNoSpace(b))
            {
                Skip();
                b = Peek();
                // Return empty char if we meet an linefeed or empty char
                if (IsEndline(b)) { return 0; }
            }
 
            return Read();
        }

        /// <summary>
        /// Writes the given char to the console output
        /// </summary>
        /// <param name="c">Character to write</param>
        public void Write(char c)
        {
            this.outStream.Write(c);
            if (this.writeToConsole) { Console.Write(c); }
        }
 
        /// <summary>
        /// Writes the given char buffer to the console output
        /// </summary>
        /// <param name="buffer">Char buffer to write</param>
        public void Write(char[] buffer)
        {
            this.outStream.Write(buffer);
            if (this.writeToConsole) { Console.Write(buffer); }
        }

        /// <summary>
        /// Writes the given string to the console output
        /// </summary>
        /// <param name="s">String to write</param>
        public void Write(string s)
        {
            this.outStream.Write(s);
            if (this.writeToConsole) { Console.Write(s); }
        }
 
        /// <summary>
        /// Writes the given integer to the console output
        /// </summary>
        /// <param name="n">Integer to write</param>
        public void Write(int n)
        {
            int head = GetIntBuffer(n);
            this.outStream.Write(numBuffer, head, 20 - head);
            if (this.writeToConsole) { Console.Write(numBuffer, head, 20 - head); }
        }
 
        /// <summary>
        /// Writes the given long to the console output
        /// </summary>
        /// <param name="n">Long to write</param>
        public void Write(long n)
        {
            int head = GetLongBuffer(n);
            this.outStream.Write(numBuffer, head, 20 - head);
            if (this.writeToConsole) { Console.Write(numBuffer, head, 20 - head); }
        }

        /// <summary>
        /// Writes the given object to the console output using the object's ToString method
        /// </summary>
        /// <param name="o">Object to write</param>
        public void Write(object o) 
        {
            this.outStream.Write(o);
            if (this.writeToConsole) { Console.Write(o); }
        }

        /// <summary>
        /// Writes the contents of the StringBuilder to the console output
        /// </summary>
        /// <param name="sb">StringBuilder to write to the output</param>
        public void Write(StringBuilder sb) => Write(sb.ToCharArray());

        /// <summary>
        /// Writes a sequence to the console output with the given string separator
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence</typeparam>
        /// <param name="e">Sequence to print</param>
        /// <param name="separator">String separator between each element, defaults to the empty string</param>
        public void Write<T>(IEnumerable<T> e, string separator = "") => Write(new StringBuilder().AppendJoin(e, separator).ToCharArray());
 
        /// <summary>
        /// Writes a sequence to the console output with the given string separator
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence</typeparam>
        /// <param name="e">Sequence to print</param>
        /// <param name="separator">Character separator between each element</param>
        public void Write<T>(IEnumerable<T> e, char separator) => Write(new StringBuilder().AppendJoin(e, separator).ToCharArray());

        /// <summary>
        /// Writes a linefeed to the console output
        /// </summary>
        public void WriteLine()
        {
            this.outStream.WriteLine();
            if (this.writeToConsole) { Console.WriteLine(); }
        }
 
        /// <summary>
        /// Writes the given char to the console output, followed by a linefeed
        /// </summary>
        /// <param name="c">Character to write</param>
        public void WriteLine(char c)
        {
            this.outStream.WriteLine(c);
            if (this.writeToConsole) { Console.WriteLine(c); }
        }
 
        /// <summary>
        /// Writes the given char buffer to the console output, followed by a linefeed
        /// </summary>
        /// <param name="buffer">Char buffer to write to the output</param>
        public void WriteLine(char[] buffer)
        {
            this.outStream.WriteLine(buffer);
            if (this.writeToConsole) { Console.WriteLine(buffer); }
        }
 
        /// <summary>
        /// Writes the given string to the console output, followed by a linefeed
        /// </summary>
        /// <param name="s">String to write</param>
        public void WriteLine(string s)
        {
            this.outStream.WriteLine(s);
            if (this.writeToConsole) { Console.WriteLine(s); }
        }
 
        /// <summary>
        /// Writes the given integer to the console output, followed by a linefeed
        /// </summary>
        /// <param name="n">Integer to write</param>
        public void WriteLine(int n)
        {
            int head = GetIntBuffer(n);
            this.outStream.WriteLine(numBuffer, head, 20 - head);
            if (this.writeToConsole) { Console.WriteLine(numBuffer, head, 20 - head); }
        }
 
        /// <summary>
        /// Writes the given long to the console output, followed by a linefeed
        /// </summary>
        /// <param name="n">Long to write</param>
        public void WriteLine(long n)
        {
            int head = GetLongBuffer(n);
            this.outStream.WriteLine(numBuffer, head, 20 - head);
            if (this.writeToConsole) { Console.WriteLine(numBuffer, head, 20 - head); }
        }

        /// <summary>
        /// Writes the given object to the console output using the object's ToString method, followed by a linefeed
        /// </summary>
        /// <param name="o">Object to write</param>
        public void WriteLine(object o) => this.outStream.WriteLine(o);
 
        /// <summary>
        /// Writes the contents of the StringBuilder to the console output, followed by a linefeed
        /// </summary>
        /// <param name="sb">StringBuilder to write to the output</param>
        public void WriteLine(StringBuilder sb) => WriteLine(sb.ToCharArray());
 
        /// <summary>
        /// Writes a sequence to the console output with the given string separator, follows by a linefeed
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence</typeparam>
        /// <param name="e">Sequence to print</param>
        /// <param name="separator">String separator between each element, defaults to the empty string</param>
        public void WriteLine<T>(IEnumerable<T> e, string separator = "") => WriteLine(new StringBuilder().AppendJoin(e, separator).ToCharArray());
 
        /// <summary>
        /// Writes a sequence to the console output with the given string separator, follows by a linefeed
        /// </summary>
        /// <typeparam name="T">Type of elements in the sequence</typeparam>
        /// <param name="e">Sequence to print</param>
        /// <param name="separator">Character separator between each element</param>
        public void WriteLine<T>(IEnumerable<T> e, char separator) => WriteLine(new StringBuilder().AppendJoin(e, separator).ToCharArray());
 
        /// <summary>
        /// Flushes the output buffer to the console if the buffer is full, or if it's being forced
        /// </summary>
        public void Flush() => this.outStream.Flush();
 
        /// <inheritdoc />
        /// <summary>
        /// Disposes of the resources of this ConsoleHelper, closing all the associated streams
        /// </summary>
        public void Dispose()
        {
            if (this.Open)
            {
                Flush();
                this.inStream.Dispose();
                this.outStream.Dispose();
                this.Open = false;
            }
        }
        #endregion
    }
}
