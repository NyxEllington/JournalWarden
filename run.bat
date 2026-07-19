@echo off
echo ================================================
echo    Journal Warden - Elite Dangerous Monitor
echo ================================================
echo.
echo Building and launching...
echo.

dotnet run

if errorlevel 1 (
    echo.
    echo Error occurred. Press any key to exit.
    pause >nul
)
