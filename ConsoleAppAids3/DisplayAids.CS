/*
    ============================================================================

    File Name:			DisplayAids.cs

    Namespace:			WizardWrx.ConsoleAppAids3

    Class Name:			DisplayAids

    Synopsis:			Parse valid commands into a Dictionary, discarding 
						invalid arguments.

    Remarks:			The intent of this library is to segregate code that is
						useful only to a console mode application.

    Author:				David A. Gray

	License:            Copyright (C) 2012-2017, David A. Gray. 
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

    Date       Version By  Synopsis
    ---------- ------- --- -----------------------------------------------------
    2012/09/06 1.0     DAG Initial implementation, created from two static
                           methods that I moved from ApplicationHelpers into
                           this new class.

    2014/01/06 3.2     DAG Correct oversight, due to code copying, in this
                           flower box.

                           The code remains the same.

    2014/03/24 3.5     DAG Add new TimedWait, ReportExceptionOnceOnly, and 
                           FixedConsoleWriterAdapter methods, the latter two
                           serving the needs of the former, and made visible
                           for other uses that are sure to arise, probably
                           sooner, rather than later. By marking them public
                           now, I won't need to visit the code again just to do
                           so.

    2014/03/25 3.5     DAG 1) Colors passed into ReportExceptionOnceOnly were
                              being ignored.

                           2) Local variable uintSecondsToWait went unused, and
                              it was optimized away.

    2014/06/06 3.6     DAG Adjust for classes moved out of this class, to
                           decouple it from ApplicationHelpers.

    2014/06/23 5.1     DAG Change WaitForCarbonUnit so that it doesn't scroll up
                           until the operator presses a key.

    2015/06/08 5.51    DAG Redirect output to STDERR, so that it always goes to
                           the console, unless both output streams are
                           redirected.

    2016/04/07 6.0     DAG Finish breaking free of dependence on the old strong
                           name signed class libraries by binding to version
                           6.0.381.32883 of WizardWrx.DllServices2.

	2016/06/09 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically belong
                           in this library being found instead in DLLServices2.

						   As a bonus, I found three hard coded messages that
                           belong in the managed strings, and moved them there,
                           and I got WaitForCarbonUnit to scroll the screen up 1
                           line when standard output is redirected, and finally
                           remembered to display instructions for stopping timed
                           waits early.

	2017/12/06 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.
    ============================================================================
*/


using System;

using WizardWrx;
using WizardWrx.ConsoleStreams;
using WizardWrx.DLLConfigurationManager;


namespace WizardWrx.ConsoleAppAids3
{
    /// <summary>
	/// Use the methods of this sealed (implicitly static) class to precisely
	/// control the way your application handles pauses.
    /// </summary>
    public static class DisplayAids
    {
		/// <summary>
		/// The destructive backspace took some doing to implement.
		/// </summary>
		/// <remarks>
		/// This string should probably go into WizardWrx.Common.SpecialStrings.
		/// </remarks>
		public const string DESTRUCTIVE_BACKSPACE = "\b \b";

        /// <summary>
        /// Use this enumeration to specify whether, and under what conditions,
        /// the TimedWait method may be interrupted by user inputs.
        /// </summary>
        public enum InterruptCriterion
        {
            /// <summary>
            /// The countdown cannot be stopped.
            /// </summary>
            None ,

            /// <summary>
            /// Pressing any key stops the countdown.
            /// </summary>
            AnyKey ,

            /// <summary>
            /// Pressing the ENTER key stops the countdown.
            /// </summary>
            CarriageReturn ,
        }   // InterruptCriterion

		private static readonly string [ ] s_astrTimedWaitPrompts =
		{
			Properties.Resources.MSG_INTERRUPTCRITERION_NONE ,					// InterruptCriterion.None
			Properties.Resources.MSG_INTERRUPTCRITERION_ANYKEY ,				// InterruptCriterion.AnyKey
			Properties.Resources.MSG_INTERRUPTCRITERION_CARRIAGERETURN			// InterruptCriterion.CarriageReturn
		};

