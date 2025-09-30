/*
    ============================================================================

    Namespace:          WizardWrx.ConsoleAppAids3

    Class Name:         ConsoleAppStateManager

    File Name:          ConsoleAppStateManager.cs

    Synopsis:           Extend class WizardWrx.DllServices2.StateManager with
                        methods tailored to the needs of character mode,
                        a. k. a. console mode, applications.

    Remarks:            This class adds features applicable to character mode
                        programs, extending WizardWrx.DllServices2.StateManager,
                        by incorporating an instance as a property of itself. I
						considered inheritance, but opted against it, since both
						classes are singletons, and I didn't want to investigate
						how a singleton that derives from another singleton
						would behave. Besides, implementing it as a property
						maintains a clear distinction between its properties and
						those of the embedded StateManager. The final nail in
						the inheritance coffin is that the current versions of
						both classes now inherit from GenericSingletonBase, a
						generic abstract base class defined in WizardWrs.Core 
                        that hides most of the plumbing required for the
                        Singleton in their common base class.

    Created:            Sunday, 18 May 2014

	Author:             David A. Gray

	License:            Copyright (C) 2014-2024, David A. Gray. 
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
    ---------- ------- --- -----------------------------------------------------
    2014/05/18 4.1     DAG Initial implementation.

    2014/06/23 5.1     DAG Further fine tuning of the namespace.

    2014/07/23 5.2     DAG Allow ErrorExit to fail gracefully if the message
                           table is empty.

    2014/12/15 5.4     DAG Swap the order of the calls to the WaitForCarbonUnit
                           and DisplayEOJMessage methods, so that the end-of-job
                           message is displayed before the carbon unit prompt.

    2016/04/09 6.0     DAG Finish breaking free of dependence on the old strong
                           name signed class libraries by binding to version
                           6.0.381.32883 of WizardWrx.DllServices2.

	2016/06/08 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically belong
                           in this library being found instead in DLLServices2.

	2017/08/05 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.

	2018/09/18 7.0     DAG Mark GetTheSingleInstance as a New (override) method.

	2018/11/26 7.0     DAG Eliminate the unreferenced system namespaces, and tag
                           my own assembly references with explanatory notes.

	2020/10/23 7.2     DAG 1) Explicitly hide the default constructor to prevent
                              the compilers from allowing it to be called. This
                              oversight came to the surface when I inadvertently
                              wrote a call to the default constructor in another
                              assembly, WWDisplayAssemblyInfo.exe.

                           2) Implement Semantic Version Numbering. This build
                              also incorporates the current stable versions of
                              the WizardWrx .NET API asseblies, which finally
                              fully implement Semantic Version Numbering as of a
                              few days ago.

    2021/03/05 8.0     DAG LoadBasicErrorMessages replaces LoadErrorMessageTable
                           on the attached BaseStateManager object.

    2021/06/06 8.0.535 DAG Build against the most recent WizardWrx .NET API
                           library constellation.

    2022/04/13 8.1.555 DAG 1) Method NormalExit on a ConsoleAppStateManager 
                              object neglected to print the message that
                              corresponds to the status code from the table of
                              static error messages. Hence, relying on this
                              method left you with a status code (somewhat
                              useful) but no explanatory message, nor any
                              evidence of the nonzero status code in the console
                              stream.

                           2) Fixing the above-noted issue exposed a bug that
                              caused the new LoadBasicErrorMessages method that
                              was intended to supersede the StateManager
                              LoadErrorMessageTable method to throw away the
                              table that it built, leaving the message table
                              perpertually uninitialized.

    2022/10/15 8.1.570 DAG Add a method that takes a NormalExitAction and a
                           Boolean value that, when True, causes the method to
                           disregard the status code when its value exceeds the
                           size of the message table because its objective is to
                           control the action of its calling process, which is
                           usually a shell script.

    2022/12/31 9.0.0   DAG SetCWDRelativeToEntryAssemblyPath establishes the CWD
                           relative to the entry assembly path, returning the
                           resulting absolute (fully qualified) path.

    2024/01/11 9.0.571 DAG Display the message associated with a nonzero exit
                           code.

    2025/08/13 9.0.573 DAG Add an optional NormalExitAction parameter to the
                           ErrorExit method.
    ============================================================================
*/


using System;

using WizardWrx.DLLConfigurationManager;                                        // This class wraps objects exposed by this library.


namespace WizardWrx.ConsoleAppAids3
{
    /// <summary>
	/// ConsoleAppStateManager is an WizardWrx.DLLServices2.StateManager
	/// adapter, which exposes the adapted object through its read only
	/// BaseStateManager property, and extends it with methods that provide
	/// services applicable exclusively to character mode (console mode)
	/// programs.
    /// </summary>
	/// <remarks>
	/// Internally, ConsoleAppStateManager uses some StateManager services that
	/// could reasonably be expected to be defined locally. In particular, the
	/// services for ascertaining the redirection state of the three standard
	/// console handles and the names of the files to which they are redirected
	/// seem logically out of place.
    /// 
    /// They are in that library because the ExceptionLogger class, which 
    /// belongs where it is because it meets needs that are substantially
    /// identical, regardless of the Windows subsystem in which it runs. While I
    /// could have made a wrapper that inherits from the ExceptionLogger, that
    /// seemed pointless for the few lines of code that would have gone into the
    /// derived class, when this library has unrelated dependencies on
    /// WizardWrx.Core.
	/// </remarks>
    public class ConsoleAppStateManager : GenericSingletonBase<ConsoleAppStateManager>
    {
        #region Public Constants and Enumerations
        /// <summary>
        /// This enumeration governs when the NormalExit method suspends a task
        /// by calling WaitForCarbonUnit.
        /// </summary>
        public enum NormalExitAction
        {
            /// <summary>
            /// Always exit immediately, regardless of the status code.
            /// </summary>
            ExitImmediately ,

