@echo off
@set path=C:\Program Files (x86)\Inno Setup 6\;%path%

cd 
md publish
dotnet publish ..\KitKey.Service -c Debug -o publish
iscc Installer.iss
rd publish /q /s
