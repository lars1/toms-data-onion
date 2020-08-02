#!/bin/sh
dotnet build
dotnet run -p layer0 toms-data-onion.txt > layer0-decoded.txt
dotnet run -p layer1 layer0-decoded.txt > layer1-decoded.txt
dotnet run -p layer2 layer1-decoded.txt > layer2-decoded.txt
dotnet run -p layer3 layer2-decoded.txt > layer3-decoded.txt
dotnet run -p layer4 layer3-decoded.txt > layer4-decoded.txt
dotnet run -p layer5 layer4-decoded.txt > layer5-decoded.txt
