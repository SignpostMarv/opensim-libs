C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild Mono.Addins.sln /t:clean

rmdir /S /Q bin
rmdir /S /Q Mono.Addins.CecilReflector\obj
rmdir /S /Q Mono.Addins.Setup\obj
rmdir /S /Q Mono.Addins\obj
rmdir /S /Q mautil\obj

Prebuild.exe /clean
Prebuild.exe /target vs2010

echo C:\WINDOWS\Microsoft.NET\Framework\v3.5\msbuild Mono.Addins.sln /p:Configuration=Release > compile.bat
