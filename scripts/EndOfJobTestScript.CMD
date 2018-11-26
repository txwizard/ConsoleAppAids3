@echo off

::	============================================================================
::
::	Script Name			EndOfJobTestScript.CMD
::
::	Synopsis:			Exercise the NormalExit methods in class library
::						WizardWrx.ApplicationHelpers.
::
::	Prerequisites:		Test stand program, EOJTEST.exe, must be in the RELEASE
::                      directory of its Visual Studio source code tree.
::
::	Remarks:			The test cases were generated with the help of an Excel
::						spreadsheet.
::
::	References:			Test_Case_Matrix.xlsx, in the same directory as is this
::						script.
::
::	Author:				David A. Gray
::
::	----------------------------------------------------------------------------
::	Revision History
::	----------------------------------------------------------------------------
::
::	Date       By  Synopsis
::	---------- --- -------------------------------------------------------------
::	2012/09/04 DAG Initial implementation and use.
::  2017/08/06 DAG Add my standard version and time stamp displays.
::	============================================================================

echo BOJ %~0, version %~t0
call ShowTime.CMD
echo Script Directory  = %~dp0
echo.
echo Working Directory = %CD%
echo.
setlocal

..\EOJTEST\bin\Release\EOJTEST.exe 100 1 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 100 2 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 100 3 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 1 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 2 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 3 1
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 100 1 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 100 2 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 100 3 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 1 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 2 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

..\EOJTEST\bin\Release\EOJTEST.exe 0 3 2
echo.
echo EOJTEST.exe exit code = %ERRORLEVEL%
echo.

echo.
echo %~n0 Done!
echo.
wwpause.exe