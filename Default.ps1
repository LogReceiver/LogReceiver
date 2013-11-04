Framework "4.0x64"

$MyDir = Split-Path $MyInvocation.MyCommand.Definition
Import-Module $MyDir"\Modules\CiPsLib\tools\CiPsLib.Common.psm1" -Force

Properties {
        $BuildDir = $MyDir
        $TmpDir = "$BuildDir\tmp"

        $MsBuild = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\MSBuild.exe"
        $LawnchPack = "$BuildDir\modules\Lawnch\Source\Code\Lawnch.Pack\bin\$Configuration\Pack.exe"
        $NuGet = "$BuildDir\Configuration\Tools\NuGet.exe"

        $CiPsLibDir = "$BuildDir\modules\CiPsLib\tools"

        $SolutionFilePath = "$BuildDir\src\Solutions\LogReceiver.sln"
        $LauncherFilePath = "$BuildDir\src\Launcher\LogReceiver.Launcher\bin\LogReceiver.Launcher.exe"

        $OutputDirLogReceiver = "$BuildDir\src\Client\LogReceiver\bin\$Configuration"
        $OutputDirPack = "$TmpDir\Pack"
        $OutputDirNuGet = "$TmpDir\NuGet"
        $LawnchPackDir = "$BuildDir\Configuration\Launcher\Octopus.LogReceiver.Launcher\Lawnch"
        $LawnchNuSpecDir = "$BuildDir\Configuration\Launcher"
        $LawnchNuSpecFilePath = "$LawnchNuSpecDir\Octopus.LogReceiver.Launcher.nuspec"

}

FormatTaskName (("-"*25) + "[{0}]" + ("-"*25))

Task Default -Depends Pack_Binaries, Pack_Launcher

Task Clean {
        Exec { &$MsBuild $SolutionFilePath /t:Clean /p:Configuration=$Configuration } "Failed to clean LogReceiver.sln"
}

Task Build -Depends Clean {
        Exec { &$MsBuild $SolutionFilePath /t:Build /p:Configuration=$Configuration } "Failed to build LogReceiver.sln"
}

Task Pack_Binaries -Depends Build {
        [IO.Directory]::CreateDirectory($OutputDirPack)
        Copy-Item -Path "$OutputDirLogReceiver\*" -Exclude "*.pdb","*.xml" -Destination $OutputDirPack -Force -Recurse -Verbose | Write-Host
}

Task Pack_Launcher -depends Build {
        Write-Host "### COPING APPLICATION BINARIES ##################################"
        Copy-Item -Path "$OutputDirLogReceiver\*" -Exclude "*.pdb","*.xml" -Destination $LawnchPackDir -Force -Recurse -Verbose | Write-Host

        Write-Host "### COPING LAWNCH PACK ###########################################"
        Copy-Item -Path $LawnchPack -Destination "$LawnchPackDir\..\" -Force -Verbose | Write-Host

        Write-Host "### COPING LAWNCH PACK ###########################################"
        Copy-Item -Path $LauncherFilePath -Destination "$LawnchPackDir\..\Launcher\" -Force -Verbose | Write-Host

        Write-Host "### COPING CSPSLIB FILES #########################################"
        Copy-Item -Path "$CiPsLibDir\*" -Destination "$BuildDir\Configuration\Launcher\Octopus.LogReceiver.Launcher\" -Recurse -Force -Verbose | Write-Host
        
        Write-Host "### CREATING NUGET PACKAGE #######################################"
        [IO.Directory]::CreateDirectory($OutputDirNuGet)
        Exec { &$NuGet pack $LawnchNuSpecFilePath "-NoDefaultExcludes" "-Version" "$Version" "-Properties" "Configuration=Release" "-BasePath" $LawnchNuSpecDir "-OutputDirectory" $OutputDirNuGet } "Failed to create Octopus.LogReceiver.Launcher.nupkg"
}
