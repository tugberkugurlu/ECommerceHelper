##################################################
#resources
##################################################
#http://www.powershellpro.com/powershell-tutorial-introduction/powershell-tutorial-conditional-logic/
#http://technet.microsoft.com/en-us/library/ee176935.aspx
#http://weblogs.asp.net/soever/archive/2007/02/06/powershell-regerencing-files-relative-to-the-currently-executing-script.aspx
##################################################
#resources
##################################################

$isPublishing = Read-Host "Would you like to publish your package? (y: Yes, n: No)"
if($isPublishing -eq "y") { 
    
    #safely find the solutionDir
    $ps1Dir = Split-Path -Path $MyInvocation.MyCommand.Path -Parent
    $solutionDir = Split-Path -Path $ps1Dir -Parent
    
    $packages = dir "$solutionDir\artifacts\packages\ECommerceHelper.*.nupkg"
    sal ___nuget .\Nuget.exe
    
    $nugetApiKey = Read-Host "Please provide your Nuget API key"
    
    foreach($package in $packages) { 
        #$package is type of System.IO.FileInfo
        ___nuget push $package.FullName $nugetApiKey
    }
    
} elseif ($isPublishing -eq "n") { 
    
    
} else { 

    Write-Host "Invalid value!" -ForegroundColor yellow
}