            /// <summary>
            /// Exit immediately if the status code is ERROR_SUCCESS (zero).
            /// Otherwise, call WaitForCarbonUnit.
            /// </summary>
            HaltOnError ,

            /// <summary>
            /// Exit silently, without displaying any text on the console.
            /// </summary>
            Silent ,

            /// <summary>
            /// Exit after allowing a specified amount of time for an operator
            /// to read the error message.
            /// </summary>
            Timed ,

            /// <summary>
            /// Always call WaitForCarbonUnit.
            /// </summary>
            WaitForOperator
        }   // public enum NormalExitAction


        /// <summary>
        /// This constant specifies the default number of seconds to wait when
        /// NormalExitAction is Timed. Its current value of zero causes a wait
        /// of up to 30 seconds, which should be about right most of the time.
        /// </summary>
		public const uint TIMED_WAIT_DEFAULT_SECONDS = MagicNumbers.ZERO;


        /// <summary>
        /// This constant specifies a default description of the event that
        /// happens when the time expires or the wait is interrupted (canceled),
        /// when NormalExitAction is Timed.
        /// 
        /// Currently, the default description is "Program ending," which is
        /// taken from a resource string in the WizardWrx.ConsoleAids class that
        /// implements this feature.
        /// </summary>
        public const string TIMED_WAIT_WAITING_FOR_DEFAULT = SpecialStrings.EMPTY_STRING;

        /// <summary>
        /// This constant specifies the default method by which the timed wait
        /// can be interrupted (canceled) when NormalExitAction is Timed.
        /// 
        /// Currently, the default method of interrupting the timed wait is by
        /// pressing the ENTER (Return) key once (CarriageReturn).
        /// </summary>
        public const DisplayAids.InterruptCriterion TIMED_WAIT_INTERRUPT_CRITERION = DisplayAids.InterruptCriterion.CarriageReturn;


        /// <summary>
        /// Use this member of the ConsoleColor enumeration, along with the
        /// TIMED_WAIT_BACKGROUND_COLOR_DEFAULT, to instruct an ExceptionLogger
        /// object to render text displayed in toe standard output window of a
        /// console mode application in its default screen colors.
        /// </summary>
        public const ConsoleColor TIMED_WAIT_TEXT_COLOR_DEFAULT = ConsoleColor.Black;


        /// <summary>
        /// Use this member of the ConsoleColor enumeration, along with the
        /// TIMED_WAIT_TEXT_COLOR_DEFAULT, to instruct an ExceptionLogger object
        /// to render text displayed in toe standard output window of a console
        /// mode application in its default screen colors.
        /// </summary>
        public const ConsoleColor TIMED_WAIT_BACKGROUND_COLOR_DEFAULT = ConsoleColor.Black;
        #endregion  // Public Constants and Enumerations


        #region Public Instance Properties
        /// <summary>
        /// Expose the real state manager behind this adapter.
        /// </summary>
        public StateManager BaseStateManager
        { get { return _me; } }
        #endregion  // Public Instance Properties


        #region Private Instance Storage
        private StateManager _me = null;
        private bool _fBoJMessageGenerated = false;
        #endregion  // Private Instance Storage


        #region Singleton Infrastructure
		private static readonly SyncRoot s_srCriticalSection = new SyncRoot ( typeof ( ConsoleAppStateManager ).ToString ( ) );


        /// <summary>
        /// A fundamental tenet of the Singleton design pattern is that the
        /// default constructor must be hidden, and it must be the only
        /// constructor of any kind.
        /// 
        /// Since the base class has a protected default constructor, derived
        /// classes must explicitly hide theirs to prevent the compiler allowing
        /// one to be called.
        /// </summary>
        private ConsoleAppStateManager ( )
        {
        }   // Since the base class has a protected default constructor, derived classes must explicitly hide theirs.

        /// <summary>
        /// Get a reference to the ConsoleAppStateManager singleton, which
        /// organizes a host of useful application state information under one
        /// object.
        /// </summary>
        /// <returns>
        /// The return value is the initialized singleton.
        /// </returns>
        /// <remarks>
        /// This method must override and hide the like named method on the base
        /// class, because it has extra work to do.
        /// </remarks>
        public new static ConsoleAppStateManager GetTheSingleInstance ( )
        {
			if ( s_genTheOnlyInstance._me == null )
				lock ( s_srCriticalSection )
					if ( s_genTheOnlyInstance._me == null )
						s_genTheOnlyInstance._me = StateManager.GetTheSingleInstance ( );

			return s_genTheOnlyInstance;
        }   // public static ConsoleAppStateManager GetTheSingleInstance ( )
        #endregion  // Singleton Infrastructure


