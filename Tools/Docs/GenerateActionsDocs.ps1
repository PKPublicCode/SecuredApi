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

function GetGroupForProperty([string] $str) {
    $namespace = $str.Split(':')[1]
    $tmp = $namespace.Split('.')
    return $tmp[$tmp.Length - 3]
}

function GetSourcePath([string] $str) {
    return "$sourceBasePath$(GetGroupForType $str)/$(GetShortName $str).cs"
}

function EnsureObjectsCreated([string] $group, [string] $fullActionName) {
    if($null -eq $groupped[$group]) {
        $groupped[$group] = @{}
    }
    if($null -eq $groupped[$group][$fullActionName]) {
        $groupped[$group][$fullActionName] = @{}
    }
    if($null -eq $groupped[$group][$fullActionName].properties) {
        $groupped[$group][$fullActionName].properties = @()
    }
}

$docMap = @{}
$groupped = @{}

foreach ($member in $content.doc.members.member) {
    if ($member.name -clike "T:*") {
        $fullName = GetFullClassNameForType $member.name
        $group = GetGroupForType $member.name
        EnsureObjectsCreated $group $fullName
        $groupped[$group][$fullName].shortName = GetShortName $member.name
        $groupped[$group][$fullName].group = GetGroupForType $member.name
        $groupped[$group][$fullName].sourcePath = GetSourcePath $member.name
        $groupped[$group][$fullName].summary = NormilizeContent $member.summary
        $groupped[$group][$fullName].remarks = NormilizeContent $member.remarks
        $groupped[$group][$fullName].example = $member.example
        $groupped[$group][$fullName].returns = NormilizeContent $member.return
    }
    elseif ($member.name -clike "P:*") {
        $prop = @{
            shortName = GetShortName $member.name
            summary = NormilizeContent $member.summary
            default = NormilizeContent $member.value
        }
        $fullName = GetFullClassNameForProperty $member.name
        $group = GetGroupForProperty $member.name
        EnsureObjectsCreated $group $fullName
        $groupped[$group][$fullName].properties += $prop
    }
}


"# Actions" | Out-File $outputFile
"## Summary" | Out-File $outputFile -Append
foreach($group in $groupped.Keys) {
    $docMap = $groupped[$group]
    "### $($group)" | Out-File $outputFile -Append
    "|Type|Guard|Description|" | Out-File $outputFile -Append
    "|----|------|-----------|" | Out-File $outputFile -Append
    foreach($action in $docMap.Values){
        "|[$($action.shortName)](#$($action.shortName))|$($action.returns ? "Yes" : "No")|$($action.summary)|" | Out-File $outputFile -Append
    }
}

foreach($group in $groupped.Keys) {
    $docMap = $groupped[$group]
    "## $($group)" | Out-File $outputFile -Append
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
}
