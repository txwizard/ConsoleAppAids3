# WizardWrx AssemblyUtils ReadMe

The purpose of this class library is to provide other libraries and assemblies
with easy access to classes that implement features that permit enumerating
embedded resources and assembly dependencies.

To maximize compatibility with client code, the library targets version 3.5 of
the Microsoft .NET Framework, enabling it to support projects that target that
version, or any later version, of the framework. Since its implementation needs
only core features of the Base Class Library, apart from the time zone features
that pushed it to version 3.5, I have yet to discover an issue in using it with
any of the newer frameworks.

The class belongs to the `WizardWrx` namespace, which I created to organize the
helper libraries that I use in virtually every production assembly, regardless
of what framework version is its target, and whether its surface is a Windows
console, the Windows desktop, or the ASP.NET Web server. To date, I have used
classes and methods in these libraries in all three environments and against
virtually every version of the framework and Base Class Library.

## New in Version 7.23.121

This release corrects an omission from the overloaded DependentAssemblies
constructor that rendered it inoperable.

## New in Version 7.23.120

This release adds a new instance method, `GetDependentAssemblyInfos`, to class
`DependentAssemblies`, which returns a sorted list of dependent assemblies.
Though this could be implemted as a read-only property, I chose to implement it
as a method.

## New in Version 7.20.117

This build is a bug fix. I discovered that the iten number written into the
optional tab-delimited output file generated by
`SortableManagedResourceItem.ListResourcesInAssemblyByName` was always the
total item count, rather than the actual item number.

## New in Version 7.20

* `SortableManagedResourceItem.ListResourcesInAssemblyByName` gets an optional
`StreamWriter` argument that causes it to create a tab delimited list of the
managed string resources in an assembly.

* `ReportGenerators.ListKeyAssemblyPropertiess` had lost the summary paragraph
of its XML documentation, which is restored.

## Using the Library

Since there are no name collisions, you may safely set references to any library
that exposes objects in the the `WizardWrx` namespace in a project, and add a
using directive for any of its subsidiary namespaces to any source module.

This library belongs to the WizardWrx .NET API, which is documented at
[https://txwizard.github.io/WizardWrx_NET_API/](https://txwizard.github.io/WizardWrx_NET_API/).

The easiest way to incorporate the library into your own code is by installing the NuGet package at
[https://www.nuget.org/packages/WizardWrx.AssemblyUtils](https://www.nuget.org/packages/WizardWrx.AssemblyUtils).

## Revision History

**Note:** Going forward, every release will include a new build of every library in the WizardWrx
.NET API, so that all versions stay reasonably well synchronized. _However_, unless the code changes,
the change log will not cite every library for every release. Moreover, the release notes reported by
the NuGet Package Manager will remain static unless there is a noteworthy change that affects that
library.

The objective of this policy change is to attempt to eliminate dependency diamonds that otherwise
arise inevitably because of unavoidable cascading dependencies among the libraries at the bottom of
the call stack, such as WizardWrx.Common.dll, WizardWrx.ASCIIInfo.dll, and others.

At present, the GitHub repository is not updated between releases. However, changes tend to be developed
and released quickly when the need for a change becomes evident.

This was originally released with version 7.17 of the WizardWrx .NET API, of which it is a component.
To track changes, please see
[https://github.com/txwizard/WizardWrx_NET_API/blob/master/ChangeLog.md](https://github.com/txwizard/WizardWrx_NET_API/blob/master/ChangeLog.md),
which reports changes by namespace and class for each release.