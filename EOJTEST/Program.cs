/*
    ============================================================================

    Namespace Name:     EOJTEST

    File Name:          program.cs

    Class Name:         Program

    Synopsis:           Exercise the NormalExit methods in class library
                        WizardWrx.ApplicationHelpers2.

    Remarks:            My normal test stand is inadequate for testing a routine
                        that evaluates the program's exit code and terminates in
                        some way. 

                        By fulfilling that need, this program supplements the
                        regular test stand, ApplicationHelpersTestHarness. It is
                        intended to run in a batch file. EndOfJobTestScript.CMD,
                        which lives in the _Notes_and_References directory that
                        is associated with this project.

    Reference:

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

    Created:            Monday, 03 September 2012

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version By  Description
    ---------- ------- --- -----------------------------------------------------
    2012/03/03 2.6     DAG Complete the constellation by adding ReformatNow and
                           ReformatUtcNow.

    2013/11/19 3.0     DAG Implement a new NormalExitAction type, Silent, for
                           which this routine provides no test case, because
                           another project, EOJTest, does so.

    2014/06/06 5.0     DAG Major namespace reorganization.

    2014/12/15 5.4     DAG Swap the order of the calls to the WaitForCarbonUnit
                           and DisplayEOJMessage methods, so that the end-of-job
                           message is displayed before the carbon unit prompt.

                           The changes in this module are entirely cosmetic, as
                           the changes made in the test subject are internal.
                           Hence, the testing protocol is unchanged.

	2016/04/09 6.0     DAG Test the new redirection reporting and processing.
						   Finish breaking free of dependence on the old strong
                           name signed class libraries.

	2016/05/10 6.0     DAG Incorporate the assembly properties report that I
                           added to WizardWrrx.DllServices2.dll.

	2016/05/20 6.1     DAG Synchronize this version number with that of its 
                           dependent library, WizardWrx.DllServices2.dll.

	2016/06/09 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically
                           belong in this library being found instead in
                           DLLServices2.

	2017/08/06 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.

	2018/11/26 7.0     DAG Eliminate the unreferenced system namespaces, and tag
                           my own assembly references with explanatory notes.

	2020/10/23 7.2     DAG 1) Implement Semantic Version Numbering. This build
                              also incorporates the current stable versions of
                              the WizardWrx .NET API asseblies, which finally
                              fully implement Semantic Version Numbering as of a
                              few days ago.

                           2) Add the new required second argument to method call
                              ReportGenerators.ShowKeyAssemblyProperties.

    2022/10/15 8.1.532 DAG Test the new method that takes a NormalExitAction and
                           a Boolean value that, when True, causes the method to
                           disregard the status code when its value exceeds the
                           size of the message table because its objective is to
                           control the action of its calling process, which is
                           usually a shell script.

                           As well, implement the capability to execute
                           individual test scenarios in the Visual Studio
                           debugger, obviating the need for the shell script
                           that runs each test sequentially.
    ============================================================================
*/


using System;

using WizardWrx;                                                                // This is the mother of all my library namespaces.
using WizardWrx.AssemblyUtils;													// Shorten the absolute call path to ReportGenerators.ShowKeyAssemblyProperties.
using WizardWrx.ConsoleAppAids3;												// This is the library under test.


namespace EOJTEST
{
    class Program
    {
        enum ExitCodeDisposition
        {
            PassAsArg ,
            StoreInMy
        }   // ExitCodeDisposition

        struct ExitRules
        {
            public int ExitCode;
            public ExitCodeDisposition Disposition;
            public ConsoleAppStateManager.NormalExitAction Action;
            internal bool IsForCallingProcess;
        }   // ExitRules

        static string [ ] astrExitAction =
        {
            "    Program will exit immediately and unconditionally." ,                          // 0 = ConsoleAppStateManager.NormalExitAction.ExitImmediately
            "    Program will exit immediately if exit code is ERROR_SUCCESS." ,                // 1 = ConsoleAppStateManager.NormalExitAction.HaltOnError
            "    Program will exit immediately, unconditionally, and silently.",                // 2 = ConsoleAppStateManager.NormalExitAction.Silent
            SpecialStrings.EMPTY_STRING ,                                                       // 3 = UNUSED 
            "    Program will wait for a carbon unit."                                          // 4 = ConsoleAppStateManager.NormalExitAction.WaitForOperator
        };  // static string [ ] astrExitAction

