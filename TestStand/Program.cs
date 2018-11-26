/*
    ============================================================================

    Namespace Name:     TestStand

    Module Name:        Program.cs

    Class Name:         Program

    Synopsis:           This command line utility exercises the routines in the
                        WizardWrx.ConsoleAppAids3 class.

    Remarks:            This class module implements the Program class, which is
                        composed exclusively of the static void Main method,
                        which is functionally equivalent to the main() routine
                        of a standard C program.

                        The initial version number is set to 3.2, to correspond
                        with the version of the library that was current when it
                        came into being.

    Created:            Saturday, 11 January 2014 - Wednesday, 05 February 2014
    
	License:            Copyright (C) 2014-2018, David A. Gray. 
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
    2014/02/05 3.3     DAG This is the first version.

    2014/03/22 3.5     DAG Move the FixedConsoleWriter test code here from my
                           Visual C# playpen project, and extend it to cover
                           exception reporting and delayed program shutdown.

                           Testing the accompanying FixedConsoleWriterAdapter
                           method is covered by test cases of the TimedWait
                           method that it implements.

    2014/03/22 3.6     DAG Adjust for reorganized utility classes moved from
                           this library and WizardWrx.ApplicationHelpers into
                           WizardWrx.DLLServices.

    2014/06/06 5.0     DAG Major namespace reorganization.

    2014/07/20 5.1     DAG 1) Further fine tuning of the namespace, a tweak or
                              two to the internal documentation comments, and
                              fix an output error in the MessageInColor tests.

                           2) Account for the rationalization of the color
                              console writer, MessageInColor.

    2014/12/15 5.4     DAG Swap the order of the calls to the WaitForCarbonUnit
                           and DisplayEOJMessage methods, so that the end-of-job
                           message is displayed before the carbon unit prompt.

                           The changes in this module are entirely cosmetic, as
                           the changes made in the test subject are internal.
                           Hence, the testing protocol is unchanged.

	2015/06/18 5.5     DAG 1) Suppress Code Analysis warning message CA2202, 
                              which I think is the result of a misinterpretation
                              of the program flow by the code analyzer.

                           2) Comment out constant BAD_EXAMPLE_LABELS, which is
                              unreferenced, and is intended as an example.

                           3) Incorporate the FixedConsoleWriter constructors
                              that set the cursor column.

    2016/04/04 6.0     DAG 1) Leverage the features of version 5.8.334.40074 of
                              WizaerdWrx.DllServices2.dll to suppress the output
                              of the Write and WriteLine methods of this class
                              when a handle is redirected.

                           2) Finish breaking free of dependence on the old
                              strong name signed class libraries.

                           3) Move the trace listener output to the Test_Data
                              directory.

                           4) Eliminate the deprecated property references, and
                              build against WizardWrx.DllServices2, version
                              6.0.381.32883.

	2016/05/10 6.0     DAG Incorporate the assembly properties report that I
                           added to WizardWrrx.DllServices2.dll.

	2016/05/20 6.1     DAG Synchronize this version number with that of its 
                           dependent library, WizardWrx.DllServices2.dll.

	2016/06/09 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically
                           belong in this library being found instead in
                           DLLServices2.

	2017/08/07 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.

	2018/08/06 7.0     DAG Make the TimedWaitTestCasesFQFN configuration string
	                       a path relative to the working directory, which is
						   set to the solution directory in the debugging page
						   of the project proerty sheets, which I disovered was
						   an issue when I tested it against the new release
						   candidate of the main WizardWrx library collection.

						   Since I'm using a new machine and a very different
						   directory structure, it was wrong.

	2018/11/26 7.0     DAG Eliminate the unreferenced WizardWrx.Common namespace
                           from the using directives. Although WizardWrx.Common
                           is used, everything in it, other than its resource
                           strings, which go unused in this class, is in the
                           parent namespace.
    ============================================================================
*/


using System;                                                                   // Apart from it being the mother of all namespaces, the Console class lives in this mamespace.
using System.Text;																// We use System.Text.Encoding, and I'm not fond of long lines.
using System.IO;																// We read some files.

using WizardWrx;                                                                // This is the mother of all my library namespaces.
using WizardWrx.ConsoleAppAids3;												// This is the library under test.
using WizardWrx.AssemblyUtils;													// I split WizardWrx.DLLServeices2.dll into 7 smaller, more focused libraries,
using WizardWrx.ConsoleStreams;                                                 // of which this class uses all but one, which I listed alphabetically, not
using WizardWrx.Core;       													// necessarily reflecting the chain of dependencies.
using WizardWrx.DLLConfigurationManager;


namespace TestStand
{
    class Program
    {
        enum OutputFormat
        {
			Verbose = 0 ,														// Tell all
			Terse = 1 ,															// Just the facts
			None = 2 ,															// Silence!
			Quiet = 2 ,															// Equivalent to None
			V = 0 ,																// Equivalent to Verbose
			T = 1 ,																// Equivalent to Terse
			N = 2 ,																// Equivalent to None
			Q = 2																// Equivalent to None
        };  // OutputFormat

        enum ReportFormat
        {
			Undefined ,															// Format is undefined.
			Full ,																// Report the exception through the full blown reporting object.
			Terse																// Just show the message.
        }   // ReportFormat

        const int ERR_ERROR_STOP_TEST = 2;

        const bool SEND_LINEFEED = true;
        const bool OMIT_LINEFEED = false;

        static string [ ] s_astrErrorMessages =
        {
            Properties.Resources.ERRMSG_SUCCESS ,								// ERROR_SUCCESS
            Properties.Resources.ERRMSG_RUNTIME ,								// ERR_RUNTIME
            Properties.Resources.ERRMSG_ERROR_STOP_TEST ,						// ERR_ERROR_STOP_TEST
        };  // static string [ ] s_astrErrorMessages

        //        static ApplicationInstance s_theApp;							// Succeeded by ConsoleAppStateManager
        static ConsoleAppStateManager s_theApp;

        static MessageInColor [ ] s_micColors = 
        { 
            null , 
            new MessageInColor (
                ConsoleColor.White ,        									// White text
                ConsoleColor.Red ) ,        									// Red background
            new MessageInColor (
                ConsoleColor.Yellow ,       									// Yellow text
                ConsoleColor.Red ) ,        									// Red background
            new MessageInColor (
                ConsoleColor.White ,        									// White text
                ConsoleColor.DarkRed ) ,										// Dark Red background
            new MessageInColor (
                ConsoleColor.Yellow ,											// Yellow text,
                ConsoleColor.DarkRed )											// Dark Red background
        };  // static MessageInColor [ ] s_micColors

        static ReportFormat [ ] s_enmReportFormat =
        {
            ReportFormat.Full ,													// Report through the exception reporting object.
            ReportFormat.Terse              									// Report only the message text stored in the Exception object.
        };  // static ReportFormat [ ] s_enmReportFormat

		const char SW_ONE_TEST = 't';       									// Argument specifies name of test to run, to expedite testing new routines.
		const char SW_OUTPUT = 'o';         									// Argument specifies one of three supported formats for the STDOUT display.
		const char SW_WAITTIME = 'w';       									// Argument specifies time, in seconds, to wait.

        const uint SW_WAITTIME_DEFAULT = 30;

        static char [ ] s_achrValidSwitches =
        {
            SW_ONE_TEST ,
            SW_OUTPUT ,
            SW_WAITTIME ,
        };  // static char [ ] s_achrValidSwitches

        const string SW_OT_COLOR_WRITER = @"colorwriter";
        const string SW_OT_FIXED_WRITER = @"fixedwriter";
        const string SW_OT_TIMED_EXIT = @"timedexit";
        const string SW_OT_TIMED_WAIT = @"timedwait";
        const string SW_OT_TIMED_STOP = @"timedstop";
        const string SW_OT_TIMED_ERRS = @"timederrs";

        const string ARG_COUNT = @"count";
        const string ARG_INTERVAL = @"interval";

        static string [ ] s_astrNamedArgs =
        {
            ARG_COUNT ,
            ARG_INTERVAL
        };  // static string [ ] s_astrNamedArgs

        const string TEST_BEGIN_TPL = "{1}Test # {0} begin\n";
        const string TEST_END_TPL = "\nTest # {0} end";
        const string FCW_TEST_MSG_TPL_3 = @"Waiting for {0:N0} milliseconds ({1:N0} seconds). This is pause {2:N0} of {3:N0}.";
        const string FCW_TEST_MSG_TPL_4 = @"Waiting for {0:N0} milliseconds ({1:N0} seconds). This is pause {2:N0}.";

        const string FIXED_CONSOLE_WRITER_TEST_1 = @"This is a test.";
        const string FIXED_CONSOLE_WRITER_TEST_2 = @"This is another test.";
        const string FIXED_CONSOLE_WRITER_TEST_3 = @"This is a third test.";
        const string FIXED_CONSOLE_WRITER_TEST_4 = @"This is a 4th test.";

        const int FIXED_CONSOLE_WRITER_OFFSET_1 = 0;
        const int FIXED_CONSOLE_WRITER_OFFSET_2 = 4;
        const int FIXED_CONSOLE_WRITER_OFFSET_3 = 8;
        const int FIXED_CONSOLE_WRITER_OFFSET_4 = 12;
        const int FIXED_CONSOLE_WRITER_OFFSET_5 = 16;

        //  --------------------------------------------------------------------
        //  Use these arrays of strings and unsigned integers to exercise the 
        //  FixedConsoleWriter class.
        //  --------------------------------------------------------------------