        /// <summary>
        /// Filter the FixedConsoleWriter constructor inputs, returning a new
        /// FixedConsoleWriter that uses the current screen colors if the inputs
        /// are equal, and one that uses the specified colors if not.
        /// </summary>
        /// <param name="pclrTextColor"></param>
        /// Use a member of the ConsoleColor enumeration to specify a color in
        /// which to display the countdown message.
        /// <param name="pclrTextBackgroundColor">
        /// Use a member of the ConsoleColor enumeration to specify a color to
        /// use as a background behind the countdown message text.
        /// </param>
        /// <returns>
        /// This method returns a FixedConsoleWriter object that writes text in
        /// the current console colors if both of the specified ConsoleColor
        /// values are the same. Otherwise, it returns a FixedConsoleWriter that
        /// writes in the specified colors.
        /// </returns>
        /// <remarks>
        /// This method ensures that the countdown text is never hidden, without
        /// altering the behavior of the MessageInColor object through which it
        /// displays text on the console. Specifying the same color code for the
        /// foreground and background color permits a MessageInColor object to
        /// display hidden text; which this filter prevents by substituting the
		/// current console colors when the foreground and background colors are
		/// the same.
        /// 
        /// Though intended for use by the companion method TimedWait, I marked
        /// this method public because I feel sure that other applications will
        /// emerge.
		/// 
		/// For the benefit of anyone who want to know whether the author knows
		/// anything about Design Patterns, although the base class is concrete,
		/// in all other respects, this is a straightforward Class Factory.
		/// I was using them long before I knew that anyone had given them a
		/// name.
        /// </remarks>
        public static FixedConsoleWriter FixedConsoleWriterAdapter (
            ConsoleColor pclrTextColor ,
            ConsoleColor pclrTextBackgroundColor )
        {
            if ( pclrTextColor == pclrTextBackgroundColor )
                return new FixedConsoleWriter (
                    Console.ForegroundColor ,
                    Console.BackgroundColor );
            else
                return new FixedConsoleWriter (
                    pclrTextColor ,
                    pclrTextBackgroundColor );
        }   // FixedConsoleWriterAdapter


