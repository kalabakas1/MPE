param (
   [string]$directoryPath,
   [int32]$secDiffMax
)

$lastRunTime = (Get-ChildItem $directoryPath | sort LastWriteTime | Select -Last 1)
$timeDifference = New-TimeSpan -Start $lastRunTime -End (Get-Date)
$diffInSec = ([int32]($timeDifference.TotalSeconds))

if($secDiffMax -gt $diffInSec)
{
    $true
}
else
{
    $false
}