        #region LoadBasicErrorMessages Methods
        /// <summary>
        /// Load a set of invariant message strings for use with the program end
        /// message generators.
        /// </summary>
        /// <param name="pastrAdditionalMessages">
        /// When the program has only the basic status messages for clean 
        /// completion and runtime exceptions, this argument may be either NULL
        /// or a reference to an empty array, both of which get the same
        /// treatment.
        /// </param>
        /// <returns>
        /// The return value is total count of exception messages, which is
        /// always at least two.
        /// </returns>
        public int LoadBasicErrorMessages ( string [ ] pastrAdditionalMessages = null )
        {
            int rintMessageCount = ComputeMessageCount ( pastrAdditionalMessages );

            string [ ] astrAllMessages = new string [ rintMessageCount ];

            astrAllMessages [ MagicNumbers.ERROR_SUCCESS ] = Common.Properties.Resources.ERRMSG_SUCCESS;
            astrAllMessages [ MagicNumbers.ERROR_RUNTIME ] = Common.Properties.Resources.ERRMSG_RUNTIME;

            int intNewMessages2Add = rintMessageCount - STANDARD_MESSAGE_COUNT;

            //  ----------------------------------------------------------------
            //  This FOR loop uses a double index to keep two array indices in
            //  sync, of which only the second, intK, needs a loop limit test.
            //  ----------------------------------------------------------------

            for ( int intJ = STANDARD_MESSAGE_COUNT,
                      intK = ArrayInfo.ARRAY_FIRST_ELEMENT ;

                      intK < intNewMessages2Add ;

                      intJ++,
                      intK++ )
            {
                astrAllMessages [ intJ ] = pastrAdditionalMessages [ intK ];
            }   // for ( int intJ = STANDARD_MESSAGE_COUNT, intK = ArrayInfo.ARRAY_FIRST_ELEMENT ; intK < intNewMessages2Add ; intJ++, intK++ )

            //  ----------------------------------------------------------------
            //  Since the table of error messages that belongs to StateManager
            //  object BaseStateManager is read-only, its LoadErrorMessageTable
            //  method must be employed to load the locally constructed message
            //  list into it.
            //  ----------------------------------------------------------------

            _me.LoadErrorMessageTable ( astrAllMessages );
            return rintMessageCount;
        }   // public int LoadBasicErrorMessages
        #endregion  // LoadBasicErrorMessages Methods


        #region BOJ and EOJ Message Displays
        /// <summary>
        /// When called for the first time, this method displays a BOJ message
        /// on the console. Subsequent calls return immediately, without taking
        /// any action.
        /// </summary>
        /// <remarks>
        /// This method is a wafer-thin wrapper around GetBOJMessage, and it
        /// uses a thread-safe method to write its output on the console.
        /// </remarks>
        public void DisplayBOJMessage ( )
        {
            if ( !_fBoJMessageGenerated )
                Console.WriteLine ( GetBOJMessage ( ) );
        }   // public void DisplayBOJMessage method


		/// <summary>
        /// This method takes GetEOJMessage a step further by passing its return
        /// value to a thread-safe console writer.
        /// </summary>
        /// <remarks>
        /// Access to this method is synchronized by an internally managed
        /// object lock.
        /// </remarks>
        public void DisplayEOJMessage ( )
        {
            Console.WriteLine ( GetEOJMessage ( ) );
        }   // public void DisplayEOJMessage method
        #endregion // BOJ and EOJ Message Displays                                 


        #region ErrorExit Methods
        /// <summary>
        /// Display an error message, read from a table of static strings, and
        /// exit, returning the exit code. See Remarks.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the subscript of the message, and it
        /// becomes the program's exit code. See Remarks.
        /// </param>
        /// <param name="penmNormalExitAction">
        /// When specified, this optioal NormalExitAction enumeration member is
        /// used to override the default, Timed.
        /// </param>
        /// <remarks>
        /// You must supply the messages as an array of strings, by calling
        /// instance method LoadErrorMessageTable.
        ///
        /// After the message is displayed, static method WaitForCarbonUnit
        /// is called with a null string reference, causing it to display its
        /// default prompt, and wait until an operator presses the RETURN key.
        ///
        /// When WaitForCarbonUnit returns, the DisplayEOJMessage method on the
        /// singleton instance is called to display the end of job message,
        /// along with the ending time and elapsed time, and control is returned
        /// to the OS, sending along the exit code.
        /// </remarks>
        public void ErrorExit ( uint puintStatusCode , NormalExitAction penmNormalExitAction = NormalExitAction.Timed )
        {
            ReportNonZeroStatusCode ( ( int ) puintStatusCode );

            DisplayEOJMessage ( );

            if ( System.Diagnostics.Debugger.IsAttached )
            {
                DisplayAids.WaitForCarbonUnit ( null );
			}   // TRUE (The process is attached to a debugger.) block, if ( System.Diagnostics.Debugger.IsAttached )
			else if ( puintStatusCode > MagicNumbers.ERROR_SUCCESS )
            {
                switch ( penmNormalExitAction )
                {
                    case NormalExitAction.Timed:
                        DisplayAids.TimedWait (
                            TIMED_WAIT_DEFAULT_SECONDS ,                        // uint puintWaitSeconds
                            TIMED_WAIT_WAITING_FOR_DEFAULT ,                    // string pstrCountdownWaitingFor
                            TIMED_WAIT_TEXT_COLOR_DEFAULT ,                     // ConsoleColor pclrTextColor
                            TIMED_WAIT_BACKGROUND_COLOR_DEFAULT ,               // ConsoleColor pclrTextBackgroundColor
                            TIMED_WAIT_INTERRUPT_CRITERION );                   // InterruptCriterion penmInterruptCriterion
                        break;
                    case NormalExitAction.ExitImmediately:
					case NormalExitAction.Silent:
						break;
                    case NormalExitAction.HaltOnError:
					case NormalExitAction.WaitForOperator:
						DisplayAids.WaitForCarbonUnit ( $"{Properties.Resources.MSG_NONZERO_EXIT_CODE}{Environment.NewLine}{Environment.NewLine}{Properties.Resources.MSG_CARBON_UNIT_DEFAULT}" );
						break;
				}   // switch ( penmNormalExitAction )
			}   // TRUE (The process has a positive return code, and it is detached from all debuggers.) block, else if ( puintStatusCode > MagicNumbers.ERROR_SUCCESS )

			Environment.Exit ( ( int ) puintStatusCode );
        }   // public void ErrorExit
        #endregion   // ErrorExit Methods


