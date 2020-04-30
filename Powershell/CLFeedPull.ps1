 <#
.SYNOPSIS
    Retrieves a typed feed from Cloverleaf and writes it to a local file. *** WARNING: Requires Powershell 6 or higher!

.DESCRIPTION
    This is a sample Powershell 6 script showing how to retrieve all data from a Cloverleaf feed and save it locally. 
    
    This does not pull multiple feeds, obviously, and should not be used in a production environment as-is; it exists
    merely to familiarize the user with the concepts required to pull data from Cloverleaf, and (if necessary) provide an
    example of how one might do that via powershell.

    The most up-to-date information on Cloverleaf endpoints, implementation, feeds, and examples can be found at https://api.experientdata.com/

.PARAMETER EventCode
	Experient Event Code for the event in question, typically of the form XXX000
.PARAMETER FeedType
    The type of feed to retrieve.  Acceptable FeedType values are:
        Booth
        Company
        Facility
        FieldDetail
        Map
        Person
        Product
    See https://api.experientdata.com/ for more documentation and a full list
.PARAMETER clUserName
    Your Experient-assigned Cloverleaf account's username
.PARAMETER clPassWord
    Your Experient-assigned Cloverleaf account's password
.PARAMETER Since
    Optional.  Starting "Since" value for the feed pull.  If unspecified, you start from the beginning of the feed.
.PARAMETER out
    Optional.  Path to the file to write the stream output to.  If unspecified, one is created in the current working directory.

.LINK
    https://api.experientdata.com/
.LINK
    https://docs.microsoft.com/en-us/powershell/scripting/install/installing-powershell-core-on-windows?view=powershell-6
.LINK
    https://github.com/PowerShell/PowerShell/releases

.EXAMPLE
    .\CLFeedPull.ps1 -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName

    Prompts you for your password, then retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file

.EXAMPLE
    .\CLFeedPull.ps1 -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -Since 637068494249371789

    Prompts you for your password, then retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file for all records occuring
    after -Since value "637068494249371789", which would have been returned from a previous pull.  In this way, you can do incremental feeds without
    back-tracking.

.EXAMPLE
    .\CLFeedPull.ps1 -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -clPassword yourPassword

    Retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file

.EXAMPLE
    .\CLFeedPull.ps1 -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -clPassword yourPassword -out .\myfile.txt

    Retrieves the FieldDetial feed for XXX000 and stores it in the current working directory as "myfile.txt"
#>
param 
(
    [Parameter(Mandatory=$true)]
    [string] $EventCode,
    [Parameter(Mandatory=$true)]
    [string] $FeedType,
    [Parameter(Mandatory=$true)]
    [string] $clUserName,
    [Parameter()]
    [string] $clPassword,
    [Parameter()]
    [long] $Since,
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

$allRecordCnt = 0;
$nextSince = $null;
$written = $false;
if($Since -and $Since -gt 0) { $nextSince = $Since }
do
{
    $url = ("https://api.experientdata.com/Feed{0}?Event={1}" -f $FeedType, $EventCode);
    if($nextSince -ne $null) {
        $url = $url + "&Since=" + $nextSince;
    }
    "$url"

    $pull = $null;
    $pull = Invoke-RestMethod -uri $url -Method GET -Authentication Basic -Credential $clCreds
    "Retrieved {0} records; {1} estimated remaining" -f $pull.Entities.Count, $pull.EstMoreRecords

    $allRecordCnt += $pull.Entities.Count;

    $nextSince = $null;
    $nextSince = $pull.NextSince;

    $pull.Entities | foreach-object { 
        $ent = ConvertTo-Json $_;

        if(!$written) {
            add-content -path $out -Value ("[{0}" -f $ent.ToString());
            $written = $true;
        }
        else {
            add-content -path $out -Value (",{0}" -f $ent.ToString())
        }
    }
}
while($pull.Entities.Count -gt 0)

add-content -path $out -Value "]";

"Feed complete!  {0} total records retrieved" -f $allRecordCnt;