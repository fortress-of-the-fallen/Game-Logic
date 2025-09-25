# list_files.ps1
Get-ChildItem -Recurse | Where-Object { $_.FullName -notmatch '\\(bin|obj)\\' }
