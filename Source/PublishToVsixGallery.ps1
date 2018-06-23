(new-object Net.WebClient).DownloadString("https://raw.github.com/madskristensen/ExtensionScripts/master/AppVeyor/vsix.ps1") | Invoke-Expression
Vsix-PublishToGallery