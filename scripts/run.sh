#!/bin/sh

DIR=`dirname $0`
mono $DIR/../Ficdown.Console/bin/Debug/Ficdown.Console.exe "$@"
