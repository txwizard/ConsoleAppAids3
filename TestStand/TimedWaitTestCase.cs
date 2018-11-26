/*
    ============================================================================

    Namespace:          TestStand

    Class Name:         TimedWaitTestCase

    File Name:          TimedWaitTestCase.cs

    Synopsis:           This class encapsulates the data for a test case to
                        test a WizardWrx.ConsoleAppAids3.DisplayAids.TimedWait
                        instance.

    Remarks:            The public constructor takes a string, which is assumed
                        to be the input from a TAB delimited text file. A static
                        constructor takes another string, loaded from a string
                        resource, that defines the label row and, by inference,
                        the field order.

    References:         Another Way to Escape Sequences in .NET Resource Files
                        http://www.devx.com/tips/Tip/34769

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

    Created:            Friday, 14 March 2014

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version By  Description
    ---------- ------- --- -----------------------------------------------------
    2014/03/14 3.5     DAG Initial implementation.
    
	2014/06/06 5.0     DAG Major namespace reorganization.
    
	2014/06/23 5.1     DAG Further fine tuning of the namespace.
	
	2015/06/05 5.5     DAG Replace exceptions with messages fed into the
                           Standard Error stream.

	2015/11/07 6.0     DAG Finish breaking free of dependence on the old strong
                           name signed class libraries.

	2016/06/09 6.3     DAG Technical clarifications to internal documentation
                           account for changes of plans and other technical
                           reasons for some of the classes that logically
                           belong in this library being found instead in
                           DLLServices2.

	2017/08/06 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.
    ============================================================================
*/


using System;
using System.Collections.Generic;
using System.Text;

using WizardWrx;
using WizardWrx.ConsoleAppAids3;

namespace TestStand
{
    internal class TimedWaitTestCase
    {
        #region Infrastructure
        static string [ ] s_astrTimedWaitTestCaseInfo;
        static char [ ] s_achrFieldDelimiter;

        static int s_intPos_puintWaitSeconds =  ArrayInfo.ARRAY_INVALID_INDEX;
        static int s_intPos_pstrCountdownWaitingFor = ArrayInfo.ARRAY_INVALID_INDEX;
        static int s_intPos_pclrTextColor = ArrayInfo.ARRAY_INVALID_INDEX;
        static int s_intPos_pclrTextBackgroundColor = ArrayInfo.ARRAY_INVALID_INDEX;
        static int s_intPos_penmInterruptCriterion = ArrayInfo.ARRAY_INVALID_INDEX;

        static TimedWaitTestCase ( )
        {
            const string LBL_PUINTWAITSECONDS = @"puintWaitSeconds";
            const string LBL_PSTRCOUNTDOWNWAITINGFOR = @"pstrCountdownWaitingFor";
            const string LBL_PCLRTEXTCOLOR = @"pclrTextColor";
            const string LBL_PCLRTEXTBACKGROUNDCOLOR = @"pclrTextBackgroundColor";
            const string LBL_PENMINTERRUPTCRITERION = @"penmInterruptCriterion";

            s_astrTimedWaitTestCaseInfo = Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.Split ( new char [ ] { SpecialCharacters.COMMA } );

            int intPosition = ArrayInfo.ARRAY_INVALID_INDEX;

            foreach ( string strThisLabel in s_astrTimedWaitTestCaseInfo )
            {
                intPosition++;

                switch ( strThisLabel )
                {
                    case LBL_PUINTWAITSECONDS:
                        s_intPos_puintWaitSeconds = intPosition;
                        break;

                    case LBL_PSTRCOUNTDOWNWAITINGFOR:
                        s_intPos_pstrCountdownWaitingFor = intPosition;
                        break;

                    case LBL_PCLRTEXTCOLOR:
                        s_intPos_pclrTextColor = intPosition;
                        break;

                    case LBL_PCLRTEXTBACKGROUNDCOLOR:
                        s_intPos_pclrTextBackgroundColor = intPosition;
                        break;

                    case LBL_PENMINTERRUPTCRITERION:
                        s_intPos_penmInterruptCriterion = intPosition;
                        break;

                    default:
                        string strMsg = string.Format (
                            Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_1 ,         // Message template
                            new string [ ]
                                {
                                    Properties.Resources.TIMED_WAIT_TEST_CASE_INFO ,    // Token 0
                                    strThisLabel ,                                      // Token 1
                                    intPosition.ToString ( ) ,                          // Token 2
                                    Environment.NewLine                                 // Token 3
                                } );
						Console.Error.WriteLine ( strMsg );								// Rather than throwing an exception, emit a message into the Standard Error stream.
						break;
                }   // switch ( strThisLabel )
            }   // foreach ( string strThisLabel in s_astrTimedWaitTestCaseInfo )

            if ( s_intPos_puintWaitSeconds == ArrayInfo.ARRAY_INVALID_INDEX )
            {
                string strMsg = string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_2 ,                 // Message template
                    LBL_PUINTWAITSECONDS ,                                              // Token 0
                    Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.QuoteString ( ) );   // Token 1
				Console.Error.WriteLine ( strMsg );										// Rather than throwing an exception, emit a message into the Standard Error stream.
			}   // if ( s_intPos_puintWaitSeconds == ArrayInfo.ARRAY_INVALID_INDEX )

