using System;
using System.Collections.Generic;
using System.Security;
using System.Text;

namespace CLFeedPull.NetCore
{
    public class FeedContext
    {
        

        /// <summary>
        /// Experient Event Code for the event in question, typically of the form XXX000
        /// </summary>
        public string EventCode { get; set; }

        /// <summary>
        /// The type of feed to retrieve.  Acceptable FeedType values are:
        /// Booth
        /// Company
        /// Facility
        /// FieldDetail
        /// Map
        /// Person
        /// Product
        /// See https://api.experientdata.com/ for more documentation and a full list
        /// </summary>
        public string FeedType { get; set; }

        /// <summary>
        /// Your Experient-assigned Cloverleaf account username
        /// </summary>
        public string CLUserName { get; set; }

        /// <summary>
        /// Your Experient-assigned Cloverleaf account password.  If not specified on the command line, you will be prompted to enter it.
        /// </summary>
        public string CLPassword { get; set; }

        /// <summary>
        /// Optional.  Starting "Since" value for the feed pull.  If unspecified, you start from the beginning of the feed.
        /// </summary>
        public string Since { get; set; }

        /// <summary>
        /// Optional.  Path to the file to write the stream output to.  If unspecified, one is created in the current working directory.
        /// </summary>
        public string OutFile { get; set; }
    }
}
