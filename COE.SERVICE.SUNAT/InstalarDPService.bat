@ECHO OFF

REM Para instalar se necesita .Net Framework 4.0 o superior
REM Para instalar ejecutar este Bath como Administrador
REM Para instalar use el parametro /i
REM Para desistalar use el parametro /u
set DOTNETFX2=%SystemRoot%\Microsoft.NET\Framework\v4.0.30319\
set PATH=%PATH%;%DOTNETFX2%

echo Instalando Servicio de DigitalPro V.7
echo ---------------------------------------------------
InstallUtil /i D:\Produccion\DPS_SunatXML\DigitalPro.Service.EnvioSunat.exe
echo ---------------------------------------------------
echo El proceso termino con exito!!!
pause