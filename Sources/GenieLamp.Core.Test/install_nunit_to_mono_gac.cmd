@echo off

set GACUTIL_CMD=C:\Program Files\Mono-2.10.9\bin\gacutil.bat
set NUNIT_HOME=C:\Program Files\NUnit 2.6\bin

call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\lib\nunit-console-runner.dll"
call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\lib\nunit.core.dll"
call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\lib\nunit.core.interfaces.dll"
call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\lib\nunit.util.dll"
call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\framework\nunit.framework.dll"
call "%GACUTIL_CMD%" -i "%NUNIT_HOME%\framework\nunit.mocks.dll"