param (
   [string]$directoryPath,
   [int32]$secDiffMax
)

$lastRunTime = (Get-ChildItem -File $directoryPath | sort LastWriteTime | Select -Last 1 | Select -ExpandProperty "LastWriteTime")
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