$inputFile = "$PSScriptRoot/../../SecuredApi/Logic/Routing.Model/Model.xml"
$actionsOutputFile = "$PSScriptRoot/../../Docs/Product/Actions.md"
$variablesOutputFile = "$PSScriptRoot/../../Docs/Product/RuntimeVariables.md"
$actionsSourceBasePath = "../../SecuredApi/Logic/Routing.Model/Actions"

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
    return "$($tmp[$tmp.Length - 3]).$($tmp[$tmp.Length - 2])"
}

function GetGroupForProperty([string] $str) {
    $namespace = $str.Split(':')[1]
    $tmp = $namespace.Split('.')
    return "$($tmp[$tmp.Length - 4]).$($tmp[$tmp.Length - 3])"
}

function GetSourcePath([string] $str) {
    return "$actionsSourceBasePath$(GetGroupForType $str)/$(GetShortName $str).cs"
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
    if($null -eq $groupped[$group][$fullActionName].fields) {
        $groupped[$group][$fullActionName].fields = @()
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
    elseif ($member.name -clike "F:*") {
        $field = @{
            shortName = GetShortName $member.name
            summary = NormilizeContent $member.summary
            value = NormilizeContent $member.value
        }
        $fullName = GetFullClassNameForProperty $member.name
        $group = GetGroupForProperty $member.name
        EnsureObjectsCreated $group $fullName
        $groupped[$group][$fullName].fields += $field
    }
}

$grouppedActions = @{}
foreach($group in $groupped.Keys) {
    if($group -like 'Actions.*') {
        $shortGroup = $group.Split('.')[1]
        $grouppedActions[$shortGroup] = $groupped[$group]
    }
}


"# Actions" | Out-File $actionsOutputFile
"## Summary" | Out-File $actionsOutputFile -Append
foreach($group in $grouppedActions.Keys) {
    $docMap = $grouppedActions[$group]
    "### $($group)" | Out-File $actionsOutputFile -Append
    "|Type|Fallible|Description|" | Out-File $actionsOutputFile -Append
    "|----|------|-----------|" | Out-File $actionsOutputFile -Append
    foreach($action in $docMap.Values){
        "|[$($action.shortName)](#$($action.shortName))|$($action.returns ? "Yes" : "No")|$($action.summary)|" | Out-File $actionsOutputFile -Append
    }
}

foreach($group in $grouppedActions.Keys) {
    $docMap = $grouppedActions[$group]
    "## $($group)" | Out-File $actionsOutputFile -Append
    foreach($action in $docMap.Values){
        "### [$($action.shortName)]($($action.sourcePath))" | Out-File $actionsOutputFile -Append
        "#### Summary" | Out-File $actionsOutputFile -Append
        $action.summary | Out-File $actionsOutputFile -Append
        if ($action.remarks) {
            "#### Remarks" | Out-File $actionsOutputFile -Append
            $action.remarks | Out-File $actionsOutputFile -Append
        }
        
        "#### Parameters" | Out-File $actionsOutputFile -Append
        if ($action.properties) {
            "|Name|Optional|Default Value|Description|" | Out-File $actionsOutputFile -Append
            "|----|--------|-------------|-----------|" | Out-File $actionsOutputFile -Append
            foreach($prop in $action.properties){
                "|$($prop.shortName)|$($prop.default ? "Yes" : "No")|$($prop.default ? $prop.default : '')|$($prop.summary)|" | Out-File $actionsOutputFile -Append
            }
        }
        else {
            "No parameters" | Out-File $actionsOutputFile -Append
        }
    
        if ($action.returns) {
            "#### Return" | Out-File $actionsOutputFile -Append
            $action.returns | Out-File $actionsOutputFile -Append
        }
        if ($action.example) {
            "#### Example" | Out-File $actionsOutputFile -Append
            "``````jsonc" | Out-File $actionsOutputFile -Append
            $action.example | Out-File $actionsOutputFile -Append
            "``````" | Out-File $actionsOutputFile -Append
        }
    }
}


$grouppedVariables = @{}
foreach($group in $groupped.Keys) {
    if($group -like 'Model.RuntimeVariables') {
        foreach($var in $groupped[$group].Keys) { # only one in each group
            if (-not $groupped[$group][$var].exclude){
                $tmp = $var.Split('.');
                $shortGroup = $tmp[$tmp.Length - 1]
                if ($null -eq $grouppedVariables[$shortGroup]) {
                    $grouppedVariables[$shortGroup] = @{}
                }
                $grouppedVariables[$shortGroup] = $groupped[$group][$var]
                #$shortGroup
            }
        }
    }
}

"#Runtime Variables" | Out-File $variablesOutputFile
foreach($group in $grouppedVariables.Keys) {
    $docMap = $grouppedVariables[$group]
    "## $($group)" | Out-File $variablesOutputFile -Append
    "|Name|Description|" | Out-File $variablesOutputFile -Append
    "|----|-----------|" | Out-File $variablesOutputFile -Append
    foreach($var in $docMap.fields){ 
        "|$($var.value)|$($var.summary)|" | Out-File $variablesOutputFile -Append
    }
}


