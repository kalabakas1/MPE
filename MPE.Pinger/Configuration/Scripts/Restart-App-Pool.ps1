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