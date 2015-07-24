#!/bin/sh

DIR=`dirname $0`
if [ ! -d "$DIR/xunit.runner.console.2.0.0" ]; then
  nuget install xunit.runner.console -Version 2.0.0 -OutputDirectory $DIR
fi
mono --debug $DIR/xunit.runner.console.2.0.0/tools/xunit.console.exe $DIR/../Ficdown.Parser.Tests/bin/Debug/Ficdown.Parser.Tests.dll