        #region BOJ and EOJ Methods
        /// <summary>
        /// When called for the first time, this method returns a BOJ message,
        /// ready for display on the console. Subsequent calls return an empty
        /// string.
        /// </summary>
        /// <returns>
        /// The first call returns a message for display on the console. All
        /// subsequent calls return an empty string, indicating that another
        /// thread already requested a message, and has, presumably, displayed
        /// it.
        /// </returns>
        /// <remarks>
        /// Access to this method is synchronized by an internally managed
        /// object lock.
        /// </remarks>
        public string GetBOJMessage ( )
        {
            if ( !_fBoJMessageGenerated )
            {   // Method has never been called against this instance.
				lock ( s_srCriticalSection )
				{   // Thread-safe we must always be.
					_fBoJMessageGenerated = true;

					if ( _me.ShowUTCTime )
					{   // By default, the local and UTC startup times are shown.
						return string.Format (
							Properties.Resources.CONSOLE_APP_BOJ ,							// Message Template
							new object [ ]
                                {
                                    _me.GetAssemblyProductAndVersion ( ) ,					// Token 0
                                    Environment.NewLine ,									// Token 1
                                    _me.ConsoleMessageTimeFormat.FormatThisTime (
                                        _me.AppStartupTimeLocal ) ,							// Token 2
                                    _me.ConsoleMessageTimeFormat.FormatThisTime (
                                        _me.AppStartupTimeUtc ) } );						// Token 3

					}   // TRUE (default) block, if ( _me.ShowUTCTime )
					else
					{   // User doesn't want the UTC times.
						return string.Format (
							Properties.Resources.CONSOLE_APP_BOJ_NO_UTC ,					// Message Template
							new object [ ] {
                                _me.GetAssemblyProductAndVersion ( ) ,						// Token 0
                                Environment.NewLine ,										// Token 1
                                _me.ConsoleMessageTimeFormat.FormatThisTime (
                                    _me.AppStartupTimeLocal ) } );							// Token 2
					}   // FALSE (alternate) block, if ( _me.ShowUTCTime )
				}   // lock ( lock ( s_srCriticalSection ) )
            }   // TRUE block, if ( !_fBoJMessageGenerated )
            else
            {   // One BOJ message per instance, please!
                return string.Empty;
            }   // FALSE block, if ( !_fBoJMessageGenerated )
        }   // public string GetBOJMessage method


        /// <summary>
        /// This method returns a new EOJ message each time it is called.
        /// </summary>
        /// <returns>
        /// The returned string is ready to print on the console, by calling the
        /// Console.WriteLine method.
        /// </returns>
        /// <remarks>
        /// Access to this method is synchronized by an internally managed
        /// object lock.
        /// </remarks>
        public string GetEOJMessage ( )
        {
            string strMsgWork = null;

			lock ( s_srCriticalSection )
            {
                DateTime dtmEndOfJob = DateTime.Now;

                if ( _me.ShowUTCTime )
                {   // By default, the local and UTC startup times are shown.
                    strMsgWork = string.Format (
                        Properties.Resources.CONSOLE_APP_EOJ ,								// Format string
                        new object [ ] {
                             Environment.NewLine ,                                  		// Token 0
                             _me.AppRootAssemblyFileBaseName ,                   			// Token 1
                             _me.ConsoleMessageTimeFormat.FormatThisTime (
                                dtmEndOfJob ) ,                                     		// Token 2
                             _me.ConsoleMessageTimeFormat.FormatThisTime (
                                dtmEndOfJob.ToUniversalTime ( ) ),                  		// Token 3
                             _me.AppUptime } ) ;                         					// Token 4
                }  // TRUE (default) block, if ( _me.ShowUTCTime )
                else
                {  // User doesn't want the UTC times.
                    strMsgWork = string.Format (
						Properties.Resources.CONSOLE_APP_EOJ_NO_UTC ,               		// Format string
                        new object [ ] {
                             Environment.NewLine ,                                  		// Token 0
                             _me.AppRootAssemblyFileBaseName ,                       		// Token 1
                             _me.ConsoleMessageTimeFormat.FormatThisTime (
                                dtmEndOfJob ) ,                                     		// Token 2
                             _me.AppUptime } );                         					// Token 3
                }   // FALSE (alternate) block, if ( _me.ShowUTCTime )
			}   // lock ( s_srCriticalSection )

            //  ----------------------------------------------------------------
            //  If the line is too wide to fit on one line, we'll decide where
            //  to split it.
            //  ----------------------------------------------------------------

            if ( strMsgWork.Length > Console.WindowWidth )
                return strMsgWork.Replace (
					Properties.Resources.CONSOLE_APP_EOJ_SPLIT_TOKEN ,              		// Format template
                    string.Format (
						Properties.Resources.CONSOLE_APP_EOJ_REPLACEMENT_TOKEN ,    		// Token 0
						Environment.NewLine ) );                                    		// Token 1
            else
                return strMsgWork;
        }  // public string GetEOJMessage method
		#endregion	// BOJ and EOJ Pseudo-Properties


