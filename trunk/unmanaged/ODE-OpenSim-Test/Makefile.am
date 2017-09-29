AUTOMAKE_OPTIONS = foreign
ACLOCAL_AMFLAGS = -I m4 --install

if ENABLE_OU
OU_DIR = ou
endif

OPCODE_DIR = OPCODE

SUBDIRS = include \
          $(OPCODE_DIR) \
          $(OU_DIR) \
          ode

bin_SCRIPTS = ode-config

# Utility rule for making a release
release: dist-gzip dist-bzip2
	@echo Created release packages for ${PACKAGE}-${VERSION}.

EXTRA_DIST = bootstrap build tools \
        CHANGELOG.txt COPYING INSTALL.txt README.md LICENSE.TXT \
        bindings

pkgconfigdir = $(libdir)/pkgconfig
pkgconfig_DATA = ode.pc