        /// <summary>
        /// Freeze the screen and display a countdown clock for a specified
        /// number of seconds.
        /// </summary>
        /// <param name="puintWaitSeconds">
        /// Specify the time to wait, in seconds. If this value is zero, a
        /// default value, WAIT_TIME_DEFAULT (currently 30 seconds) is used.
        /// </param>
        /// <param name="pstrCountdownWaitingFor"></param>
        /// Describe what will happen when the time expires. If this value is
        /// null (Nothing in Visual Basic) or the empty string, a default value,
        /// COUNTDOWN_WAITING_FOR (currently "Program ending") is displayed in
        /// the countdown message.
        /// <param name="pclrTextColor"></param>
        /// Use a member of the ConsoleColor enumeration to specify a color in
        /// which to display the countdown message.
        /// <param name="pclrTextBackgroundColor">
        /// Use a member of the ConsoleColor enumeration to specify a color to
        /// use as a background behind the countdown message text.
        /// </param>
        /// <param name="penmInterruptCriterion">
        /// The WizardWrx.ConsoleAppAids3.DisplayAidsInterruptCriterion
        /// enumeration specifies options that can be used to control whether,
        /// and under what conditions, the countdown may be interrupted. The 
        /// default value, None, prevents the user stopping the countdown.
        /// </param>
        /// <remarks>
        /// Rather than spawn 4 overloads, there is a single version of this
        /// method, and all arguments have built-in defaults.
        /// </remarks>
        public static void TimedWait ( 
            uint puintWaitSeconds ,
            string pstrCountdownWaitingFor ,
            ConsoleColor pclrTextColor ,
            ConsoleColor pclrTextBackgroundColor ,
            InterruptCriterion penmInterruptCriterion )
        {
			const int ONE_SECOND = MagicNumbers.MILLISECONDS_PER_SECOND;
			const uint OUT_OF_TIME = MagicNumbers.ZERO;
            const uint STOP_NOW = OUT_OF_TIME + MagicNumbers.PLUS_ONE;			// Set a notch above the stop value, since the loop indexer decrements it before the test.
            const bool SWALLOW_IT = true;
            const uint WAIT_TIME_DEFAULT = 30;

			//  ----------------------------------------------------------------
			//	Evaluate arguments consumed directly by this routine, both of
			//	which have acceptable defaults, so that the method can continue,
			//	even if one or both are invalid.
			//  ----------------------------------------------------------------

			uint uintSecondsToWait = puintWaitSeconds == OUT_OF_TIME
				                                         ? WAIT_TIME_DEFAULT
														 : puintWaitSeconds;

			string strCountdownWaitingFor = string.IsNullOrEmpty ( pstrCountdownWaitingFor )
				                            ? Properties.Resources.TIMEDWAIT_COUNTDOWN_WAITING_FOR
											: pstrCountdownWaitingFor;

			//  ----------------------------------------------------------------
			//	Since an invalid value in penmInterruptCriterion is a coding
			//	error, it throws. Adequately tested production code should never
			//	throw this exception. Regardless, since the exception is caught
			//	and logged locally, the net effect is that this method should
			//	never throw.
			//  ----------------------------------------------------------------

            switch ( penmInterruptCriterion )
            {
                case InterruptCriterion.None:
                case InterruptCriterion.AnyKey:
                case InterruptCriterion.CarriageReturn:
					try
					{	// Believe it or not, writing to the console can throw exceptions, especially when your code supports redirection of the standard console streams.
						if ( pclrTextColor == pclrTextBackgroundColor )
						{	// Fall back to console colors, as will the FixedConsoleWriter object.
							Console.Error.WriteLine ( s_astrTimedWaitPrompts [ ( int ) penmInterruptCriterion ] );
						}	// TRUE (degenerate case) block, if ( pclrTextColor == pclrTextBackgroundColor )
						else
						{	// Display the message in the colors in which the countdown will appear.
							ErrorMessagesInColor.RGBWriteLine (									// During the lifetime of this method, this is a one-off event, precisely why the RGBWriteLine methods exist.
								pclrTextColor ,													// To clarify that the instruction applies to the countdown that is about to start,
								pclrTextBackgroundColor ,										//		display it in the same colors.
								s_astrTimedWaitPrompts [ ( int ) penmInterruptCriterion ] );	// The message is read from a static array of managed string resources.
						}	// FALSE (standard case) block, if ( pclrTextColor == pclrTextBackgroundColor )
					}
					catch ( System.IO.IOException errInputOutput )
					{
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInputOutput );
					}
					catch ( InvalidOperationException errInvalidOperation )
					{
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInvalidOperation );
					}
					catch ( Exception errOtherKinds )
					{
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errOtherKinds );
					}
					break;																	// Using a switch block simplifies adding options.

