# WizardWrx.AnyCSV Change Log

This file is a running history of fixes and improvements from version 4.0
onwards. Changes are documented for the newest version first. Within each
version, classes are listed alphabetically.

# Version 9.0.145, released 2024/03/02

Correct a bug that manifested when the parser was presented with a
delimited string in which all fields are enclosed in guard characters AND two or
more successive fields are empty. 

Since I had to visit this code, the library got an overdue .NET Framework
version upgrade from 2.0 to 4.8.

# Version 7.2, released 2019/08/10

Suppress generation of an interface for the CSVParseEngine class, but keep the
symbolic constants and the instance methods on the Parser class visible.

# Version 7.1, released 2019/07/03

Synchronize the format control string used by the ToString override to align
with the format items added in version 7.0, and add the overlooked abstract
marking, and correct errors in the XML help text of two GuardChar enumeration
members.

As of this version, the typelib generator is responsible for dispensing GUIDs.

# Version 7.0, released 2018/09/03

Amend ToString to display character codes as both raw characters and decimal
values.

# Version 7.0, released 2018/01/03

Sign the assembly with a strong name so that it can go into a Global Assembly
Cache.

# Version 5.0, released 2017/08/05

Change CPU architecture from x86 to MSIL and record the COM registration in the
64 bit Registry.

Since the interface is unchanged, I kept the original GUID.

# Version 4.0, released 2016/10/01

Move the guts of the Parser class, the original implementation of my CSV parsing
algorithm, into the CSVParsingEngine abstract class.

# Version 3.1, released 2016/06/10

Embed my three-clause BSD license, and improve the internal documentation.

# Version 3.0, released 2014/07/07

Initial implementation.