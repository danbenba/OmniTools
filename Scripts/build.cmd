@echo off
cd ..\
echo [100mBuilding...[0m
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish
echo [42mProcess Executed[0m
pause