"""Basic Python requests example using Basic Authentication"""

import os
import sys
import json
import requests
from requests.auth import HTTPBasicAuth
from pathlib import Path
from getpass import getpass
from datetime import datetime as dt

# the requests library handles REST methods: GET, POST, PUT, DELETE
# Conventionally, assign to variable 'response'

"""
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
    """

def main(*args):
    # This is dumb
    args = args[0]
    # Collect command line arguments and assign
    try:
        EventCode = args[args.index("-EventCode") + 1]
        FeedType = args[args.index("-FeedType") + 1]
        clUserName = args[args.index("-clUserName") + 1]
    except ValueError:
        print("You did not suppl one or more of EventCode, FeedType, and/or UserName.")
        print("Rerun command supplying these arguments")
        sys.exit()
    if "-clPassword" in args:
        clPassword = args[args.index("-clPassword") + 1]
    # Handle no password provided
    else:
        clPassword = getpass("Enter your cloverleaf password: ")
    
    # Create authentication package for request
    auth = HTTPBasicAuth(clUserName, clPassword)
    
    nextSince = None
    if "-Since" in args:
        since = int(args[args.index("-Since") + 1])
        if since > 0:
            nextSince = since

    if "-out" in args:
        out = args[args.index("-out") + 1]
    # Handle default output document
    else:
        out = Path(os.getcwd() + "/{Event}_{Feed}_{Date}.txt".format(
            Event=EventCode,
            Feed=FeedType,
            Date=dt.now().strftime("%Y%m%d%H%M%S%f")
            )
                   )

    allRecordCnt = 0
    EstMoreRecords = 1

    while EstMoreRecords > 0:
        url = "https://api.experientdata.com/Feed{feed}?Event={event}".format(
            feed=FeedType,
            event=EventCode
        )
        if nextSince:
            url += "&Since={since}".format(since=nextSince)

        print(url)
            
        response = requests.get(url, auth=auth)

        if response.status_code == 200:
            try:
                EstMoreRecords = response.json()["EstMoreRecords"]
            
            except KeyError:
                EstMoreRecords = 0

            print("Retrieved {a} records; {b} estimated remaining".format(
                a=len(response.json()["Entities"]),
                b=EstMoreRecords
                ))
        
            allRecordCnt += len(response.json()["Entities"])

            try:
                nextSince = response.json()["NextSince"]
            except KeyError:
                nextSince = None

            with open(out, "a") as out_file:
                for entity in response.json()["Entities"]:
                    json.dump(entity, out_file)
               

    print("Feed complete! {a} total records retrieved".format(a=allRecordCnt))
    return 0

if __name__ == "__main__":
    main(sys.argv)
