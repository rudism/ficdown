#!/bin/sh

DIR=`dirname $0`
rm -rf $DIR/../*/bin $DIR/../*/obj
xbuild /p:Configuration=Release $DIR/../Ficdown.sln
cp -R $DIR/../Ficdown.Console/bin/Release /tmp/ficdown
rm -f /tmp/ficdown/*.mdb /tmp/ficdown/*.pdb
rm /tmp/ficdown/Ficdown.Console.exe.config
mv /tmp/ficdown/Ficdown.Console.exe /tmp/ficdown/ficdown.exe
7z a -tzip /tmp/ficdown.zip -w /tmp/ficdown/*
rm -rf /tmp/ficdown
