using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SafeRapidPdf.Lexical
{
	public interface ILexer
	{
		/// <summary>
		/// Read next token
		/// </summary>
		/// <returns></returns>
		String ReadToken();

		/// <summary>
		/// Preview the next token without advancing in the stream
		/// </summary>
		/// <returns></returns>
		String PeekToken1();
		String PeekToken2();

		/// <summary>
		/// One step back in the stream
		/// </summary>
		void Putc();

		/// <summary>
		/// Consume and check for the specified token
		/// </summary>
		/// <param name="token"></param>
		void Expects(String token);

		/// <summary>
		/// Read all chars until an EOL char
		/// </summary>
		/// <returns></returns>
		String ReadUntilEol();

		/// <summary>
		/// Read a block of bytes from the stream (used for stream objects)
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		byte[] ReadBytes(int length);

		/// <summary>
		/// Read the next character
		/// </summary>
		/// <returns></returns>
		char ReadChar();

		/// <summary>
		/// Retrieve objects directly using this interface
		/// </summary>
		IIndirectReferenceResolver IndirectReferenceResolver { get; }

        /// <summary>
        /// Push the current position and seek to the new position
        /// - if newPosition is <0, seek from end of file
        /// - else seek from the beginning of the file
        /// </summary>
        /// <param name="newPosition"></param>
		void PushPosition(long newPosition);

        /// <summary>
        /// Get back to the last pushed position
        /// </summary>
		void PopPosition();

		/// <summary>
		/// Relative position in % inside the file
		/// </summary>
		int Percentage
		{
			get;
		}
	}
}
