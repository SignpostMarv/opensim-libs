BUILD=nhibernate-build
DRIVER=$(BUILD)/nhibernate/src/NHibernate/Driver/SqliteClientDriver.cs
MONO1=$(BUILD)/nhibernate/lib/mono/1.0
MONO2=$(BUILD)/nhibernate/lib/mono/2.0
OUTDIR=$(BUILD)/nhibernate/build/NHibernate-1.2.1.GA/bin/mono-2.0/
NHIB=$(OUTDIR)/NHibernate.dll

all: $(NHIB)
	mkdir nhibernate-dll
	cp $(NHIB) $(OUTDIR)/NHibernate.Mapping.Attributes.dll $(OUTDIR)/Castle.DynamicProxy.dll $(OUTDIR)/Iesi.Collections.dll nhibernate-dll

$(NHIB): $(MONO2) $(DRIVER)
	cd $(BUILD)/nhibernate && nant -D:project.config=release clean build 

$(MONO2): $(BUILD)
	cp -a $(MONO1) $(MONO2) 

$(DRIVER): $(BUILD)
	cd $(BUILD) && patch -p0 < ../sqlite-driver-mono-1.2.x.patch

$(BUILD):
	svn co https://nhibernate.svn.sourceforge.net/svnroot/nhibernate/tags/1.2.1.GA $(BUILD)

clean:
	rm -rf $(BUILD) nhibernate-dll
