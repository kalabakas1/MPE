param 
( 
  [string]$sourceIp, 
  [string]$sourceFile,
  [string]$usernameWithDomain,
  [string]$password,
  [string]$targetPath
)

net use \\$sourceIp /USER:$usernameWithDomain $password
xcopy \\$sourceIp\$sourceFile $targetPath

# .\Copy-File-From-Server.ps1 -sourceIp XXX -sourceFile "D$\XXX" -usernameWithDomain XXX -password XXX -targetPath .