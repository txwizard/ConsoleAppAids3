
The pre and post build tesks and the test scripts found in the /scripts
directory use a number of tools that I have developed over many years. Since
they live in a directory that is on the PATH list on my machine, they are "just
there" when I need them, and I seldom give them a second thought. To simplify
matters for anybody who wants to run the test scripts or build the project, they
are in DAGDevTOOLS.ZIP, which can be extracted into any directory that happens
to be on your PATH list. None of them requires installation, none of the DLLs is
registered for COM, and none of them or their DLLs use the Windows Registry.

A few use MSVCR120.dll, which is not included, but you probably have it if you
have a compatible version of Microsoft Visual Studio. The rest use MSVCRT.DLL,
which ships with Microsoft Windows.

The following table lists and briefly describes the programs and scripts that
are included in the archive.

Program or Script Name      Purpose
--------------------------- ----------------------------------------------------
Date2FN.exe                 Format the local time as YYYYMMDD_HHMMSS, and append
                            it to the base name of a file. This is a simple way
                            to guarantee unique names for sets of log files and
                            test reports that are easy to sort by creation date.

wwBldNbrMgr.exe             Increment the build number in the AssemblyInfo.cs OF
                            a project. Only C# projects are supported.

WWLogger.exe                Create or append to a text long, each entry having a
                            time stamp.

                            The default format is YYYY/MM/DD hh:mm:ss, which can
                            be changed via command line options, discussed with
                            examples in WWLOGGER.pdf, which is also included.
                            The time stamp is always followed by a single space,
                            then the message, which has no pre-set length limit.

                            Additional lines can be appended to the message by
                            writing them into a text file, then piping the text
                            to standard input by way of redirection characters.

WWLOGGERD.EXE               This is a Unicode version of WWLogger.exe; it works
                            identically, except that it outputs 16-bit Unicode
                            characters. Though the files created by the current
                            version lack a Byte Order Mark of any kind, I have
                            never had a problem reading them into a text editor
                            or spreadsheet program.

WWPause.exe                 I got fed up eons ago with batch files that took off
                            when ANY key was pressed, and wrote a DOS version of
                            this program. This is its 32 bit character mode son.

WWSleep.EXE                 This program allows a batch file to be suspended for
                            a user-defined number of seconds, with a default of
                            30 seconds if unspecified, to allow time for you to
                            read its output before the window in which it ran is
                            closed when the script exits.

ExportFromEventLogNDays.CMD This script uses the SysInternals PSLogList utility
                            to export records from a Windows event log. You must
                            supply your own copy of PSLogList.exe.

ShowArgs.BAT                This script displays the command line arguments of a
                            batch file. It can display up to nine arguments, but
                            it displays only actual arguments.

ShowTime.CMD                This script displays the current date and time, so a
                            script listing is time stamped.

XCOPYD.CMD                  This wrapper around stalwart system utility XCOPY is
                            the most dependable way I know to automatically give
                            the correct answer to the prompt about whether the
                            destination is a file or a directory when the answer
                            is that it is a directory.

XcopyDestIsDir.TXT          This response file answers the prompt from XCOPY.EXE
                            when the destination is a directory. XCOPYF.CMD
                            pipes it into XCOPY.EXE to automatically supply the
                            correct answer. This not only eliminate guesswork,
                            and a risk of responding incorrectly when using
                            XCOPY interactively in a command prompt window, but
                            it permits XCOPY to be run safely in unatteended
                            scripts, including build scripts, such as the one in
                            the Visual C++ build projects for my native DLLs.

                            This file and XCOPYD.CMD inhabit the same directory,
                            where XCOPYD.CMD parses its own fully qualified path
                            to work out where to find its response file.

XcopyDestIsFile.TXT         This is the response file for XCOPYF.CMD; it works
                            exactly like XcopyDestIsDir.TXT when the correct
                            response is that the destination is a file.

XCOPYF.CMD                  This wrapper around stalwart system utility XCOPY is
                            the most dependable way I know to automatically give
                            the correct answer to the prompt about whether the
                            destination is a file or a directory when the answer
                            is that it is a file.
