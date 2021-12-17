
SET APIDIR=BioRad_CFX_API_v1.7.5
SET TOOLSDIR=DataProcessorApplication_v1.8

SET APIPATH=C:\PCR_BioRad
SET LOGDIR=log

@echo off

echo Copying files for directory %APIDIR% to path %APIPATH%
pause
xcopy %APIDIR% %APIPATH%\APIs\%APIDIR% /y /e /i
xcopy %TOOLSDIR% %APIPATH%\tools\%TOOLSDIR% /y /e /i
xcopy Changelog.md C:\PCR_BioRad\APIs\   /y 

echo Checking log directory
if NOT EXIST %APIPATH%\%LOGDIR% (
	echo Creating...
	mkdir %APIPATH%\%LOGDIR%
)


echo Opening notepad
notepad %userprofile%\covmatic.conf


start C:\PCR_BioRad\APIs\
pause