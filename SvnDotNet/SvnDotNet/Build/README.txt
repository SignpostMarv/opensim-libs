Following are the list of available Nant targets for Svn.NET. You must specify
at least an ACTION to perform.

    CONFIGURATION
        Specify the Configuration target *before* any Action targets.

        release         (default) Compiles libraries / binaries in release mode.
        debug           Compiles libraries / binaries in debug mode.

    PROJECT
        Specify the Project target *before* any Action targets.

        clr             (default) Compiles Subversion client DLL, AprSharp, and
                        SubversionSharp.
        subversion      Compiles the Subversion libraries and all dependencies.
                        Currently supports Visual Studio 2005 on Win32 only.
        all             Compiles 'subversion' target then 'clr' target.

    FRAMEWORK
        Specify the Framework target *before* any Action targets.

        dotnet          (default) Compiles against the .NET 2.0 Framework.
        mono            Compiles against the Mono 2.0 Framework.

    ACTION
        build           Compiles libraries / binaries.
        clean           Cleans libraries, binaries, and intermediate files from
                        a previous build.
        rebuild         Performs a 'clean' followed by 'build'.
        test            Runs NUnit test suite; currently defined for 'clr'
                        project only.
        deploy          Copies all binary and managed DLLs to "deploy" folder
                        for easy deployment.

    OTHER
        get             Downloads and unzips "recommended" versions of
                        Subversion and all dependencies.
        bootstrap       Runs 'get' target followed by build of 'all'.
        help            Displays this help info.

DLLs and binaries will be placed into the folder:
    bin/<framework>/<configuration>
