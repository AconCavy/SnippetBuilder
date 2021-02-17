function Build-Release {
    param (
        [Parameter(Mandatory = $true)] [string] $Path,
        [PSDefaultValue(Help = "./publish")] [string] $Output = './publish'
    )

    $Targets = @("win-x64", "linux-x64", "osx-x64")

    foreach ($Target in $Targets) {
        Write-Output "Start building: $Target"
        dotnet publish $Path -c Release --self-contained false -p:PublishSingleFile=true -r $Target -o $Output/$Target
        Write-Output "Finish building: $Target"
        Write-Output "---"
    }  
}

function Publish-Release {
    param (
        [Parameter(Mandatory = $true)] [string] $Tag,
        [PSDefaultValue(Help = "./publish")] [string] $Path = "./publish"
    )

    $PreRelease = $Tag -like 'v*-*' ? '-p' : ""
    Set-Location $Path

    $Assets = @()

    Get-ChildItem -Directory |
    ForEach-Object {
        $Target = $_.Name
        $Extension = ""
        if ($_ -like '*win-*') {
            $Extension = ".zip"
            Compress-Archive -Path $Target -DestinationPath $Target$Extension
        }
        else {
            $Extension = ".tar.gz"
            tar -zcvf $Target$Extension $Target
        }
        $Assets += "./$Target$Extension"
    }

    gh release create $Tag $Assets $PreRelease
}