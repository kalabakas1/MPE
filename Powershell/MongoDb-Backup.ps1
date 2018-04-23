$date = (Get-Date).ToString("yy-MM-dd_HHmm")
$backupFolder = $date;
$basePath = "C:\Servers\MongoDB-staging\db";
$destinationPath = Join-Path "C:\Servers\MongoDB-staging\backup" $backupFolder;

if(!(Test-Path -Path $destinationPath)) {
    New-Item -ItemType directory -Path $destinationPath;
    & "C:\Servers\MongoDB-staging\Server\3.6\bin\mongodump.exe" --out $destinationPath
}