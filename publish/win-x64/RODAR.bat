@echo off
echo Iniciando Dungeon of Algorithms...
DungeonOfAlgorithms.exe
if errorlevel 1 (
    echo.
    echo ERRO: O jogo encontrou um problema.
    echo Verifique se o .NET Runtime esta instalado.
    pause
)
