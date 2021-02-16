Param([string] $publish, [string] $tag)

echo "publish = $publish"
echo "tag = $tag"

$pre = $tag -like 'v*-*' ? '-p' : ""
Set-Location $publish

$assets = @()

Get-ChildItem -Directory |
ForEach-Object {
    $target = $_.Name
    $ext = ""
    if ($_ -like '*win-*') {
        $ext = ".zip"
        Compress-Archive -Path $target -DestinationPath $target$ext
    }
    else {
        $ext = ".tar.gz"
        tar -zcvf $target$ext $target
    }
    $assets += "./$target$ext"
}

gh release create $tag $assets $pre