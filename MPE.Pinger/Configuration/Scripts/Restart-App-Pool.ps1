param (
   [string]$pool
)

Import-Module WebAdministration

if((Get-WebAppPoolState -Name $pool).Value -eq 'Stopped')
{
	Start-WebAppPool $pool
}
else
{
	Restart-WebAppPool $pool
}

Invoke-WebRequest $pool

Start-Sleep -Seconds