		#region NortmalExit Methods
		/// <summary>
        /// Exit the program normally, returning the status code stored in this
        /// instance, and optionally call WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard. If WaitForCarbonUnit is called, its default
        /// message is used.
        /// </summary>
        /// <remarks>
        /// This should have been the way the original version worked. Since it
        /// is the simplest, I forced the original implementation to surrender
        /// its hold on the number one slot.
        ///
        /// This method calls overload 4, passing in a null reference for the
        /// carbon unit prompt and an immediate exit instruction.
        /// </remarks>
        public void NormalExit ( )
        {
            NormalExit (
                ( uint ) _me.AppReturnCode ,												// Use the exit code stored in the instance.
                null ,																		// A null pstrOperatorPrompt means use default message.
                NormalExitAction.ExitImmediately ) ;										// Assume we are batch, and maybe even headless.
        }   // public void NormalExit (1 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code. If running in debug mode, use WaitForCarbonUnit to block until
        /// the tester has a chance to read the output or capture it into the
        /// clipboard.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the program's exit code.
        /// </param>
        /// <param name="pstrOperatorPrompt">
        /// This string specifies an alternative message for method
        /// WaitForCarbonUnit to display. If this is an empty string or null
        /// reference, a default message, "Please press the ENTER (Return) key
        /// to exit the program." is shown.
        /// </param>
        /// <remarks>
        /// This is the original implementation, since pre-empted by the simpler
        /// method call that takes no arguments.
        ///
        /// When I implemented the #if DEBUG conditional compilation block, I
        /// didn't take into consideration that the only time that #if DEBUG is
        /// true is when the debug version of this library is built. I decided
        /// to leave it in, as a reminder to myself of how it can be effectively
        /// used with some of the new overloads.
        /// </remarks>
        public void NormalExit (
            uint puintStatusCode ,
            string pstrOperatorPrompt )
        {
            ReportNonZeroStatusCode ( ( int ) puintStatusCode );
            DisplayEOJMessage ( );
            DisplayAids.WaitForCarbonUnit ( pstrOperatorPrompt );
            Environment.Exit ( ( int ) puintStatusCode );
        }   // public void NormalExit (2 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code. If running in debug mode, use WaitForCarbonUnit to block until
        /// the tester has a chance to read the output or capture it into the
        /// clipboard. Regardless, if WaitForCarbonUnit is called, its default
        /// message is displayed.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the program's exit code.
        /// </param>
        /// <remarks>
        /// This method calls overload 4, passing in a null reference for the
        /// carbon unit prompt and an immediate exit instruction.
        /// </remarks>
        public void NormalExit ( uint puintStatusCode )
        {
            NormalExit (
                puintStatusCode ,															// Pass along the caller's exit code.
                null ,																		// A null pstrOperatorPrompt means use default message.
				NormalExitAction.ExitImmediately );											// Assume we are batch, and maybe even headless.
        }   // public void NormalExit (3 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code, and optionally calling WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the program's exit code.
        /// </param>
        /// <param name="pstrOperatorPrompt">
        /// This string specifies an alternative message for method
        /// WaitForCarbonUnit to display. If this is an empty string or null
        /// reference, a default message, "Please press the ENTER (Return) key
        /// to exit the program." is shown.
        /// </param>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <remarks>
        /// This method differs sufficiently from overload 2 that it stands on
        /// its own. Theoretically, every other overload could call this one.
        /// </remarks>
        public void NormalExit (
            uint puintStatusCode ,
            string pstrOperatorPrompt ,
            NormalExitAction penmNormalExitAction )
        {
            NormalExit (
                puintStatusCode ,
                pstrOperatorPrompt ,
                penmNormalExitAction ,
                TIMED_WAIT_DEFAULT_SECONDS ,
                TIMED_WAIT_WAITING_FOR_DEFAULT ,
                TIMED_WAIT_TEXT_COLOR_DEFAULT ,
                TIMED_WAIT_BACKGROUND_COLOR_DEFAULT ,
                TIMED_WAIT_INTERRUPT_CRITERION );
        }   // public void NormalExit (4 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code, and optionally calling WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard.
        /// </summary>
        /// <param name="pstrOperatorPrompt">
        /// This string specifies an alternative message for method
        /// WaitForCarbonUnit to display. If this is an empty string or null
        /// reference, a default message, "Please press the ENTER (Return) key
        /// to exit the program." is shown.
        /// </param>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <remarks>
        /// This method calls the fourth overload, the most coprehensive
        /// implementation, passing in the return code stored in the instance,
        /// which I explicitly cast to the unsigned integer type of its first
        /// argument.
        /// </remarks>
        public void NormalExit (
            string pstrOperatorPrompt ,
            NormalExitAction penmNormalExitAction )
        {
			NormalExit ( ( uint ) _me.AppReturnCode ,       								// Use the exit code stored in the instance.
				pstrOperatorPrompt ,                        								// Pass along the caller's message for WaitForCarbonUnit.
				penmNormalExitAction );                     								// Pass along the caller's instructions.
        }   // public void NormalExit (5 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code, and optionally calling WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard. If WaitForCarbonUnit is called, its default
        /// message is used.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the program's exit code.
        /// </param>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <remarks>
        /// This method calls the fourth overload, the most coprehensive
        /// implementation, passing in a null reference for the carbon unit
        /// prompt message, forcing it to use its default message.
        /// </remarks>
        public void NormalExit (
            uint puintStatusCode ,
            NormalExitAction penmNormalExitAction )
        {
            NormalExit (
				puintStatusCode ,               											// Pass along the caller's exit code.
				null ,																		// A null pstrOperatorPrompt means use default message.
				penmNormalExitAction );														// Pass along the caller's instructions.
        }   // public void NormalExit (6 of 9)


