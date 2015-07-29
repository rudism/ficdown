#!/bin/sh

DIR=`dirname $0`
rm -rf $DIR/../*/bin $DIR/../*/obj
xbuild /p:Configuration=Release $DIR/../Ficdown.sln
cp -R $DIR/../Ficdown.Console/bin/Release /tmp/ficdown
rm -f /tmp/ficdown/*.mdb
mv /tmp/ficdown/Ficdown.Console.exe /tmp/ficdown/ficdown.exe
zip -j /tmp/ficdown.zip /tmp/ficdown/*
rm -rf /tmp/ficdown