                default:
					throw new System.ComponentModel.InvalidEnumArgumentException (			// This being thrown is a programming error, period.
						"penmInterruptCriterion" ,											// Since they are code names, which never change, argument names never go into the managed string resources.
						( int ) penmInterruptCriterion ,									// Casting the enumeration to integer makes its numeric value visible.
						penmInterruptCriterion.GetType ( ) );								// Though it might be a bit more expensive, I prefer calling GetType to using the TypeOf operator, because I think it's clearer.
            }   // switch ( penmInterruptCriterion )

            //  ----------------------------------------------------------------
            //  Prevent jumping the gun if the method is interruptible.
            //  ----------------------------------------------------------------

            if ( penmInterruptCriterion != InterruptCriterion.None )
                DrainKeyboardBuffer ( );

			//  ----------------------------------------------------------------
			//	It's finally safe to hijack the console. Doing so any sooner
			//	causes the instructional prompt displayed by the RGBWriteLine
			//	to be overwritten. This is by design, since a FixedConsoleWriter
			//	must lock the vertical position of the cursor at the current 
			//	line, which it will reuse until the countdown expires or is 
			//	interrupted (input parameters permitting).
			//  ----------------------------------------------------------------

			FixedConsoleWriter cwDisplayColors = FixedConsoleWriterAdapter (	// This method is an adapter, because it hides the WriteLine methods, which are inappropriate in this context.
				pclrTextColor ,													// While it would be slightly more efficient to set the console colors directly,
				pclrTextBackgroundColor );										// the result would be too much of the screen being painted with the background color.
			System.Text.StringBuilder sbOfBackspaces = null;					// This is used only when standard output is redirected, but it needs method scope, because it must survive iterations of a For loop.

			//	------------------------------------------------------------
			//	Hoist these expensive trips deep into the object graph above
			//	the loop.
			//	------------------------------------------------------------

			StandardHandleInfo.StandardHandleState enmStdoutState = StateManager.GetTheSingleInstance ( ).StandardHandleState ( StandardHandleInfo.StandardConsoleHandle.StandardOutput );
			string strprogressMsgTpl = Properties.Resources.TIMEDWAIT_COUNTDOWN_MSG_TPL;

			//  ----------------------------------------------------------------
			//	Displaying the initial and final counts requires method scope
			//	for uintSecondsWaited, which must be initialized to zero, even
			//	though the first iteration of the FOR loop skips its third
			//	clause. This is because the increment clause executes one extra
			//	time, on the terminal iteration, when the body of the IF block
			//	is skipped.
			//  ----------------------------------------------------------------

			uint uintSecondsWaited = OUT_OF_TIME;

			for ( uint uintSecodsLeft = uintSecondsToWait ;
                       uintSecodsLeft > OUT_OF_TIME ;
                       uintSecodsLeft--,										// Not only is this the criterion index, but the code within the loop uses it.
					   uintSecondsWaited++ )									// Document that this is incremented once on each iteration. Otherwise, the body of the loop ignores it.
            {
				//	------------------------------------------------------------
				//	By constructing the entire message in a string, we can get
				//	its length with almost no effort, and use that to work out
				//	how many backspaces we need to return the carriage to the
				//	beginning of the line, clearing the screen as we go.
				//	------------------------------------------------------------

				try
				{
					if ( enmStdoutState == StandardHandleInfo.StandardHandleState.Redirected )
					{	// Unless standard output is redirected, this extra effort is wasteful.
						string strProgressMessage = string.Format (
							strprogressMsgTpl ,									// Format control string
							strCountdownWaitingFor ,							// Format Item 0 = Terse description of reason for pause
							uintSecodsLeft );									// Format Item 1 = Seconds remaining on countdown clock

						if ( sbOfBackspaces  != null )
						{	// On the first iteration, there is nothing to do, and the StringBuilder is uninitialized.
							cwDisplayColors.Write ( sbOfBackspaces.ToString ( ) );
						}	// if ( sbOfBackspaces  != null )
						
						cwDisplayColors.Write ( strProgressMessage );

						int intNBackspaces = strProgressMessage.Length;

						sbOfBackspaces = new System.Text.StringBuilder ( intNBackspaces );

						for ( int intChrIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intChrIndex < intNBackspaces ; intChrIndex++ )
						{
							sbOfBackspaces.Append ( DESTRUCTIVE_BACKSPACE );
						}	// for ( int intChrIndex = ArrayInfo.ARRAY_FIRST_ELEMENT ; intChrIndex < intNBackspaces ; intChrIndex++ )
					}	// TRUE (Standard Output is redirected. Do it the harder way.) block, if ( enmStdoutState == StandardHandleInfo.StandardHandleState.Redirected )
					else
					{	// The shortcut is a direct call to the Write method, using the implicit string formatter that hands the string directly to the stream.
						cwDisplayColors.Write (
							strprogressMsgTpl ,									// Format control string
							strCountdownWaitingFor ,							// Format Item 0 = Terse description of reason for pause
							uintSecodsLeft );									// Format Item 1 = Seconds remaining on countdown clock
					}	// FALSE (Standard Output is attached to its console. Take the shortcut.) block, if ( enmStdoutState == StandardHandleInfo.StandardHandleState.Redirected )
				}
				catch ( System.IO.IOException errInputOutput )
				{
					ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInputOutput );
				}
				catch ( InvalidOperationException errInvalidOperation )
				{
					ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInvalidOperation );
				}
				catch ( Exception errOtherKinds )
				{
					ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errOtherKinds );
				}

                System.Threading.Thread.Sleep ( ONE_SECOND );

                if ( penmInterruptCriterion == InterruptCriterion.AnyKey || penmInterruptCriterion == InterruptCriterion.CarriageReturn )
                {	// If interruption is permitted, check for the keys(s) allowed to signal a desire to do so.
                    try
                    {	// Reading the console is an I/O operation that must occur within a try block. 
						ConsoleKeyInfo cki;

                        if ( Console.KeyAvailable )
                        {   // Check for a key.
                            if ( penmInterruptCriterion == InterruptCriterion.AnyKey )
                            {   // Any key will do.
                                cki = Console.ReadKey ( SWALLOW_IT );
                                uintSecodsLeft = STOP_NOW;
                            }   // TRUE block, if ( penmInterruptCriterion == InterruptCriterion.AnyKey )
                            else
                            {   // Only certain keys do something.
                                cki = Console.ReadKey ( SWALLOW_IT );

                                if ( cki.Key == ConsoleKey.Enter )
                                {   // We brake for ENTER keys.
                                    uintSecodsLeft = STOP_NOW;
                                }   // if ( cki.Key == ConsoleKey.Enter )
                            }   // FALSE block, if ( penmInterruptCriterion == InterruptCriterion.AnyKey )
                        }   // if ( Console.KeyAvailable )
                    }
                    catch ( System.IO.IOException errInputOutput )
                    {
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInputOutput );
                    }
                    catch ( InvalidOperationException errInvalidOperation )
                    {
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errInvalidOperation );
                    }
                    catch ( Exception errOtherKinds )
                    {
						ExceptionLogger.GetTheSingleInstance ( ).ReportException ( errOtherKinds );
                    }
                }   // if ( penmInterruptCriterion == InterruptCriterion.AnyKey || penmInterruptCriterion == InterruptCriterion.CarriageReturn )
            }   // for ( uintSecodsLeft = uintSecondsToWait ; uintSecodsLeft > TIME_IS_UP ; uintSecodsLeft-- )

			//	----------------------------------------------------------------
			//	StringBuilder sbOfBackspaces is null unless standard output is
			//	redirected. In that case, not only is it initialized, but its
			//	contents must be written onto the console, to back the display
			//	up to the beginning of the area where the progress messages go.
			//	----------------------------------------------------------------

			if ( sbOfBackspaces != null )
				cwDisplayColors.Write ( sbOfBackspaces.ToString ( ) );

			if ( uintSecondsWaited == uintSecondsToWait )
			{	// The wait was uninterrupted.
				cwDisplayColors.Write ( 
					Properties.Resources.TIMEDWAIT_DONE_MSG_TPL ,				// Format control string
					strCountdownWaitingFor );									// Format Item 0 = Format Item 0 = Terse description of reason for pause
			}	// TRUE (default case) block, if ( uintSecondsWaited == uintSecondsToWait )
			else
			{	// A carbon unit said "that's long enough."
				cwDisplayColors.Write (
					Properties.Resources.TIMEDWAIT_DONE_EARLY_MSG_TPL ,			// Format control string
					strCountdownWaitingFor ,									// Format Item 0 = Format Item 0 = Terse description of reason for pause
					uintSecondsWaited ,											// Format Item 1 = Time actually waited
					uintSecondsToWait );										// Format Item 2 = Maximum wait time
			}	// FALSE (A carbon unit intervened.) block, if ( uintSecondsWaited == uintSecondsToWait )

			cwDisplayColors.ScrollUp ( );										// Scroll the screen up one line.
			DrainKeyboardBuffer ( );											// To prevent them being misinterpreted as input to a subsequent prompt, discard any remaining keys.
        }   // public static void TimedWait


        /// <summary>
        /// Display a message, and wait until an operator presses the ENTER
        /// (Return) key.
        /// </summary>
        /// <remarks>
        /// This simple overload, which actually came into being second,
        /// displays a default prompt, "Please press the ENTER (Return) key to
        /// exit the program."
        /// 
        /// The intent of this routine is to ensure that a report or important
        /// message is actually read by a human.
        /// 
        /// Consequently, this routine is very picky about the responses it
        /// accepts; only the ENTER key is accepted. To prevent the prompt being
        /// scrolled off the screen, all input is eaten (not echoed).
        /// </remarks>
        public static void WaitForCarbonUnit ( )
        {
            WaitForCarbonUnit ( null );
        }   // static void WaitForCarbonUnit (1 of 2)


        /// <summary>
        /// Display a message, and wait until an operator presses the ENTER
        /// (Return) key. All cursor motion is suppressed, so that, on exit, the
        /// cursor is exactly where the Console.Write method that displays the
        /// prompt left it. Bo this means does this method avoid unexpected
        /// screen scrolling.
        /// </summary>
        /// <param name="pstrPrompt">
        /// This string specifies an alternative message to display. If this is
        /// an empty string or null reference, a default message, "Please press
        /// the ENTER (Return) key to exit the program." is shown.
        /// 
        /// If you want the default message, there is a new overload, which has
        /// no arguments, that does so, by calling this method, internally, with
        /// a null reference. Having one routine that does the work simplifies
        /// changing the behavior of both overloads, should the need arise.
        /// </param>
        /// <remarks>
        /// The intent of this routine is to ensure that a report or important
        /// message stands a better chance of actually being seen and read by a
        /// human.
        /// 
        /// Consequently, this routine is very picky about the responses it
        /// accepts; only the ENTER key is accepted. To prevent the prompt being
        /// scrolled off the screen, all input is eaten (not echoed).
        /// 
        /// Moreover, you can't jump the gun. Any characters that accumulate in
        /// the keyboard buffer before the prompt is displayed are swallowed
        /// whole, with no outward sign that they were eaten. Though I could
        /// display a message, doing so would, in most cases, be seen as noise
        /// I might be persuaded to add it to a future version, as an option.
        /// </remarks>
        public static void WaitForCarbonUnit ( string pstrPrompt )
        {
            const bool ECHO_OFF = true;

			//	----------------------------------------------------------------
			//	Draining the keyboard buffer prevents a stray ENTER key that
			//	happens to be in the buffer from being misinterpreted as an
			//	acknowledgment from a carbon unit.
			//	----------------------------------------------------------------

			DrainKeyboardBuffer ( );

            if ( string.IsNullOrEmpty ( pstrPrompt ) )
                Console.Error.Write ( Properties.Resources.MSG_CARBON_UNIT_DEFAULT );
            else
				Console.Error.Write ( pstrPrompt );

            Console.Beep ( );   // A default beep is sufficient.
            ConsoleKeyInfo cki;

            do
            {   // Only the ENTER key will do.
                cki = Console.ReadKey ( ECHO_OFF );
            } while ( cki.Key != ConsoleKey.Enter );

			//	----------------------------------------------------------------
			//	When standard output is redirected, there is nothing to follow
			//	that will return the logical carriage. Make it so.
			//	----------------------------------------------------------------

			if ( ConsoleAppStateManager.GetTheSingleInstance ( ).BaseStateManager.StandardHandleState ( StandardHandleInfo.StandardConsoleHandle.StandardOutput ) == StandardHandleInfo.StandardHandleState.Redirected )
				Console.Error.WriteLine ( );
        }   // public static void WaitForCarbonUnit (2 of 2)


        /// <summary>
        /// Discard all remaining keys in the console input buffer.
        /// </summary>
        /// <remarks>
        /// There are several reasons that you might want to do this. The method
        /// began as an internal loop in WaitForCarbonUnit. Then, while testing
        /// the library, I realized that I have another routine that needs to do
        /// the same thing, which caused me to realize that there might be other
        /// uses, too.
        /// </remarks>
        public static void DrainKeyboardBuffer ( )
        {
            const bool ECHO_OFF = true;

            if ( Console.KeyAvailable )
            {   // If there are any, eat them.
                ConsoleKeyInfo cki;

                do
                {
                    cki = Console.ReadKey ( ECHO_OFF );
                } while ( Console.KeyAvailable );
            }   // if ( Console.KeyAvailable )
        }   // public static void DrainKeyboardBuffer
    }   // public sealed class DisplayAids
}   // partial namespace WizardWrx.ConsoleAppAids3