        /// <summary>
        /// Exit the program normally, returning the status code stored in this
        /// instance, and optionally call WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard. If WaitForCarbonUnit is called, its default
        /// message is used.
        /// </summary>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <remarks>
        /// This method calls the fourth overload, the most coprehensive
        /// implementation, passing in the return code stored in the instance,
        /// which I explicitly cast to the unsigned integer type of its first
        /// argument, and a null reference for the carbon unit prompt message,
        /// forcing it to use its default message.
        /// </remarks>
        public void NormalExit ( NormalExitAction penmNormalExitAction )
        {
            NormalExit (
				( uint ) _me.AppReturnCode ,    											// Use the exit code stored in the instance.
				null ,                          											// A null pstrOperatorPrompt means use default message.
				penmNormalExitAction );         											// Pass along the caller's instructions.
        }   // public void NormalExit (7 of 9)


        /// <summary>
        /// Exit the program normally, returning the status code stored in this
        /// instance, and optionally call WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard. If WaitForCarbonUnit is called, its default
        /// message is used.
        /// </summary>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <param name="pfIgnoreOutOfBoundsStatusCode">
        /// If True, when the status code is greater than the highest numbered
        /// message in the table, ignore the out-of-bounds condition, but pass
        /// the status code to the operating system so that the calling process
        /// or shell script can use it.
        /// </param>
        public void NormalExit (
            NormalExitAction penmNormalExitAction ,
            bool pfIgnoreOutOfBoundsStatusCode )
        {
            NormalExit (
                ( uint ) _me.AppReturnCode ,
                null ,
                penmNormalExitAction ,
                TIMED_WAIT_DEFAULT_SECONDS ,
                TIMED_WAIT_WAITING_FOR_DEFAULT ,
                TIMED_WAIT_TEXT_COLOR_DEFAULT ,
                TIMED_WAIT_BACKGROUND_COLOR_DEFAULT ,
                TIMED_WAIT_INTERRUPT_CRITERION ,
                pfIgnoreOutOfBoundsStatusCode );
        }   // public void NormalExit (8 of 9)


        /// <summary>
        /// Exit the program normally, optionally returning a nonzero status
        /// code, and optionally calling WaitForCarbonUnit to suspend execution
        /// until the operator has a chance to read the output or capture it
        /// into the clipboard.
        /// </summary>
        /// <param name="puintStatusCode">
        /// This unsigned integer specifies the program's exit code.
        /// </param>
        /// <param name="pstrOperatorPrompt">
        /// This string specifies an alternative message for method
        /// WaitForCarbonUnit to display. If this is an empty string or null
        /// reference, a default message, "Please press the ENTER (Return) key
        /// to exit the program." is shown.
        /// </param>
        /// <param name="penmNormalExitAction">
        /// This member of the NormalExitAction enumeration controls whether to
        /// use WaitForCarbonUnit to suspend execution until an operator has a
        /// chance to read the output. See the NormalExitAction enumeration for
        /// details.
        /// </param>
        /// <param name="puintSecondsToWait">
        /// Specify the number of seconds to wait, which must be a whole number
        /// greater than or equal to zero. Setting this value to zero causes the
        /// method to wait for 30 seconds.
        /// </param>
        /// <param name="pstrCountdownWaitingFor">
        /// Specify the text to display along with the remaining time. If this
        /// argument is null (Nothing in Visual Basic) or the empy string, the
        /// method uses a default message.
        /// 
        /// Currently, the default description is "Program ending," which is
        /// taken from a resource string in the WizardWrx.ConsoleAids class that
        /// implements this feature.
        /// </param>
        /// <param name="pclrTextColor">
        /// Specify a member of the ConsoleColor enumeration to control the text
        /// color used to display the countdown message.
        /// 
        /// To use the default (current) screen colors, specify the same
        /// ConsoleColor value for pclrTextColor and pclrTextBackgroundColor.
        /// </param>
        /// <param name="pclrTextBackgroundColor">
        /// Specify a member of the ConsoleColor enumeration to control the
        /// background color used to display the countdown message.
        /// 
        /// To use the default (current) screen colors, specify the same
        /// ConsoleColor value for pclrTextColor and pclrTextBackgroundColor.
        /// </param>
        /// <param name="penmInterruptCriterion">
        /// Specify a member of the DisplayAids.InterruptCriterion enumeration
        /// to indicate whether you want the timed wait to be interruptible, and
        /// under what conditions.
        /// </param>
        /// <param name="pfIgnoreOutOfBoundsStatusCode">
        /// If True, when the status code is greater than the highest numbered
        /// message in the table, ignore the out-of-bounds condition, but pass
        /// the status code to the operating system so that the calling process
        /// or shell script can use it.
        /// </param>
        /// <remarks>
        /// This method differs sufficiently from overload 2 that it stands on
        /// its own. Theoretically, every other overload could call this one.
        /// </remarks>
        public void NormalExit (
            uint puintStatusCode ,
            string pstrOperatorPrompt ,
            NormalExitAction penmNormalExitAction ,
            uint puintSecondsToWait ,
            string pstrCountdownWaitingFor ,
            ConsoleColor pclrTextColor ,
            ConsoleColor pclrTextBackgroundColor ,
            DisplayAids.InterruptCriterion penmInterruptCriterion ,
            bool pfIgnoreOutOfBoundsStatusCode = false)
        {
            switch ( penmNormalExitAction )
            {
                case NormalExitAction.ExitImmediately:
                case NormalExitAction.Silent:
                    Environment.Exit ( ( int ) puintStatusCode );
                    break;

                case NormalExitAction.Timed:
                    ReportNonZeroStatusCode ( ( int ) puintStatusCode );
                    Console.WriteLine ( pstrOperatorPrompt );
                    DisplayAids.TimedWait (
						puintSecondsToWait ,            // puintWaitSeconds
						pstrCountdownWaitingFor ,       // pstrCountdownWaitingFor
						ConsoleColor.Black ,            // pclrTextColor
						ConsoleColor.Black ,            // pclrTextBackgroundColor
                        penmInterruptCriterion );       // penmInterruptCriterion
                    Environment.Exit ( ( int ) puintStatusCode );
                    break;

                case NormalExitAction.HaltOnError:
                    ReportNonZeroStatusCode ( ( int ) puintStatusCode );

                    if ( puintStatusCode == MagicNumbers.ERROR_SUCCESS )
                    {   // No news is good news. Keep going.
                        Environment.Exit ( MagicNumbers.ERROR_SUCCESS );
                    }   // TRUE block, if ( puintStatusCode == MagicNumbers.ERROR_SUCCESS )
                    else
                    {   // There was an error. Halt the script.
                        DisplayAids.WaitForCarbonUnit ( pstrOperatorPrompt );
                        Environment.Exit ( ( int ) puintStatusCode );
                    }   // FALSE block, if ( puintStatusCode == MagicNumbers.ERROR_SUCCESS )
                    break;

                case NormalExitAction.WaitForOperator:
                    ReportNonZeroStatusCode ( ( int ) puintStatusCode );
                    DisplayAids.WaitForCarbonUnit ( pstrOperatorPrompt );
                    Environment.Exit ( ( int ) puintStatusCode );
                    break;

                default:
                    ReportNonZeroStatusCode ( ( int ) puintStatusCode );
                    Console.WriteLine (
						Properties.Resources.NORMAL_EXIT_INTERNAL_ERROR ,           		// Message template
						penmNormalExitAction ,                                      		// Token 0
						NormalExitAction.WaitForOperator ,                          		// Token 1
                        Environment.NewLine );                                      		// Token 2
                    DisplayAids.WaitForCarbonUnit ( pstrOperatorPrompt );
                    Environment.Exit ( ( int ) puintStatusCode );
                    break;
            }   // switch ( penmNormalExitAction )
        }   // public void NormalExit (9 of 9)
        #endregion // NortmalExit Methods


