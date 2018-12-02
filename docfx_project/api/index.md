# WizardWrx.ConsoleAppAids3 Microsoft .NET Class Llibrary

The following sections summarize the classes in the library. To use any of them,
add a reference to `WizardWrx.ConsoleAppAids3.dll` to your project, and add a
using directive `WizardWrx.ConsoleAppAids3` to any source file in which you want
ready access to its classes.

Use the links in the table of contents along the left side of this page to view
the documentation.

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