        static string [ ] astrExitCodeDisposition =
        {
            SpecialStrings.EMPTY_STRING,                                                        //
            "    Exit code will be passed through the argument list.",                          // case ExitCodeDisposition.PassAsArg
            "    Exit code will be retrieved from the ApplicationInstance."                     // case ExitCodeDisposition.StoreInMy
        };  // static string [ ] astrExitCodeDisposition

        static ConsoleAppStateManager _me = ConsoleAppStateManager.GetTheSingleInstance ( );

        static void Main ( string [ ] args )
        {
            const int ERR_NO_ARGS = 999;
            const int ERR_ONE_ARG = 998;
            const int ERR_TWO_ARG = 997;

            const string ERRMSG_INTERNAL = @"An internal error has occurred in the NormalExit testing block.";
            const string ERRMSG_NO_ARGS = @"    Called without arguments. Two are required.";
            const string ERRMSG_ONE_ARG = @"    Called with one argument. We need three of them.";
            const string ERRMSG_TWO_ARG = @"    Called with two argument. We need three of them.";

            _me.DisplayBOJMessage ( );

            ReportGenerators.ShowKeyAssemblyProperties (
                System.Reflection.Assembly.GetAssembly (    // Assembly pasmSubject = Assembly for which to display key properties
                    _me.GetType ( ) ) ,                     // Type type            = Given a type, identify the assembly that exposes it.
                MagicNumbers.ZERO ,                         // int pintJ            = ordinal number, incremented by the method
                MagicNumbers.PLUS_ONE );                    // int pintNDependents  = Number of items in list being processed

            try
            {
                switch ( args.Length )
                {
                    case MagicNumbers.ZERO:
                        NeedMoreArgs ( ERRMSG_NO_ARGS , ERR_NO_ARGS );
                        break;

					case MagicNumbers.PLUS_ONE:
                        NeedMoreArgs ( ERRMSG_ONE_ARG , ERR_ONE_ARG );
                        break;

					case MagicNumbers.PLUS_TWO:
                        NeedMoreArgs ( ERRMSG_TWO_ARG , ERR_TWO_ARG );
                        break;

                    default:
                        ShowArgs ( args );
                        ExitRules utpExitRulesMap = EvaluateArguments ( args );

                        if ( utpExitRulesMap.IsForCallingProcess )
                        {
                            _me.BaseStateManager.AppReturnCode = utpExitRulesMap.ExitCode;
                            _me.NormalExit ( utpExitRulesMap.Action , utpExitRulesMap.IsForCallingProcess );
                        }   // TRUE block, if ( utpExitRulesMap.IsForCallingProcess )
                        else
                        {
                            switch ( utpExitRulesMap.Disposition )
                            {
                                case ExitCodeDisposition.PassAsArg:
                                    _me.NormalExit (
                                        ( uint ) utpExitRulesMap.ExitCode ,
                                        utpExitRulesMap.Action );
                                    break;

                                case ExitCodeDisposition.StoreInMy:
                                    _me.BaseStateManager.AppReturnCode = ( int ) utpExitRulesMap.ExitCode;
                                    _me.NormalExit ( utpExitRulesMap.Action );
                                    break;

                                default:
                                    throw new Exception ( ERRMSG_INTERNAL );
                            }   // switch ( utpExitRulesMap.Disposition )
                        }   // FALSE block, if ( utpExitRulesMap.IsForCallingProcess )

                        break;
                }   // switch ( args.Length )
            }   // The normal execution path ends here.
            catch ( Exception errAllKinds )
            {
				_me.BaseStateManager.AppExceptionLogger.ReportException ( errAllKinds );
                _me.DisplayEOJMessage ( );
            }
        }   // static void Main


        private static void NeedMoreArgs (
            string pstrMsg ,
            int pintExitCode )
        {
            Console.WriteLine ( pstrMsg );
            _me.DisplayEOJMessage ( );
            DisplayAids.WaitForCarbonUnit  ( );
            Environment.Exit ( pintExitCode );
        }   // private static void NeedMoreArgs


