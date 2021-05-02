@echo off
goto SKIPREM

::  ============================================================================
::
::  Script Name:        Activate_Regressions.CMD
::
::  Script Synopsis:    Activate the Regressions test set.
::
::  Remarks:            This script uses long established techniques to identify
::                      the directory from which a shell script was loaded that
::                      only recently came to my attention.
::
::  Script Author:      David A. Gray
::
::  ----------------------------------------------------------------------------
::  Revision History
::  ----------------------------------------------------------------------------
::
::  Date       By  Synopsis
::  ---------- --- -------------------------------------------------------------
::  2014/03/22 DAG Initial implementation created, tested, and deployed.
::  2017/08/06 DAG Replace XCOPY.EXE with XCOPYF.CMD, eliminating the requirment
::                 for a local copy of the XCOPY.EXE response file. XCOPYF.CMD
::                 didn't yet exist when I created this script.
::  2021/02/21 DAG Replace ShowTime.CMD with ShowTime.exe, a small 64-bit C++
::                 utility that affords much greater control over the format of
::                 the time display.
::  ============================================================================


:SKIPREM

    echo BOJ %~0, version %~t0
    ShowTime.exe
    echo Script Directory  = %~dp0
    setlocal
    pushd %~dp0
    cd ..\NOTES\Test_Data

    echo.
    echo Working Directory = %CD%
    set TESTFILENAME=TimedWaitTests_Regression.TXT

    echo.
    echo Making %TESTFILENAME% the active test data set.

    if exist %TESTFILENAME% goto FOUND_IT

    echo.
    echo     ERROR: %TESTFILENAME%
    echo            not found in test data directory.
    goto DONE


:FOUND_IT

    echo.
    echo.
    echo Activating %TESTFILENAME%
    echo for use with the ConsoleAppAids test stand.
    echo.

    call XCopyF.CMD %TESTFILENAME% TimedWaitTests.TXT /f /k /r /v /y

    if ERRORLEVEL 5 goto COPY_ERR5
    if ERRORLEVEL 5 goto COPY_ERR4
    if ERRORLEVEL 5 goto COPY_ERR3
    if ERRORLEVEL 5 goto COPY_ERR2
    if ERRORLEVEL 5 goto COPY_ERR1

    fc    %TESTFILENAME% TimedWaitTests.TXT /b
    goto DONE


:COPY_ERR5

    echo.
    echo     ERROR: The XCOPY system utility reported a disk write error.
    echo            File copying failed.
    goto DONE


:COPY_ERR4

    echo.
    echo     ERROR: The XCOPY system utility reported an initialization error.
    echo            There is not enough memory or disk space, or you entered
    echo            an invalid drive name or invalid syntax on the command line.
    echo            File copying failed.
    goto DONE


:COPY_ERR2

    echo.
    echo     ERROR: The XCOPY system utility returned status code 3, which is
    echo            undocumented. File copying failed.
    goto DONE


:COPY_ERR2

    echo.
    echo     ERROR: The XCOPY system utility detected a CTRL-C to terminate the
    echo            task. File copying failed.
    goto DONE


COPY_ERR1

    echo.
    echo     ERROR: The XCOPY system utility found no files that matched the
    echo            source file specification. File copying failed.
    goto DONE


:DONE

    echo.
    echo %~n0 Done!
    echo.
    WWPause
