#!/bin/sh

case "$1" in

  'clean')
    xbuild /t:clean
    echo y|mono Prebuild.exe /clean
    rm -Rf ./bin
    for i in `find . -name obj`
      do rm -Rf $i
    done
  ;;

  *)
    mono Prebuild.exe /target vs2010
  ;;

esac
