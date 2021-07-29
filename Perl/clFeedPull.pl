#!/usr/bin/perl -w
use strict;
use warnings;
use Cwd;
use Encode qw(decode encode);
use JSON;
use MIME::Base64 qw(encode_base64);
use REST::Client;

=head1 clFeedPull.pl

This is a sample perl script showing how to retrieve all data from a Cloverleaf feed and save it locally. 

Syntax:
perl clFeedPull.pl <EventCode> <FeedType> <clUserName> <clPassword> [<Since>]
    
This does not pull multiple feeds, obviously, and should not be used in a production environment as-is; it exists
merely to familiarize the user with the concepts required to pull data from Cloverleaf, and (if necessary) provide an
example of how one might do that via perl (if required).

The most up-to-date information on Cloverleaf endpoints, implementation, feeds, and examples can be found at https://api.cloverleaf.mge360.com/

EXAMPLES:

    ./clFeedPull.pl XXX000 FieldDetail yourUserName "yourPassword"

    Retrieves the FieldDetail feed for XXX000 and stores it locally in an auto-generated file


    ./clFeedPull.pl XXX000 FieldDetail yourUserName "yourPassword" 637068494249371789

    Prompts you for your password, then retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file for all records occuring
    after "Since" value "637068494249371789", which would have been returned from a previous pull.  In this way, you can do incremental feeds without
    back-tracking.
    
=cut

# Required CPAN installations (if not already present):
# JSON
# REST::Client

# parse arguments
scalar(@ARGV) < 4 and die("usage: $0 <EventCode> <FeedType> <clUserName> <clPassword> [<Since>]");
my $eventCode = shift(@ARGV);
my $feedType = shift(@ARGV);
my $clUserName = shift(@ARGV);
my $clPassword = shift(@ARGV);

my $sinceVal= undef;
if( scalar(@ARGV) == 6) {
    $sinceVal = shift(@ARGV);
}

# set environment (by url)
my $url = "https://api.cloverleaf.mge360.com";
#my $url = "https://api-qa.cloverleaf.mge360.com";

# get path for output file
(my $sec,my $min,my $hour,my $mday,my $mon,my $year,my $wday,my $yday,my $isdst) = localtime();
my $odir = getcwd() . "/feedRes";
if (-e $odir and -d $odir) {
    print "Feed output directory already exists\n";
} else {
    print "Creating feed output directory...\n";
    mkdir $odir;
}
my $out = $odir . "/" . $eventCode . "_" . $feedType . "_" . $year . $mon . $mday . $hour . $min . $sec . ".json";
print "Output file:\n$out\n";

# start loop
my $allRecordCount = 0;
my $nextSince = undef;

# create REST client
my $client = REST::Client->new();

# add auth header; compute basic value from passed username and password
my $joinedPW = encode_base64(encode('ASCII', "$clUserName:$clPassword"));
$client->addHeader('Authorization',"Basic $joinedPW");

# loop through data
my $pull = undef;
my $startEntry = 0;
my $allRecordCnt = 0;
my $entCnt = 0;

# open output file for write
open(my $fh, '>>:encoding(UTF-8)', $out) or die "Could not open file '$out' $!";
print $fh "[";

if($sinceVal && $sinceVal > 0) { $nextSince = $sinceVal; }
do
{
    my $url =  "$url/Feed$feedType?Event=$eventCode";
    if($nextSince) {
        $url = $url . "&Since=" . $nextSince;
    }
    print "$url\n";

    $pull = $client->GET($url)->responseContent();
    if($pull =~ /Authorization required to access this resource./) {
        die "AUTHORIZATION ERROR: '$pull'.  Please confirm your cloverleaf credentials are accurate and that you have access to the requested show!";
    }
    my $json = from_json($pull) or die "Cloverleaf did not return valid JSON!  Please ensure you have the correct endpoint specified.  Returned: $pull";

    my @entArr = @{$json->{Entities}};
    $entCnt= scalar(@entArr);
    my $remains = $json->{EstMoreRecords};
    if(!$remains) {
        $remains = 0;
    }
    print "Retrieved $entCnt records; $remains estimated remaining\n";

    $allRecordCnt += $entCnt;

    $nextSince = undef;
    $nextSince = $json->{NextSince};

    foreach my $ent(@entArr) {
        if(!$startEntry) {
            print $fh to_json($ent);
            $startEntry = 1;
        }
        else {
            print $fh "," . to_json($ent);
        }
        
    }
} 
while($entCnt > 0);

print $fh "]";
close $fh;

print "Feed complete! $allRecordCnt records retrieved.\n";