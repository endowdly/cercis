@echo off

rem USAGE
rem At the "solution" level this file needs a target.build file which just enumerates project folders on a newline.
rem At the "project" level, this file needs a response file and a copy of itself in the project to call.
rem If this is just used to build one project, only needs the response file.

rem Get the 'Project' Name
for %%I in (.) do set CurrDirName=%%~nxI

if /i "%~1"=="build" goto Build

echo Building %CurrDirName%
echo.

title %CurrDirName% Build 
color 08

rem Some basic colors; only works in new Console hosts
set good=[1m[32m
set bad=[1m[31m
set reset=[0m
set magenta=[1m[35m
set cyan=[1m[36m

if exist *.build (
    echo Using target file: %magenta%target.build%reset%
    echo.

    for /f "delims=" %%t in (target.build) do call :process %%t

    echo %good%Done%reset%
    goto die
)

:Build

setlocal

echo Building %CurrDirName%
echo.

rem Check for framework tools
set framework=C:\Windows\Microsoft.NET\Framework64\
if not exist %framework% (
    echo %bad%Framework tools missing%reset%
    goto fail
)
rem This for loop should get the highest framework available
for /f %%f in ('dir /b /ad %framework% /on') do set fDir=%%f
set toolPath="%framework%%fDir%"

rem Response File
if exist *.rsp (
    for /f %%f in ('dir /b *.rsp') do set rf=%%f
) else (
    echo %bad%Response file missing%reset%
    goto fail 
)

:Clean 

echo Cleaning...

rem Deletes the files in out
if exist out (
    for %%f in (.\out\*) do echo  %%~nxf
    del /q /f out\*
)
rem Deletes the files in doc
if exist doc (
    for %%f in (.\doc\*) do echo  %%~nxf
    del /q /f doc\*
)
rem Deletes resource files
if exist res (
    for %%f in (.\res\*.resource) do echo  %%~nxf
    del /q /f res\*.resource
)
if errorlevel 1 (
    goto fail
) else (
    echo  %good%Clean%reset%
)

echo.
echo Building...
echo.
echo  Using response file: %magenta%%rf%%reset%
echo.

rem Build Resource File first
if exist res\*.cs (
    for %%f in (.\res\*.cs) do echo  %%~nxf 
    %toolPath%\csc.exe /t:library /out:.\res\Disker.resource /nologo .\res\*.cs 
) 

rem List all source files
if exist src (
    for %%f in (.\src\*.cs) do echo %%~nfx
) 
if exist *.cs (
    for %%f in (*.cs) do echo  %%~nxf
) else (
    echo.
    echo  %bad%Nothing to build!%reset%
    goto die 
)

rem Make sure out dir exists
if not exist .\out mkdir out 
if not exist .\doc mkdir doc 

rem Make the call to csc 
%toolPath%\csc.exe @%rf% /nologo
endlocal

if errorlevel 1 (
    goto fail
) else (
    echo  %good%Built%reset%
)

:die 
    echo.
    echo.
    REM pause
    goto:eof

:fail 
    echo.
    echo %bad%Fail%reset%
    echo.
    REM pause
    exit \b 1

:process
    echo %cyan%%1%reset%
    echo.
    pushd %1
    call build.cmd Build
    popd