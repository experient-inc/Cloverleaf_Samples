
param 
(
    [Parameter(Mandatory=$true)]
    [string] $clUserName,
    [Parameter()]
    [string] $clPassword,
    [Parameter()]
    [string] $out
)

if($host.version.Major -lt 6) {
    throw "Powershell 6 or higher is required to run this script!  Execution cannot continue.  Powershell 6 is available here: https://aka.ms/pscore6 "
    return;
}

[SecureString] $pass = $null;
if(!$clPassWord) {
    $pass = Read-Host -AsSecureString -Prompt "Enter your cloverleaf password"
}
else {
    $pass = convertto-securestring -string $clPassword -AsPlainText -Force
}
$clCreds = New-Object -TypeName System.Management.Automation.PSCredential -ArgumentList $clUserName, $pass

if(!$out) {
    $out = (get-location).Path + "\feedRes\" + $EventCode + "_" + $FeedType + "_" + [DateTime]::Now.ToString("yyyyMMddHHmmssfff") + ".json";

    if(!(test-path ((get-location).Path + "\feedRes\"))) {
        $x = mkdir feedRes;
    }
}
"Output file: $out";

$url = ("https://api.cloverleaf.mge360.com/FeedEvent");

$pull = $null;
$pull = Invoke-RestMethod -uri $url -Method GET -Authentication Basic -Credential $clCreds -AllowUnencryptedAuthentication

$pull

#add-content -path $out -Value ("{0}" -f $pull.ToString())
