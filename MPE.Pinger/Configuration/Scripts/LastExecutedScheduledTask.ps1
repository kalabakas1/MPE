param (
   [string]$jobName,
   [int32]$secDiffMax
)

$lastRunTime = (Get-ScheduledTask -TaskName $jobName | Get-ScheduledTaskInfo | Select -ExpandProperty "LastRunTime")
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