using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using System.Text;
using System.Web;

namespace CLFeedPull.NetCore
{
    public static class Program
    {
        #region ParseArgs

        /// <summary>
        /// Writes expected usage of sample app to console.
        /// 
        /// NOTE: if running this in debug mode via "dotnet run", actual expected usage would look like this:
        /// 
        /// dotnet run -- -EventCode <eventCode> -FeedType <feedtype> -CLUserName <yourUserName> [-CLPassword <yourPassword>] [-Since <sinceValue] [-OutFile <explicitFilePath>]
        /// </summary>
        private static void WriteExpectedUsage()
        {
            Trace.WriteLine("  Expected usage:");
            Trace.WriteLine("  CLFeedPull.exe -EventCode <eventCode> -FeedType <feedtype> -CLUserName <yourUserName> [-CLPassword <yourPassword>] [-Since <sinceValue] [-OutFile <explicitFilePath>]");
        }

        /// <summary>
        /// Parse provided command-line arguments into feed context, and does basic input validation.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        private static FeedContext ParseArgs(string[] args)
        {
            FeedContext ctx = new FeedContext();

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i].Equals("-EventCode", StringComparison.CurrentCultureIgnoreCase))
                    ctx.EventCode = args[++i];
                if (args[i].Equals("-FeedType", StringComparison.CurrentCultureIgnoreCase))
                    ctx.FeedType = args[++i];
                if (args[i].Equals("-CLUserName", StringComparison.CurrentCultureIgnoreCase))
                    ctx.CLUserName = args[++i];
                if (args[i].Equals("-CLPassword", StringComparison.CurrentCultureIgnoreCase))
                    ctx.CLPassword = args[++i];
                if (args[i].Equals("-Since", StringComparison.CurrentCultureIgnoreCase))
                    ctx.Since = args[++i];
                if (args[i].Equals("-OutFile", StringComparison.CurrentCultureIgnoreCase))
                    ctx.OutFile = args[++i];
            }

            // validate minimum required values for execution have been provided (minus password...we'll get to that in a minute)
            if (string.IsNullOrWhiteSpace(ctx.EventCode)
                || string.IsNullOrWhiteSpace(ctx.FeedType)
                || string.IsNullOrWhiteSpace(ctx.CLUserName))
            {
                Trace.WriteLine("EventCode, FeedType, CLUserName, and CLPassword are required!");
                WriteExpectedUsage();
                return null;
            }


            // if no password was provided on the command line, ask for one.
            if (ctx.CLPassword == null)
            {
                ctx.CLPassword = "";
                Console.Write("Enter your cloverleaf password: ");
                ConsoleKeyInfo key;
                do
                {
                    key = Console.ReadKey();

                    if (!char.IsControl(key.KeyChar))
                    {
                        ctx.CLPassword.Append(key.KeyChar);
                        Console.Write("*");
                    }
                    else
                    {
                        // handle backspace
                        if (key.Key == ConsoleKey.Backspace && ctx.CLPassword.Length > 0)
                        {
                            ctx.CLPassword.Remove(ctx.CLPassword.Length - 1);
                            Console.Write("\b \b");
                        }
                    }
                }
                while (key.Key != ConsoleKey.Enter);
            }

            // validate minimum required values for execution have been provided
            if (string.IsNullOrWhiteSpace(ctx.EventCode)
                || string.IsNullOrWhiteSpace(ctx.FeedType)
                || string.IsNullOrWhiteSpace(ctx.CLUserName)
                || ctx.CLPassword.Length == 0)
            {
                Trace.WriteLine("EventCode, FeedType, CLUserName, and CLPassword are required!");
                WriteExpectedUsage();
                return null;
            }

            // if no output file specified, create one
            if (string.IsNullOrWhiteSpace(ctx.OutFile))
            {
                string resDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "CLFeedRes");
                if (!Directory.Exists(resDir))
                    Directory.CreateDirectory(resDir);

                ctx.OutFile = Path.Combine(resDir, $"{ctx.EventCode}_{ctx.FeedType}_{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.json");
            }

            // verify folder exists for output file
            if (!Directory.Exists(Path.GetDirectoryName(ctx.OutFile)))
                Directory.CreateDirectory(Path.GetDirectoryName(ctx.OutFile));

            return ctx;
        }

        #endregion

        #region BSONFetch w/ shared HttpClient

        /// <summary>
        /// Root Uri for Cloverleaf API 
        /// </summary>
        private static readonly Uri CloverleafBaseURI = new Uri("https://api.experientdata.com/");

        /// <summary>
        /// Shared HttpClient instance for making REST calls to Cloverleaf API.  Per .net best practices, we reuse the 
        /// same one vs creating and disposing of them as required.
        /// </summary>
        private static System.Net.Http.HttpClient CLClient = null;

        /// <summary>
        /// Check for known cases where an "immediate" retry is a reasonable course of action
        /// </summary>
        /// <returns></returns>
        private static bool IsTimeoutOrIO(this Exception ex)
        {
            if (ex is TimeoutException)
                return true;
            if (ex is IOException)
                return true;
            if (ex is AggregateException)
                return ((AggregateException)ex).InnerExceptions.All(X => X.IsTimeoutOrIO());
            return ex.InnerException?.IsTimeoutOrIO() ?? false;
        }

        /// <summary>
        /// Process parameters into a valid Cloverleaf request querystring
        /// </summary>
        /// <param name="relURI"></param>
        /// <param name="queryParams"></param>
        /// <returns></returns>
        private static Uri GetFetchURI(string relURI, params string[] queryParams)
        {
            var Query = HttpUtility.ParseQueryString("?");
            queryParams = queryParams ?? new string[0];
            for (var I = 0; I < (queryParams.Length - 1); I += 2)
                Query[queryParams[I]] = queryParams[I + 1];
            Query["BSON"] = "1";

            return new Uri(CloverleafBaseURI, relURI + "?" + Query.ToString().TrimStart('?'));
        }

        /// <summary>
        /// Primary method for performing typed data retrieval from Cloverleaf in pages.
        /// </summary>
        /// <remarks>
        /// This uses the BSON flag by default, which instructs the Cloverleaf API to use BSON to transfer the data as opposed to JSON, as BSON 
        /// typically results in a smaller payload.  Each invocation of this method returns a single page, usually a typed FeedResponse wrapper, 
        /// although it can also be used for the FeedMeta and FeedEvent endpoints as desired.
        /// 
        /// We illustrate this method here to demonstrate how one may use the automatically-generated class definitions available at 
        /// https://api.experientdata.com/entityschema?fmt=cs, but it is not explicitly required that you use these wrappers if they do not fit your use
        /// cases; the FeedResponse could also be retooled to use a BsonArray in place of the typed "Entities" property, or one could omit the "bson" flag 
        /// entirely and process the entire resultset as json objects (see some of the other samples in this repository for examples of straight-json 
        /// implementations).
        /// </remarks>
        /// <typeparam name="T">Type of feedresponse to retrieve</typeparam>
        /// <param name="relURI">Relative feed uri</param>
        /// <param name="queryParams">Additional parameters, such as EventCode and Since values</param>
        /// <returns></returns>
        public static T BSONFetch<T>(string relURI, params string[] queryParams)
        {
            Uri myUrl = GetFetchURI(relURI, queryParams);

            // we retry on timeouts and certain I/O exceptions, because they typically indicate transient states of either our local system or the upstream service.
            const int MAX = 10;
            for (var Attempt = 1; Attempt <= MAX; Attempt++)
                try
                {
                    var resp = CLClient.GetAsync(myUrl).Result;
                    using (var MS = new MemoryStream())
                    {
                        resp.Content.ReadAsStreamAsync().Result.CopyTo(MS);
                        MS.Position = 0;
                        return BsonSerializer.Deserialize<T>(MS);
                    }
                }
                catch (Exception ex)
                {
                    if (!ex.IsTimeoutOrIO() || (Attempt == MAX))
                        throw;
                }

            throw new Exception("BSONFetch: retry timeouts loop exited.");
        }

        #endregion

        #region ProcessTypedFeed

        /// <summary>
        /// Process a typed feed, pulling all pages and saving them to a target datastore.  In our sample, we merely write the results to a 
        /// .json file.
        /// </summary>
        /// <typeparam name="T">Type of feed we're pulling</typeparam>
        /// <param name="ctx">the configured FeedContext, based on command-line arguments</param>
        private static void ProcessTypedFeed<T>(FeedContext ctx)
        {
            // initialize all records retrieved counter (for display purposes only)
            int allRecordsRetrieved = 0;
            bool started = false;

            // initialize nextSince value
            string nextSince = null;
            if (!string.IsNullOrWhiteSpace(ctx.Since))
                nextSince = ctx.Since;

            // create working feedresponse reference
            FeedResponse<T> feedRes = null;

            // create our output file
            using (FileStream fs = !File.Exists(ctx.OutFile) ? File.Create(ctx.OutFile) : File.OpenWrite(ctx.OutFile))
            using (StreamWriter sw = new StreamWriter(fs))
            {
                /* In order to save our sample output json file as something resembling valid json, write an open bracket here so we get an array afterwards.
                 * This *could* be done via constructing a full array of all feed results in memory first and then serializing in bulk, but that's typically
                 * not a great implementation for an open-ended feed scenario, as you could easily run out of memory before you collected all upstream entries
                 * in the feed.  Thus, we provide an admittedly-contrived example of how to do it incrementally.
                 * 
                 * It's worth noting that we'd do the same in an actual datastore (mssql, mongo, etc); it would just be a more meaningful save.
                 */

                sw.Write("[\n");

                /* Attempt to pull the next page of data while we're told there is more data to pull.  If entities are returned, save themm locally.
                 * We repeat the process until we are told that no more records are available at this time.  In a production/standard implementation, we
                 * would then preserve our final "nextSince" value in a local datastore, to be used as our initial "Since" starting point for the next time
                 * we attempt to run a sync for this event. 
                 */
                do
                {
                    feedRes = !string.IsNullOrWhiteSpace(nextSince)
                        ? Program.BSONFetch<FeedResponse<T>>($"Feed{ctx.FeedType}", "Event", ctx.EventCode, "Since", nextSince)
                        : Program.BSONFetch<FeedResponse<T>>($"Feed{ctx.FeedType}", "Event", ctx.EventCode);

                    Trace.WriteLine($"[{nextSince ?? "(beginning)"}] Retrieved {feedRes.Entities.Length} values; {feedRes.EstMoreRecords} estimated remaining.");
                    nextSince = feedRes.NextSince;
                    allRecordsRetrieved += feedRes.Entities.Length;

                    // write records
                    foreach (T ent in feedRes.Entities)
                    {
                        string toWrite = $"{(started ? "," : "")}{ent.ToJson(typeof(T), new MongoDB.Bson.IO.JsonWriterSettings() { Indent = false })}";
                        if (!started)
                            started = true;
                        sw.Write(toWrite);
                    }
                    // make sure this page is written to disk before we continue, in case something untoward happens.
                    sw.Flush();
                }
                while (feedRes != null && feedRes.EstMoreRecords > 0);

                // Finalize our json array, since we're done with our feed for now.
                sw.Write("\n]\n");
            }

            Console.WriteLine($"Feed complete! {allRecordsRetrieved} records retrieved.");
        }

        #endregion

        static int Main(string[] args)
        {
            return Utilities.ExecuteWrapped(() =>
            {
                Trace.WriteLine("Cloverleaf Sample Data Retrieval Client - C#/.NET Core");
                Trace.Flush();

                // gather information from the command line.  If we don't have a viable configuration, abort.
                FeedContext feedctx = ParseArgs(args);
                if (feedctx == null)
                {
                    Trace.WriteLine("Invalid parameters, aborting.");
                    return;
                }

                // initialize shared HttpClient (per .net best practices) and assign credentials for CL pull
                CLClient = new System.Net.Http.HttpClient();
                string clCreds = Convert.ToBase64String(System.Text.Encoding.ASCII.GetBytes($"{feedctx.CLUserName}:{feedctx.CLPassword}"));
                CLClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", clCreds);
                CLClient.Timeout = TimeSpan.FromMinutes(2);

                /* At this point, we iterate through the feed and performing CRUD operations against our local datastore.  To avoid overly complicating
                 * this sample, we are doing a simple pull (based on feed type) and writing it to a local json file in the user's data directory.  Obviously, this
                 * portion of the process serves only as an example, and you should modify the parsing and writing as befits your own datastore.
                 */

                switch (feedctx.FeedType)
                {
                    case "Booth":
                        ProcessTypedFeed<Booth>(feedctx);
                        break;
                    case "Company":
                        ProcessTypedFeed<Company>(feedctx);
                        break;
                    case "Facility":
                        ProcessTypedFeed<Facility>(feedctx);
                        break;
                    case "FieldDetail":
                        ProcessTypedFeed<FieldDetail>(feedctx);
                        break;
                    case "Map":
                        ProcessTypedFeed<Map>(feedctx);
                        break;
                    case "Person":
                        ProcessTypedFeed<Person>(feedctx);
                        break;
                    case "Product":
                        ProcessTypedFeed<Product>(feedctx);
                        break;
                    default:
                        throw new Exception($"Unrecognized feed type '{feedctx.FeedType}'!  Execution cannot continue!");
                }

                Trace.WriteLine("Process complete!  Output file:");
                Trace.WriteLine(feedctx.OutFile);
            });
        }
    }
}
