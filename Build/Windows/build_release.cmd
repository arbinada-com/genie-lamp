@echo off

rem if "%1"=="" (
rem 	echo Input version number
rem	goto batch_failed
rem )

call "%~pd0.\set_env.cmd"

set APP_NAME=GenieLamp_MDD
set PRJ_SRC_ROOT=%PROJECTS_ROOT%\Sources
set APP_RELEASE_ROOT=%PROJECTS_ROOT%\%APP_NAME%
set RELEASE_SRC_DIR=%PROJECTS_ROOT%\Bin\Release
set PROJECTS_RELEASE_ROOT=%PROJECTS_ROOT%\Build

rem "%MONO_BIN%\xbuild" /? > xbuild.txt
set COMMON_BUILD_OPTIONS=/target:Build /verbosity:quiet
call "%MONO_BIN%\xbuild" "%PRJ_SRC_ROOT%\GenieLamp.Solution\GenieLamp.Solution.sln" %COMMON_BUILD_OPTIONS% /property:configuration=Debug
call "%MONO_BIN%\xbuild" "%PRJ_SRC_ROOT%\GenieLamp.Solution\GenieLamp.Solution.sln" %COMMON_BUILD_OPTIONS% /property:configuration=Release
if errorlevel 1 goto batch_failed
goto do_package

rem Core & spell
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Core\GenieLamp.Core.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Spell\GenieLamp.Spell.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Utils\AppVersion\AppVersion.csproj"
if errorlevel 1 goto batch_failed
rem Genies
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Genies\GenieLamp.Genies.DbSchemaImport\GenieLamp.Genies.DbSchemaImport.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Genies\GenieLamp.Genies.NHibernate\GenieLamp.Genies.NHibernate.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Genies\GenieLamp.Genies.Oracle\GenieLamp.Genies.Oracle.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Genies\GenieLamp.Genies.ServicesLayer\GenieLamp.Genies.ServicesLayer.csproj"
if errorlevel 1 goto batch_failed
call:BuildProject "%PRJ_SRC_ROOT%\GenieLamp.Genies\GenieLamp.Genies.Sqlite\GenieLamp.Genies.Sqlite.csproj"
if errorlevel 1 goto batch_failed

:do_package
if not "%1"=="package" goto all_done
echo Copying files...
md "%APP_RELEASE_ROOT%"
copy /y "%RELEASE_SRC_DIR%\*" "%APP_RELEASE_ROOT%\*"
if errorlevel 1 goto batch_failed
md "%APP_RELEASE_ROOT%\Data"
copy /y "%PRJ_SRC_ROOT%\GenieLamp.Core\XMLSchema\*.xsd" "%APP_RELEASE_ROOT%\Data\*"
if errorlevel 1 goto batch_failed

:extract_version
set PATH=%SVN_BIN%;%PATH%

rem for /F "tokens=2 skip=4" %%i in ('svn info -rHEAD') do if not defined GLCORE_REVISION set GLCORE_REVISION=%%i
for /f "tokens=1,2* delims=." %%i in ('%APP_RELEASE_ROOT%\AppVersion extract --assembly %APP_RELEASE_ROOT%\GenieLamp.Core.dll') do set GLCORE_ASSEMBLY_VERSION=%%i.%%j
rem for /f "tokens=*" %%v in ('%APP_RELEASE_ROOT%\AppVersion extract --assembly %APP_RELEASE_ROOT%\GenieLamp.Core.dll') do set GLCORE_VERSION=%%v
set GLCORE_VERSION=%GLCORE_ASSEMBLY_VERSION%r%GLCORE_REVISION%
echo.GL Core version is %GLCORE_VERSION%
set APP_RELEASE_FILENAME=%APP_NAME%_%GLCORE_VERSION%.zip
del /q "%PROJECTS_RELEASE_ROOT%\%APP_RELEASE_FILENAME%"

"%ZIP_HOME%\7z.exe" a "%PROJECTS_RELEASE_ROOT%\%APP_RELEASE_FILENAME%" -tzip -ir!"%APP_RELEASE_ROOT%\*"
if errorlevel 1 goto batch_failed

rmdir "%APP_RELEASE_ROOT%" /q /s
goto all_done

:batch_failed
echo %APP_NAME% build failed
exit /b 1

:all_done

:batch_ok
echo %APP_NAME% built and copied OK
exit /b 0

:BuildProject
call "%MONO_BIN%\xbuild" "%~1" %COMMON_BUILD_OPTIONS%
goto:eof