        static readonly string [ ] s_astrFCWTestStrings =
        {
            FIXED_CONSOLE_WRITER_TEST_1 ,
            FIXED_CONSOLE_WRITER_TEST_2 ,
            FIXED_CONSOLE_WRITER_TEST_3 ,
            FIXED_CONSOLE_WRITER_TEST_4
        };  // static read only string [ ] s_astrFCWTestStrings

        static readonly int [ ] s_uintOffsetFromLeft =
        {
            FIXED_CONSOLE_WRITER_OFFSET_1 ,
            FIXED_CONSOLE_WRITER_OFFSET_2 ,
            FIXED_CONSOLE_WRITER_OFFSET_3 ,
            FIXED_CONSOLE_WRITER_OFFSET_4 ,
            FIXED_CONSOLE_WRITER_OFFSET_5
        };  // static read only int [ ] s_uintOffsetFromLeft

        static int s_intCase = 0;

        const bool BOOL2SHOW = true;
        const char CHAR2SHOW = 'E';

        static char [ ] s_achr2Show = { 'F' , 'G' , 'H' };

        const int SUBARRAY_INDEX = 1;
        const int SUBARRAY_COUNT = 2;

        const decimal DEC2SHOW = 1.25M;
        const double DBL2SHOW = 3.333333;

        const float FLT2SHOW = 3.3333F;
        
        const int INT2SHOW = -255;

        const long LNG2SHOW = -125000;
        const string STR2SHOW = "This is it.";

        const uint UINT2SHOW = 512;
        const ulong ULNG2SHOW = 250000;        

        static readonly object [ ] s_aobj2Show =
		{
			"Stuff 1" ,
			"Stuff 2" ,
			"Stuff 3" ,
			"Stuff 4"
		};	// static read only object [ ] s_aobj2Show

        const int OBJECT_ARG0 = MagicNumbers.ZERO;
		const int OBJECT_ARG1 = OBJECT_ARG0 + MagicNumbers.PLUS_ONE;
        const int OBJECT_ARG2 = OBJECT_ARG1 + MagicNumbers.PLUS_ONE;
        const int OBJECT_ARG3 = OBJECT_ARG2 + MagicNumbers.PLUS_ONE;

        const string MSG_TEST_STARTING = @"Test {0} {1}:";
        const string MSG_TEST_OVER = @"Test {0} {1} is over.";
        const string MSG_TIMED_WAIT_TEST = @"TimeedWaitTest";


        static void Main ( string [ ] pastrArgs )
        {
            s_theApp = ConsoleAppStateManager.GetTheSingleInstance ( );
            CmdLneArgsBasic cmdArgs = new CmdLneArgsBasic (
                s_achrValidSwitches ,
                s_astrNamedArgs ,
                CmdLneArgsBasic.ArgMatching.CaseInsensitive );
            cmdArgs.AllowEmptyStringAsDefault = CmdLneArgsBasic.BLANK_AS_DEFAULT_ALLOWED;

            //  ----------------------------------------------------------------
            //  The default value of the AppSubsystem property is GUI, which
            //  disables output to the console. Since ReportException returns
            //  the message that would have been written, you still have the
            //  option of displaying or discarding it. If EventLoggingState is
            //  set to Enabled, the message is written into the Application
            //  Event log, where it is preserved until the event log record is
            //  purged by the aging rules or some other method.
            //  ----------------------------------------------------------------

            #if ALT_EXCPT_LOG_PATH
                SetExceptionLoggingProperties ( );
            #else
                s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags = s_theApp.BaseStateManager.AppExceptionLogger.OptionFlags | ExceptionLogger.OutputOptions.Stack | ExceptionLogger.OutputOptions.EventLog | ExceptionLogger.OutputOptions.StandardError;
//                s_theApp.AppExceptionLogger.EventLoggingState = ExceptionLogger.RecordinEventLog.Enabled;
//                s_theApp.AppExceptionLogger.AppSubsystem = ExceptionLogger.Subsystem.Console;
            #endif  // #if ALT_EXCPT_LOG_PATH

            s_theApp.BaseStateManager.LoadErrorMessageTable ( s_astrErrorMessages );

            string strDeferredMessage = null;

            OutputFormat enmOutputFormat = SetOutputFormat (
                cmdArgs ,
                ref strDeferredMessage );

            if ( enmOutputFormat != OutputFormat.None )
            {   // Unless output is suppressed, display the standard BOJ message.
                s_theApp.DisplayBOJMessage ( );
            }   // if ( enmOutputFormat != OutputFormat.None )

            if ( !string.IsNullOrEmpty ( strDeferredMessage ) )
            {   // SetOutputFormat saves its error message, if any, in SetOutputFormat.
                Console.WriteLine ( strDeferredMessage );
            }   // if ( !string.IsNullOrEmpty ( s_strDeferredMessage ) )

			//	----------------------------------------------------------------
			//	Generate a report about the library under test.
			//	----------------------------------------------------------------

			ReportGenerators.ShowKeyAssemblyProperties ( System.Reflection.Assembly.GetAssembly ( s_theApp.GetType ( ) ) );

            string strSingleTest = cmdArgs.GetSwitchByName (
                SW_ONE_TEST ,
                SpecialStrings.EMPTY_STRING ).ToLower ( );

            //  ----------------------------------------------------------------
            //  Run these two tests the way they should behave in a production
            //  program.
            //  ----------------------------------------------------------------

			if ( string.IsNullOrEmpty ( strSingleTest ) )
			{	// The degenerate case executes all tests.
				Console.WriteLine (
					"{0}All tests will execute.{0}" ,
					Environment.NewLine );
			}	// TRUE block, if ( string.IsNullOrEmpty ( strSingleTest ) )
			else
			{	// All other cases execute a single set of tests.
				Console.WriteLine (
					"{1}The {0} test will run.{1}" ,
					strSingleTest ,
					Environment.NewLine );
			}	// FALSE block, if ( string.IsNullOrEmpty ( strSingleTest ) )

			#if DEBUG
				if (System.Diagnostics.Debugger.IsAttached )
				{
					Console.Error.WriteLine (
						"This program is already running under a debugger.{0}" ,
						Environment.NewLine );
				}	// TRUE block, if (System.Diagnostics.Debugger.IsAttached )
				else
				{
					Console.Error.WriteLine (
						"This program is attaching itself to a debugger.{0}" ,
						Environment.NewLine );
					System.Diagnostics.Debugger.Launch ( );
			}	// FALSE block, if (System.Diagnostics.Debugger.IsAttached )
			#endif	// #if DEBUG

			if ( strSingleTest == SW_OT_TIMED_STOP )
            {
                Console.WriteLine (
                    Properties.Resources.MSG_TIMED_STOP_TEST ,
                    Environment.NewLine );
                WizardWrx.ConsoleAppAids3.DisplayAids.WaitForCarbonUnit (
                    Properties.Resources.MSG_TIMED_STOP_PROMPT );
                s_theApp.NormalExit (
                    ConsoleAppStateManager.NormalExitAction.Timed );
            }   // if ( strSingleTest == SW_OT_TIMED_STOP )

            if ( strSingleTest == SW_OT_TIMED_ERRS )
            {
				Console.WriteLine (
					Properties.Resources.MSG_TIMED_ERROR_TEST ,
					Environment.NewLine );
				DisplayAids.WaitForCarbonUnit ( Properties.Resources.MSG_TIMED_STOP_PROMPT );
				s_theApp.ErrorExit ( ERR_ERROR_STOP_TEST );
            }   // if ( strSingleTest == SW_OT_TIMED_ERRS )

            if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_COLOR_WRITER )
            {
                try
                {
                    //  --------------------------------------------------------
                    //  I thought that I might need to follow this block with a 
                    //  screen clear, but the screen scrolls perfectly without 
                    //  it.
                    //  --------------------------------------------------------

                    {   // An anonymous string would do, but I'd rather not.
                        string strMsg = string.Format (
                            Properties.Resources.MSG_PREPARE_CAMERA ,
                            Environment.NewLine );
                        DisplayAids.WaitForCarbonUnit ( strMsg );
                    }   // String strMsg goes out of scope.

                    Console.WriteLine (
                        Properties.Resources.MSG_WRITELINE_TESTS ,
                        Properties.Resources.MSG_BEGIN ,
                        Environment.NewLine );

                    ExerciseStaticWriteLine ( );

                    Console.WriteLine (
                        Properties.Resources.MSG_WRITELINE_TESTS ,
                        Properties.Resources.MSG_END ,
                        SpecialStrings.EMPTY_STRING );

                    DisplayAids.WaitForCarbonUnit ( Properties.Resources.MSG_PROMPT_FOR_PICTURE );

                    //  --------------------------------------------------------
                    //  Reset the case counter.
                    //  --------------------------------------------------------

                    s_intCase = MagicNumbers.ZERO;

                    Console.WriteLine (
                        Properties.Resources.MSG_WRITE_TESTS ,
                        Properties.Resources.MSG_BEGIN ,
                        Environment.NewLine );

                    ExerciseStaticWrite ( );

                    Console.WriteLine (
                        Properties.Resources.MSG_WRITE_TESTS ,
                        Properties.Resources.MSG_END ,
                        SpecialStrings.EMPTY_STRING );

                    DisplayAids.WaitForCarbonUnit ( Properties.Resources.MSG_PROMPT_FOR_PICTURE );

                    //  --------------------------------------------------------
                    //  Reset the case counter.
                    //  --------------------------------------------------------

                    s_intCase = MagicNumbers.ZERO;

                    {   // Confine the scope of a MessageInColor instance.
                        Console.WriteLine (
                            Properties.Resources.MSG_WRITELINE_IN_COLOR_TESTS ,
                            Properties.Resources.MSG_BEGIN ,
                            Environment.NewLine );
                        MessageInColor msgColored = new MessageInColor (
							ConsoleColor.Cyan ,         						// Text (foreground) color
							ConsoleColor.DarkMagenta ); 						// Background color
                        ExerciseInstanceWriteLine ( msgColored );
                        Console.WriteLine (
                            Properties.Resources.MSG_WRITELINE_IN_COLOR_TESTS ,
                            Properties.Resources.MSG_END ,
                            SpecialStrings.EMPTY_STRING );
                    }   // MessageInColor instance variable msgColored goes out of scope.

                    DisplayAids.WaitForCarbonUnit ( Properties.Resources.MSG_PROMPT_FOR_PICTURE );

                    //  --------------------------------------------------------
                    //  Reset the case counter.
                    //  --------------------------------------------------------

                    s_intCase = MagicNumbers.ZERO;

                    {   // Confine the scope of another MessageInColor instance, required since I am reusing the symbol name.
                        Console.WriteLine (
                            Properties.Resources.MSG_WRITE_IN_COLOR_TESTS ,
                            Properties.Resources.MSG_BEGIN ,
                            Environment.NewLine );
                        MessageInColor msgColored = new MessageInColor (
							ConsoleColor.Black ,        						// Text (foreground) color
                            ConsoleColor.White );								// Background color
                        ExerciseInstanceWriteLine ( msgColored );
                        Console.WriteLine (
                            Properties.Resources.MSG_WRITE_IN_COLOR_TESTS ,
                            Properties.Resources.MSG_END ,
                            SpecialStrings.EMPTY_STRING );
                    }   // MessageInColor instance variable msgColored goes out of scope.

                    //  --------------------------------------------------------
                    //  Scrolling must stop here, or the first couple of
                    //  overloads will scroll off and be lost.
                    //  --------------------------------------------------------

                    DisplayAids.WaitForCarbonUnit (
                        Properties.Resources.MSG_PROMPT_FOR_PICTURE );

                    ExerciseColorExceptionReporting ( );
                }
                catch ( Exception exAll )
                {   // The Message string is displayed, but the complete exception goes to the event log.
					s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exAll );
                    Console.WriteLine ( exAll.Message );

                    ExitWithError (
                        enmOutputFormat ,
						MagicNumbers.ERROR_RUNTIME );							// At last, this constant is where it belongs, in a library.
                }   // Providing a catch block is enough to cause the program to fall through.
            }   // if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_COLOR_WRITER )