        #region SetCWDRelativeToEntryAssemblyPath
        /// <summary>
        /// SetCWDRelativeToEntryAssemblyPath establishes the CWD relative to
        /// the entry assembly path, returning the resulting absolute (fully
        /// qualified) path.
        /// </summary>
        /// <param name="pstrRelativeDirectoryPath">
        /// Path string, expressed relative to the entry assembly location.
        /// </param>
        /// <returns>
        /// <para>
        /// If it succeeds, the return path is the absolute path that is the new
        /// Current Working Directory.
        /// </para>
        /// <para>
        /// The returned string is guaranteed to be backslash terminated.
        /// </para>
        /// </returns>
        /// <example>
        /// If you call <c>SetCWDRelativeToEntryAssemblyPath</c> from an entry
        /// assembly that loaded from <c>C:\Users\Me\Repositories\ConsoleAppAids3\TestStand\bin\Release</c>
        /// and you pass in <c>..\..\App_Data</c>, the return value wound be
        /// <c>C:\Users\Me\Repositories\ConsoleAppAids3\TestStand\App_Data</c>,
        /// ideal for unit test assemblies distributed via GitHub, BitBucket,
        /// Sourceforge, and character-mode assemblies incorporated into Visual
        /// Studio solutions that are shared amont people whose machine
        /// configurations are not standardized.
        /// </example>
        /// <exception cref="ArgumentNullException">
        /// <para>
        /// An ArgumentNullException Exception arises when <paramref name="pstrRelativeDirectoryPath"/>
        /// is either a null reference or the empty string.
        /// </para>
        /// <para>
        /// You must pass a string, even if it is <c>.\</c> to designate the
        /// assembly location directory.
        /// </para>
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// <para>
        /// An InvalidOperationException Exception arises when the directory
        /// path specified by <paramref name="pstrRelativeDirectoryPath"/>
        /// cannot be found with respect to the directory from which the entry
        /// assembly loaded.
        /// </para>
        /// <para>
        /// Though <c>Environment.CurrentDirectory</c> raises an
        /// <c>DirectoryNofFoundException</c> Exception if the path resolves to
        /// an invalid directory, its message omits critical information that
        /// would help resolve the cause. Though it might display the resolved
        /// directory name, it would omit the path that was passed into the
        /// method and the name of the directory from which the entry assembly
        /// loaded.
        /// </para>
        /// </exception>
        public string SetCWDRelativeToEntryAssemblyPath ( string pstrRelativeDirectoryPath )
        {
            if ( !string.IsNullOrEmpty ( pstrRelativeDirectoryPath ) )
            {
                string strAssemblyLocationDirectoryName = _me.AppRootAssemblyFileDirName;
                Environment.CurrentDirectory = strAssemblyLocationDirectoryName;
                System.IO.DirectoryInfo directoryInfo = new System.IO.DirectoryInfo ( pstrRelativeDirectoryPath );

                if ( directoryInfo.Exists )
                {
                    Environment.CurrentDirectory = directoryInfo.FullName;
                    return WizardWrx.FileNameTricks.EnsureHasTerminalBackslash ( directoryInfo.FullName );
                }   // TRUE (anticipated outcome) block, if ( directoryInfo.Exists )
                else
                {
                    throw new InvalidOperationException (
                        string.Format (
                            Properties.Resources.ERRMSG_RELATIVE_DIRECTORY_NOT_FOUND ,              // Format Control String like: Directory {0} cannot be found with respect to entry assembly location directory {1}. The relative directory name that was passed into the routine is {2}.
                            directoryInfo.FullName ,                                                // Format Item 0: Directory {0} cannot be found
                            strAssemblyLocationDirectoryName ,                                      // Format Item 1: with respect to entry assembly location directory {1}.
                            pstrRelativeDirectoryPath ) );                                          // Format Item 2: The relative directory name that was passed into the routine is {2}.
                }   // FALSE (unanticipated outcome) block, if ( directoryInfo.Exists )
            }   // TRUE (anticpated outcome) block, if ( !string.IsNullOrEmpty ( pstrRelativeDirectoryPath ) )
            else
            {
                throw new ArgumentNullException (
                    nameof ( pstrRelativeDirectoryPath ) ,
                    WizardWrx.Common.Properties.Resources.ERRMSG_ARG_IS_NULL_OR_EMPTY );
            }   // FALSE (unanticpated outcome) block, if ( !string.IsNullOrEmpty ( pstrRelativeDirectoryPath ) )
        }   // public string SetCWDRelativeToEntryAssemblyPath
        #endregion  // SetCWDRelativeToEntryAssemblyPath