        private static ExitRules EvaluateArguments ( string [ ] pastrArgs )
        {
            const int ARG_EXIT_CODE = ArrayInfo.ARRAY_FIRST_ELEMENT;
            const int ARG_EXIT_ACTION = ARG_EXIT_CODE + MagicNumbers.PLUS_ONE;
			const int ARG_EXIT_DISPOSITION = ARG_EXIT_ACTION + MagicNumbers.PLUS_ONE;
            const int ARG_EXIT_CODE_FOR_CALLING_PROCESS = ARG_EXIT_DISPOSITION + MagicNumbers.PLUS_ONE;

            const string ENUM_NAME_EXIT_CODE_DISP = @"ExitCodeDisposition";
            const string ENUM_NAME_EXIT_ACTION = @"NormalExitAction";

            const string ERRMSG_INTERNAL_ERROR = @"An internal error has occurred.";
            const string ERRMSG_MUST_BE_NUMERIC = @"Argument {0} must be an integer. The specified value, {1}, is not.";
            const string ERRMSG_UNNECESSARY_ARG = @"Argument {0} value = {1} - SKIPPED";
            const string ERRMSG_VALUE_OUT_OF_RANGE = @"Argument {0} value = {1} - Value must be in {2} enumeration.";

            ExitRules rutpExitRulesMap;

            rutpExitRulesMap.Action = 0;
            rutpExitRulesMap.Disposition = 0;
            rutpExitRulesMap.ExitCode = 0;
            rutpExitRulesMap.IsForCallingProcess = false;

            bool fRequirementsMet = false;

            uint uintNArgs = ( uint ) pastrArgs.Length;

            for ( uint uintCurrArg = ArrayInfo.ARRAY_FIRST_ELEMENT ;
                  uintCurrArg < uintNArgs ;
                  uintCurrArg++ )
            {
                int intToTest = MagicNumbers.ZERO;
				int intRealValue = MagicNumbers.ZERO;

                if ( int.TryParse ( pastrArgs [ uintCurrArg ] , out intToTest ) )
                {
                    switch ( uintCurrArg )
                    {
                        case ARG_EXIT_CODE:
                            rutpExitRulesMap.ExitCode = intToTest;
                            break;

                        case ARG_EXIT_ACTION:
                            Console.WriteLine ( $"{Environment.NewLine}ConsoleAppStateManager.NormalExitAction.ExitImmediately = {( int ) ConsoleAppStateManager.NormalExitAction.ExitImmediately}" );
                            Console.WriteLine ( $"ConsoleAppStateManager.NormalExitAction.HaltOnError     = {( int ) ConsoleAppStateManager.NormalExitAction.HaltOnError}" );
                            Console.WriteLine ( $"ConsoleAppStateManager.NormalExitAction.WaitForOperator = {( int ) ConsoleAppStateManager.NormalExitAction.WaitForOperator}" );
                            Console.WriteLine ( $"ConsoleAppStateManager.NormalExitAction.Silent          = {( int ) ConsoleAppStateManager.NormalExitAction.Silent}{Environment.NewLine}" );

                            intRealValue = intToTest + ArrayInfo.INDEX_FROM_ORDINAL;
                            ConsoleAppStateManager.NormalExitAction enmExitAction = ( ConsoleAppStateManager.NormalExitAction ) intRealValue;

                            switch ( enmExitAction )
                            {
                                case ConsoleAppStateManager.NormalExitAction.ExitImmediately:
                                case ConsoleAppStateManager.NormalExitAction.HaltOnError:
                                case ConsoleAppStateManager.NormalExitAction.WaitForOperator:
                                case ConsoleAppStateManager.NormalExitAction.Silent:
                                    Console.WriteLine ( astrExitAction [ intRealValue ] );
                                    rutpExitRulesMap.Action = ( ConsoleAppStateManager.NormalExitAction ) intRealValue;
                                    break;

                                default:
                                     string strMessage = string.Format (
                                         ERRMSG_VALUE_OUT_OF_RANGE ,
                                         uintCurrArg + ArrayInfo.INDEX_FROM_ORDINAL ,
                                         intRealValue ,
                                         ENUM_NAME_EXIT_ACTION );
                                     throw new Exception ( strMessage );
                            }   // switch ( enmExitAction )

                            break;  // case ARG_EXIT_ACTION of switch ( uintCurrArg )

                        case ARG_EXIT_DISPOSITION:
                            Console.WriteLine ( $"{Environment.NewLine}ExitCodeDisposition.PassAsArg = {( int ) ExitCodeDisposition.PassAsArg}" );
                            Console.WriteLine ( $"ExitCodeDisposition.StoreInMy = {( int ) ExitCodeDisposition.StoreInMy}{Environment.NewLine}" );

                            intRealValue = intToTest + ArrayInfo.INDEX_FROM_ORDINAL;
                            ExitCodeDisposition enmExitCodeDisposition = ( ExitCodeDisposition ) intRealValue;

                            switch ( enmExitCodeDisposition )
                            {
                                 case ExitCodeDisposition.PassAsArg:
                                 case ExitCodeDisposition.StoreInMy:
                                     Console.WriteLine ( astrExitCodeDisposition [ intRealValue ] );
                                    rutpExitRulesMap.Disposition = enmExitCodeDisposition;
                                     fRequirementsMet = true;
                                     break;

                                 default:
                                     string strMessage = string.Format (
                                         ERRMSG_VALUE_OUT_OF_RANGE ,
                                         uintCurrArg + ArrayInfo.INDEX_FROM_ORDINAL ,
                                         intRealValue ,
                                         ENUM_NAME_EXIT_CODE_DISP );
                                     throw new Exception ( strMessage );
                            }   // switch ( enmExitCodeDisposition )

                            break;  // case ARG_EXIT_DISPOSITION of switch ( uintCurrArg )

                        case ARG_EXIT_CODE_FOR_CALLING_PROCESS:
                            rutpExitRulesMap.IsForCallingProcess = true;

                            break;  // case ARG_EXIT_CODE_FOR_CALLING_PROCESS of switch ( uintCurrArg )

                        default:
                            Console.WriteLine (
                                ERRMSG_UNNECESSARY_ARG ,
                                uintCurrArg + ArrayInfo.INDEX_FROM_ORDINAL ,
                                pastrArgs [ uintCurrArg ] );
                            break;
                    }   // switch ( uintCurrArg )
                }   // TRUE block, if ( int.TryParse ( pastrArgs [ uintCurrArg ] , out intToTest ) )
                else
                {   // Depending on their position, non-integral arguments are ignored, if extra, or provoke an exception.
                    if ( uintCurrArg > ARG_EXIT_DISPOSITION )
                    {   // It's extra. List it for the record, and discard it.
                        Console.WriteLine (
                            ERRMSG_UNNECESSARY_ARG ,
                            uintCurrArg + ArrayInfo.INDEX_FROM_ORDINAL ,
                            pastrArgs [ uintCurrArg ] );
                    }   // TRUE block, if ( uintCurrArg > ARG_EXIT_DISPOSITION )
                    else
                    {   // It's one of the required three. Die.
                        string strMessage = string.Format (
                            ERRMSG_MUST_BE_NUMERIC ,
                            uintCurrArg + ArrayInfo.INDEX_FROM_ORDINAL ,
                            pastrArgs [ uintCurrArg ] );
                        throw new Exception ( strMessage );
                    }   // FALSE block, if ( uintCurrArg > ARG_EXIT_DISPOSITION )
                }   // FALSE block, if ( int.TryParse ( pastrArgs [ uintCurrArg ] , out intToTest ) )
            }   // for ( uint uintCurrArg = StandardConstants.ARRAY_FIRST_ELEMENT ; uintCurrArg < uintNArgs ; uintCurrArg++ )

            if ( fRequirementsMet )
            {
                DisplayAids.WaitForCarbonUnit  ( );
                return rutpExitRulesMap;
            }
            else
            {
                throw new Exception ( ERRMSG_INTERNAL_ERROR );
            }   // if ( fRequirementsMet )
        }   // private static ExitRules EvaluateArguments


        private static void ShowArgs ( string [ ] pastrArgs )
        {
            const int REQUIRED_ARGS = 3;

            const string MSG_ARG_1 = @"    Argument 1: Status Code          = {0}";
            const string MSG_ARG_2 = @"    Argument 2: NormalExit Action    = {0}";
            const string MSG_ARG_3 = @"    Argument 2: ExitCode Disposition = {0}";
            const string MSG_EXTRA_ARGS = "\n    There are a total of {0} arguments\n    The extra {1} arguments will be discarded.";

            Console.WriteLine ( MSG_ARG_1 , pastrArgs [ ArrayInfo.ARRAY_FIRST_ELEMENT ] );
            Console.WriteLine ( MSG_ARG_2 , pastrArgs [ MagicNumbers.PLUS_ONE ] );
			Console.WriteLine ( MSG_ARG_3 , pastrArgs [ MagicNumbers.PLUS_TWO ] );

            if ( pastrArgs.Length > REQUIRED_ARGS )
                Console.WriteLine ( MSG_EXTRA_ARGS , pastrArgs.Length , pastrArgs.Length - REQUIRED_ARGS );
        }   // private static void ShowArgs
    }   // class Program
}   // namespace EOJTEST