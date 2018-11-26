/*
    ============================================================================

    Namespace:          WizardWrx.ConsoleAppAids3

    Class Name:         FixedConsoleWriter

    File Name:          FixedConsoleWriter.cs

    Synopsis:           Use this class when you need to continuously update a
                        message, such as a progress report or countdown clock,
                        while keeping the text above it visible.

    Remarks:            This class was motivated by the need for a countdown
                        clock, to display when an exception is reported, so that
                        the application can allow time for someone to read the
                        message, without completely halting a task that may run
                        unattended.

						Along the way, I discovered another benefit of using it
                        to report progress, which is that reusing a fixed spot
						on the screen yields an astonishingly high performance
                        increase, because there is so much less screen buffer
						motion.

    Created:            Saturday, 01 March 2014 through Monday, 03 March 2014

    Author:             David A. Gray

	License:            Copyright (C) 2014-2017, David A. Gray. 
						All rights reserved.

                        Redistribution and use in source and binary forms, with
                        or without modification, are permitted provided that the
                        following conditions are met:

                        *   Redistributions of source code must retain the above
                            copyright notice, this list of conditions and the
                            following disclaimer.

                        *   Redistributions in binary form must reproduce the
                            above copyright notice, this list of conditions and
                            the following disclaimer in the documentation and/or
                            other materials provided with the distribution.

                        *   Neither the name of David A. Gray, nor the names of
                            his contributors may be used to endorse or promote
                            products derived from this software without specific
                            prior written permission.

                        THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND
                        CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED
                        WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
                        WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A
                        PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL
                        David A. Gray BE LIABLE FOR ANY DIRECT, INDIRECT,
                        INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
                        (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF
                        SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
                        PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
                        ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
                        LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
                        ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN
                        IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version By  Description
    ---------- ------- --- --------------------------------------------------
    2014/03/03 2.5     DAG Develop proof of concept in the context of a test
                           application, MyPlayPen.

    2014/03/23 3.5     DAG Integrate into WizardWrx.ConsoleAppAids3 namespace.

    2014/05/31 3.6     DAG Adjust for classes moved out of this class, to
                           decouple it from ApplicationHelpers.

    2014/06/06 5.0     DAG Major namespace reorganization.

    2014/06/22 5.1     DAG 1) Further fine tuning of the namespaces.

                           2) Account for the rationalization of the color
                              console writer, ErrorMessagesInColor, since 
                              FixedConsoleWriter is an adapter, which exposes an
                              analogous set of writer methods.

    2015/06/18 5.5     DAG 1) Correct a system state error discovered during
                              tests of the console speed test demonstration.

                           2) Redirect output to STDERR, so that it always goes
                              to the console, unless both output streams are
                              redirected.

                           3) Add constructors that takes an offset from the
                              left edge of the window, with and without the
                              console color overrides.

                           3) Make the existing Reset a tad smarter, so that it
                              takes into account line advances caused by a call
                              to the ordinary ConsoleWriteLine method.

    2016/04/07 6.0     DAG 1) Leverage the features of version 5.8.334.40074 of
                              WizaerdWrx.DllServices2.dll to suppress the
                              actions that depend on a handle being attached to
                              a working console when that handle has been
                              redirected to a file.

                           2) Finish breaking free of dependence on the old
                              strong name signed class libraries by binding to
                              WizardWrx.DllServices2, version 6.0.405.41682.

	2016/06/09 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically
                           belong in this library being found instead in
                           DLLServices2.

	2017/12/06 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
                           the constellation of DLLs that replaced it, which
                           also requires upgrading the target framework version.

	2018/11/26 7.0     DAG Eliminate the unreferenced WizardWrx.Core, and its
                           parent, WizardWrx, from the using directives.
    ============================================================================
*/


using System;

using WizardWrx.ConsoleStreams;
using WizardWrx.DLLConfigurationManager;


namespace WizardWrx.ConsoleAppAids3
{
    /// <summary>
    /// Instances of this class permit a line of a console window to be used
    /// repeatedly for successive lines of text, replacing the contents of the
    /// previous print statement, so that the lines above it don't scroll off
    /// the screen.  Once instantiated, instances of this class behave almost
	/// exactly like Console.WriteLine, and you can drop them into your code
	/// in its place, because its overloads have identical signatures.
	/// 
	/// Please see the Remarks.
    /// </summary>
    /// <remarks>
    /// The magic of this class depends on keeping track of the starting and
    /// current cursor positions. When a new instance is constructed, the cursor
    /// position is stored into a pair of private integers, so that it can be
    /// used whenever one of its Write methods is about to send text to the
    /// console to determine how far right and/or down the last such operation
    /// moved the cursor.
    /// 
    /// Although Console.SetCursorPosition can be used to do so, and is used
    /// internally, the point of this class is to relieve you of the burden of
    /// keeping track of the cursor.
    /// 
    /// Let it manage the cursor, so you don't have to.
    /// </remarks>
    public class FixedConsoleWriter
    {
        #region Public Enumerations for Use as Arguments
        /// <summary>
        /// Use the members of this enumeration to control the behavior of the
        /// ReturnCarriage method.
        /// </summary>
        public enum CRBehavior
        {
            /// <summary>
            /// Return to the starting column. This is the default behavior if
            /// the default form of the ReturnCarriage method is called.
            /// </summary>
            ReturnToStartingColumn,

