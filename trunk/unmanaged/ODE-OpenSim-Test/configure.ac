dnl AC_INIT does not take a macro as a version nr: set it separately! - Bram
AC_INIT([ODE],[0.13.3],[ode@ode.org])
ODE_VERSION=OS0.13.3
AC_SUBST(ODE_VERSION)

# Those are instructions from the Libtool manual:
#  1. Start with version information of `0:0:0' for each libtool library.
#
#  2. Update the version information only immediately before a public
#     release of your software.  More frequent updates are unnecessary,
#     and only guarantee that the current interface number gets larger
#     faster.
#
#  3. If the library source code has changed at all since the last
#     update, then increment REVISION (`C:R:A' becomes `C:r+1:A').
#
#  4. If any interfaces have been added, removed, or changed since the
#     last update, increment CURRENT, and set REVISION to 0.
#
#  5. If any interfaces have been added since the last public release,
#     then increment AGE.
#
#  6. If any interfaces have been removed since the last public release,
#     then set AGE to 0.
CURRENT=5
REVISION=0
AGE=1

ODE_VERSION_INFO="-version-info $CURRENT:$REVISION:$AGE"
AC_SUBST(ODE_VERSION_INFO)

AC_CONFIG_SRCDIR([ode/src/ode.cpp])
AC_CONFIG_MACRO_DIR([m4])

AC_CANONICAL_HOST

AM_INIT_AUTOMAKE([1.10 foreign])
AC_CONFIG_HEADERS([ode/src/config.h])

dnl This is needed because we have subdirectories
AC_PROG_MAKE_SET
AC_PROG_CXX
AC_PROG_CC
AM_PROG_CC_C_O
AC_PROG_CPP
AC_PROG_AWK
AC_PROG_INSTALL
AC_PROG_LN_S
AC_PROG_MKDIR_P
LT_INIT([disable-shared win32-dll])
AC_CHECK_TOOLS([WINDRES], [windres])

AC_C_BIGENDIAN
AC_C_INLINE
AC_C_VOLATILE
PKG_PROG_PKG_CONFIG

AC_ARG_VAR([DOXYGEN], [set to doxygen binary to generate doxygen docs])
AC_CHECK_PROGS([DOXYGEN], [doxygen])
AM_CONDITIONAL([HAVE_DOXYGEN], [test x$DOXYGEN = xdoxygen])

dnl this may NOT be the machine on which the code is going to run in,
dnl so allow users to compile programs for their target machine.
pentium=no
cpu64=no
case "$host_cpu" in
  i586 | i686 | i786 )
        pentium=yes
        AC_DEFINE(PENTIUM,1,[compiling for a pentium on a gcc-based platform?])
    ;;
  x86_64* )
        pentium=yes
        cpu64=yes
        AC_DEFINE(X86_64_SYSTEM,1,[compiling for a X86_64 system on a gcc-based platform?])
    ;;
esac

AM_CONDITIONAL(X86_64_SYSTEM, test x$cpu64 = xyes)

dnl check for required headers
AC_CHECK_HEADERS( [alloca.h stdio.h inttypes.h stdint.h stdlib.h math.h \
                  string.h stdarg.h malloc.h float.h time.h sys/time.h \
                  limits.h stddef.h])

ODE_PRECISION=dSINGLE
AC_SUBST(ODE_PRECISION)

dnl Set some Platform Specific Variables
EXTRA_LIBTOOL_LDFLAGS=
case "$host_os" in
  cygwin* | mingw*)
    EXTRA_LIBTOOL_LDFLAGS="-no-undefined"
    ;;
  *apple* | *darwin*) # For Mac OS X
    dnl We need to use C++ compilation and linking for ode on Mac
    dnl Might as well do it for all code.
    CC="$CXX"
    LINK="$CXXLINK"
    ;;
  *)
    ;;
esac
AC_SUBST(EXTRA_LIBTOOL_LDFLAGS)

