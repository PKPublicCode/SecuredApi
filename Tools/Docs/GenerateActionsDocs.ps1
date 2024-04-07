$inputFile = "$PSScriptRoot/../../SecuredApi/Logic/Routing.Actions.Model/XmlDocs/Model.xml"
$outputFile = "$PSScriptRoot/../../Docs/Product/AllActions.md"
$sourceBasePath = "../../SecuredApi/Logic/Routing.Actions.Model/"

[xml]$content = Get-Content $inputFile

function NormilizeContent([string] $content) {
     #works!!!
        # $summary = $member.summary `
        #         -replace '(^ +)|( (?= ))|((?<=\n) +)', '' `
        #         -replace '\n(?!\n)', '' `
        #         -replace '\n +', '<br\>' `
        #         -replace '(^ +)', ''
        
        # $summary = $member.summary `
        #         -replace '(^\s+)|( (?= ))|((?<=\s) +)', ''

    return $content `
            -replace '(^\s+)|( (?= ))|((?<=\s) +)', '' `
            -replace '((?<!\n)\n(?!\n))', ' ' `
            -replace '\n\n', '<br>'
}

function NormilizeContentPreserveLineBreaks([string] $content) {
    #works!!!
       # $summary = $member.summary `
       #         -replace '(^ +)|( (?= ))|((?<=\n) +)', '' `
       #         -replace '\n(?!\n)', '' `
       #         -replace '\n +', '<br\>' `
       #         -replace '(^ +)', ''
       
       # $summary = $member.summary `
       #         -replace '(^\s+)|( (?= ))|((?<=\s) +)', ''

   return $content `
           -replace '(^\s+)|( (?= ))|((?<=\s) +)', ''
}

function GetShortName([string] $str) {
    $namespace = $str.Split(':')[1]
    $tmp = $namespace.Split('.')
    return $tmp[$tmp.Length - 1]
}

function GetFullClassName([string] $str, 
                            [int] $skip) {
    $namespace = $str.Split(':')[1]
    return ($namespace.Split('.') | Select-Object -SkipLast $skip) -join '.'
}

function GetFullClassNameForType([string] $str) {
    return GetFullClassName $str 0
}

function GetFullClassNameForProperty([string] $str) {
    return GetFullClassName $str 1
}

function GetGroupForType([string] $str) {
    $namespace = $str.Split(':')[1]
    $tmp = $namespace.Split('.')
    return $tmp[$tmp.Length - 2]
}

function GetSourcePath([string] $str) {
    return "$sourceBasePath$(GetGroupForType $str)/$(GetShortName $str).cs"
}

$docMap = @{}

foreach ($member in $content.doc.members.member) {
    if ($member.name -clike "T:*") {
        $fullName = GetFullClassNameForType $member.name
        $docMap[$fullName] = @{
            shortName = GetShortName $member.name
            group = GetGroupForType $member.name
            sourcePath = GetSourcePath $member.name
            summary = NormilizeContent $member.summary
            remarks = NormilizeContent $member.remarks
            example = $member.example
            returns = NormilizeContent $member.return
            properties = @()
        }
    }
    elseif ($member.name -clike "P:*") {
        $prop = @{
            shortName = GetShortName $member.name
            summary = NormilizeContent $member.summary
            default = NormilizeContent $member.default
        }
        $classFullName = GetFullClassNameForProperty $member.name
        $docMap[$classFullName].properties += $prop
    }
}

"# Actions" | Out-File $outputFile
"## Summary" | Out-File $outputFile -Append
"|Type|Gruard|Description|" | Out-File $outputFile -Append
"|----|------|-----------|" | Out-File $outputFile -Append
foreach($action in $docMap.Values){
    "|[$($action.shortName)]($($action.sourcePath))|$($action.returns ? "Yes" : "No")|$($action.summary)|" | Out-File $outputFile -Append
}

"## Details" | Out-File $outputFile -Append
foreach($action in $docMap.Values){
    "### [$($action.shortName)]($($action.sourcePath))" | Out-File $outputFile -Append
    "#### Summary" | Out-File $outputFile -Append
    $action.summary | Out-File $outputFile -Append
    if ($action.remarks) {
        "#### Remarks" | Out-File $outputFile -Append
        $action.remarks | Out-File $outputFile -Append
    }

    "#### Parameters" | Out-File $outputFile -Append
    "|Name|Optional|Default Value|Description|" | Out-File $outputFile -Append
    "|----|--------|-------------|-----------|" | Out-File $outputFile -Append
    foreach($prop in $action.properties){
        "|$($prop.shortName)|$($prop.default ? "Yes" : "No")|$($prop.default ? $prop.default : '')|$($prop.summary)|" | Out-File $outputFile -Append
    }

    if ($action.returns) {
        "#### Return" | Out-File $outputFile -Append
        $action.returns | Out-File $outputFile -Append
    }
    if ($action.example) {
        "#### Example" | Out-File $outputFile -Append
        "``````jsonc" | Out-File $outputFile -Append
        $action.example | Out-File $outputFile -Append
        "``````" | Out-File $outputFile -Append
    }
}