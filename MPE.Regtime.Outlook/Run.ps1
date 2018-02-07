Set-Location $PSScriptRoot
.\MPE.Regtime.Outlook.App.exe --Command "register" --Date ((Get-Date).AddDays(-1).ToString('yyyy-MM-dd'))