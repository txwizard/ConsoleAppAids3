/*
	============================================================================
    File Name:          AssemblyInfo.cs

    Synopsis:           This AssemblyInfo file belongs to Visual Studio project
                        ConsoleAppids3.

    Remarks:            I don't usually include revision histories in my
                        AssemblyInfo files. This is an exception, because the
                        version update is to account for a change that affects
                        only the related C# project file. Hence, I won't
                        routinely update this history.

						Going forward, this library will be periodically
						refreshed by building it against newer versions of the
						WizardWrx .NET API. If this source file and ChangeLog.md
						are unchanged, it is safe to assume that the only change
						is a refresh of its dependencies.

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

	2018/11/26 7.0     DAG Eliminate the unreferenced system namespaces, and tag
                           my own assembly references with explanatory notes.

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

	2021/03/05 8.0     DAG Implement LoadBasicErrorMessages and build against my
                           current version (8.0) of the WizardWrx .NET API. The
                           library versions are now leveled up, and will stay so
                           for the foreseeable future.

    2021/05/02 8.0.533 DAG Build against the most recent WizardWrx .NET API
                           library constellation, and add the ChangeLog to the
                           NuGet package.

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
[assembly: AssemblyTitle ( "WizardWrx.ConsoleAppAids3" )]
[assembly: AssemblyDescription ( "Helper routines for console mode programs" )]
[assembly: AssemblyConfiguration ( "" )]
[assembly: AssemblyCompany ( "David A. Gray" )]
[assembly: AssemblyProduct ( "WizardWrx .NET Utility Libraries" )]
[assembly: AssemblyCopyright ( "Copyright © 2012-2021, David A. Gray" )]
[assembly: AssemblyTrademark ( "This library is distributed under a three-clause BSD license." )]
[assembly: AssemblyCulture ( "" )]

// Setting ComVisible to false makes the types in this assembly not visible
// to COM components.  If you need to access a type in this assembly from
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible ( false )]

// The following GUID is for the ID of the type library if this project is exposed to COM
[assembly: Guid ( "04dca54e-6aff-4ec8-ad04-06c75036a989" )]

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
[assembly: AssemblyVersion ( "8.0.551.0" )]
[assembly: AssemblyFileVersion ( "8.0.551.0" )]