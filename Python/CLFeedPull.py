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
These examples assume Python3 is your default python interpreter.  If your system default
Python version is lower than 3, run with python3

EXAMPLE
    python CLFeedPull.py -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName
    
    Prompts you for your password, then retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file

EXAMPLE
    python CLFeedPull.py -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -Since 637068494249371789

    Prompts you for your password, then retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file for all records occuring
    after -Since value "637068494249371789", which would have been returned from a previous pull.  In this way, you can do incremental feeds without
    back-tracking.

EXAMPLE
    python CLFeedPull.py -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -clPassword yourPassword

    Retrieves the FieldDetial feed for XXX000 and stores it locally in an auto-generated file

EXAMPLE
    python CLFeedPull.py -EventCode XXX000 -FeedType FieldDetail -clUserName yourUserName -clPassword yourPassword -out myfile.txt

    Retrieves the FieldDetial feed for XXX000 and stores it in the current working directory as "myfile.txt"
    """

def main(*args):
    # This is dumb
    args = args[0]
    # Collect command line arguments and assign
    if "-EventCode" in args:
        EventCode = args[args.index("-EventCode") + 1]
    else:
        EventCode = input("Please provide EventCode: ")

    if "-FeedType" in args:
        FeedType = args[args.index("-FeedType") + 1]
    else:
        FeedType = input("Please provide FeedType: ")

    if "-clUserName" in args:
        clUserName = args[args.index("-clUserName") + 1]
    else:
        clUserName = input("Please provide Cloverleaf UserName: ")
        
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
        out = Path(args[args.index("-out") + 1])
    # Handle default output document
    else:
        os.mkdir(Path(os.getcwd() +"/FeedRes"))
        out = Path(os.getcwd() + "/FeedRes/{Event}_{Feed}_{Date}.txt".format(
            Event=EventCode,
            Feed=FeedType,
            Date=dt.now().strftime("%Y%m%d%H%M%S%f")
            )
                   )

    print("Output file: ", out)

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

            s = ""
            if out.exists():
                with open(out, "r") as in_file:
                    s = in_file.readlines()

            for entity in response.json()["Entities"]:
                s += json.dumps(entity, indent=4, sort_keys=True)

            with open(out, "w") as out_file:
                out_file.writelines(s)
               

    print("Feed complete! {a} total records retrieved".format(a=allRecordCnt))
    return 0

if __name__ == "__main__":
    main(sys.argv)
