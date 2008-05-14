Building SVN.NET assemblies (AprSharp.dll and SubversionSharp.dll)

The NAnt (http://nant.sourceforge.net) required to build SVN.NET assemblies.
If you want to build Mono assemblies you will require Mono development 
packages also (see http://www.mono-project.com).

The following build configurations supported by buildfiles:
1. .NET release build
   command: nant build
2. .NET debug build
   command: nant debug build
3. Mono release build
   command: nant mono build
4. Mono debug build
   command: nant debug mono build

The following additional build targets are allowed:

* nant get: download and unzip Subversion and dependencies

For more info on possible targets, type 'nant help' from the base directory.

Assemblies placed into bin/{Framework}/{Configuration} directory.
Framework can be 'net-2.0' or 'mono-2.0'.
Configuration can be 'release' or 'debug'.
