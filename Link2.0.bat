::将此Link文件放到Unity项目文件夹内（Assets同级目录）



@echo off
cd %~p0

set "FOLDER=%~dp0"
for %%J in ("%FOLDER%") do for %%I in ("%%~dpJ.") do (
set target=%%~nxI_mirro
)



if exist ..\%target% (
     rd /s /Q ..\%target%
     )

md ..\%target%
mklink /J "%~dp0..\%target%/Assets" "%~dp0/Assets"
mklink /J "%~dp0..\%target%/Packages" "%~dp0/Packages"
mklink /J "%~dp0..\%target%/ProjectSettings" "%~dp0/ProjectSettings"
mklink /J "%~dp0..\%target%/Library" "%~dp0/Library"

cd ..\%target%
echo 镜像文件夹生成在 %cd%
pause