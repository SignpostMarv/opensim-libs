----------------------------------------------------------------------
-- Premake4 configuration script for OpenDE
-- Contributed by Jason Perkins (starkos@industriousone.com)
-- For more information on Premake: http://industriousone.com/premake
----------------------------------------------------------------------

  ode_version = "OS0.13.3"

----------------------------------------------------------------------
-- Configuration options
----------------------------------------------------------------------

newoption {
    trigger     = "enable-avx",
    description = "compile for AVX SIMD."
  }  
    
  newoption {
    trigger     = "with-ou",
    description = "Use TLS for global caches (allows threaded collision checks for separated spaces)"
  }

  newoption {
    trigger     = "no-threading-intf",
    description = "Disable threading interface support (external implementations may not be assigned; overrides with-builtin-threading-impl)"
  }

  newoption {
    trigger     = "16bit-indices",
    description = "Use 16-bit indices for trimeshes (default is 32-bit)"
  }
 
  newoption {
    trigger     = "to",
    value       = "path",
    description = "Set the output location for the generated project files"
  }
  
  newoption {
    trigger     = "only-shared",
	description = "Only build shared (DLL) version of the library"
  }
  
  newoption {
    trigger     = "only-static",
	description = "Only build static versions of the library"
  }
 
  -- always clean all of the optional components and toolsets
  if _ACTION == "clean" then
    for action in premake.action.each() do
      os.rmdir(action.trigger)
    end
    os.remove("../ode/src/config.h")
    os.remove("../include/ode/version.h")
    os.remove("../include/ode/precision.h")
    os.remove("../libccd/src/ccd/precision.h")
  end
  
  -- special validation for Xcode  
  if _ACTION == "xcode3" and (not _OPTIONS["only-static"] and not _OPTIONS["only-shared"]) then
    error(
	  "Xcode does not support different library types in a single project.\n" ..
	  "Please use one of the flags: --only-static or --only-shared", 0)
  end

  -- build the list of configurations, based on the flags. Ends up
  -- with configurations like "Debug", "DebugSingle" or "DebugSingleShared"
  local configs = { "Debug", "Release" }
  
  local function addconfigs(...)
	local newconfigs = { }
	for _, root in ipairs(configs) do
	  for _, suffix in ipairs(arg) do
		table.insert(newconfigs, root .. suffix)
	  end
	end
	configs = newconfigs
  end
	
  if not _OPTIONS["only-shared"] and not _OPTIONS["only-static"] then
    addconfigs("DLL", "Lib")
  end
  
----------------------------------------------------------------------
-- The solution, and solution-wide settings
----------------------------------------------------------------------

  solution "ode"

    language "C++"
    uuid     "4DA77C12-15E5-497B-B1BB-5100D5161E15"
    location ( _OPTIONS["to"] or _ACTION )

    includedirs {
      "../include",
      "../ode/src"
    }
    
    defines { "_MT" }
    
    -- apply the configuration list built above
    configurations (configs)
    
    configuration { "Debug*" }
	    if _OPTIONS["enable-avx"] then
		   defines { "_DEBUG", "dIDESINGLE"}
		else
		   defines { "_DEBUG", "dIDESINGLE"}
        end
        flags   { "Symbols"}
      
    configuration { "Release*" }
	    if _OPTIONS["enable-avx"] then
		   defines { "NDEBUG", "dNODEBUG", "dIDESINGLE"}
		else
		   defines { "NDEBUG", "dNODEBUG", "dIDESINGLE"}
        end
		flags   { "OptimizeSpeed", "NoFramePointer"}
   
    configuration { "Windows" }
      defines { "WIN32" }

    configuration { "MacOSX" }
      linkoptions { "-framework Carbon" }
      
    -- give each configuration a unique output directory
    for _, name in ipairs(configurations()) do
      configuration { name }
        targetdir ( "../lib/" .. name )
    end
      
    -- disable Visual Studio security warnings
    configuration { "vs*" }
      defines { "_CRT_SECURE_NO_DEPRECATE" }

    -- enable M_* macros from math.h
    configuration { "vs*" }
      defines { "_USE_MATH_DEFINES" }

    -- don't remember why we had to do this	
    configuration { "vs2002 or vs2003", "*Lib" }
      flags  { "StaticRuntime" }
	 
