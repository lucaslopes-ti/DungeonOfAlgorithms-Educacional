@echo off
REM ========================================
REM    DUNGEON OF ALGORITHMS - SETUP E RUN
REM    Use este script apos clonar o repo!
REM ========================================
echo.
echo ========================================
echo    DUNGEON OF ALGORITHMS
echo ========================================
echo.

cd /d "%~dp0"

echo [1/3] Restaurando dependencias...
dotnet restore DungeonOfAlgorithms.csproj
if %ERRORLEVEL% NEQ 0 (
    echo [ERRO] Falha ao restaurar dependencias!
    pause
    exit /b 1
)

echo.
echo [2/3] Compilando o projeto...
dotnet build DungeonOfAlgorithms.csproj
if %ERRORLEVEL% NEQ 0 (
    echo [ERRO] Falha na compilacao!
    pause
    exit /b 1
)

echo.
echo [3/3] Iniciando o jogo...
dotnet run --project DungeonOfAlgorithms.csproj

pause
