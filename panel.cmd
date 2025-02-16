@echo off
title OmniTool Panel

REM Si des arguments sont passés, on les traite directement
if not "%1"=="" goto processArgs
:: if not "%2"=="" goto processArgs

:menu
:: goto SetDefaultAppDir
cls
echo =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
echo          OmniTools Core Menu
echo =-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=
echo.
echo 1. Run OmniTools
echo 2. Build
echo 3. Add Package
echo 4. Quitter
echo.
set /p choice="Faites votre choix (1-4) : "

if "%choice%"=="1" goto runApp
if "%choice%"=="2" goto buildApp
if "%choice%"=="3" goto addPackage
if "%choice%"=="4" goto end

echo.
echo Choix invalide. Veuillez recommencer.
pause
goto menu

:processArgs
REM Traitement des arguments en ligne de commande
if /I "%1"=="/run" goto runApp
if /I "%1"=="/build" goto buildApp
if /I "%1"=="/add" goto addPackageArg
:: if /I "%2"=="/appdefaultdir" goto SetDefaultAppDir

echo [7;31mArgument invalide.[0m
echo Utilisation depuis la ligne de commande :
echo   /run             -> Run
echo   /build           -> Build
echo   /add NomDuPackage-> Add Package "NomDuPackage"
pause
goto end

:runApp
echo.
echo [100mLaunching...[0m
dotnet run
echo.
echo [42mProcess Executed[0m
pause
goto end

:buildApp
echo.
echo [100mBuilding...[0m
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -o ./publish
echo.
echo [42mProcess Executed[0m
pause
goto end

:addPackage
echo.
set /p packageName="Entrez le nom du package a ajouter : "
if "%packageName%"=="" (
    echo Aucun package saisi.
    pause
    goto menu
)
echo.
echo [100mAjout du package %packageName%...[0m
dotnet add package %packageName%
echo.
echo [42mProcess Executed[0m
pause
goto menu

:addPackageArg
REM Pour l'argument /add, le nom du package doit être passé en second paramètre
if "%2"=="" (
    echo [7;31mVeuillez fournir le nom du package en argument.[0m
    pause
    goto end
)
echo.
echo [100mAjout du package %2...[0m
dotnet add package %2
echo.
echo [42mProcess Executed[0m
pause
goto end

:end
echo.
echo Exiting...
exit