dnl stdc++ is required when linking C programs against ode
AC_CHECK_LIB(stdc++,main,[LIBSTDCXX="-lstdc++"],[LIBSTDCXX=])
AC_SUBST(LIBSTDCXX)
AC_CHECK_LIB(pthread,main,[LIBS="$LIBS -lpthread"])

dnl Check if the user wants to profile ODE using gprof
AC_MSG_CHECKING(for gprof)
AC_ARG_ENABLE([gprof],
        AS_HELP_STRING([--enable-gprof],[enable profiling with gprof]),
        gprof=$enableval,
        gprof=no)
if test "$gprof" != no
then
    CFLAGS="-pg $CFLAGS"
    CXXFLAGS="-pg $CXXFLAGS"
    AC_MSG_RESULT(enabled)
    AC_CHECK_LIB(gmon, main,[LIBS="$LIBS -lgmon"])
else
    AC_MSG_RESULT(no)
fi

# Checks for typedefs, structures, and compiler characteristics.
AC_HEADER_STDBOOL
AC_C_INLINE
AC_TYPE_INT32_T
AC_FUNC_OBSTACK
AC_TYPE_SIZE_T
AC_TYPE_UINT32_T

dnl Check for autoscan sugested functions
AC_CHECK_LIB(m, [main])
AC_CHECK_LIB(sunmath, [main])
AC_CHECK_FUNCS([floor memmove memset sqrt sqrtf sinf cosf fabsf atan2f fmodf copysignf copysign snprintf vsnprintf gettimeofday isnan isnanf _isnan _isnanf __isnan __isnanf strchr strstr pthread_attr_setstacklazy clock_gettime])
AC_FUNC_ALLOCA 

AC_ARG_ENABLE([threading-intf],
        AS_HELP_STRING([--disable-threading-intf],
            [disable threading interface support (external implementations may not be assigned; overrides --enable-builtin-threading-impl)]
        ),
        threading_intf=$enableval,threading_intf=yes)
AC_ARG_ENABLE([ou],
        AS_HELP_STRING([--enable-ou],
            [use TLS for global caches (allows threaded collision checks for isolated spaces)]
        ),
        use_ou_tls=$enableval,use_ou_tls=no)
use_ou="no"
if test x$use_ou_tls = xyes -o x$threading_intf = xyes
then
    use_ou="yes"
fi
if test x$use_ou = xyes
then
    OU_NAMESPACE=odeou
    AC_CONFIG_COMMANDS_POST([export OU_NAMESPACE=odeou])
    AC_DEFINE([_OU_NAMESPACE],[odeou],[libou namespace for ODE])
    AC_DEFINE([dOU_ENABLED],[1],[Generic OU features are enabled])
    AC_DEFINE([dATOMICS_ENABLED],[1],[Atomic API of OU is enabled])
    if test x$use_ou_tls = xyes
    then
      AC_DEFINE([dTLS_ENABLED],[1],[Thread Local Storage API of OU is enabled])
    fi

    case "$host_os" in
      cygwin* | mingw*)
        targetos=_OU_TARGET_OS_WINDOWS
        ;;
      *qnx*)
        targetos=_OU_TARGET_OS_QNX
        ;;
      *apple* | *darwin*)
        targetos=_OU_TARGET_OS_MAC
        ;;
      *sunos*)
        targetos=_OU_TARGET_OS_SUNOS
        ;;
      *aix*)
        targetos=_OU_TARGET_OS_AIX
        ;;
      *)
        targetos=_OU_TARGET_OS_GENUNIX
        ;;
    esac

    if test $targetos = _OU_TARGET_OS_MAC
    then
        MAC_OS_X_VERSION=1000
        AC_CHECK_FUNC([OSAtomicAdd32Barrier], [MAC_OS_X_VERSION=1040])
        AC_CHECK_FUNC([OSAtomicAnd32OrigBarrier], [MAC_OS_X_VERSION=1050])
        AC_DEFINE_UNQUOTED(MAC_OS_X_VERSION, $MAC_OS_X_VERSION, [Mac OS X version setting for OU Library])
    fi
    
    if test $targetos = _OU_TARGET_OS_SUNOS
    then
        AC_CHECK_FUNC(atomic_inc_32_nv, [],
            [targetos=_OU_TARGET_OS_GENUNIX])
    fi
    
    AC_DEFINE_UNQUOTED(_OU_TARGET_OS, $targetos, [Target OS setting for OU Library])
