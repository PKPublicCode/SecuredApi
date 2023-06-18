$Path = "./.."
$Files = "*.cs"

Get-ChildItem -Recurse $Path -Include $Files 
    | Where { -not $_.PSIsContainer } 
    | ForEach-Object {
        [string[]]$arrayFromFile = Get-Content $_
        [string[]]$result = @()
        [bool]$header = $true
        foreach($line in $arrayFromFile) {
            if($header) {
                # criterias specific for the header structure.
                if((-not $line.StartsWith("/*")) `
                    -and (-not $line.StartsWith(" *")) `
                    -and (-not $line.StartsWith(" */")) `
                    -and ($line.trim() -ne "")) {
                    $result += $line
                    $header = $false
                }
            }
            else {
                $result += $line
            }
        }
        Remove-Item $_
        $result | Set-Content $_
    }

#Clean doesn't correctly work on Mac. Need to manuall drop all build artifacts. 
#get-childitem -Include obj -Recurse -force | Where { $_.PSIsContainer } | Remove-Item -Force -Recurse