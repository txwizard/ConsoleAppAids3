# WizardWrx.ConsoleAppAids3 Change Log

This file is a running history of fixes and improvements from version 7.0
onwards. Changes are documented for the newest version first. Within each
version, classes are listed alphabetically.

# AssemblyVersion 9.0.0 Updated 09/09/2023, AssemblyFileVersion 9.0.567

To maintain compatibility with the WizardWrx .NET API upon which this assembly
relies, its target framework is upgraded to 4.8. Unless your project targets a
lower version of the framework, this change is transparent.

If you cannot retarget a project that imports this assembly, your only loss will
be access to improvements incorporated into future versions, of which there are
currently none planned.

# AssemblyVersion 9.0.0 Updated 12/30/2022, AssemblyFileVersion 9.0.565

For the first time in four years, this library gets a new feature that enables
it to establish the Current Working Directory (CWD) to a directory specified
relative to the directory from which the calling assembly loaded.

For example, if you call `SetCWDRelativeToEntryAssemblyPath` from an entry
assembly that loaded from `C:\Users\Me\Repositories\ConsoleAppAids3\TestStand\bin\Release`
and you pass in `..\..\App_Data` the return value wound be
`C:\Users\Me\Repositories\ConsoleAppAids3\TestStand\App_Data' - ideal for unit
test assemblies distributed via GitHub, BitBucket, Sourceforge, and character-mode
assemblies incorporated into Visual Studio solutions that are shared amont people
whose machine configurations are not standardized.

The new major version number (9.0) raligns the version numbers with the assembly
version numbering of the WizardWrx .NET API libraries upon which this library
relies.

This package ships with the latest versions of thos libraries.

# Version 8.1.562 Updated 10/15/2022

This re-release corresponds to an update of the associated GitHub repository
that includes versions of the dependent assemblies published as NuGet packages
that didn't advertise their dependencies. Please take this as your notice to
update your package, which should now cause your NuGet Package Manager to follow
its dependency chain to the end.

# Version 8.1.559 Updated 05/21/2022

To compensate for an apparent change in the behavior of the NuGet Package
Manager implementation in Visual Studio 2019, all NuGet in the WizardWrx .NET
API constellation of libraries are being re-released with dependencies named
explicitly in the generated .nuspec file that goes into the package. This is
accomplished by adding the IncludeReferencedProjects to the NuGet Pack command
line. Otherwise, the program source code is unchanged, nor are the binaries.
Henceforth, independend release schedules as described in version 8.0.551 of
this library will resume.

# Version 8.1.554 Updated 04/13/2022

Instance method `NormalExit` on a `ConsoleAppStateManager` object neglected to
print the message that corresponds to the status code from the table of static
error messages. Hence, relying on this method left you with a status code
(somewhat useful) but no explanatory message, nor any evidence of the nonzero
status code in the console stream.

Worse yet, investigating this issue exposed a genuine bug in instance method
`LoadBasicErrorMessages` on the same object that caused it to leave the table in
the `StateManager` object that was expected to store the messages to be left
empty, meaning that no code that relied on `LoadBasicErrorMessages` to initialize
the table of error messages would never be able to display an error message.

The way the routine that displays error messages implemented was implemented led
to a silent failure.

# Version 8.0.551 Updated 04/08/2022

Effective immediately, this library will no longer be updated as described
in the change log entry for version 8.0.533, released 05/02/2021 because the
practice is wasteful of storage space on GitHub and the NuGet Gallery, not to
mention the amount of time and extra disk space occupied by endless backups
and package refreshes.

Since all dependencies are under my control, have dedicated NuGet packages and
GitHub repositories, updates that do nothing but refresh the dependencies serve
no practical purpose. It is simpler for everyone to follow instead one of two
straightforward plans.

1) Configure your NuGet Package Manager to always Get Latest by adding a
   `config` node, inserting a `dependencyVersion` key, and setting its
   value to `Highest`.

2) Disable automatic package restore and limit updates to cases in which you
   need a new feature or learn about a bug fix that affects your application.

Though it promotes code stability, disabling automatic package restore does so
at the cost of depriving you of automatic access to improvements. While this can
in some cases be risky, in the case of the WizardWrx packages, over the last
fifteen years, only one or two cases have arisen that required a breaking change
and those handful of instances are prominently documented.

Accrdingly, this is the last release that will be confined to up-to-date
dependencies. All future releases will be feature updates or bug fixes.

# Version 8.0.533 Updated 07/06/2021

Correct the documentation URL shown in the ReadMe file, which was missing a
required trailing backslash.

The repository is otherwise unchanged.

# Version 8.0.533 Released 05/02/2021 03:05:40

Build against the most recent WizardWrx .NET API library constellation.

Going forward, this library will be periodically refreshed by building it
against newer versions of the WizardWrx .NET API. If ChangeLog.md (this file) is
unchanged, it is safe to assume that the only change is a refresh of its
dependencies.

# Version 8.0, Released 03/05/2021 18:55:45

This update of the `WizardWrx.ConsoleAppAids3` library defines a new method,
`LoadBasicErrorMessages`, that loads the appropriate string described by the
following table.

|Message Text                      |Common Resource| Common Code |Code Value|
|----------------------------------|---------------|-------------|----------|
|The task completed successfully.  |ERRMSG_SUCCESS |ERROR_SUCCESS|0x00000000|
|A run-time exception has occurred.|ERRMSG_RUNTIME |ERROR_RUNTIME|0x00000001|

Additional error messages can be loaded by passing in an optional array of
strings, which must follow the order of exit code assignments, which do double
duty as an index into the arry of messages. If you need only those two, both of
which are required to enable other parts of the library to work correctly, pass
in a NULL.

# Version 7.2

This update of the `WizardWrx.ConsoleAppAids3` library resolves an issue that
allowed a calling routine to instantiate an uninitialized, utterly useless
instance of the `ConsoleAppStateManager` object. In so doing, the bug breaks a
fundamental tenet of the Singleton design pattern which it is intended to
implement. Although the compiler allowed the instance to be created, it would
fail with a null reference exception on first use.

Additionally, this release implements Semantic Version Numbering and the related
Deterministic compiler switch.

This build also incorporates the current stable versions of the `WizardWrx .NET
API` asseblies, which finally fully implement Semantic Version Numbering as of a
few days ago.

# Version 7.1

Reset the console row indicator when `ScrollUp` is called on a
`FixedConsoleWriter` object, so that the object need not be re-created when you
want to reuse it further down screen. I am calling this a point release,
although it is only a bug fix.

This is the first release to be accompanied by a NuGet package.

# Version 7.0

Following is a summary of changes made in version 7.0, released Monday, 26 November 2018.

## Class: WizardWrx.ConsoleAppStateManager

Mark `GetTheSingleInstance` as a New (override) method, eliminating a compiler
warning.

## Class: DisplayAids

Replace the `WizardWrx.DllServices2.dll` monolith with the constellation of DLLs
that replaced it, which also requires upgrading the target framework version
from 2.0 to 3.5 Client Profile.

## Class: FixedConsoleWriter

Replace the `WizardWrx.DllServices2.dll` monolith with the constellation of DLLs
that replaced it, which also requires upgrading the target framework version
from 2.0 to 3.5 Client Profile.

This library supersedes [WizardWrx.ConsoleAppAids2](https://github.com/txwizard/ConsoleAppAids2),
which is hereby deprected. I stopped using its predecessor well over a year ago. Since this library is
essentially the same code base, the revisions made prior to the upgrade remain as headnotes in
each source file.