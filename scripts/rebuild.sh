#!/bin/sh

DIR=`dirname $0`
rm -rf $DIR/../*/bin $DIR/../*/obj
xbuild $DIR/../Ficdown.sln