        #region Private Symbolic Constants
        const int STANDARD_MESSAGE_COUNT = 2;
        #endregion  // STANDARD_MESSAGE_COUNT


        #region Private Instance Methods
        /// <summary>
        /// Unless status code <paramref name="pintStatusCode"/> is
        /// ERROR_SUCCESS (zero), display the corresponding static error message
        /// along with the status code value.
        /// </summary>
        /// <param name="pintStatusCode">
        /// Unless this integer, which represents the program exit code, ia
        /// equal to ERROR_SUCCESS (zero), display the corresponding message
        /// from the AppErrorMessages table, along with the status code.
        /// </param>
        /// <remarks>
        /// Rather than have every routine that calls this routine test
        /// <paramref name="pintStatusCode"/>, the test is delegated to this
        /// routine, which returns wihtout taking action when its value is zero.
        /// </remarks>
        private void ReportNonZeroStatusCode ( int pintStatusCode )
        {
            if ( pintStatusCode != MagicNumbers.ERROR_SUCCESS )
            {
                if ( _me.AppErrorMessages != null )
                {
                    if ( pintStatusCode < _me.AppErrorMessages.Length && pintStatusCode >= MagicNumbers.ERROR_SUCCESS )
                    {   // ERROR_SUCCESS should have its own message in the table.
                        Console.WriteLine (
                            _me.AppErrorMessages [ pintStatusCode ] );
                    }   // TRUE (anticipated outcome) block, if ( puintStatusCode < _me.AppErrorMessages.Length && puintStatusCode >= MagicNumbers.ERROR_SUCCESS )
                    else
                    {
                        Console.WriteLine (
                            Properties.Resources.ERRMSG_UNKNOWN_EXIT_CODE ,
                            pintStatusCode );
                    }   // FALSE (UNanticipated outcome) block, if ( puintStatusCode < _me.AppErrorMessages.Length && puintStatusCode >= MagicNumbers.ERROR_SUCCESS )

                    Console.WriteLine (
                        Properties.Resources.CONSOLE_APP_EXIT_CODE ,
                        pintStatusCode ,
                        Environment.NewLine );
                }   // if ( _me.AppErrorMessages != null )
            }   // if ( pintStatusCode != MagicNumbers.ERROR_SUCCESS )
        }   // private void ReportNonZeroStatusCode
        #endregion  // Private Instance Methods


        #region Private Static Methods
        /// <summary>
        /// The message count is always at least 2. Whem <paramref name="pastrAdditionalMessages"/>
        /// is a null reference or a reference to the empty array, it is also 2.
        /// However, when <paramref name="pastrAdditionalMessages"/> containes
        /// one or more elements, the count is increased to accommodee them.
        /// </summary>
        /// <param name="pastrAdditionalMessages">
        /// Pass in the optional list of additonal messages if needed. Status
        /// codes zero and one are covered by stanard messages that are pulled
        /// from the resource strings stored in WizardWrx.Common.dll.
        /// </param>
        /// <returns>
        /// The total number of messages is as described under <paramref name="pastrAdditionalMessages"/>.
        /// </returns>
        private static int ComputeMessageCount ( string [ ] pastrAdditionalMessages )
        {
            return pastrAdditionalMessages == null ? STANDARD_MESSAGE_COUNT : pastrAdditionalMessages.Length + STANDARD_MESSAGE_COUNT;
        }   // private static int ComputeMessageCount
        #endregion  // Private Static Method
    }   // class ConsoleAppStateManager
}   // partial namespace WizardWrx.ConsoleAppAids3