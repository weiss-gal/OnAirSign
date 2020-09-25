 ### NOTE: You should only run this script from a powershell console
 ###       IE - don't try to run in from ISE or from VSCode, there will be trouble
 
 [System.IO.Ports.SerialPort]::getportnames()
 $portName = "COM10"

 $port= new-Object System.IO.Ports.SerialPort $portName,9600,None,8,one
 $port.NewLine = "\r"
 $port.close()
 $port.open()

 #Error Reason codes
 New-Variable -Name 'ERR_CODE_OK' -Value 0 -Option Constant
 New-Variable -Name 'ERR_CODE_UNKNOWN_ARG' -Value 1 -Option Constant


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