----------------------------------------------------------------------
-- The ODE library project
----------------------------------------------------------------------

  project "ode"

    -- kind     "StaticLib"
    location ( _OPTIONS["to"] or _ACTION )

    includedirs {
      "../ode/src/joints",
      "../OPCODE",
    }

    files {
      "../include/ode/*.h",
      "../ode/src/joints/*.h", 
      "../ode/src/joints/*.cpp",
      "../ode/src/*.h", 
      "../ode/src/*.c", 
      "../ode/src/*.cpp",
      "../OPCODE/**.h", "../OPCODE/**.cpp"
    }

    excludes {
      "../ode/src/collision_std.cpp",
    }

      includedirs { "../ou/include" }
      files   { "../ou/include/**.h", "../ou/src/**.h", "../ou/src/**.cpp" }
      defines { "_OU_NAMESPACE=odeou" }

      if _ACTION == "gmake" and ( os.get() == "linux" or os.get() == "bsd" ) then
        buildoptions { "-pthread" }
        linkoptions { "-pthread" }
      end

      if _ACTION == "gmake" and os.get() == "windows" then
        buildoptions { "-mthreads" }
        linkoptions { "-mthreads" }
      end

      -- TODO: MacOSX probably needs something too
      
    excludes { "../ode/src/collision_libccd.cpp", "../ode/src/collision_libccd.h" }

    configuration { "windows" }
      links   { "user32" }
            
    configuration { "only-static or *Lib" }
      kind    "StaticLib"
      defines "ODE_LIB"
      
    configuration { "only-shared or *DLL" }
      kind    "SharedLib"
      defines "ODE_DLL"

    configuration { "*DLL" }
      defines "_DLL"

    configuration { "Debug" }
      targetname "oded"
	  
    configuration { "Release" }
      targetname "ode"

----------------------------------------------------------------------
-- Write a custom <config.h> to build, based on the supplied flags
----------------------------------------------------------------------

  if _ACTION and _ACTION ~= "clean" then
    local infile = io.open("config-default.h", "r")
    local text = infile:read("*a")

    text = string.gsub(text, "/%* #define dOU_ENABLED 1 %*/", "#define dOU_ENABLED 1")
    text = string.gsub(text, "/%* #define dATOMICS_ENABLED 1 %*/", "#define dATOMICS_ENABLED 1")

    text = string.gsub(text, "/%* #define dTLS_ENABLED 1 %*/", "#define dTLS_ENABLED 1")

    text = string.gsub(text, "/%* #define dTHREADING_INTF_DISABLED 1 %*/", "#define dTHREADING_INTF_DISABLED 1")

    if _OPTIONS["16bit-indices"] then
      text = string.gsub(text, "#define dTRIMESH_16BIT_INDICES 0", "#define dTRIMESH_16BIT_INDICES 1")
    end
    
    local outfile = io.open("../ode/src/config.h", "w")
    outfile:write(text)
    outfile:close()
  end

----------------------------
-- Write precision headers
----------------------------
  if _ACTION and _ACTION ~= "clean" then
    function generateheader(headerfile, placeholder, precstr)
      local outfile = io.open(headerfile, "w")
      for i in io.lines(headerfile .. ".in")
      do
        local j,_ = string.gsub(i, placeholder, precstr)
        --print("writing " .. j .. " into " .. headerfile)
        outfile:write(j .. "\n")
      end
      outfile:close()
    end
    
    function generate(precstr)
      generateheader("../include/ode/precision.h", "@ODE_PRECISION@", "d" .. precstr)
    end
    
    generate("SINGLE")
    generateheader("../include/ode/version.h", "@ODE_VERSION@", ode_version)

  end
