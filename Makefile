build:
	dotnet build

clean:
	rm -rf */bin */obj

rebuild: clean build

test:
	dotnet test Ficdown.Parser.Tests

publish: clean
	dotnet publish --self-contained -c Release -r linux-x64 Ficdown.Console
	tar -C Ficdown.Console/bin/Release/netcoreapp2.1/linux-x64/publish -cvzf /tmp/ficdown-linux64.tar.gz .
	dotnet publish --self-contained -c Release -r win-x64 Ficdown.Console
	7z a -tzip /tmp/ficdown-win64.zip -w ./Ficdown.Console/bin/Release/netcoreapp2.1/win-x64/publish/*
	dotnet publish --self-contained -c Release -r osx-x64 Ficdown.Console
	tar -C Ficdown.Console/bin/Release/netcoreapp2.1/osx-x64/publish -cvzf /tmp/ficdown-osx64.tar.gz .
