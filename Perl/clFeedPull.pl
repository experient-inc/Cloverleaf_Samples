#!/usr/bin/perl -w
use strict;
use warnings;
use Cwd;
use Encode qw(decode encode);
use JSON;
use MIME::Base64 qw(encode_base64);
use REST::Client;

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

# get path for output file
(my $sec,my $min,my $hour,my $mday,my $mon,my $year,my $wday,my $yday,my $isdst) = localtime();
my $out = getcwd() . "/" . $eventCode . "_" . $feedType . "_" . $year . $mon . $mday . $hour . $min . $sec . ".txt";
print "Output file:\n$out\n";

# start loop
my $allRecordCount = 0;
my $nextSince = undef;

# create REST client
my $client = REST::Client->new();

# add auth header; compute basic value from passed username and password
my $joinedPW = encode_base64(encode('ASCII', "$clUserName:$clPassword"));
$client->addHeader('Authorization',"Basic $joinedPW");

print "$joinedPW\n";

# loop through data
my $pull = undef;
if($sinceVal && $sinceVal > 0) { $nextSince = $sinceVal; }
# do
# {
    my $url =  "https://api.experientdata.com/Feed$feedType?Event=$eventCode";
    if($nextSince) {
        $url += "&Since=" . $nextSince;
    }
    print "$url\n";

    $pull = $client->GET($url)->responseContent();
    print "$pull\n";
# } 
# while()