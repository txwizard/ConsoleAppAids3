/*
	============================================================================
    File Name:          AssemblyInfo.cs

    Synopsis:           This AssemblyInfo file belongs to Visual Studio project
                        TestStand, which is part of solution ConsoleAppids3.

    Remarks:            I don't usually include revision histories in my
                        AssemblyInfo files. This is an exception, because the
                        version update is to account for a change that affects
                        only the related C Sharp project file. Hence, I won't
                        routinely update this history.

    ----------------------------------------------------------------------------
    Revision History
    ----------------------------------------------------------------------------

    Date       Version By  Synopsis
    ---------- ------- --- -----------------------------------------------------
	2016/05/20 6.1     DAG Synchronize this version number with that of its
                           dependent library, WizardWrx.DllServices2.dll.

	2016/06/09 6.3     DAG Keep the version numbers of DllServices2 and this
                           library in sync; this upgrade also covers technical
                           clarifications in the internal documentation.

	2017/08/05 7.0     DAG Replace the WizardWrx.DllServices2.dll monolith with
	                       the constellation of DLLs that replaced it, which
	                       also requires upgrading the target framework version.

	2018/08/06 7.0     DAG Manually update the copyright year to coincide with a
	                       new build against the final release candidate of the
	                       main WizardWrx class library constellation.

	2019/01/30 7.1     DAG Reset the console row indicator when ScrollUp is
	                       called on a FixedConsoleWriter object, so that the
	                       object need not be re-created when you want to reuse
	                       it further down screen. I'm calling this a point
	                       release, although it is only a bug fix.

	2020/10/23 7.2     DAG Implement Semantic Version Numbering. This build also
                           incorporates the current stable versions of the
                           WizardWrx .NET API asseblies, which finally fully
                           implement Semantic Version Numbering as of a few days
                           ago.

    2021/06/06 8.0.535 DAG Build against the most recent WizardWrx .NET API
                           library constellation.
    ============================================================================
*/

using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle ( "ConsoleAppAids3 TestStand" )]
[assembly: AssemblyDescription ( "" )]
[assembly: AssemblyConfiguration ( "" )]
[assembly: AssemblyCompany ( "David A. Gray" )]
[assembly: AssemblyProduct ( "WizardWrx .NET Utility Libraries" )]
[assembly: AssemblyCopyright ( "Copyright 2014-2022, David A. Gray" )]
[assembly: AssemblyTrademark ( "This library is distributed under a three-clause BSD license." )]
[assembly: AssemblyCulture ( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible ( false )]

// The following GUID is for the ID of the type library if this project is exposed to COM
[assembly: Guid ( "2cdef76d-4013-4932-8046-15353d465d8d" )]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion ( "7.2.561.0" )]