            /// <summary>
            /// Return to the left edge, even if the original starting point was
            /// to the right of it. 
            /// 
            /// Use this setting to overwrite the entire line, including text
            /// that was written before the first call to a Write method of this
            /// class.
            /// </summary>
            ReturnToLefEdge,
        }   // CRBehavior
		#endregion	// Public Enumerations for Use as Arguments


		#region Private Constants and Local Storage for Instance
		private const int LEFT_EDGE = MagicNumbers.ZERO;
		private const int NONE = MagicNumbers.ZERO;
		private const int WHOLE = MagicNumbers.MINUS_ONE;
		private const int COORDINATE_NOT_SET = MagicNumbers.MINUS_ONE;
		private const int REDIRECTED_HANDLE_POSITION = MagicNumbers.MINUS_ONE;

		//	--------------------------------------------------------------------
		//	The static constructor initializes all of these.
		//	--------------------------------------------------------------------

		private static string s_strStdErrFile = null;
		private static string s_strStdInpFile = null;
		private static string s_strStdOutFile = null;

		private static StandardHandleInfo.StandardHandleState s_enmStdErrHandleState = StandardHandleInfo.StandardHandleState.Undetermined;
		private static StandardHandleInfo.StandardHandleState s_enmStdInpHandleState = StandardHandleInfo.StandardHandleState.Undetermined;
		private static StandardHandleInfo.StandardHandleState s_enmStdOutHandleState = StandardHandleInfo.StandardHandleState.Undetermined;

		private int _intInitialCol = COORDINATE_NOT_SET;
		private int _intInitialRow = COORDINATE_NOT_SET;
		private int _intLastWriteCol = COORDINATE_NOT_SET;
		private int _intLastWriteRow = COORDINATE_NOT_SET;

		private bool _fSuppressOutput = true;

		private string _strFiller;
		private ErrorMessagesInColor _cw;
		#endregion  // Private Constants and Local Storage for Instance


        #region Constructors
		/// <summary>
        /// Construct a default instance that uses the current console colors.
        /// </summary>
        public FixedConsoleWriter ( )
        {
			Initialize (
				Console.ForegroundColor ,	// Keep the current text (foreground) color.
				Console.BackgroundColor ,	// Keep the current background color.
				COORDINATE_NOT_SET );		// Keep the current cursor position.
        }   // Default constructor (1 of 4)


        /// <summary>
        /// Construct an instance that has color properties that are independent
        /// of the current console foreground and background colors.
        /// </summary>
        /// <param name="pclrTextForeColor">
        /// Specify the ConsoleColor property to use as the text (foreground)
        /// color in generated messages.
        /// </param>
        /// <param name="pclrTextBackColor">
        /// Specify the ConsoleColor property to use as the background color in
        /// generated messages.
        /// </param>
        public FixedConsoleWriter (
            ConsoleColor pclrTextForeColor ,
            ConsoleColor pclrTextBackColor )
        {
			Initialize (
				pclrTextForeColor ,			// Use the text (foreground) color chosen by the caller.
				pclrTextBackColor ,			// Keep the background color chosen by the caller.
				COORDINATE_NOT_SET );		// Keep the current cursor position.
        }   // Overloaded constructor for setting colors (2 of 4)


		/// <summary>
		/// Construct an instance that uses the current console colors, and 
		/// writes text starting at a specified position relative to the left
		/// edge of the window.
		/// </summary>
		/// <param name="pintOffset">
		/// Specify the position relative to the left edge where writing should
		/// start. Columns are counted from zero.
		/// 
		/// Initializing private properties _intInitialCol and _intLastWriteCol from
		/// Console.CursorTop is always deferred until the first time that a Write
		/// method is called, because the correct cursor row is unknown until then.
		/// </param>
		/// <example>
		/// To start writing at column 9, leaving 8 blanks to the left, pass 8
		/// as the value of pintOffset.
		/// </example>
		public FixedConsoleWriter ( int pintOffset )
		{
			Initialize (
				Console.ForegroundColor ,	// Keep the current text (foreground) color.
				Console.BackgroundColor ,	// Keep the current background color.
				pintOffset );				// Set the cursor at the specified position, where the left column is zero.
		}   // Overloaded constructor for setting a left indent (3 of 4)


		/// <summary>
		/// Construct an instance that has color properties that are independent
		/// of the current console foreground and background colors and its left
		/// margin set a specified number of columns in from the left margin.
		/// </summary>
		/// <param name="pclrTextForeColor">
		/// Specify the ConsoleColor property to use as the text (foreground)
		/// color in generated messages.
		/// </param>
		/// <param name="pclrTextBackColor">
		/// Specify the ConsoleColor property to use as the background color in
		/// generated messages.
		/// </param>
		/// <param name="pintOffset">
		/// Specify the position relative to the left edge where writing should
		/// start. Columns are counted from zero.
		/// 
		/// Initializing private properties _intInitialCol and _intLastWriteCol from
		/// Console.CursorTop is always deferred until the first time that a Write
		/// method is called, because the correct cursor row is unknown until then.
		/// </param>
		/// <example>
		/// To start writing at column 9, leaving 8 blanks to the left, pass 8
		/// as the value of pintOffset.
		/// </example>
		public FixedConsoleWriter (
			ConsoleColor pclrTextForeColor ,
            ConsoleColor pclrTextBackColor ,
			int pintOffset )
		{
			Initialize (
				pclrTextForeColor ,			// Use the text (foreground) color chosen by the caller.
				pclrTextBackColor ,			// Keep the background color chosen by the caller.
				pintOffset );				// Set the cursor at the specified position, where the left column is zero.
		}   // Overloaded constructor for setting colors and a left indent (4 of 4)
        #endregion  // Constructors


