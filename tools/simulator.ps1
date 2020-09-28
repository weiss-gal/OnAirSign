 ### NOTE: You should only run this script from a powershell console
 ###       IE - don't try to run in from ISE or from VSCode, there will be trouble
 
 [System.IO.Ports.SerialPort]::getportnames()
 $portName = "COM10"

 $port= new-Object System.IO.Ports.SerialPort $portName,9600,None,8,one
 $port.NewLine = "`r"
 $port.close()
 $port.open()

#Error Reason codes
New-Variable -Name 'ERR_CODE_OK' -Value 0 -Option Constant
New-Variable -Name 'ERR_CODE_UNKNOWN_ARG' -Value 1 -Option Constant
New-Variable -Name 'ERR_CODE_MISSING_MANDATORY_OPTION' -Value 2 -Option Constant
New-Variable -Name 'ERR_CODE_INVALID_OPTION' -Value 3 -Option Constant

# parses state, if sucessfull returns a descriptive string such as:
#   "Audio playing=on, Audio capturing=off, Video capturing=off"
# if failes, returns null
function parseState{
    param(
        [String]$newState
    )

    if (!($newState -match '^([01])([01])([01])$')) {
        return $null
    }

    $writeThis = "after parsing state, Matches are " + $Matches.0 + " " + $Matches.1 + " " + $Matches.2 + " " + $Matches.3
    Write-Host $writeThis

    $audioPlayStr = "Audio playing=$(if ($Matches.1 -eq '0') {'off'} else {'on'})"
    $audioCaptureStr = ", Audio Capturing=$(if ($Matches.2 -eq '0') {'off'} else {'on'})"
    $videoCaptureStr = ", Video Capturing=$(if ($Matches.3 -eq '0') {'off'} else {'on'})"

    return $audioPlayStr + $audioCaptureStr + $videoCaptureStr

}

function handleCommandSetDisplay {
    param (
        [String]$commandArgs
    )
    Write-Host "Handling command 'SET_DISPLAY' with arguments: '$commandArgs'"

    $errReason = $ERR_CODE_OK
    $cmdId = "0"

    $options = $commandArgs.Split(" ")
    $optionsSet = @{}
    foreach($option in $options) {
        Write-Host "parsing option '$option'"
        if ($option -match '(\S+)=(\S+)') {
            $writeThis = "after parsing option, Matches are " + $Matches.0 + " " + $Matches.1 + " " + $Matches.2 + " " + $Matches.3
            Write-Host $writeThis
            $optionsSet.Add($Matches.1, $Matches.2)
        } else {
            # option without parameter (no '=' afterwards)
            $optionsSet.Add($option, "")
        }
    }

    if ($optionsSet.ContainsKey('cmdid')) {
        $cmdId = $optionsSet['cmdid']
    }

    if ($optionsSet.ContainsKey('state')) {
        $newStateStr = parseState $optionsSet['state']
        if ($null -eq $newStateStr){
            Write-Host 'Failed to parse state'
            $errReason = $ERR_CODE_INVALID_OPTION
        } else {
            Write-Host "Setting State to $newStateStr"
        }
        
    } else {
        $errReason = $ERR_CODE_MISSING_MANDATORY_OPTION
    }

    $retStatus = if ($errReason -eq $ERR_CODE_OK) {" status=ok"} else {" status=nok"}
    $retReason = if ($errReason -eq $ERR_CODE_OK) {""} else {" reason=$errReason"}
    
    return "RE SET_DISPLAY cmdId=$cmdId" + $retStatus + $retReason
}

function handleCommandHello {
    param (
        [String]$commandArgs
    )
    Write-Host "Handling command 'HELLO' with arguments: '$commandArgs'"


    $errReason = $ERR_CODE_OK
    $cmdId = "0"

    if ($commandArgs.Trim().Length -gt 0) {
        $argsLine = $commandArgs.Trim().Split(" ")
        $arg1Parts = $argsLine.Split("=")
        if ($arg1Parts[0].Equals("cmdid")) {
            $cmdId = $arg1Parts[1]
        } else {
            # unknown arg
            $errReason = $ERR_CODE_UNKNOWN_ARG
        }
    }
    
    $errReasonArg = if ($errReason -eq 0) {""} else {" reason=$errReason"}
    return "RE HELLO cmdid=$cmdId" + $errReasonArg
 }

 function handleCommand {
    param (
        [String]$commandLine
    )

    Write-Host "Starting to handle command: '$commandLine'"

    $parts = $commandLine.Split(" ")
    $response = switch($parts[0]) {
        "HELLO" {handleCommandHello (($parts[1..100]) -join " ")}
        "SET_DISPLAY" {handleCommandSetDisplay (($parts[1..100]) -join " ")}
        default {Write-Host "Unknown command: '$commandLine'"}
    }

    $port.WriteLine($response)
 }

 $port.WriteLine("Arduino led display simulator started at port $portName")
 
 $exit = $false
 $inputBuffer = ""

 While (!$exit) {
    if ($port.BytesToRead -gt 0) {
        $byte = $port.ReadByte()
        if ($byte -eq 13) { #end of line, process command
            handleCommand $inputBuffer
            $inputBuffer = ""
        } else {
            $inputBuffer = $inputBuffer + [char]$byte
        }
    }


    if ([Console]::KeyAvailable) {
        $exit = $true
    }
 }
 $port.close()