fi
AC_CONFIG_SUBDIRS([ou])
AM_CONDITIONAL(ENABLE_OU, test x$use_ou = xyes)

if test x$threading_intf = xyes
then
    AC_ARG_ENABLE([builtin-threading-impl],
        AS_HELP_STRING([--enable-builtin-threading-impl],
            [include built-in multithreaded threading implementation (still must be created and assigned to be used)]
        ),
        use_builtin_threading_impl=$enableval,use_builtin_threading_impl=no)
    if test x$use_builtin_threading_impl = xyes
    then
        AC_DEFINE([dBUILTIN_THREADING_IMPL_ENABLED],[1],[Built-in multithreaded threading implementation included])
    fi
else
    AC_DEFINE([dTHREADING_INTF_DISABLED],[1],[Threading interface is disabled])
    use_builtin_threading_impl=no
fi

AC_ARG_ENABLE([asserts],
		AS_HELP_STRING([--disable-asserts],[disables debug error checking]),
        asserts=$enableval,asserts=yes)
if test x$asserts = xno
then
    CPPFLAGS="$CPPFLAGS -DdNODEBUG"
    if test x$use_ou = xyes
    then
        CPPFLAGS="$CPPFLAGS -DNDEBUG"
    fi
fi

AC_ARG_ENABLE([avx],
        AS_HELP_STRING([--enable-avx],[use AVX instructions]),
        avx=$enableval,
		avx=no)

if test x$avx = xyes
then
    CFLAGS="$CFLAGS -ftree-vectorize -mavx -ffast-math"
    CXXFLAGS="$CXXFLAGS -ftree-vectorize -mavx -ffast-math"
fi

dnl include found system headers into config.h
AH_TOP([
#ifndef ODE_CONFIG_H
#define ODE_CONFIG_H
])
AH_BOTTOM([

#ifdef HAVE_ALLOCA_H
#include <alloca.h>
#endif
#ifdef HAVE_MALLOC_H
#include <malloc.h>
#endif
#ifdef HAVE_STDINT_H
#include <stdint.h>
#endif
#ifdef HAVE_INTTYPES_H
#include <inttypes.h>
#endif

/* an integer type that we can safely cast a pointer to and
 * from without loss of bits.
 */
typedef uintptr_t intP;

#define dEpsilon  FLT_EPSILON

#include "typedefs.h"

#endif /* #define ODE_CONFIG_H */
])

dnl Finally write our Makefiles
AC_CONFIG_FILES([
 Makefile
 include/Makefile
 include/ode/Makefile
 include/ode/version.h
 include/ode/precision.h
 ode/Makefile
 ode/doc/Doxyfile
 ode/doc/Makefile
 ode/src/Makefile
 ode/src/joints/Makefile
 OPCODE/Makefile
 OPCODE/Ice/Makefile
 ode-config
 ode.pc
 ])
AC_OUTPUT

chmod +x ode-config

BUILDDIR=`pwd`

dnl Print some useful information
echo "Configuration:"
echo "  Build  system type:      $build"
echo "  Host   system type:      $host"
echo "  Is target a Pentium:     $pentium"
echo "  Is target x86-64:        $cpu64"
echo "  TLS for global caches:   $use_ou_tls"
echo "  Threading intf enabled:  $threading_intf"
echo "  Use AVX enabled:         $avx"
echo "  Built-in threading included: $use_builtin_threading_impl"
echo "  Enable debug error check: $asserts"
echo "  Headers will be installed in $includedir/ode"
echo "  Libraries will be installed in $libdir"
echo "  Building in directory    $BUILDDIR"

