$MyDir = Split-Path $MyInvocation.MyCommand.Definition

Import-Module $MyDir"\CiPsLib.Common.psm1" -Force

# Octopus variables
CheckSetVarDefault -Name "OctopusEnvironmentName" -Default "Production" -Verbose
CheckSetVarDefault -Name "OctopusPackageVersion" -Default "0.0.0.0" -Verbose

# Custom variables
CheckSetVarDefault -Name "DeploymentBaseUrl" -Default "http://dummy" -Verbose
CheckSetVarDefault -Name "ApplicationShortName" -Default "LogReceiver"
CheckSetVarDefault -Name "DeploymentDir" -Verbose
CheckSetVarDefault -Name "LogLevel" -Default "INFO" -Verbose

###############################################################################
$TempDirectoryPath = "$MyDir\Temp"
$ApplicationSourceDirectory = "$MyDir\Lawnch"
$ConfigFilePath = "$ApplicationSourceDirectory\LogReceiver.exe.config"
$LawnchPack = "$MyDir\Pack.exe"
$LauncherFilePath = "$MyDir\Launcher\LogReceiver.Launcher.exe"

###############################################################################

Write-Host "Creating $DeploymentDir"
[IO.Directory]::CreateDirectory($DeploymentDir)

if (Get-Item $TempDirectoryPath -ErrorAction SilentlyContinue) {
        Write-Host "Deleting '$TempDirectoryPath'"
        Remove-Item $TempDirectoryPath -Recurse -Force
}
New-Item -ItemType directory $TempDirectoryPath

cd $MyDir
Call { & $LawnchPack $ApplicationShortName $ApplicationSourceDirectory $OctopusPackageVersion $LauncherFilePath $OctopusPackageVersion $DeploymentBaseUrl $TempDirectoryPath $OctopusEnvironmentName | Write-Host } "LawnchPack failed"

#            Console.WriteLine(@"INPUT - Application name: {0}", applicationName);
#            Console.WriteLine(@"INPUT - Application source directory: {0}", applicationSourceDirectory);
#            Console.WriteLine(@"INPUT - Application version: {0}", applicationVersion);
#            Console.WriteLine(@"INPUT - Launcher EXE path: {0}", launcherExeSourcePath);
#            Console.WriteLine(@"INPUT - Launcher version: {0}", launcherVersion);
#            Console.WriteLine(@"INPUT - Base URL: {0}", baseUrl);
#            Console.WriteLine(@"INPUT - Destination directory: {0}", destinationDirectory);

###############################################################################
if ($ClickOnceDeploymentDir -ne "") {
        Remove-Item $DeploymentDir"\*" -Force -Recurse | Write-Host
        Copy-Item -Force -Recurse "$TempDirectoryPath\*" $DeploymentDir | Write-Host
}

Remove-Item $TempDirectoryPath -Recurse -Force
