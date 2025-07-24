@echo off
echo ================================
echo HL7 to PDF Parser - Build Script
echo ================================
echo.

echo Restoring NuGet packages...
nuget restore HSM-PROJECT.sln
if %ERRORLEVEL% neq 0 (
    echo ERROR: Failed to restore NuGet packages
    pause
    exit /b 1
)

echo.
echo Building solution...
msbuild HSM-PROJECT.sln /p:Configuration=Release /p:Platform="Any CPU"
if %ERRORLEVEL% neq 0 (
    echo ERROR: Build failed
    pause
    exit /b 1
)

echo.
echo ================================
echo Build completed successfully!
echo ================================
echo.
echo To run the application:
echo 1. Open HSM-PROJECT.sln in Visual Studio
echo 2. Set HSM-PROJECT as startup project
echo 3. Press F5 to run
echo 4. Navigate to /HL7Parser for the main interface
echo.
echo For testing:
echo - Use TestHL7Parser.TestConversion() method
echo - Check the sample HL7 message in the web interface
echo - Review the README.md for full documentation
echo.
pause