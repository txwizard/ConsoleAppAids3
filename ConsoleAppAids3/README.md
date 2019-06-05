# WizardWrx.ConsoleAppAids3 ReadMe

The purpose of this class library is to expedite development of new character
mode (Console) applications that target any version of the Microsoft .NET
Framework, from version 3.5 up. The classes in this library define numerous
utility classes, many of which build on the __WizardWrx .NET API__
available at [https://github.com/txwizard/WizardWrx_NET_API](https://github.com/txwizard/WizardWrx_NET_API)
under a three-clause BSD license, by adding capabilities that meet the
specialized requirements of console programs.

This library is fully documented at [https://txwizard.github.io/ConsoleAppAids3](https://txwizard.github.io/ConsoleAppAids3),
and the __WizardWrx .NET API__ is fully documented online at [https://txwizard.github.io/WizardWrx_NET_API](https://txwizard.github.io/WizardWrx_NET_API).

Everything in this library belongs to the `WizardWrx.ConsoleAppAids3` namespace,
corresponding to the name of the DLL that exports them. Following is a summary
of the classes.

*	__ConsoleAppStateManager__ is an `WizardWrx.DLLServices2.StateManager`
adapter, which exposes the adapted object through its read only
`BaseStateManager` property, and extends it with methods that provide
services applicable exclusively to character mode (console mode) programs, such
as looking after the logo and shutdown messages, and keeping track of elapsed
running time for inclusion in the final message. `WizardWrx.DLLServices2.StateManager`,
itself, exposes many useful properties, such as a robust exception logging class,
and an enumeration that reports the Windows subsystem in which it is running
(Character mode or GUI).

*	__DisplayAids__ is a sealed (implicitly static) class that precisely
controls the way your application handles pauses.

*	__FixedConsoleWriter__ permits a line of a console window to be used
repeatedly for successive lines of text, replacing the contents of the previous
print statement, so that the lines above it don't scroll off the screen. Once
instantiated, instances of this class behave almost exactly like
`Console.WriteLine`, and you can drop them into your code in its place, because
its overloads have identical signatures.

This library supersedes [WizardWrx.ConsoleAppAids2](https://github.com/txwizard/ConsoleAppAids2),
which is hereby deprected. I stopped using it well over a year ago.

## Compatibility Counts!

To maximize compatibility with client code, the library targets version 3.5 of
the Microsoft .NET Framework, enabling it to support projects that target that
version, or any later version, of the framework. I have yet to discover an issue
in using it with any of the newer frameworks.

__Note:__ I have not tested this library with .NET Core.

The class belongs to the `WizardWrx` namespace, which I created to organize the
helper libraries that I use in virtually every production assembly, regardless
of what framework version is its target, and whether its surface is a Windows
console, the Windows desktop, or the ASP.NET Web server. To date, I have used
classes and methods in these libraries in all three environments. Of course,
since this library targets console applications, its use is confined to that
domain.

The next several sections cover special considerations of which you must be
aware if you incorporate this package into your code as is or if you want to
modify it.

## Using These Libraries

Since there are no name collisions, you may safely set references to all
namespaces in this library and its dependents to the same source module. When
you incorporate this library, all but one of the __WizardWrx .NET API__
libraries, `WizardWrx.EmbeddedTextFile.dll`, are copied into your project. Since
the dependencies are also NuGet packages, the NuGet package manager adds them as
dependencies, too.

Detailed API documentation is at [https://txwizard.github.io/WizardWrx_ConsoleAppAids3](https://txwizard.github.io/WizardWrx_ConsoleAppAids3).

For those who just want to use them, and wish to forego the NuGet package, debug
and release builds of the libraries, their dependent assemblies, and the unit
test program are available as archives in the project root directory.

*	`WizardWrx_ConsoleAppAids3_Binaries_Debug.7z` is the debug build of the binaries.

*	`WizardWrx_ConsoleAppAids3_Binaries_Release.7z` is the release build of the binaries.

There is a DLL, PDB, and XML file for each library. To derive maximum benefit,
including support for the Visual Studio managed code debugger and IntelliSense
in the text editor, take everything.

## Change Log

See `ChangeLog.md` for a cumulative history of changes, listed from newest to
oldest, beginning with version 7.0. Previous changes are meticulously documented
in the top of each source file.

## Required External Tools

The pre and post build tesks and the test scripts found in the `/scripts`
directory use a number of tools that I have developed over many years. Since
they live in a directory that is on the PATH list on my machine, they are "just
there" when I need them, and I seldom give them a second thought. To simplify
matters for anybody who wants to run the test scripts or build the project, they
are in `DAGDevTOOLS.ZIP`, which can be extracted into any directory that happens
to be on your PATH list. None of them requires installation, none of the DLLs is
registered for COM, and none of them or their DLLs use the Windows Registry.

A few use `MSVCR120.dll`, which is not included, but you probably have it if you
have a compatible version of Microsoft Visual Studio. The rest use `MSVCRT.DLL`,
which ships with Microsoft Windows.

Please see `ReadMe_External_Tools.TXT` for details about these programs and shell
scripts.

## Internal Documentation

The source code includes comprehenisve technical documentation, including XML to
generate IntelliSense help, from which the build engine generates XML documents,
which are included herein. Argument names follow Hungarian notation, to make the
type immediately evident in most cases. A lower case "p" precedes a type prefix,
to differentiate arguments from local variables, followed by a lower case "a" to
designate arguments that are arrays. Object variables have an initial underscore
and static variables begin with "s _ "; this naming scheme makes variable scope
crystal clear, and flags arrays that neeed subscripts for anything but passing a
reference to the entire array.

The classes are thoroughly cross referenced, and many properties and methods
have working links to relevant MSDN pages.

## Versioning

Up to version 7, my approach to versioning has been as follows.

__AssemblyVersion__ tracks the build numbers as we go, and changes with every
release, regardless of whether the release contains breaking changes. By
design, breaking changes in my code are _exteremely_ rare.

__AssemblyFileVersion__ ties together all the assemblies in this API. This is
achived by way of a spilt AssemblyInfo.cs class.

__AssemblyInformationalVersion__ went unused.

Though this numbering scheme shall remain unchanged for the present, evaluating
whether to continue or alter it is going onto the road map.

## Road Map

Although I got the timed wait to work correctly when standard output is
redirected, I deferred doing what must be doone to fix the same issue in the
underlying FixedConsoleWriter class, because it requires modifying its seventeen
Write methods to use the same approach that I implemented directly in the timed
wait method. The change will also require a new private data store and a new
instance method, to allow consumers the option of returning the carriage. There
is enough other good material here that I decided against waiting to publish.

1.	__A NuGet Package__ arrives with this release.

2.	Evaluate the current versioning protocol in light of lessons learned from
recent studies of the matter.

## Contributing

Though I created this library to meet my individual development needs, I have
put a good bit of thought and care into its design. Moreover, since I will not
live forever, and I want these libraries to outlive me, I would be honored to
add contributions from others to it. My expectations are few, simple, easy to
meet, and intended to maintain the consistency of the code base and its API.

1.	__Naming Conventions__: I use Hungarian notation. Some claim that it has
outlived its usefulness. I think it remains useful because it encodes data
about the objects to which the names are applied that follows them wherever they
go, and convey it without help from IntelliSense.

2.	__Coding Style__: I have my editor set to leave spaces around every token.
This spacing has helped me quicly spot misplaced puncuation, such as the right
bracket that closes an array initializer that is in the wrong place, and it
makes mathematical expressions easier to read and mentally parse.

3.	__Comments__: I comment liberally and very deliberately. Of particular
importance are the comments that I append to the bracket that closes a block. It
does either or both of two things: link it to the opening statement, and
document which of two paths an __if__ statement is expected to follow most of
the time. When blocks get nested two, three, or four deep, they really earn
their keep.

4.	__Negative Conditions__: I do my best to avoid them, because they almost
always cause confusion. Astute observers will notice that they occur from time
to time, because there are _a few cases_ where they happen to be less confusing.

5.	__Array Initializers__: Arrays that have more than a very few initializers,
or that are initialized to long strings, are laid out as lists, with line
comments, if necessary, that describe the intent of each item.

6.	__Format Item Lists__: Lists of items that are paired with format items in
calls to `string.Format`, `Console.WriteLine`, and their relatives, are laid out
as arrays, even if there are too few to warrant one, and the comments show the
corresponding format item in context. This helps ensure that the items are
listed in the correct order, and that all format items are covered.

7.	__Symbolic Constants__: I use symbolic constants to document what a literal
value means in the context in which it appears, and to disambiguate tokens that
are easy to confuse, suzh as `1` and `l` (lower case L), `0` and `o` (lower case O),
literal spaces (1 and 2 spaces are common), underscores, the number `-1`, and so
forth. Literals that are widely applicable are defined in a set of classes that
comprise the majority of the root `WizardWrx` namespace.

8.	__Argument Lists__: I treat argument lists as arrays, and often comment each
argument with the name of the paramter that it represents. These lists help
guaranteeing that a long list of positional arguments is specified in the
correct order, especially when several are of the same type (e. g., two or more
integer arguments).

9.	__Triple-slash Comments__: These go on _everything_, even private members and
methods, so that everything supports IntelliSense, and it's easy to apply a tool
(I use DocFX.) to generate reference documentation.

With respect to the above items, you can expect me to be a nazi, though I shall
endeavor to give contributors a fair hearing when they have a good case.
Otherwise, please exercise your imagination, and submit your pull requests. When
I get NuGet packages implemented, I'll take care of rolling the contributions
into the appropriate packages, and contributors will get prominent credit on the
package page in the official public repository. If you skim the headnotes of the
code, you'll see that I take great pains to give others credit when I icorporate
their code into my projects, even to the point of disclaiming copyright or
leaving their copyright notice intact. Along the same lines, the comments are
liberally sprinkled with references to articles and Stack Overflow discussions
that contributed to the work.