            if ( s_intPos_pstrCountdownWaitingFor == ArrayInfo.ARRAY_INVALID_INDEX )
            {
                string strMsg = string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_2 ,                 // Message template
                    LBL_PSTRCOUNTDOWNWAITINGFOR ,                                       // Token 0
                    Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.QuoteString ( ) );   // Token 1
				Console.Error.WriteLine ( strMsg );										// Rather than throwing an exception, emit a message into the Standard Error stream.
			}   // if ( s_intPos_pstrCountdownWaitingFor == ArrayInfo.ARRAY_INVALID_INDEX )

			if ( s_intPos_pclrTextColor == ArrayInfo.ARRAY_INVALID_INDEX )
			{
				string strMsg = string.Format (
					Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_2 ,                 // Message template
					LBL_PCLRTEXTCOLOR ,                                                 // Token 0
					Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.QuoteString ( ) );   // Token 1
				Console.Error.WriteLine ( strMsg );										// Rather than throwing an exception, emit a message into the Standard Error stream.
			}   // if ( s_intPos_pclrTextColor == ArrayInfo.ARRAY_INVALID_INDEX )

            if ( s_intPos_pclrTextBackgroundColor == ArrayInfo.ARRAY_INVALID_INDEX )
            {
                string strMsg = string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_2 ,                 // Message template
                    LBL_PCLRTEXTBACKGROUNDCOLOR ,                                       // Token 0
                    Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.QuoteString ( ) );   // Token 1
				Console.Error.WriteLine ( strMsg );										// Rather than throwing an exception, emit a message into the Standard Error stream.
			}   // if ( s_intPos_pclrTextBackgroundColor == ArrayInfo.ARRAY_INVALID_INDEX )

            if ( s_intPos_penmInterruptCriterion == ArrayInfo.ARRAY_INVALID_INDEX )
            {
                string strMsg = string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_2 ,                 // Message template
                    LBL_PENMINTERRUPTCRITERION ,                                        // Token 0
                    Properties.Resources.TIMED_WAIT_TEST_CASE_INFO.QuoteString ( ) );   // Token 1
				Console.Error.WriteLine ( strMsg );										// Rather than throwing an exception, emit a message into the Standard Error stream.
			}   // if ( s_intPos_penmInterruptCriterion == ArrayInfo.ARRAY_INVALID_INDEX )

            s_achrFieldDelimiter = Properties.Resources.TIMED_WAIT_TEST_DELIMITER.ToCharArray (
                ListInfo.SUBSTR_BEGINNING ,
                MagicNumbers.PLUS_ONE );
        }   // static TimedWaitTestCase
        #endregion  // Infrastructure


        #region Instance Data
        uint _twtc_puintWaitSeconds;
        string _twtc_pstrCountdownWaitingFor;
        ConsoleColor _twtc_pclrTextColor;
        ConsoleColor _twtc_pclrTextBackgroundColor;
        DisplayAids.InterruptCriterion _twtc_penmInterruptCriterion;
        #endregion  // Instance Data


        #region Instance Constructors
        /// <summary>
        /// This constructor is marked as Public to satisfy a requirement of the
        /// IList interface, so that the records can be organized into a List.
        /// </summary>
        public TimedWaitTestCase ( ) { }    // public TimedWaitTestCase (1 of 2)

        /// <summary>
        /// Use this constructor to create fully populated objects from a string
        /// read from a file of delimited text records.
        /// </summary>
        /// <param name="pstrRecord">
        /// Pass in the record.
        /// </param>
        /// <param name="pchrDelimiter">
        /// Identify the delimiter, so that I can change my mind about my choice
        /// of field delimiter.
        /// </param>
        public TimedWaitTestCase ( string pstrRecord )
        {
            string [ ] astrFields = pstrRecord.Split ( s_achrFieldDelimiter );

            if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
            {   // Input contains the expected number of fields.
                int intPosition = ArrayInfo.ARRAY_INVALID_INDEX;

                foreach ( string strField in astrFields )
                {
                    intPosition++;

                    if ( intPosition == s_intPos_puintWaitSeconds )
                    {
                        uint.TryParse ( strField , out _twtc_puintWaitSeconds );
                    }
                    else if ( intPosition == s_intPos_pstrCountdownWaitingFor )
                    {
                        _twtc_pstrCountdownWaitingFor = strField;
                    }
                    else if ( intPosition == s_intPos_pclrTextColor )
                    {
                        _twtc_pclrTextColor = ( ConsoleColor ) System.Enum.Parse (
                            _twtc_pclrTextColor.GetType ( ) ,
                            strField );
                    }
                    else if ( intPosition == s_intPos_pclrTextBackgroundColor )
                    {
                        _twtc_pclrTextBackgroundColor  = ( ConsoleColor ) System.Enum.Parse (
                            _twtc_pclrTextBackgroundColor.GetType ( ) ,
                            strField );
                    }
                    else if ( intPosition == s_intPos_penmInterruptCriterion )
                    {
                        _twtc_penmInterruptCriterion = ( DisplayAids.InterruptCriterion ) System.Enum.Parse (
                            _twtc_penmInterruptCriterion.GetType ( ) ,
                            strField );
                    }
                }   // foreach ( string strField in astrFields )
            }   // TRUE (expected outcome) block, if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
            else
            {   // The input didn't split into the expected number of fields.
                string strMsg = string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_3 ,
                    new object [ ]
                        {
                            pstrRecord ,
                            s_astrTimedWaitTestCaseInfo.Length ,
                            astrFields.Length ,
                            Environment.NewLine
                        } );
                throw new ArgumentException (
                    strMsg ,
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ARGNAME );
            }   // FALSE (exception) block, if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
        }   // public TimedWaitTestCase (2 of 2)
        #endregion  // Instance Constructors


        #region Public Properties, all Read Only
        public uint TWTC_puintWaitSeconds { get { return _twtc_puintWaitSeconds; } }
        public string TWTC_pstrCountdownWaitingFor { get { return _twtc_pstrCountdownWaitingFor; } }
        public ConsoleColor TWTC_pclrTextColor { get { return _twtc_pclrTextColor; } }
        public ConsoleColor TWTC_pclrTextBackgroundColor { get { return _twtc_pclrTextBackgroundColor; } }
        public DisplayAids.InterruptCriterion TWTC_penmInterruptCriterion { get { return _twtc_penmInterruptCriterion; } }
        #endregion  // Public Properties, all Read Only


        #region Public Static Methods
        public static string ValidateLabelRow ( string pstrLabelRow )
        {
            StringBuilder rsbErrors = null;
            string [ ] astrFields = pstrLabelRow.Split ( s_achrFieldDelimiter );

            if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
            {   // Input contains the expected number of fields.
                int intPosition = ArrayInfo.ARRAY_INVALID_INDEX;

                foreach ( string strLabel in astrFields )
                {   // Check label against the label in its corresponding position in the template.
                    if ( strLabel != s_astrTimedWaitTestCaseInfo [ ++intPosition ] )
                    {   // Only unequal labels matter.
                        if ( rsbErrors == null )
                        {   // Initialize on first use.
                            rsbErrors = new StringBuilder (
                                string.Format (                                                     // Initialize with a preamble.
                                    Properties.Resources.TIMED_WAIT_TEST_LABEL_TESTER_ERROR_4A ,    // Message template.
                                    pstrLabelRow ,                                                  // Substitution token 0
                                    Environment.NewLine ) ,                                         // Substitution token 1
                                MagicNumbers.CAPACITY_01KB );										// Reserve room for 1024 characters.
                        }   // if ( rsbErrors == null )

                        rsbErrors.AppendFormat (
                            Properties.Resources.TIMED_WAIT_TEST_LABEL_TESTER_ERROR_4B ,            // Message template.
                            intPosition + ArrayInfo.INDEX_FROM_ORDINAL ,							// Substitution token 0
                            strLabel ,                                                              // Substitution token 1
                            Environment.NewLine );                                                  // Substitution token 2
                    }   // if ( strLabel != s_astrTimedWaitTestCaseInfo [ ++intPosition ] )
                }   // foreach ( string strLabel in astrFields )

                if ( rsbErrors == null )
                {   // String is valid.
                    return null;
                }   // TRUE (desired outcome) block, if ( rsbErrors == null )
                else
                {   // String contains errors.
                    return rsbErrors.ToString ( );
                }   // FALSE (unwelcome outcome) block, if ( rsbErrors == null )
            }   // TRUE (expected outcome) block, if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
            else
            {   // The input didn't split into the expected number of fields.
                return string.Format (
                    Properties.Resources.TIMED_WAIT_TEST_CTOR_ERROR_3 ,
                    new object [ ]
                        {
                            pstrLabelRow ,
                            s_astrTimedWaitTestCaseInfo.Length ,
                            astrFields.Length ,
                            Environment.NewLine
                        } );
            }   // FALSE (exception) block, if ( astrFields.Length == s_astrTimedWaitTestCaseInfo.Length )
        }   // public static string ValidateLabelRow
		#endregion	// Public Static Methods
	}   // class TimedWaitTestCase
}   // partial namespace TestStand