            //  ----------------------------------------------------------------
            //  Exercise the FixedConsoleWriter class.
            //  ----------------------------------------------------------------

            if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_FIXED_WRITER )
            {
/*				TimedHaltTest (
					ref s_intCase ,
					cmdArgs ,
					false );													// Scroll the console for each new test.	*/
				TimedHaltTest (
					ref s_intCase ,
					cmdArgs ,
					true );														// Reuse the console window for all tests.
			}   // if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_FIXED_WRITER )

            if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_TIMED_EXIT )
            {
                TimedExitTest (
                    ref s_intCase ,
                    cmdArgs );
            }   // if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_TIMED_EXIT )

            if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_TIMED_WAIT )
            {
                try
                {
                    TimeedWaitTest (
                        ref s_intCase ,
                        cmdArgs );
                }
                catch ( Exception exAllKinds )
                {
					s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exAllKinds );
                }
                finally
                {
                    Console.WriteLine ( 
                        MSG_TEST_OVER ,
                        MSG_TIMED_WAIT_TEST ,
                        s_intCase );
                }
            }   // if ( strSingleTest == SpecialStrings.EMPTY_STRING || strSingleTest == SW_OT_TIMED_WAIT )

            #if DEBUG
                if ( enmOutputFormat == OutputFormat.None )
                {   // Suppress all output.
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.Silent );
                }   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
                else
                {   // Display the standard exit banner.
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.WaitForOperator );
                }   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
            #else
                if ( enmOutputFormat == OutputFormat.None )
                {   // Suppress all output.
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.ExitImmediately );
                }   // TRUE block, if ( enmOutputFormat == OutputFormat.None )
                else
                {   // Display the standard exit banner.
                    s_theApp.NormalExit ( ConsoleAppStateManager.NormalExitAction.WaitForOperator );
                }   // FALSE block, if ( enmOutputFormat == OutputFormat.None )
			#endif	// #if DEBUG
		}   //  static void Main


        private static void ExerciseColorExceptionReporting ( )
        {
            string strPromptMsg = string.Format (
                Properties.Resources.MSG_SHOW_NEXT_EXAMPLE ,
                Environment.NewLine );

            foreach ( ReportFormat enmReportFormat in s_enmReportFormat )
            {   // Defining the counter inside the loop gets it reininitialized for free.
                int intTestNumber = MagicNumbers.ZERO;

                foreach ( MessageInColor msgColors in s_micColors )
                {
					MessageInColor.SafeConsoleClear ( );
                    Console.WriteLine (
                        Properties.Resources.MSG_ERROR_REPORTING_TEST ,
                        new object [ ]
                        {
                            ++intTestNumber ,									// Format Item 0
                            s_micColors.Length ,								// Format Item 1
                            Properties.Resources.MSG_BEGIN ,					// Format Item 2
                            Environment.NewLine									// Format Item 3 = Newline, My Way
                        } );

                    try
                    {
                        s_theApp.BaseStateManager.AppExceptionLogger.ErrorMessageColors = new ErrorMessagesInColor (
                            msgColors.MessageForegroundColor ,
                            msgColors.MessageBackgroundColor );

                        string strMsg = null;

                        if ( msgColors == null )
                        {
                            strMsg = string.Format (
                                Properties.Resources.MSG_EXAMPLE_ERROR ,
                                Properties.Resources.MSG_EXAMPLE_DEFAULT_COLOR ,
                                Properties.Resources.MSG_EXAMPLE_DEFAULT_COLOR );
                        }   // TRUE (degenerate case) block, if ( msgColors == null )
                        else
                        {
                            strMsg = string.Format (
                                Properties.Resources.MSG_EXAMPLE_ERROR ,
                                msgColors.MessageForegroundColor ,
                                msgColors.MessageBackgroundColor );
                        }   // FALSE (normal case) block, if ( msgColors == null )

                        throw new Exception ( strMsg );
                    }
                    catch ( Exception exGeneral )
                    {
                        if ( enmReportFormat == ReportFormat.Full )
                        {
							s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exGeneral );
                        }   // TRUE block, if ( enmReportFormat == ReportFormat.Full )
                        else
                        {
                            if ( msgColors == null )
                            {   // The first test uses the default screen colors, and the msgColors is uninitialized.
                                Console.WriteLine ( exGeneral.Message );
                            }   // TRUE (degenerate case) block, if ( msgColors == null )
                            else
                            {
                                msgColors.WriteLine ( exGeneral.Message );
                            }   // FALSE block, if ( msgColors == null )
                        }   // FALSE (normal case) block, if ( enmReportFormat == ReportFormat.Full )
                    }
                    finally
                    {
                        Console.WriteLine (
                            Properties.Resources.MSG_ERROR_REPORTING_TEST ,
                            new object [ ]
                            {
                                intTestNumber ,									// Token 0
                                s_micColors.Length ,							// Token 1
                                Properties.Resources.MSG_END ,					// Token 2
                                Environment.NewLine								// Token 3
                            } );
                        DisplayAids.WaitForCarbonUnit ( strPromptMsg );
                    }   // try/catch/finally block
                }   // foreach ( MessageInColor msgColors in s_micColors)
            }   // foreach ( ReportFormat enmReportFormat in s_enmReportFormat)
        }   // private static void ExerciseColorExceptionReporting


        private static void ExerciseStaticWrite ( )
        {
            const ConsoleColor FGCOLOR = ConsoleColor.Black;
            const ConsoleColor BGCOLOR = ConsoleColor.Yellow;

            //  ----------------------------------------------------------------
            //  Test overload 1 of 18, MessageInColor.RGBWrite ( bool value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                BOOL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 2 of 18, MessageInColor.RGBWrite ( char value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                CHAR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 3 of 18, MessageInColor.RGBWrite ( char [ ] buffer ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                s_achr2Show );

            //  ----------------------------------------------------------------
            //  Test overload 4 of 18, MessageInColor.RGBWrite ( decimal value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                DEC2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 5 of 18, MessageInColor.RGBWrite ( double value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                DBL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 6 of 18, MessageInColor.RGBWrite ( float value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                FLT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 7 of 18, MessageInColor.RGBWrite ( int value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                INT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 8 of 18, MessageInColor.RGBWrite ( long value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                LNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 9 of 18, MessageInColor.RGBWrite ( object value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 10 of 18, MessageInColor.RGBWrite ( string value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                STR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 11 of 18, MessageInColor.RGBWrite ( uint value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                UINT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 12 of 18, MessageInColor.RGBWrite ( ulong value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                ULNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 13 of 18, MessageInColor.RGBWrite ( string format, object arg0 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_13 ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 14 of 18, MessageInColor.RGBWrite ( string format, params object [ ] arg ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_14 ,
                s_aobj2Show );

            //  ----------------------------------------------------------------
            //  Test overload 15 of 18, MessageInColor.RGBWrite ( string format, char [ ] buffer, int index ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                s_achr2Show ,
                SUBARRAY_INDEX ,
                SUBARRAY_COUNT );

            //  ----------------------------------------------------------------
            //  Test overload 16 of 18, MessageInColor.RGBWrite ( string format, object arg0, object arg1 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_16 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] );

            //  ----------------------------------------------------------------
            //  Test overload 17 of 18, MessageInColor.RGBWrite ( string format, object arg0, object arg1, object arg2 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_17 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] );

            //  ----------------------------------------------------------------
            //  Test overload 18 of 18, MessageInColor.RGBWrite ( string format, object arg0, object arg1, object arg2 ,object arg3 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            MessageInColor.RGBWrite (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_18 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] ,
                s_aobj2Show [ OBJECT_ARG3 ] );

            //  ----------------------------------------------------------------
            //  Follow the last test with a newline.
            //  ----------------------------------------------------------------

            Console.WriteLine ( );
        }   // private static void ExerciseWrite


        private static void ExerciseStaticWriteLine ( )
        {
            const ConsoleColor FGCOLOR = ConsoleColor.Yellow;
            const ConsoleColor BGCOLOR = ConsoleColor.DarkMagenta;

            //  ----------------------------------------------------------------
            //  Test overload 1 of 18, MessageInColor.RGBWriteLine ( bool value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                BOOL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 2 of 18, MessageInColor.RGBWriteLine ( char value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                CHAR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 3 of 18, MessageInColor.RGBWriteLine ( char [ ] buffer ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                s_achr2Show );

            //  ----------------------------------------------------------------
            //  Test overload 4 of 18, MessageInColor.RGBWriteLine ( decimal value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                DEC2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 5 of 18, MessageInColor.RGBWriteLine ( double value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                DBL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 6 of 18, MessageInColor.RGBWriteLine ( float value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                FLT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 7 of 18, MessageInColor.RGBWriteLine ( int value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                INT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 8 of 18, MessageInColor.RGBWriteLine ( long value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                LNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 9 of 18, MessageInColor.RGBWriteLine ( object value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 10 of 18, MessageInColor.RGBWriteLine ( string value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                STR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 11 of 18, MessageInColor.RGBWriteLine ( uint value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                UINT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 12 of 18, MessageInColor.RGBWriteLine ( ulong value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                ULNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 13 of 18, MessageInColor.RGBWriteLine ( string format, object arg0 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_13 ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 14 of 18, MessageInColor.RGBWriteLine ( string format, params object [ ] arg ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_14 ,
                s_aobj2Show );

            //  ----------------------------------------------------------------
            //  Test overload 15 of 18, MessageInColor.RGBWriteLine ( string format, char [ ] buffer, int index ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                s_achr2Show ,
                SUBARRAY_INDEX ,
                SUBARRAY_COUNT );

            //  ----------------------------------------------------------------
            //  Test overload 16 of 18, MessageInColor.RGBWriteLine ( string format, object arg0, object arg1 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_16 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] );

            //  ----------------------------------------------------------------
            //  Test overload 17 of 18, MessageInColor.RGBWriteLine ( string format, object arg0, object arg1, object arg2 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_17 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] );

            //  ----------------------------------------------------------------
            //  Test overload 18 of 18, MessageInColor.RGBWriteLine ( string format, object arg0, object arg1, object arg2 ,object arg3 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            MessageInColor.RGBWriteLine (
                FGCOLOR ,
                BGCOLOR ,
                Properties.Resources.MSG_FORMAT_18 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] ,
                s_aobj2Show [ OBJECT_ARG3 ] );
        }   // private static void ExerciseWriteLine


        private static void ExerciseInstanceWrite ( MessageInColor pmsgObj )
        {
            //  ----------------------------------------------------------------
            //  Test overload 1 of 18, pmsgObj.Write ( bool value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                BOOL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 2 of 18, pmsgObj.Write ( char value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                CHAR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 3 of 18, pmsgObj.Write ( char [ ] buffer ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                s_achr2Show );

            //  ----------------------------------------------------------------
            //  Test overload 4 of 18, pmsgObj.Write ( decimal value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                DEC2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 5 of 18, pmsgObj.Write ( double value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                DBL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 6 of 18, pmsgObj.Write ( float value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                FLT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 7 of 18, pmsgObj.Write ( int value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                INT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 8 of 18, pmsgObj.Write ( long value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                LNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 9 of 18, pmsgObj.Write ( object value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 10 of 18, pmsgObj.Write ( string value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                STR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 11 of 18, pmsgObj.Write ( uint value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                UINT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 12 of 18, pmsgObj.Write ( ulong value ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                ULNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 13 of 18, pmsgObj.Write ( string format, object arg0 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                Properties.Resources.MSG_FORMAT_13 ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 14 of 18, pmsgObj.Write ( string format, params object [ ] arg ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                Properties.Resources.MSG_FORMAT_14 ,
                s_aobj2Show );

            //  ----------------------------------------------------------------
            //  Test overload 15 of 18, pmsgObj.Write ( string format, char [ ] buffer, int index ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                s_achr2Show ,
                SUBARRAY_INDEX ,
                SUBARRAY_COUNT );

            //  ----------------------------------------------------------------
            //  Test overload 16 of 18, pmsgObj.Write ( string format, object arg0, object arg1 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                Properties.Resources.MSG_FORMAT_16 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] );

            //  ----------------------------------------------------------------
            //  Test overload 17 of 18, pmsgObj.Write ( string format, object arg0, object arg1, object arg2 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                Properties.Resources.MSG_FORMAT_17 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] );

            //  ----------------------------------------------------------------
            //  Test overload 18 of 18, pmsgObj.Write ( string format, object arg0, object arg1, object arg2 ,object arg3 ).
            //  ----------------------------------------------------------------

            NextCase ( SEND_LINEFEED );
            pmsgObj.Write (
                Properties.Resources.MSG_FORMAT_18 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] ,
                s_aobj2Show [ OBJECT_ARG3 ] );

            //  ----------------------------------------------------------------
            //  Follow the last test with a newline.
            //  ----------------------------------------------------------------

            Console.WriteLine ( );
        }   // private static void ExerciseInstanceWrite


        private static void ExerciseInstanceWriteLine ( MessageInColor pmsgObj )
        {
            //  ----------------------------------------------------------------
            //  Test overload 1 of 18, pmsgObj.WriteLine ( bool value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                BOOL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 2 of 18, pmsgObj.WriteLine ( char value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                CHAR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 3 of 18, pmsgObj.WriteLine ( char [ ] buffer ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                s_achr2Show );

            //  ----------------------------------------------------------------
            //  Test overload 4 of 18, pmsgObj.WriteLine ( decimal value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                DEC2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 5 of 18, pmsgObj.WriteLine ( double value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                DBL2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 6 of 18, pmsgObj.WriteLine ( float value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                FLT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 7 of 18, pmsgObj.WriteLine ( int value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                INT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 8 of 18, pmsgObj.WriteLine ( long value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                LNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 9 of 18, pmsgObj.WriteLine ( object value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 10 of 18, pmsgObj.WriteLine ( string value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                STR2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 11 of 18, pmsgObj.WriteLine ( uint value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                UINT2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 12 of 18, pmsgObj.WriteLine ( ulong value ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                ULNG2SHOW );

            //  ----------------------------------------------------------------
            //  Test overload 13 of 18, pmsgObj.WriteLine ( string format, object arg0 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                Properties.Resources.MSG_FORMAT_13 ,
                s_aobj2Show [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );

            //  ----------------------------------------------------------------
            //  Test overload 14 of 18, pmsgObj.WriteLine ( string format, params object [ ] arg ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                Properties.Resources.MSG_FORMAT_14 ,
                s_aobj2Show );

            //  ----------------------------------------------------------------
            //  Test overload 15 of 18, pmsgObj.WriteLine ( string format, char [ ] buffer, int index ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                s_achr2Show ,
                SUBARRAY_INDEX ,
                SUBARRAY_COUNT );

            //  ----------------------------------------------------------------
            //  Test overload 16 of 18, pmsgObj.WriteLine ( string format, object arg0, object arg1 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                Properties.Resources.MSG_FORMAT_16 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] );

            //  ----------------------------------------------------------------
            //  Test overload 17 of 18, pmsgObj.WriteLine ( string format, object arg0, object arg1, object arg2 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                Properties.Resources.MSG_FORMAT_17 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] );

            //  ----------------------------------------------------------------
            //  Test overload 18 of 18, pmsgObj.WriteLine ( string format, object arg0, object arg1, object arg2 ,object arg3 ).
            //  ----------------------------------------------------------------

            NextCase ( OMIT_LINEFEED );
            pmsgObj.WriteLine (
                Properties.Resources.MSG_FORMAT_18 ,
                s_aobj2Show [ OBJECT_ARG0 ] ,
                s_aobj2Show [ OBJECT_ARG1 ] ,
                s_aobj2Show [ OBJECT_ARG2 ] ,
                s_aobj2Show [ OBJECT_ARG3 ] );
        }   // private static void ExerciseInstanceWriteLine


        private static void ExitWithError (
            OutputFormat penmOutputFormat ,
            uint puintStatusCode )
        {
            #if DEBUG
                if ( penmOutputFormat == OutputFormat.Quiet )
                    s_theApp.NormalExit (
                        puintStatusCode ,
                        ConsoleAppStateManager.NormalExitAction.HaltOnError );
                else
                    s_theApp.NormalExit (
                        puintStatusCode ,
                        ConsoleAppStateManager.NormalExitAction.HaltOnError );
            #else
            if ( penmOutputFormat == OutputFormat.Quiet )
                s_theApp.NormalExit (
                    puintStatusCode ,
                    null ,
                    ConsoleAppStateManager.NormalExitAction.Silent );
            else
                s_theApp.NormalExit (
                    puintStatusCode ,
                    null ,
                    ConsoleAppStateManager.NormalExitAction.WaitForOperator );
        #endif  // #if DEBUG
        }   // private static void ExitWithError


        /// <summary>
        /// Isolate maintenance of the index. This could also be done with a
        /// small singleton object, but that would be overkill, when a static
        /// variable in the class and this one-statement method get it done.
        /// </summary>
        /// <param name="pfSendLinefeed">
        /// If True, representable as constant SEND_LINEFEED, first call
        /// Console.WriteLine() to send a linefeed to the console. Otherwise,
        /// omit the linefeed, which the previous test supplied. See Remarks.
        /// </param>
        /// <remarks>
        /// This method is used with tests of pairs of methods, one of which
        /// behaves like Console.WriteLine, which supplies its own linefeed. The
        /// other, which behaves like Consolw.Write, which doesn't.
        /// </remarks>
        private static void NextCase ( bool pfSendLinefeed )
        {
            const string NBR_FMT_INT_2 = @"{0,2}";

            if ( pfSendLinefeed )
                Console.WriteLine ( );

            Console.Write (
                Properties.Resources.MSG_CASE ,
                string.Format (
                    NBR_FMT_INT_2 ,
                    ++s_intCase ) );
        }   // private static void NextCase


        private static OutputFormat SetOutputFormat (
            CmdLneArgsBasic pcmdArgs ,
            ref string pstrDeferredMessage )
        {
            //  ----------------------------------------------------------------
            //  An invalid input value elicits a message similar to the 
            //  following.
            //
            //      Requested value 'Foolish' was not found.
            //
            //  The simplest way to report an invalid value is by extracting it
            //  from the Message property of the ArgumentException thrown by the
            //  Enum.Parse method.
            //
            //  I happen to have a library routine, ExtractBoundedSubstrings,
            //  which became part of a sealed class, WizardWrx.StringTricks,
            //  exported by class library WizardWrx.SharedUtl2.dll version 2.62,
            //  which came into being exactly two years ago, 2011/11/23.
            //  ----------------------------------------------------------------

            const bool IGNORE_CASE = true;
            const int NONE = 0;

            OutputFormat renmOutputFormat = OutputFormat.Verbose;

            //  ----------------------------------------------------------------
            //  Enum.Parse needs a try/catch block, because an invalid SW_OUTPUT
            //  value raises an exception that can be gracefully handled without
            //  killing the program.
            //  ----------------------------------------------------------------

			try
			{
				if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
				{
					renmOutputFormat = ( OutputFormat ) Enum.Parse (
						typeof ( OutputFormat ) ,
						pcmdArgs.GetSwitchByName (
							SW_OUTPUT ,
							OutputFormat.Verbose.ToString ( ) ) ,
						IGNORE_CASE );
				}   // if ( pcmdArgs.ValidSwitchesInCmdLine > NONE )
			}
			catch ( ArgumentException exArg )
			{   // Display of the message is deferred until the BOJ message is printed.
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exArg );
				pstrDeferredMessage = string.Format (
					Properties.Resources.ERRMSG_INVALID_OUTPUT_FORMAT ,
					exArg.Message.ExtractBoundedSubstrings ( SpecialCharacters.SINGLE_QUOTE ) ,
					renmOutputFormat ,
					Environment.NewLine );
			}	// catch ( ArgumentException exArg )

            return renmOutputFormat;
        }   // private static OutputFormat SetOutputFormat


        /// <summary>
        /// This private method was created to perfect the algorithm at the core
        /// of a new TimedWait method of the DisplayAids class. The test routine
        /// for that method is TimeedWaitTest.
        /// 
        /// Although no longer immediately useful, I intend to leave this method
        /// in the code base.
        /// </summary>
        /// <param name="pintTestNbr">
        /// This argument holds a reference to the location in the scope of the
        /// caller that holds a test counter. Since int is a BCL primitive, it
        /// must be passed by reference to achieve the desired effect, which is
        /// to maintain a continuous run of test numbers across every method,
        /// without needing to use a return value to update it.
        /// </param>
        /// <param name="pcmdArgs">
        /// This array of strings is a reference to the command line argument
        /// list that is passed into the main routine by the application startup
        /// code. Since arrays are always reference types, pass by reference is
        /// implied.
        /// </param>
        private static void TimedExitTest ( 
            ref int pintTestNbr , 
            CmdLneArgsBasic cmdArgs )
        {
			const string BIT_IS_OFF = @"OFF";
			const string BIT_IS_ON = @"ON";

            const string COUNTDOWN_MSG_TPL = @"Test ending in {0} seconds";
            const string COUNTDOWN_OVER = @"Test ended.{0}";

            const string PARAMETERS_MSG_TPL = @"{2}Test # {0}: This test displays a countdown of {1} seconds.{2}";
            const string PROPERTY_NOT_SET = @"NONE: The ErrorMessageColors property is null (uninitialized).";

			const string SAMPLE_EXCEPTION_REPORT = @"This is a test exception. This is only a test.";

            const int ONE_SECOND = 1000;

            const uint TIME_IS_UP = 0;

            uint uintWaitSeconds = GetUintSwitchByName (
                cmdArgs ,
                SW_WAITTIME ,
                SW_WAITTIME_DEFAULT );

            if ( pintTestNbr == MagicNumbers.ZERO )
                pintTestNbr++;

            Console.WriteLine (
				PARAMETERS_MSG_TPL ,											// Message template
				pintTestNbr ,													// Token 0
				uintWaitSeconds ,												// Token 1
				Environment.NewLine );											// Token 2

			//	----------------------------------------------------------------
			//	These tests use a FixedConsoleWriter instance that leaves the
			//	cursor at the left edge of the window.
			//	----------------------------------------------------------------

            FixedConsoleWriter fcwCountdownClock = new FixedConsoleWriter (
				ConsoleColor.Blue ,												// Text color
				ConsoleColor.Yellow );											// Background color

            for ( uint uintSecodsLeft = uintWaitSeconds ;
                       uintSecodsLeft > TIME_IS_UP ;
                       uintSecodsLeft-- )
            {
                fcwCountdownClock.Write (
                    COUNTDOWN_MSG_TPL ,
                    uintSecodsLeft );

                System.Threading.Thread.Sleep ( ONE_SECOND );
            }   // for ( uint uintSecodsLeft = uintWaitSeconds ; uintSecodsLeft > TIME_IS_UP ; uintSecodsLeft-- )

            fcwCountdownClock.ScrollUp ( );
            Console.WriteLine (
                COUNTDOWN_OVER ,
                Environment.NewLine );

            try
            {
                MessageInColor micParams = new MessageInColor (
                    ConsoleColor.Blue ,
                    ConsoleColor.White );
                micParams.WriteLine ( "AppExceptionLogger properties:{0}" , Environment.NewLine );
                ExceptionLogger exlMine = s_theApp.BaseStateManager.AppExceptionLogger;

				micParams.WriteLine ( "    AppEventSourceID         = {0}" , exlMine.AppEventSourceID );
				micParams.WriteLine ( "    ApplicationSubsystem     = {0}" , exlMine.ApplicationSubsystem );
				micParams.WriteLine ( "    AppStackTraceDisposition = {0}" , ( ( exlMine.OptionFlags & ExceptionLogger.OutputOptions.Stack ) == ExceptionLogger.OutputOptions.EventLog ? BIT_IS_ON : BIT_IS_OFF ) );
				micParams.WriteLine ( "    StandardError            = {0}" , ( ( exlMine.OptionFlags & ExceptionLogger.OutputOptions.StandardError ) == ExceptionLogger.OutputOptions.EventLog ? BIT_IS_ON : BIT_IS_OFF ) );
				micParams.WriteLine ( "    StandardOutput           = {0}" , ( ( exlMine.OptionFlags & ExceptionLogger.OutputOptions.StandardOutput ) == ExceptionLogger.OutputOptions.EventLog ? BIT_IS_ON : BIT_IS_OFF ) );

                if ( exlMine.ErrorMessageColors == null )
                {
                    micParams.WriteLine ( "    MessageBackgroundColor   = {0}" , PROPERTY_NOT_SET );
                    micParams.WriteLine ( "    MessageForegroundColor   = {0}" , PROPERTY_NOT_SET );
                }
                else
                {
                    micParams.WriteLine ( "    MessageBackgroundColor   = {0}" , exlMine.ErrorMessageColors.MessageBackgroundColor );
                    micParams.WriteLine ( "    MessageForegroundColor   = {0}" , exlMine.ErrorMessageColors.MessageForegroundColor );
                }   // if ( exlMine.ErrorMessageColors == null )

				micParams.WriteLine ( "    EventLoggingState        = {0}" , ( ( exlMine.OptionFlags & ExceptionLogger.OutputOptions.EventLog ) == ExceptionLogger.OutputOptions.EventLog ? BIT_IS_ON : BIT_IS_OFF ) );
                throw new Exception ( SAMPLE_EXCEPTION_REPORT );
            }
            catch ( Exception exThis )
            {
                ConsoleAppStateManager saiMe = ConsoleAppStateManager.GetTheSingleInstance ( );
				string strMsg = saiMe.BaseStateManager.AppExceptionLogger.ReportException ( exThis );

                #if SHOW_EXCPT_MSG
                    MessageInColor.RGBWriteLine (
                        ConsoleColor.White ,
                        ConsoleColor.DarkRed ,
                        strMsg );
                #endif  // #if SHOW_EXCPT_MSG
            }   // Try/Catch block (no Finally block needed)
        }   // private static void TimedExitTest


        /// <summary>
        /// Exercise the new TimedWait method of the DisplayAids class. This
        /// routine supersedes TimedExitTest, which I intend to leave in the
        /// test stand program.
        /// </summary>
        /// <param name="pintTestNbr">
        /// This argument holds a reference to the location in the scope of the
        /// caller that holds a test counter. Since int is a BCL primitive, it
        /// must be passed by reference to achieve the desired effect, which is
        /// to maintain a continuous run of test numbers across every method,
        /// without needing to use a return value to update it.
        /// </param>
        /// <param name="pcmdArgs">
        /// This array of strings is a reference to the command line argument
        /// list that is passed into the main routine by the application startup
        /// code. Since arrays are always reference types, pass by reference is
        /// implied.
		[System.Diagnostics.CodeAnalysis.SuppressMessage ( "Microsoft.Usage" , "CA2202:Do not dispose objects multiple times" )]
		private static void TimeedWaitTest (
            ref int s_intCase ,
            CmdLneArgsBasic cmdArgs )
        {
            // const string BAD_EXAMPLE_LABELS = "puintWaitSeconds\tpuintWaitSeconds\tpstrCountdownWaitingFor\tpclrTextColour\tpclrTextBackgroundColour";
            const string GOOD_EXAMPLE_LABELS = "puintWaitSeconds\tpstrCountdownWaitingFor\tpclrTextColor\tpclrTextBackgroundColor\tpenmInterruptCriterion";
            const string GOOD_EXAMPLE_DATA = "30\tA TimeedWaitTest\tGreen\tBlack\tNone";

            ErrorMessagesInColor micSaveErrorMessageColors = s_theApp.BaseStateManager.AppExceptionLogger.ErrorMessageColors;
            s_theApp.BaseStateManager.AppExceptionLogger.ErrorMessageColors = new ErrorMessagesInColor (
                ConsoleColor.White ,
                ConsoleColor.DarkRed );

            if ( s_intCase == MagicNumbers.ZERO )
                s_intCase++;

            Console.WriteLine (
                MSG_TEST_STARTING ,
                MSG_TIMED_WAIT_TEST ,
                s_intCase );

            try
            {
                StreamReader swTestCases = new StreamReader (
                    Properties.Settings.Default.TimedWaitTestCasesFQFN ,
                    Encoding.ASCII );

                int intCase = ArrayInfo.ARRAY_INVALID_INDEX;

                string strTestCase = null;

                while ( ( strTestCase = swTestCases.ReadLine ( ) ) != null )
                {
                    if ( ++intCase > ArrayInfo.ARRAY_FIRST_ELEMENT )
                    {   // Remaining rows are details. Parse them into a TimedWaitTestCase.
                        TimedWaitTestCase twtc = new TimedWaitTestCase ( strTestCase );

                        Console.WriteLine ( "Test Case {0,2:N0} Properties: TWTC_puintWaitSeconds        = {1}" , intCase , twtc.TWTC_puintWaitSeconds );
                        Console.WriteLine ( "                         TWTC_pstrCountdownWaitingFor = {0}" , twtc.TWTC_pstrCountdownWaitingFor );
                        Console.WriteLine ( "                         TWTC_pclrTextColor           = {0}" , twtc.TWTC_pclrTextColor );
                        Console.WriteLine ( "                         TWTC_pclrTextBackgroundColor = {0}" , twtc.TWTC_pclrTextBackgroundColor );
                        Console.WriteLine ( "                         TWTC_penmInterruptCriterion  = {0}" , twtc.TWTC_penmInterruptCriterion );

                        try
                        {
                             DisplayAids.TimedWait (
                                twtc.TWTC_puintWaitSeconds ,
                                twtc.TWTC_pstrCountdownWaitingFor ,
                                twtc.TWTC_pclrTextColor ,
                                twtc.TWTC_pclrTextBackgroundColor ,
                                twtc.TWTC_penmInterruptCriterion );
                        }
                        catch ( ArgumentException exBadArg )
                        {
							s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exBadArg );
                        }
                        catch ( Exception exGeneric )
                        {
							s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exGeneric );
                        }
                        finally
                        {
                            Console.WriteLine ( 
                                "{1}Test Case {0,2:N0} completed.{1}" ,
                                intCase
                                , Environment.NewLine );
                        }   // Try/Catch/Finally block
                    }   // TRUE (majority of cases) block, if ( ++intCase > ArrayInfo.ARRAY_FIRST_ELEMENT )
                    else
                    {   // First row contains labels; validate them.
                        string strMsgLabelErrors = TimedWaitTestCase.ValidateLabelRow ( strTestCase );

                        if ( !string.IsNullOrEmpty ( strMsgLabelErrors ) )
                        {
                            throw new Exception ( strMsgLabelErrors );
                        }   // if ( !string.IsNullOrEmpty ( strLabelErrors ) )
                    }   // FALSE (first time through case) block, if ( ++intCase > ArrayInfo.ARRAY_FIRST_ELEMENT )
                }   // while ( ( strTestCase = swTestCases.ReadLine ( ) ) != null )

                swTestCases.Close ( );
                swTestCases.Dispose ( );

                string strLabelErrors = TimedWaitTestCase.ValidateLabelRow ( GOOD_EXAMPLE_LABELS );

                if ( string.IsNullOrEmpty ( strLabelErrors ) )
                {   // Label row passed the smell test.
                    TimedWaitTestCase twc = new TimedWaitTestCase ( GOOD_EXAMPLE_DATA );

                    Console.WriteLine ( "TimedWaitTestCase Properties: TWTC_puintWaitSeconds        = {0}" , twc.TWTC_puintWaitSeconds );
                    Console.WriteLine ( "                              TWTC_pstrCountdownWaitingFor = {0}" , twc.TWTC_pstrCountdownWaitingFor );
                    Console.WriteLine ( "                              TWTC_pclrTextColor           = {0}" , twc.TWTC_pclrTextColor );
                    Console.WriteLine ( "                              TWTC_pclrTextBackgroundColor = {0}" , twc.TWTC_pclrTextBackgroundColor );
                    Console.WriteLine ( "                              TWTC_penmInterruptCriterion  = {0}" , twc.TWTC_penmInterruptCriterion );
                }   // TRUE (desired outcome) block, if ( string.IsNullOrEmpty ( strLabelErrors ) )
                else
                {   // Label row is invalid.
                    throw new Exception ( strLabelErrors );
                }   // FALSE (unwanted outcome) block, if ( string.IsNullOrEmpty ( strLabelErrors ) )
            }
            catch ( ArgumentException exBadArg )
            {
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exBadArg );
            }
            catch ( Exception exGeneric )
            {
				s_theApp.BaseStateManager.AppExceptionLogger.ReportException ( exGeneric );
            }
            finally
            {   // Restore the custom colors for use by other tests.
                s_theApp.BaseStateManager.AppExceptionLogger.ErrorMessageColors = micSaveErrorMessageColors;
            }   // Try/Catch/Finally blocks
        }   // private static void TimeedWaitTest


        /// <summary>
        /// This method exercises a new FixedConsoleWriter class that facilitates
        /// writing a succession of lines at a fixed location on the console, so
        /// that the text above them stays on the screen. Please see Remarks.
        /// </summary>
        /// <param name="pintTestNbr">
        /// This argument holds a reference to the location in the scope of the
        /// caller that holds a test counter. Since int is a BCL primitive, it
        /// must be passed by reference to achieve the desired effect, which is
        /// to maintain a continuous run of test numbers across every method,
        /// without needing to use a return value to update it.
        /// </param>
        /// <param name="pcmdArgs">
        /// This array of strings is a reference to the command line argument
        /// list that is passed into the main routine by the application startup
        /// code. Since arrays are always reference types, pass by reference is
        /// implied.
        /// </param>
        /// <remarks>
        /// The FixedConsoleWriter class around which this method was created is
        /// intended for displaying such things as text based countdown clocks
        /// and updates for long running processes, such as reading many records
        /// from a file, creating a large compressed archive, or computing the
        /// message digest of a big file.
        /// </remarks>
        private static void TimedHaltTest (
            ref int pintTestNbr ,
            CmdLneArgsBasic pcmdArgs ,
			bool pfDeepFreeze )
        {
            const string ARG_COUNT = @"count";
            const string ARG_INTERVAL = @"interval";
            //            const string ARG_FORMAT = @"###0";
            const string ARG_LBL_COUNT = @"Count   ";
            const string ARG_LBL_INTERVAL = @"Interval";

            //  ================================================================
            //  About the ARG_MSG_TPL_2 String
            //
            //  This string uses composite formatting, and has a little bit of
            //  almost everything, as set forth in the following table.
            //
            //  Format Item Interpretation
            //  ----------- ----------------------------------------------------
            //  {0,-8}      Format the first item on the list left aligned and
            //              padded with spaces as needed to make the total width
            //              eight characters.
            //
            //  {1,6:N0}    Format the second item on the list right aligned and
            //              padded with spaces as needed to make the total width
            //              six characters. Format the item as an integer, with
            //              zero decimal places.
            //
            //  {2,6:N0}    Format the third item on the list in the same way as
            //              the second item is formatted.
            //  ----------------------------------------------------------------
            //
            //  By converting their objects into strings, which the format item
            //  sees as such, the ToString method on the unsigned integer in the
            //  commented-out Console.WriteLine method calls trumps the
            //  formatting code in the composite formatting string. Hence, the
            //  output looks like this.
            //
            //  Count    =   1000 (Default =     10)
            //  Interval =     10 (Default =  1,000)
            //
            //  The desired output is achieved by dropping the ToString method
            //  call, leaving them as unsigned integers, as was done with the
            //  third item in each list (also unsigned int).
            //
            //  Count    =  1,000 (Default =     10)
            //  Interval =     10 (Default =  1,000)
            //
            //  Near the bottom of this routine is a block of four statements,
            //  the first two of which are commented out. The statements that
            //  are commented out yielded the output shown in the first example,
            //  while the two active statements produce the output shown in the
            //  second example, immediately above this paragraph.
            //
            //  These four statements contain the only references to ARG_MSG_TPL_2
            //  in this method.
            //
            //  Reference:  "Composite Formatting," MSDN Library
            //              http://msdn.microsoft.com/en-us/library/txafckwd(v=vs.80).aspx
            //  ================================================================

            const string ARG_MSG_TPL_1 = @"{1}The next {0} tests use the following two parameters.{1}";
            const string ARG_MSG_TPL_2 = @"    {0,-8} = {1,6:N0} (Default = {2,6:N0})";

            const string DEFAULT = @"default";

			const int WAIT_ONE_SECOND = 1000;

			const uint DFLT_COUNT = 10;
            const uint DFLT_INTERVAL = 1000;

			const string FCW_TEST_MSG_TPL_1 = @"    Test a FixedConsoleWriter with {0} foreground color and{2}           {1} background color.{2}";
			const string FCW_TEST_MSG_TPL_2 = @"    Test a FixedConsoleWriter with {0} foreground color and{3}           {1} background color. Text is indented {2} spaces from the left.{3}{3}";
			const string FCW_MAIN_SCROLLING	= @"    Scroll the output from the last test up.";
			const string FCW_MAIN_RECYCLED  = @"    Recycle the lines used by the last test.";

            if ( pintTestNbr == MagicNumbers.ZERO )
                pintTestNbr++;

			//	----------------------------------------------------------------
			//	This routine is called twice. On the first pass, the console is
			//	permitted to scroll up between tests, so that all output stays
			//	visible. In contrast, the second pass uses a FixedConsoleWriter
			//	to reuse the same rows of the console window.
			//	----------------------------------------------------------------

			FixedConsoleWriter FCoutermost = pfDeepFreeze ? new FixedConsoleWriter ( ) : null;
			FixedConsoleWriter FCNextInner = pfDeepFreeze ? new FixedConsoleWriter ( ) : null;
			
            //  ----------------------------------------------------------------
            //  Test 1: Write a series of four strings, the last one shorter
            //          than its immediate predecessor. This test demonstrates
            //          that the filler works correctly for the simplest case of
            //          one-line messages.
            //  ----------------------------------------------------------------

            {   // Constrain the scope of fcolColorOut1
				if ( pfDeepFreeze )
				{
					FCoutermost.Write (
	                    TEST_BEGIN_TPL ,
		                pintTestNbr ,
			            string.Empty );
					Console.Error.WriteLine ( FCW_MAIN_RECYCLED );
					Console.Error.WriteLine (
						FCW_TEST_MSG_TPL_1 ,    								// Message template
						DEFAULT ,               								// Substitution token 0 = Foreground color
						DEFAULT ,               								// Substitution token 1 = Background color
						Environment.NewLine );  								// Substitution token 2 = NewLine
				}	// TRUE block, if ( pfDeepFreeze )
				else
				{
					Console.Write (
						TEST_BEGIN_TPL ,
						pintTestNbr ,
						string.Empty );
					Console.Error.WriteLine ( FCW_MAIN_SCROLLING );
					Console.Error.WriteLine (
						FCW_TEST_MSG_TPL_1 ,    								// Message template
						DEFAULT ,               								// Substitution token 0 = Foreground color
						DEFAULT ,               								// Substitution token 1 = Background color
						Environment.NewLine );  								// Substitution token 2 = NewLine
				}	// FALSE block, if ( pfDeepFreeze )

				//	------------------------------------------------------------
				//	The first test uses a default FixedConsoleWriter that keeps
				//	the current screen colors, and starts writing at the left
				//	edge of the window.
				//	------------------------------------------------------------

                FixedConsoleWriter fcolColorOut1 = new FixedConsoleWriter ( );

                foreach ( string strCurrTestString in s_astrFCWTestStrings )
                {
                    fcolColorOut1.Write ( strCurrTestString );
					System.Threading.Thread.Sleep ( WAIT_ONE_SECOND );
				}   // foreach ( string strCurrTestString in s_astrFCWTestStrings )

                fcolColorOut1.ScrollUp ( );
                Console.Error.WriteLine (
                    TEST_END_TPL ,
                    pintTestNbr );
            }   // Although fcolColorOut1 goes out of scope, I have found that it isn't necessarily destroyed right away.

            //  ----------------------------------------------------------------
            //  Test 2: Repeat the first test, using yellow text on a background
            //          of DarkMagenta. I chose the background color because it
            //          is one that is seldom, if ever, used as a default. This
            //          test demonstrates that the background behind the message
            //          is just enough to cover the message. Unused spaces, if
            //          any, revert to the default background color.
            //  ----------------------------------------------------------------

			if ( pfDeepFreeze )
			{
				FCoutermost.Write (
					TEST_BEGIN_TPL ,
					++pintTestNbr ,
					Environment.NewLine );
				Console.Error.WriteLine (
					FCW_TEST_MSG_TPL_1 ,        								// Message template
					ConsoleColor.Yellow ,       								// Substitution token 0 = Foreground color
					ConsoleColor.DarkMagenta ,  								// Substitution token 1 = Background color
					Environment.NewLine );      								// Substitution token 2 = NewLine
			}	// TRUE block, if ( pfDeepFreeze )
			else
			{
				Console.Error.WriteLine (
					TEST_BEGIN_TPL ,
					++pintTestNbr ,
					Environment.NewLine );
				Console.Error.WriteLine (
					FCW_TEST_MSG_TPL_1 ,        								// Message template
					ConsoleColor.Yellow ,       								// Substitution token 0 = Foreground color
					ConsoleColor.DarkMagenta ,  								// Substitution token 1 = Background color
					Environment.NewLine );      								// Substitution token 2 = NewLine
			}	// FALSE block, if ( pfDeepFreeze )

            {   // Constrain the scope of fcolColorOut2.
				FixedConsoleWriter fcolColorOut2 = new FixedConsoleWriter (
                    ConsoleColor.Yellow ,
                    ConsoleColor.DarkMagenta );

				foreach ( string strCurrTestString in s_astrFCWTestStrings )
                {
					fcolColorOut2.Write ( strCurrTestString );
					System.Threading.Thread.Sleep ( WAIT_ONE_SECOND );
				}   // foreach ( string strCurrTestString in s_astrFCWTestStrings )

                fcolColorOut2.ScrollUp ( );
                Console.Error.WriteLine (
                    TEST_END_TPL ,
                    pintTestNbr );
            }   // While it may still exist, the compiler won't let you use fcolColorOut2 past this point.

            //  ----------------------------------------------------------------
            //  Test 3: This is the test that motivated me to design, write, and
            //  test the FixedConsoleWriter class.
            //
            //  Since all tests use the same count and interval values, parsing
            //  the argument list and reporting the specified values or their
            //  defaults has no business being inside the FOR loop that runs the
            //  test using a series of progressively further indented starting
            //  points.
            //  ----------------------------------------------------------------

            //  ----------------------------------------------------------------
            //  Please see the note headed "About the ARG_MSG_TPL_2 String" near
            //  the top of this routine.
            //  ----------------------------------------------------------------

            //  ================================================================
            //  ALERT: This commented-out block STAYS PUT. The commentary at the
            //         top of this routine cites it as an example.
            //  ================================================================

            //Console.Error.WriteLine (
            //    ARG_MSG_TPL_2 ,
            //    ARG_LBL_COUNT ,
            //    uintInterval.ToString ( ARG_FORMAT ) ,
            //    DFLT_COUNT );
            //Console.Error.WriteLine (
            //    ARG_MSG_TPL_2 ,
            //    ARG_LBL_INTERVAL ,
            //    uintCount.ToString ( ARG_FORMAT ) ,
            //    DFLT_INTERVAL );

            //  ================================================================
            //  END of Protected Commented-out code.
            //  ================================================================

            uint uintCount = GetUintArgByName (
                pcmdArgs ,
                ARG_COUNT ,
                DFLT_COUNT );
            uint uintInterval = GetUintArgByName (
                pcmdArgs ,
                ARG_INTERVAL ,
                DFLT_INTERVAL );

			if ( pfDeepFreeze )
			{
				FCoutermost.Write (
					ARG_MSG_TPL_1 ,												// This format string implements minimalist substitution tokens.
					s_uintOffsetFromLeft.Length ,   							// Substitution token 0 is the number of tests in the next group.
					Environment.NewLine );          							// Substitution token 1 is the platform dependent newline sequence.
			}	// TRUE block, if ( pfDeepFreeze )
			else
			{
				Console.Error.WriteLine (
					ARG_MSG_TPL_1 ,												// This format string implements minimalist substitution tokens.
					s_uintOffsetFromLeft.Length ,								// Substitution token 0 is the number of tests in the next group.
					Environment.NewLine );										// Substitution token 1 is the platform dependent newline sequence.
			}	// FALSE block, if ( pfDeepFreeze )

			Console.Error.WriteLine (
				ARG_MSG_TPL_2 ,                 								// This format string implements composite formats.
				ARG_LBL_COUNT ,                 								// Substitution token 0 is the label, which is padded to a minimum width of 8 characters.
				uintCount ,                     								// Substitution token 1 is the value of the actual count argument, which is right aligned to a minimum width of 6 characters, and formatted as a Number with zero decimal places (an integer).
				DFLT_COUNT );                   								// Substitution token 2 is the value of the default count argument, which is right aligned to a minimum width of 6 characters, and formatted as a Number with zero decimal places (an integer).
			Console.Error.WriteLine (
				ARG_MSG_TPL_2 ,                 								// String formatting template
				ARG_LBL_INTERVAL ,              								// Substitution token 0 = Parameter Name (label)
				uintInterval ,                  								// Substitution token 1 = Actual Parameter Value
				DFLT_INTERVAL );                								// Substitution token 2 = Default Parameter Value

            foreach ( int intOffsetFromLeft in s_uintOffsetFromLeft )
            {
				if ( pfDeepFreeze )
				{
					FCNextInner.Write (
						TEST_BEGIN_TPL ,
						++pintTestNbr ,
						Environment.NewLine );
				}	// TRUE block, if ( pfDeepFreeze )
				else
				{
					Console.Error.WriteLine (
						TEST_BEGIN_TPL ,
						++pintTestNbr ,
						Environment.NewLine );
				}	// FALSE block, if ( pfDeepFreeze )

				Console.Error.WriteLine (
					FCW_TEST_MSG_TPL_2 ,										// Message template
					new object [ ]
                    {
                        ConsoleColor.Black ,									// Substitution token 0 = Foreground color
                        ConsoleColor.White ,									// Substitution token 1 = Background color
                        intOffsetFromLeft ,										// Substitution token 2 = Characters to indent
                        Environment.NewLine										// Substitution token 3 = NewLine
                    } );

				//	------------------------------------------------------------
				//	All remaining tests use a FixedConsoleWriter that overrides
				//	the current screen colors, and starts writing at a preset
				//	indent from the left edge of the window.
				//	------------------------------------------------------------

				FixedConsoleWriter fcolColorOut3 = new FixedConsoleWriter (
                    ConsoleColor.Black ,
                    ConsoleColor.White ,
					intOffsetFromLeft );

                for ( uint uintIteration = 1 ;
                           uintIteration <= uintCount ;
                           uintIteration++ )
                {
                    //  --------------------------------------------------------
                    //  This parameter array MUST be cast to object. Casting it
					//	to int causes the wrong overload to be called. I might
					//  be able to fix the problem by using generics, which is
                    //  probably how Console.Write and Console.WriteLine are
                    //  implemented.
                    //  --------------------------------------------------------

                    fcolColorOut3.Write (
                        SelectIterationMsg (
                            uintCount ,
							uintIteration ) ,									// Message template.
                        new object [ ]
                            {
                                uintInterval ,									// Substitution token 0 = Interval (ms.)
                                uintInterval / 1000 ,   						// Substitution token 1 = interval (secs.)
                                uintIteration ,         						// Substitution token 2 = Current iteration number
                                uintCount               						// Substitution token 3 = Total iterations.
                            } );
                    System.Threading.Thread.Sleep ( ( int ) uintInterval );
                }   // for ( uint uintIteration = 1 ; uintIteration <= uintCount ; uintIteration++ )

				Console.Error.WriteLine ( 
					TEST_END_TPL ,
					pintTestNbr );
			}   // foreach ( int intOffsetFromLeft in s_uintOffsetFromLeft )
        }   // private static void TimedHaltTest


        /// <summary>
        /// Scan the program argument list for a named argument whose value is
        /// an unsigned integer. Please see the Remarks.
        /// </summary>
        /// <param name="pcmdArgs">
        /// Pass in a reference to your initialized CmdLneArgsBasic object.
        /// </param>
        /// <param name="pstrArgName">
        /// Pass in the name of the desired argument.
        /// </param>
        /// <param name="puintDefault">
        /// Pass in an unsigned integer to return if the named argument is
        /// missing from the command line.
        /// </param>
        /// <returns>
        /// The return value is either the unsigned integer that was passed into
        /// the program on its command line or the default value that was passed
        /// into this method.
        /// </returns>
        /// <remarks>
        /// The CmdLneArgsBasic class offers "block box" methods that return the
        /// string representation of an named argument, but I have yet to create
        /// overloads that further process them through the TryParse methods of
        /// the various integer classes. Hence, this method is essentially an
        /// overload of CmdLneArgsBasic.GetArgByName.
        /// </remarks>
        private static uint GetUintArgByName ( 
            CmdLneArgsBasic pcmdArgs ,
            string pstrArgName ,
            uint puintDefault )
        {
            uint ruintValue;

            string strValue = pcmdArgs.GetArgByName (
                pstrArgName ,
                puintDefault.ToString ( ) ,
                CmdLneArgsBasic.BLANK_AS_DEFAULT_ALLOWED );

            if ( uint.TryParse ( strValue , out ruintValue ) )
                return ruintValue;
            else
                return puintDefault;
        }   // private static uint GetUintArgByName


        /// <summary>
        /// Scan the program argument list for a named switch whose value is
        /// an unsigned integer. Please see the Remarks.
        /// </summary>
        /// <param name="pcmdArgs">
        /// Pass in a reference to your initialized CmdLneArgsBasic object.
        /// </param>
        /// <param name="pchrArgName">
        /// Pass in the character representation of the desired switch. I use a
        /// symbolic constant that looks more like a name.
        /// </param>
        /// <param name="puintDefault">
        /// Pass in an unsigned integer to return if the named argument is
        /// missing from the command line.
        /// </param>
        /// <returns>
        /// The return value is either the unsigned integer that was passed into
        /// the program on its command line or the default value that was passed
        /// into this method.
        /// </returns>
        /// <remarks>
        /// The CmdLneArgsBasic class offers "block box" methods that return the
        /// string representation of a command line switch, but I have yet to 
        /// create overloads that further process them through the TryParse 
        /// methods of the various integer classes. Hence, this method is
        /// essentially the missing overload of CmdLneArgsBasic.GetSwitchByName.
        /// </remarks>
        private static uint GetUintSwitchByName (
            CmdLneArgsBasic pcmdArgs ,
            char pchrArgName ,
            uint puintDefault )
        {
            uint ruintValue;

            string strValue = pcmdArgs.GetSwitchByName ( pchrArgName );

            if ( uint.TryParse ( strValue , out ruintValue ) )
                return ruintValue;
            else
                return puintDefault;
        }   // private static uint GetUintSwitchByName


        /// <summary>
        /// The goal of this method is to print the iteration information on the
        /// first and last iterations, and show only the time in the middle
        /// iterations. Please see the Remarks.
        /// </summary>
        /// <param name="puintCount">
        /// Pass in the total number of iterations.
        /// </param>
        /// <param name="puintIteration">
        /// Pass in the number of the current iteration. Iterations are numbered
        /// from one, and go to count.
        /// </param>
        /// <returns>
        /// The return value is one of two format strings, either of which can
        /// be populated from the same array.
        /// </returns>
        /// <remarks>
        /// The intention is to use short strings in the middle messages to test
        /// the reset method on wrap-around strings.
        /// 
        /// This method could easily be replaced by an inline IF statement, but
        /// I prefer this more compact expression of my intent, which does with
        /// one string.Format method call what would otherwise require two
        /// virtually identical calls.
        /// </remarks>
        private static string SelectIterationMsg (
            uint puintCount ,
            uint puintIteration )
        {
            const uint FIRST = WizardWrx.MagicNumbers.PLUS_ONE;

            if ( puintIteration == FIRST || puintIteration == puintCount )
                return FCW_TEST_MSG_TPL_3;
            else
                return FCW_TEST_MSG_TPL_4;
        }   // private static string SelectIterationMsg


        #if ALT_EXCPT_LOG_PATH
            private static void SetExceptionLoggingProperties ( )
            {
                ExceptionLogger exLog = s_theApp.BaseStateManager.AppExceptionLogger;

                #if HIDE_STACKTRACE
	    			exLog.OptionFlags = exLog.OptionFlags & ( ~ExceptionLogger.OutputOptions.Stack );
                #endif  // #if HIDE_STACKTRACE

    			exLog.OptionFlags = exLog.OptionFlags | ExceptionLogger.OutputOptions.EventLog;

                #if ALT_EXCPT_LOG_COLOURS
                    exLog.ErrorMessageColors = new ErrorMessagesInColor (
                        ConsoleColor.Red ,
                        ConsoleColor.White );
                #endif  // #if ALT_EXCPT_LOG_COLOURS
            }   // private static void SetExceptionLoggingProperties
        #endif  // ALT_EXCPT_LOG_PATH


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
		private static void HandleConsoleIOException ( System.IO.IOException pexIo )
		{
			#if TRACE
				System.Diagnostics.Trace.WriteLine (
					string.Format (
						Properties.Resources.ERRMSG_EXCEPTION_FOR_TRACE ,						// Message template.
						new object [ ]
						{																		// Construct a parameter array of objects on the stack.
							SysDateFormatters.FormatDateTimeForShow ( DateTime.Now ) ,			// Format Item 0 = {0}: An {1} exception
							pexIo.GetType ( ).Name ,											// Format Item 1 = An {1} exception
							pexIo.TargetSite ,													// Format Item 2 = routine {2} - The TargetSite property has it.
							pexIo.Message ,														// Format Item 3 = Message            = {3}
							Console.CursorLeft ,												// Format Item 4 = Console.CursorLeft = {4}
							Console.CursorTop ,													// Format Item 5 = Console.CursorTop  = {5}
							Environment.NewLine													// Format Item 6 = Newline, My Way
						}																		// Array of format items
					)																			// string.Format method
				);																				// System.Diagnostics.Trace.WriteLine
			#endif	// #if TRACE
		}	// private static void HandleConsoleIOException
	}   // class Program
}   // namespace TestStand