        #region Public Void Write Methods
        /// <summary>
        /// Write the string representation of a bool (Boolean) variable.
        /// </summary>
        /// <param name="value">
        /// Specify the Boolean value to display.
        /// </param>
        public void Write (
            bool value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (1 of 17)


        /// <summary>
        /// Write the string representation of a char (a Unicode character).
        /// </summary>
        /// <param name="value">
        /// Specify the Unicode character to display.
        /// </param>
        public void Write (
            char value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (2 of 17)


        /// <summary>
        /// Write the string representation of a character array.
        /// </summary>
        /// <param name="buffer">
        /// Specify the array of Unicode characters to display.
        /// </param>
        public void Write (
            char [ ] buffer )
        {
            Reset ( );
            _cw.Write ( buffer );
            SaveState ( );
        }   // public void Write (3 of 17)


        /// <summary>
        /// Write the string representation of a decimal variable.
        /// </summary>
        /// <param name="value">
        /// Specify the decimal value to display.
        /// </param>
        public void Write (
            decimal value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (4 of 17)


        /// <summary>
        /// Write the string representation of a double precision variable.
        /// </summary>
        /// <param name="value">
        /// Specify the double precision value to display.
        /// </param>
        public void Write (
            double value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (5 of 17)


        /// <summary>
        /// Write the string representation of a floating point variable.
        /// </summary>
        /// <param name="value">
        /// Specify the floating point value to display.
        /// </param>
        public void Write (
            float value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (6 of 17)


        /// <summary>
        /// Write the string representation of an integer variable.
        /// </summary>
        /// <param name="value">
        /// Specify the integer value to display.
        /// </param>
        public void Write (
            int value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (7 of 17)


        /// <summary>
        /// Write the string representation of a long integer variable.
        /// </summary>
        /// <param name="value">
        /// Specify the long integer value to display.
        /// </param>
        public void Write (
            long value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (8 of 17)


        /// <summary>
        /// Write the string representation of a generic Object variable.
        /// </summary>
        /// <param name="value">
        /// Specify the object value to display.
        /// </param>
        public void Write (
            object value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (9 of 17)


        /// <summary>
        /// Write a string variable.
        /// </summary>
        /// <param name="value">
        /// Specify the string value to display.
        /// </param>
        public void Write (
            string value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (10 of 17)


        /// <summary>
        /// Write the string representation of a uint (unsigned integer)
        /// variable.
        /// </summary>
        /// <param name="value">
        /// Specify the uint (unsigned integer) value to display.
        /// </param>
        public void Write (
            uint value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (11 of 17)


        /// <summary>
        /// Write the string representation of a unsigned long integer
        /// variable.
        /// </summary>
        /// <param name="value">
        /// Specify the unsigned long integer value to display.
        /// </param>
        public void Write (
            ulong value )
        {
            Reset ( );
            _cw.Write ( value );
            SaveState ( );
        }   // public void Write (12 of 17)


        /// <summary>
        /// Write a formatted message that includes the string representation of
        /// an generic object variable.
        /// </summary>
        /// <param name="format">
        /// Use this string as the message template, which may include at most
        /// one substitution token.
        /// </param>
        /// <param name="arg0">
        /// Replace the substitution token with the string representation of this
        /// generic object.
        /// </param>
        public void Write (
            string format ,
            object arg0 )
        {
            Reset ( );
            _cw.Write ( format , arg0 );
            SaveState ( );
        }   // public void Write (13 of 17)


        /// <summary>
        /// Write a formatted message that includes the string representation of
        /// an generic object variable.
        /// </summary>
        /// <param name="format">
        /// Use this string as the message template, which may contains
        /// substitution tokens for each object in an array of generic Object
        /// variables.
        /// </param>
        /// <param name="arg">
        /// Substitute elements from this array of generic Object variables into
        /// the format string, replacing tokens with the element whose subscript
        /// is the number within its brackets.
        /// </param>
        public void Write (
            string format ,
            params object [ ] arg )
        {
            Reset ( );
            _cw.Write ( format , arg );
            SaveState ( );
        }   // public void Write (14 of 17)


        /// <summary>
        /// Write a formatted message that includes a range of characters taken
        /// from an array of Unicode characters.
        /// </summary>
        /// <param name="buffer">
        /// Extract characters from this array of Unicode characters.
        /// </param>
        /// <param name="index">
        /// Subscript of character to substitute for token {0} in format.
        /// </param>
        /// <param name="count">
        /// Number of characters from buffer to substitute into string, which
        /// must contain at least count - 1 substitution tokens.
        /// </param>
        public void Write (
            char [ ] buffer ,
            int index ,
            int count )
        {
            Reset ( );
            _cw.Write ( buffer , index , count );
            SaveState ( );
        }   // public void Write (15 of 17)


        /// <summary>
        /// Write a formatted message that includes up to two substitution
        /// tokens.
        /// </summary>
        /// <param name="format">
        /// Use this string as the message template, which may contain up to two
        /// substitution tokens, {0} and {1}.
        /// </param>
        /// <param name="arg0">
        /// The default string representation of this generic Object variable is
        /// substituted into format for token {0}.
        /// </param>
        /// <param name="arg1">
        /// The default string representation of this generic Object variable is
        /// substituted into format for token {1}.
        /// </param>
        public void Write (
            string format ,
            object arg0 ,
            object arg1 )
        {
            Reset ( );
            _cw.Write ( format , arg0 , arg1 );
            SaveState ( );
        }   // public void Write (16 of 17)


        /// <summary>
        /// Write a formatted message that includes up to three substitution
        /// tokens.
        /// </summary>
        /// <param name="format">
        /// Use this string as the message template, which may contain up to 3
        /// substitution tokens, {0}, {1}, and {2}.
        /// </param>
        /// <param name="arg0">
        /// The default string representation of this generic Object variable is
        /// substituted into format for token {0}.
        /// </param>
        /// <param name="arg1">
        /// The default string representation of this generic Object variable is
        /// substituted into format for token {1}.
        /// </param>
        /// <param name="arg2">
        /// The default string representation of this generic Object variable is
        /// substituted into format for token {2}.
        /// </param>
        public void Write (
            string format ,
            object arg0 ,
            object arg1 ,
            object arg2 )
        {
            Reset ( );
            _cw.Write ( format , arg0 , arg1 , arg2 );
            SaveState ( );
        }   // public void Write (17 of 17)
        #endregion  // Public Void Write Methods


        #region Public Methods for Repositioning the Cursor
        /// <summary>
        /// Calling this method resets the console writing cursor to the point
        /// at which it rested when the instance came into being.
        /// </summary>
        public void ReturnCarriage ( )
        {
            ReturnCarriage ( CRBehavior.ReturnToStartingColumn );
        }   // public void ReturnCarriage (1 of 2)


        /// <summary>
        /// Calling this method resets the console writing cursor to the point
        /// specified by the CRBehavior enumeration member that is passed into
        /// it. Please see the Remarks.
        /// </summary>
        /// <param name="penmCRBehavior">
        /// Use this enumeration to control whether the console writing cursor
        /// is reset to the column where it was when the instance came into
        /// being or to the beginning of that line. Regardless, it is reset to
        /// the row where it was when the instance came into being.
        /// 
        /// If the integral value of penmCRBehavior doesn't map to a member of
        /// the CRBehavior enumeration, the method behaves as it would if its
        /// actual value was ReturnToStartingColumn. This prevents overwriting
        /// text that you intended to keep.
        /// 
        /// Please see the Remarks.
        /// </param>
        /// <remarks>
        /// If the initial cursor position was at the left edge of the console
        /// window, it doesn't matter which CRBehavior member is specified, and
        /// you may as well call the default method that takes no arguments.
        /// </remarks>
        public void ReturnCarriage ( CRBehavior penmCRBehavior )
        {
            switch ( penmCRBehavior )
            {
                case CRBehavior.ReturnToStartingColumn:
                    Reset ( );
                    break;

                case CRBehavior.ReturnToLefEdge:
                    _intInitialCol = LEFT_EDGE;
                    Reset ( );
                    break;

                default:
                    Reset ( );
                    break;
            }   // switch ( penmCRBehavior )
        }   // public void ReturnCarriage (2 of 2)


        /// <summary>
        /// You could just as well call Console.Error.WriteLine() directly, but
		/// please read the remarks.
        /// </summary>
        /// <remarks>
        /// The last Write method call leaves the cursor at the end of the text.
        /// Unless you want your next call to Console.Error.WriteLine() to start
        /// writing there, you must call either this method, which, in turn,
        /// calls Console.Error.WriteLine(), or call it directly.
        /// 
        /// If, instead, you want to overwrite the last message, call the
        /// ReturnCarriage method.
        /// </remarks>
        public void ScrollUp ( )
        {
            Console.Error.WriteLine ( );
        }   // public void ScrollUp
        #endregion  // Public Methods for Repositioning the Cursor


		#region Public Static Methods
		/// <summary>
		/// Gets the redirection state of the Standard Error handle.
		/// </summary>
		/// <returns>
		/// The return value is the member of the 
		/// StandardHandleInfo.StandardHandleState enumeration that corresponds
		/// to the state of the handle (either attached or redirected).
		/// </returns>
		/// <remarks>
		/// Due to the relatively high cost of obtaining this information, it is
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdInpState"/>
		/// <seealso cref="GetStdOutState"/>
		/// <seealso cref="GetStdErrFileName"/>
		/// <seealso cref="GetStdInpFileName"/>
		/// <seealso cref="GetStdOutFileName"/>
		public static StandardHandleInfo.StandardHandleState GetStdErrState ( )
		{
			if ( s_enmStdErrHandleState == StandardHandleInfo.StandardHandleState.Undetermined )
			{	// Due to its computational cost, the information is obtained to satisfy the first request, and saved to satisfy subsequent requests.
				s_enmStdErrHandleState = StateManager.GetTheSingleInstance ( ).StandardHandleState ( StandardHandleInfo.StandardConsoleHandle.StandardError );
			}	// if ( s_enmStdErrHandleState == StandardHandleInfo.StandardHandleState.Undetermined )

			return s_enmStdErrHandleState;
		}	// StdErrState


		/// <summary>
		/// Gets the redirection state of the Standard Input handle.
		/// </summary>
		/// <returns>
		/// The return value is the member of the 
		/// StandardHandleInfo.StandardHandleState enumeration that corresponds
		/// to the state of the handle (either attached or redirected).
		/// </returns>
		/// <remarks>
		/// Due to the relatively high cost of obtaining this information, it is
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdErrState"/>
		/// <seealso cref="GetStdOutState"/>
		/// <seealso cref="GetStdErrFileName"/>
		/// <seealso cref="GetStdInpFileName"/>
		/// <seealso cref="GetStdOutFileName"/>
		public static StandardHandleInfo.StandardHandleState GetStdInpState ( )
		{
			if ( s_enmStdInpHandleState == StandardHandleInfo.StandardHandleState.Undetermined )
			{	// Due to its computational cost, the information is obtained to satisfy the first request, and saved to satisfy subsequent requests.
				s_enmStdInpHandleState = s_enmStdErrHandleState = StateManager.GetTheSingleInstance ( ).StandardHandleState ( StandardHandleInfo.StandardConsoleHandle.StandardInput );
			}	// if ( s_enmStdInpHandleState == StandardHandleInfo.StandardHandleState.Undetermined )

			return s_enmStdInpHandleState;
		}	// GetStdInpState


		/// <summary>
		/// Gets the redirection state of the Standard Output handle.
		/// </summary>
		/// <returns>
		/// The return value is the member of the 
		/// StandardHandleInfo.StandardHandleState enumeration that corresponds
		/// to the state of the handle (either attached or redirected).
		/// </returns>
		/// <remarks>
		/// Due to the relatively high cost of obtaining this information, it is
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdErrState"/>
		/// <seealso cref="GetStdInpState"/>
		/// <seealso cref="GetStdErrFileName"/>
		/// <seealso cref="GetStdInpFileName"/>
		/// <seealso cref="GetStdOutFileName"/>
		public static StandardHandleInfo.StandardHandleState GetStdOutState ( )
		{
			if ( s_enmStdOutHandleState == StandardHandleInfo.StandardHandleState.Undetermined )
			{	// Due to its computational cost, the information is obtained to satisfy the first request, and saved to satisfy subsequent requests.
				s_enmStdOutHandleState = s_enmStdErrHandleState = StateManager.GetTheSingleInstance ( ).StandardHandleState ( StandardHandleInfo.StandardConsoleHandle.StandardOutput );
			}	// if ( s_enmStdOutHandleState == StandardHandleInfo.StandardHandleState.Undetermined )

			return s_enmStdOutHandleState;
		}	// GetStdOutState


		/// <summary>
		/// Gets the absolute (fully qualified) name of the file to which the
		/// Standard Output console handle is redirected.
		/// </summary>
		/// <returns>
		/// If the Standard Error console handle is redirected, this method
		/// returns is absolute (fully qualified) name. Otherwise, it returns
		/// the empty string. 
		/// 
		/// Please see the Remarks section for important information.
		/// </returns>
		/// <remarks>
		/// If you just need to know whether the handle is redirected, use the
		/// much cheaper static GetStdErrState method.
		/// 
		/// Due to the behavior of the underlying Win32 API that retrieves the
		/// name of the file, the returned string begins with "\\?\d:\", 
		/// where "d" is the letter assigned to the drive on which it resides,
		/// and is always fully qualified.
		/// </remarks>
		/// <remarks>
		/// Due to the relatively high cost of obtaining this information, it is
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdErrState"/>
		/// <seealso cref="GetStdInpState"/>
		/// <seealso cref="GetStdOutState"/>
		/// <seealso cref="GetStdInpFileName"/>
		/// <seealso cref="GetStdOutFileName"/>
		public static string GetStdErrFileName ( )
		{
			if ( GetStdErrState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{	// Handle is attached to a console.
				return SpecialStrings.EMPTY_STRING;
			}	// TRUE (default state) block, if ( GetStdErrState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			else
			{	// Handle is redirected. The first time this method is called, s_strStdOutFile is null. 
				if ( s_strStdOutFile == null )
				{	// Due to its significant cost, the filename is retrieved to satisfy the first request, then saved.
					s_strStdErrFile = StateManager.GetTheSingleInstance ( ).GetStdHandleFQFN ( StandardHandleInfo.StandardConsoleHandle.StandardError );
				}	// if ( s_strStdOutFile == null )

				return s_strStdErrFile;		// Whether it was just retrieved, or was already there, return a copy of the static string.
			}	// FALSE (redirected  state) block, if ( GetStdErrState ( ) == StandardHandleInfo.StandardHandleState.Attached )
		}	// public static string GetStdErrFileName


		/// <summary>
		/// Gets the absolute (fully qualified) name of the file to which the
		/// Standard Input console handle is redirected.
		/// </summary>
		/// <returns>
		/// If the Standard Input console handle is redirected, this method
		/// returns is absolute (fully qualified) name. Otherwise, it returns
		/// the empty string. 
		/// 
		/// Please see the Remarks section for important information.
		/// </returns>
		/// <remarks>
		/// If you just need to know whether the handle is redirected, use the
		/// static GetStdInpState method, which is much cheaper.
		/// 
		/// Due to the behavior of the underlying Win32 API that retrieves the
		/// name of the file, the returned string begins with "\\?\d:\", 
		/// where "d" is the letter assigned to the drive on which it resides,
		/// and is always fully qualified.
		/// 
		/// Due to the relatively high cost of obtaining this information, it
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdErrState"/>
		/// <seealso cref="GetStdInpState"/>
		/// <seealso cref="GetStdOutState"/>
		/// <seealso cref="GetStdErrFileName"/>
		/// <seealso cref="GetStdOutFileName"/>
		public static string GetStdInpFileName ( )
		{
			if ( GetStdInpState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{	// Handle is attached to a console.
				return SpecialStrings.EMPTY_STRING;
			}	// TRUE (default state) block, if ( GetStdInpState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			else
			{	// Handle is redirected. The first time this method is called, s_strStdInpFile is null. 
				if ( s_strStdInpFile == null )
				{	// Due to its significant cost, the filename is retrieved to satisfy the first request, then saved.
					s_strStdInpFile = StateManager.GetTheSingleInstance ( ).GetStdHandleFQFN ( StandardHandleInfo.StandardConsoleHandle.StandardInput );
				}	// if ( s_strStdOutFile == null )

				return s_strStdInpFile;		// Whether it was just retrieved, or was already there, return a copy of the static string.
			}	// FALSE (redirected  state) block, if ( GetStdInpState ( ) == StandardHandleInfo.StandardHandleState.Attached )
		}	// GetStdInpFileName


		/// <summary>
		/// Gets the absolute (fully qualified) name of the file to which the
		/// Standard Output console handle is redirected.
		/// </summary>
		/// <returns>
		/// If the Standard Output console handle is redirected, this method
		/// returns is absolute (fully qualified) name. Otherwise, it returns
		/// the empty string. 
		/// 
		/// Please see the Remarks section for important information.
		/// </returns>
		/// <remarks>
		/// If you just need to know whether the handle is redirected, use the
		/// static GetStdOutState method, which is much cheaper.
		/// 
		/// Due to the behavior of the underlying Win32 API that retrieves the
		/// name of the file, the returned string begins with "\\?\d:\", 
		/// where "d" is the letter assigned to the drive on which it resides,
		/// and is always fully qualified.
		/// 
		/// Due to the relatively high cost of obtaining this information, it
		/// retrieved to satisfy the first request, and cached in a private
		/// static member, which is returned to satisfy subsequent requests.
		/// </remarks>
		/// <seealso cref="GetStdErrState"/>
		/// <seealso cref="GetStdInpState"/>
		/// <seealso cref="GetStdOutState"/>
		/// <seealso cref="GetStdErrFileName"/>
		/// <seealso cref="GetStdInpFileName"/>
		public static string GetStdOutFileName ( )
		{
			if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{	// Handle is attached to a console.
				return string.Empty;
			}	// TRUE (default state) block, if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			else
			{	// Handle is redirected. The first time this method is called, s_strStdOutFile is null. 
				if ( s_strStdOutFile == null )
				{	// Due to its significant cost, the filename is retrieved to satisfy the first request, then saved.
					s_strStdOutFile = StateManager.GetTheSingleInstance ( ).GetStdHandleFQFN ( StandardHandleInfo.StandardConsoleHandle.StandardOutput );
				}	// if ( s_strStdOutFile == null )

				return s_strStdOutFile;		// Whether it was just retrieved, or was already there, return a copy of the static string.
			}	// FALSE (redirected  state) block, if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
		}	// public static string GetStdOutFileName
		#endregion	// Public Static Methods


		#region Private Instance Methods
		/// <summary>
		/// Handle the I/O exception thrown when a console program whose
		/// standard output is redirected attempts to call any method on the
		/// Console singleton that changes the cursor position.
		/// </summary>
		/// <param name="pexIo">
		/// Pass in the exception so that its properties can go into the message
		/// that gets sent to the active trace listeners, if any.
		/// </param>
		/// <remarks>
		/// Extracting this code simplifies handling this exception, which can
		/// arise in several locations within the class, consistently.
		/// </remarks>
		private void HandleConsoleIOException ( System.IO.IOException pexIo )
		{
#if TRACE
			System.Diagnostics.Trace.WriteLine (
				string.Format (
				Properties.Resources.ERRMSG_EXCEPTION_BARE_BONES ,
				new object [ ]
				{
					pexIo.GetType ( ).Name ,									// Format Item 0 = An {0} exception
					pexIo.TargetSite ,											// Format Item 1 = routine {1} - The TargetSite property has it.
					pexIo.Message ,												// Format Item 2 = Message            = {2}
					GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached
						? Console.CursorLeft
						: REDIRECTED_HANDLE_POSITION ,							// Format Item 3 = Console.CursorLeft = {3}
					GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached
						? Console.CursorTop 
						: REDIRECTED_HANDLE_POSITION ,							// Format Item 4 = Console.CursorTop  = {4}
					_intInitialCol ,											// Format Item 5 = _intInitialCol     = {5}
					_intInitialRow ,											// Format Item 6 = _intInitialRow     = {6}
					Environment.NewLine											// Format Item 7 = Newline, My Way
				} ) );
#endif	// TRACE
			_fSuppressOutput = true;
		}	// private void HandleConsoleIOException


		/// <summary>
		/// All four constructors call upon this private routine to initialize
		/// everything.
		/// </summary>
		/// <param name="pclrTextForeColor">
		/// Specify the ConsoleColor property to use as the text (foreground)
		/// color in generated messages.
		/// </param>
		/// <param name="pclrTextBackColor">
		/// Specify the ConsoleColor property to use as the background color in
		/// generated messages.
		/// </param>
		/// <param name="pintOffset">
		/// Specify the position relative to the left edge where writing should
		/// start. Columns are counted from zero.
		/// </param>
		private void Initialize (
			ConsoleColor pclrTextForeColor ,
			ConsoleColor pclrTextBackColor ,
			int pintOffset )
		{
			_cw = new ErrorMessagesInColor (
				pclrTextForeColor ,
				pclrTextBackColor );
			_strFiller = new string (
				SpecialCharacters.SPACE_CHAR ,
				Console.WindowWidth );

			if ( pintOffset > COORDINATE_NOT_SET )
			{	// Leave these two properties undisturbed unless the offset is overridden.
				_intInitialCol = pintOffset;
				_intLastWriteCol = pintOffset;
			}	// if ( pintOffset > COORDINATE_NOT_SET )
		}	// private void Initialize


		/// <summary>
        /// All writers use this method to clear the screen of characters
        /// written by the last call to a Write method, and reset the cursor to
        /// its starting position, so that the next write reuses the same space,
        /// but doesn't leave behind part of the last text written if the new
        /// message needs fewer characters. Please see the Remarks.
        /// </summary>
        /// <remarks>
        /// Since the writers must always call this method, the current cursor
        /// location is compared to the position it had at construction time. If
        /// it hasn't moved, there is nothing to do. This is always the case on
        /// the first call to a Write method, and it is theoretically possible
        /// for it to happen again, if the last write operations doesn't move
        /// the cursor from its starting point, or returns it there.
        /// 
        /// Although Console.SetCursorPosition can be used to do so, and is used
        /// internally, the point of this class is to relieve you of the burden of
        /// keeping track of the cursor. Let it manage the cursor.
        /// </remarks>
        private void Reset ( )
        {	// Initialization is deferred until first use.
			int intEndingRow, intEndingCol, intColsAdvanced, intRowsAdvanced;

			//	----------------------------------------------------------------
			//	Since two of the four constructors permit overriding the default
			//	initial column, this value is handled independently, and must be
			//	handled first, so that it is set correctly when the next block
			//	evaluates it.
			//	----------------------------------------------------------------

			if ( _intInitialCol == COORDINATE_NOT_SET )
			{	// This instance uses the default left margin.
				if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
				{
					_intInitialCol = Console.CursorLeft;
					_intLastWriteCol = Console.CursorLeft;
					intEndingCol = Console.CursorLeft;
				}	// if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			}	// TRUE (degenerate case) block, if ( _intInitialCol == COORDINATE_NOT_SET )
			else
			{	// The constructor overrode the default left margin.
				intEndingCol = _intInitialCol;
			}	// FALSE (standard case) block, if ( _intInitialCol == COORDINATE_NOT_SET )

			//	----------------------------------------------------------------
			//	In all but the degenerate case, where there is no left margin,
			//	the cursor must be moved into position before the write is
			//	issued, and this is the best place to do so.
			//	----------------------------------------------------------------

			if ( _intInitialCol > LEFT_EDGE && GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{
				try
				{
					Console.CursorLeft = _intInitialCol;
				}
				catch ( System.IO.IOException exIo )
				{
					HandleConsoleIOException ( exIo );
				}
			}	// if ( _intInitialCol > LEFT_EDGE && GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )

			//  ----------------------------------------------------------------
			//	These values cannot be initialized until it's time to use them.
			//	Otherwise, their initial values would be off by the amount of
			//	any changes brought about by calls to Console.WriteLine,
			//	Console.Write, and other methods that change the cursor 
			//	position.
			//  ----------------------------------------------------------------

			if ( _intLastWriteRow > COORDINATE_NOT_SET && GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{
				intEndingRow = Console.CursorTop > _intLastWriteRow
							   ? Console.CursorTop
							   : _intLastWriteRow;
				intEndingCol = Console.CursorLeft > _intLastWriteCol
							   ? Console.CursorLeft
							   : _intLastWriteCol;

				intColsAdvanced = intEndingCol - _intInitialCol;
				intRowsAdvanced = intEndingRow - _intInitialRow;

				//  ----------------------------------------------------------------
				//  The outermost test short circuits processing unless the cursor
				//  has left its starting position.
				//
				//  For similar reasons, unless the window width changes, the filler
				//  string that was created by the constructor is reused.
				//  ----------------------------------------------------------------

				if ( intColsAdvanced > NONE || intRowsAdvanced > NONE )
				{
					if ( Console.WindowWidth != _strFiller.Length )
					{
						_strFiller = new string (
							SpecialCharacters.SPACE_CHAR ,
							Console.WindowWidth );
					}   // if ( Console.WindowWidth != _strFiller.Length )

					//  ----------------------------------------------------------------
					//  Fully supporting writing at a fixed location requires a bit of
					//  fancy footwork to ensure that every character used by the last
					//  write operation is cleared before the next write.
					//
					//  Things get slightly more complicated if the cursor wasn't in the
					//  first column (column zero) before the first write operation.
					//  ----------------------------------------------------------------

					if ( intRowsAdvanced == NONE )
					{   // It's only necessary to write as many characters as the last write did.
						WriteFiller (
							_intInitialRow ,
							_intInitialCol ,
							intColsAdvanced );
					}   // TRUE (degenerate case) block, if ( intRowsAdvanced == NONE )
					else
					{   // Unless the line has advanced, this is the degenerate case.
						for ( int intCurrRow = _intInitialRow ;
								  intCurrRow <= intEndingRow ;
								  intCurrRow++ )
						{
							if ( _intInitialCol == LEFT_EDGE )
							{   // If writing started at the left edge, all rows are filled.
								WriteFiller (
									intCurrRow ,
									LEFT_EDGE ,
									WHOLE );
							}   // TRUE (simple case) block, if ( _intInitialCol == LEFT_EDGE )
							else
							{   // If writing started past the left edge, the first line was short.
								if ( intCurrRow == _intInitialRow )
								{   // Write the short row.
									WriteFiller (
										intCurrRow ,
										_intInitialCol ,
										_strFiller.Length - _intInitialCol );
								}   // TRUE block, if ( intCurrRow == _intInitialRow )
								else
								{   // All other rows are always full.
									WriteFiller (
										intCurrRow ,
										LEFT_EDGE ,
										WHOLE );
								}   // FALSE block, if ( intCurrRow == _intInitialRow )
							}   // FALSE (complex case) block, if ( _intInitialCol == LEFT_EDGE )

							if ( _fSuppressOutput )
							{
								break;	// There is no point in continuing.
							}	// if (_fSuppressOutput )
						}   // for ( int intCurrRow = _intInitialRow ; intCurrRow <= intEndingRow ; intCurrRow++ )
					}   // FALSE (complex case) if ( intRowsAdvanced == NONE )

					//  ------------------------------------------------------------
					//  Finally, back the cursor up to its starting point, and we
					//  are ready to start writing.
					//  ------------------------------------------------------------

					if ( _fSuppressOutput )
					{
						return;
					}	// TRUE (SetCursorPosition will fail again.) block, if (_fSuppressOutput )
					else
					{
						try
						{
							Console.SetCursorPosition (
								_intInitialCol ,
								_intInitialRow );
						}
						catch ( System.IO.IOException exIo )
						{
							HandleConsoleIOException ( exIo );
						}
					}	// FALSE (Expect SetCursorPosition to succeed.) block, (if (_fSuppressOutput )
				}  // if ( intColsAdvanced > NONE || intRowsAdvanced > NONE )	
			}	// TRUE (This is a subsequent call on the current instance.) block, if ( _intLastWriteRow > COORDINATE_NOT_SET && GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			else
			{	// The Cursor methods must NEVER be called on a redirected console handle, for the same reason that the corresponding Win32 routines are off limits.
				if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
				{
					_intInitialRow = Console.CursorTop;
					_intLastWriteRow = Console.CursorTop;
					intEndingRow = Console.CursorTop;
				}	// if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )

				intColsAdvanced = NONE;
				intRowsAdvanced = NONE;
			}	// FALSE (This is the first call on the current instance.) block, if ( _intLastWriteRow > COORDINATE_NOT_SET && GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
        }   // private void Reset


        private void SaveState ( )
        {
			if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			{	// Console state is meaningless when a standard handle is redirected.
				_intLastWriteCol = Console.CursorLeft;
				_intLastWriteRow = Console.CursorTop;
			}	// if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
        }   // private void SaveState


        /// <summary>
        /// This private instance method overwrites the reusable text area with
        /// spaces, in case the next message uses fewer characters.
        /// </summary>
        /// <param name="pintAtRow">
        /// Text is written at this row in the console window. Row numbering
        /// starts at zero, which is the top row (line) in the console window.
        /// 
        /// Please see the Remarks.
        /// </param>
        /// <param name="pintStartCol">
        /// Text is written starting at this position unless pintNCols is WHOLE
        /// (minus one), in which case this argument is ignored, and the whole
        /// line is filled, starting at the left edge of the window, which is
        /// column zero (signified by symbolic constant LEFT_EDGE).
        /// 
        /// Please see the Remarks.
        /// </param>
        /// <param name="pintNCols">
        /// This argument specifies the number of spaces to write, which varies
        /// from one to the length of the _strFiller string, which happens also
        /// to be the width, in characters, of the console window.
        /// 
        /// Please see the Remarks.
        /// </param>
        /// <remarks>
        /// The constructors establish the length of internal string variable
        /// _strFiller, which is filled with exactly enough spaces to fill the
        /// whole width of the console window. Each time the Reset method is
        /// called, it evaluates the Console.WindowWidth property, and createes
        /// a new filler string if its value has changed. This ensures that no
        /// excess characters that would cause a line wrap are written.
        /// 
        /// Unlike the other console writers in this class, this method uses the
        /// regular Console.Error.Write, which doesn't mess with the console colors.
        /// This enables it to ensure that there is no trailing white space in
        /// the instance text and background colors.
        /// </remarks>
        private void WriteFiller (
            int pintAtRow ,
            int pintStartCol ,
            int pintNCols )
        {
			_fSuppressOutput = false;

			try
			{
				if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
				{	// A redirected console handle has no position.
					Console.SetCursorPosition (
						pintStartCol ,
						pintAtRow );
				}	// if ( GetStdOutState ( ) == StandardHandleInfo.StandardHandleState.Attached )
			}
			catch ( System.IO.IOException exIo )
			{
				HandleConsoleIOException ( exIo );
			}

			if ( _fSuppressOutput )
			{
				return;
			}	// TRUE (degenerate case) block, if ( _fSuppressOutput )
			else
			{
				if ( pintNCols == WHOLE || pintNCols == _strFiller.Length )
				{   // The default case is a full line.
					Console.Error.Write ( _strFiller );
				}   // TRUE (default case) block, if ( pintNCols == WHOLE || pintNCols == _strFiller.Length )
				else
				{   // Write a short line (used when writing started past the left edge)
					Console.Error.Write (
						_strFiller.Substring (
							LEFT_EDGE ,
							pintNCols ) );
				}   // FALSE (alternate case) block, if ( pintNCols == WHOLE || pintNCols == _strFiller.Length )
			}	// FALSE (standard case) block, if ( _fSuppressOutput )
		}   // private void WriteFiller
        #endregion  // Private Instance Methods
    }   // class FixedConsoleWriter
}   // partial namespace WizardWrx.ConsoleAppAids3