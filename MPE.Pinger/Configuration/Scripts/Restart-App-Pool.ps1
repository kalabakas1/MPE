param (
   [string]$pool
)

Import-Module WebAdministration

Try{
	Restart-WebAppPool $pool
}catch{
	Start-WebAppPool $pool
}