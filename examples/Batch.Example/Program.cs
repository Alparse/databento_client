using Databento.Client.Builders;
using Databento.Client.Models;
using Databento.Client.Models.Batch;

Console.WriteLine("=== Databento Batch Downloads Example ===");
Console.WriteLine();
Console.WriteLine("This example demonstrates how to submit and manage batch download jobs.");
Console.WriteLine("Batch downloads are ideal for large historical data requests.");
Console.WriteLine();
Console.WriteLine("⚠️  WARNING: Batch job submissions will incur costs!");
Console.WriteLine("⚠️  This example will NOT submit actual jobs to avoid unexpected charges.");
Console.WriteLine("⚠️  To submit real jobs, set DATABENTO_SUBMIT_BATCH_JOBS=true");
Console.WriteLine();

var apiKey = Environment.GetEnvironmentVariable("DATABENTO_API_KEY")
    ?? throw new InvalidOperationException(
        "DATABENTO_API_KEY environment variable is not set. " +
        "Set it with your API key to authenticate.");

var submitJobs = Environment.GetEnvironmentVariable("DATABENTO_SUBMIT_BATCH_JOBS")?.ToLowerInvariant() == "true";

var client = new HistoricalClientBuilder()
    .WithApiKey(apiKey)
    .Build();

Console.WriteLine("✅ Successfully created Historical client");
Console.WriteLine();

// ============================================================================
// Example 1: List Existing Batch Jobs
// ============================================================================

Console.WriteLine("Example 1: List Existing Batch Jobs");
Console.WriteLine("------------------------------------");
Console.WriteLine("View all batch jobs you've submitted.");
Console.WriteLine();

try
{
    var jobs = await client.BatchListJobsAsync();

    if (jobs.Count == 0)
    {
        Console.WriteLine("No batch jobs found.");
        Console.WriteLine("Batch jobs you submit will appear here.");
    }
    else
    {
        Console.WriteLine($"Found {jobs.Count} batch job(s):");
        Console.WriteLine();

        foreach (var job in jobs.Take(5))
        {
            Console.WriteLine($"  Job ID: {job.Id}");
            Console.WriteLine($"    Dataset: {job.Dataset}");
            Console.WriteLine($"    Symbols: {string.Join(", ", job.Symbols)}");
            Console.WriteLine($"    Schema: {job.Schema}");
            Console.WriteLine($"    Date Range: {job.Start} to {job.End}");
            Console.WriteLine($"    State: {job.State}");
            Console.WriteLine($"    Cost: ${job.CostUsd:F2}");
            Console.WriteLine($"    Records: {job.RecordCount:N0}");
            Console.WriteLine($"    Received: {job.TsReceived}");

            if (!string.IsNullOrEmpty(job.TsProcessDone))
            {
                Console.WriteLine($"    Completed: {job.TsProcessDone}");
            }

            if (!string.IsNullOrEmpty(job.TsExpiration))
            {
                Console.WriteLine($"    Expires: {job.TsExpiration}");
            }

            Console.WriteLine();
        }

        if (jobs.Count > 5)
        {
            Console.WriteLine($"  ... and {jobs.Count - 5} more job(s)");
            Console.WriteLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing batch jobs: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 2: List Batch Jobs with Filtering
// ============================================================================

Console.WriteLine("Example 2: List Batch Jobs with Filtering");
Console.WriteLine("------------------------------------------");
Console.WriteLine("Filter batch jobs by state and date range.");
Console.WriteLine();

try
{
    // Get only completed jobs from the last 30 days
    var since = DateTimeOffset.UtcNow.AddDays(-30);
    var states = new[] { JobState.Done };

    var filteredJobs = await client.BatchListJobsAsync(states, since);

    Console.WriteLine($"Filter: State=Done, Since={since:yyyy-MM-dd}");
    Console.WriteLine($"Found {filteredJobs.Count} completed job(s) in the last 30 days");

    if (filteredJobs.Count > 0)
    {
        Console.WriteLine();
        Console.WriteLine("Most recent completed jobs:");
        foreach (var job in filteredJobs.Take(3))
        {
            Console.WriteLine($"  {job.Id}: {job.Dataset} - {string.Join(", ", job.Symbols)} ({job.Start})");
        }
    }

    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing filtered batch jobs: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 3: Submit a Simple Batch Job (Demo)
// ============================================================================

Console.WriteLine("Example 3: Submit a Simple Batch Job");
Console.WriteLine("-------------------------------------");
Console.WriteLine("Submit a batch download request with minimal parameters.");
Console.WriteLine();

if (!submitJobs)
{
    Console.WriteLine("⚠️  Job submission is DISABLED (demo mode)");
    Console.WriteLine("⚠️  Set DATABENTO_SUBMIT_BATCH_JOBS=true to submit real jobs");
    Console.WriteLine();
    Console.WriteLine("Demo parameters:");
    Console.WriteLine("  Dataset: EQUS.MINI");
    Console.WriteLine("  Symbols: NVDA");
    Console.WriteLine("  Schema: Trades");
    Console.WriteLine("  Date Range: 2024-01-02 12:00 to 2024-01-02 13:00 (1 hour)");
    Console.WriteLine();
    Console.WriteLine("This request would create a batch job to download all trades for NVDA");
    Console.WriteLine("during market hours on January 2, 2024.");
    Console.WriteLine();
}
else
{
    try
    {
        string dataset = "EQUS.MINI";
        string[] symbols = ["NVDA"];
        var startTime = new DateTimeOffset(2024, 1, 2, 12, 0, 0, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2024, 1, 2, 13, 0, 0, TimeSpan.Zero);

        Console.WriteLine($"Submitting batch job:");
        Console.WriteLine($"  Dataset: {dataset}");
        Console.WriteLine($"  Symbols: {string.Join(", ", symbols)}");
        Console.WriteLine($"  Schema: Trades");
        Console.WriteLine($"  Date Range: {startTime:yyyy-MM-dd HH:mm} to {endTime:yyyy-MM-dd HH:mm}");
        Console.WriteLine();

        var job = await client.BatchSubmitJobAsync(
            dataset,
            symbols,
            Schema.Trades,
            startTime,
            endTime);

        Console.WriteLine($"✅ Batch job submitted successfully!");
        Console.WriteLine($"  Job ID: {job.Id}");
        Console.WriteLine($"  Cost: ${job.CostUsd:F2}");
        Console.WriteLine($"  State: {job.State}");
        Console.WriteLine($"  Records: {job.RecordCount:N0}");
        Console.WriteLine();
        Console.WriteLine("The job will be processed in the background.");
        Console.WriteLine("You can monitor its status in Example 5.");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error submitting batch job: {ex.Message}");
        Console.WriteLine();
    }
}

// ============================================================================
// Example 4: Submit Batch Job with Advanced Options (Demo)
// ============================================================================

Console.WriteLine("Example 4: Submit Batch Job with Advanced Options");
Console.WriteLine("--------------------------------------------------");
Console.WriteLine("Customize encoding, compression, splitting, and output format.");
Console.WriteLine();

if (!submitJobs)
{
    Console.WriteLine("⚠️  Job submission is DISABLED (demo mode)");
    Console.WriteLine();
    Console.WriteLine("Demo parameters:");
    Console.WriteLine("  Dataset: EQUS.MINI");
    Console.WriteLine("  Symbols: NVDA, AAPL, MSFT");
    Console.WriteLine("  Schema: Ohlcv1M (1-minute bars)");
    Console.WriteLine("  Date Range: 2024-01-02 to 2024-01-03");
    Console.WriteLine("  Encoding: CSV");
    Console.WriteLine("  Compression: Zstd (fastest)");
    Console.WriteLine("  Pretty Prices: Yes (human-readable decimals)");
    Console.WriteLine("  Pretty Timestamps: Yes (ISO 8601 format)");
    Console.WriteLine("  Map Symbols: Yes (include symbol names)");
    Console.WriteLine("  Split by Symbol: Yes (one file per symbol)");
    Console.WriteLine("  Split Duration: Day (one file per day)");
    Console.WriteLine();
    Console.WriteLine("This would create separate CSV files for each symbol and day,");
    Console.WriteLine("with human-readable prices and timestamps.");
    Console.WriteLine();
}
else
{
    try
    {
        string dataset = "EQUS.MINI";
        string[] symbols = ["NVDA", "AAPL", "MSFT"];
        var startTime = new DateTimeOffset(2024, 1, 2, 0, 0, 0, TimeSpan.Zero);
        var endTime = new DateTimeOffset(2024, 1, 3, 0, 0, 0, TimeSpan.Zero);

        Console.WriteLine($"Submitting advanced batch job:");
        Console.WriteLine($"  Dataset: {dataset}");
        Console.WriteLine($"  Symbols: {string.Join(", ", symbols)}");
        Console.WriteLine($"  Schema: Ohlcv1M");
        Console.WriteLine($"  Encoding: CSV with pretty formatting");
        Console.WriteLine($"  Split by symbol and day");
        Console.WriteLine();

        var job = await client.BatchSubmitJobAsync(
            dataset: dataset,
            symbols: symbols,
            schema: Schema.Ohlcv1M,
            startTime: startTime,
            endTime: endTime,
            encoding: Encoding.Csv,
            compression: Compression.Zstd,
            prettyPx: true,
            prettyTs: true,
            mapSymbols: true,
            splitSymbols: true,
            splitDuration: SplitDuration.Day,
            splitSize: 0, // No size-based splitting
            delivery: Delivery.Download,
            stypeIn: SType.RawSymbol,
            stypeOut: SType.InstrumentId,
            limit: 0); // No limit

        Console.WriteLine($"✅ Advanced batch job submitted successfully!");
        Console.WriteLine($"  Job ID: {job.Id}");
        Console.WriteLine($"  Cost: ${job.CostUsd:F2}");
        Console.WriteLine($"  State: {job.State}");
        Console.WriteLine($"  Split Symbols: {job.SplitSymbols}");
        Console.WriteLine($"  Split Duration: {job.SplitDuration}");
        Console.WriteLine();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error submitting advanced batch job: {ex.Message}");
        Console.WriteLine();
    }
}

// ============================================================================
// Example 5: Monitor Job Status
// ============================================================================

Console.WriteLine("Example 5: Monitor Job Status");
Console.WriteLine("------------------------------");
Console.WriteLine("Check the status of your batch jobs to know when they're ready.");
Console.WriteLine();

try
{
    // Get recent jobs
    var recentJobs = await client.BatchListJobsAsync();

    if (recentJobs.Count == 0)
    {
        Console.WriteLine("No jobs to monitor.");
        Console.WriteLine("Submit a job first (see Examples 3-4).");
    }
    else
    {
        // Show status of most recent job
        var latestJob = recentJobs.First();

        Console.WriteLine($"Latest Job Status:");
        Console.WriteLine($"  Job ID: {latestJob.Id}");
        Console.WriteLine($"  State: {latestJob.State}");
        Console.WriteLine();

        // Show timeline
        Console.WriteLine("  Timeline:");
        Console.WriteLine($"    Received:  {latestJob.TsReceived}");

        if (!string.IsNullOrEmpty(latestJob.TsQueued))
        {
            Console.WriteLine($"    Queued:    {latestJob.TsQueued}");
        }

        if (!string.IsNullOrEmpty(latestJob.TsProcessStart))
        {
            Console.WriteLine($"    Started:   {latestJob.TsProcessStart}");
        }

        if (!string.IsNullOrEmpty(latestJob.TsProcessDone))
        {
            Console.WriteLine($"    Completed: {latestJob.TsProcessDone}");
        }

        Console.WriteLine();

        // Show size information
        if (latestJob.State == JobState.Done)
        {
            Console.WriteLine("  Results:");
            Console.WriteLine($"    Records:       {latestJob.RecordCount:N0}");
            Console.WriteLine($"    Billed Size:   {latestJob.BilledSize:N0} bytes ({FormatBytes(latestJob.BilledSize)})");
            Console.WriteLine($"    Actual Size:   {latestJob.ActualSize:N0} bytes ({FormatBytes(latestJob.ActualSize)})");
            Console.WriteLine($"    Package Size:  {latestJob.PackageSize:N0} bytes ({FormatBytes(latestJob.PackageSize)})");
            Console.WriteLine($"    Expires:       {latestJob.TsExpiration}");
            Console.WriteLine();
            Console.WriteLine("  ✅ Job is complete and ready for download!");
        }
        else
        {
            Console.WriteLine($"  ⏳ Job is {latestJob.State.ToString().ToLowerInvariant()}...");
        }
    }

    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error monitoring job status: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 6: List Files for a Completed Job
// ============================================================================

Console.WriteLine("Example 6: List Files for a Completed Job");
Console.WriteLine("------------------------------------------");
Console.WriteLine("See what files are available to download from a completed job.");
Console.WriteLine();

try
{
    // Find a completed job
    var completedJobs = await client.BatchListJobsAsync(
        new[] { JobState.Done },
        DateTimeOffset.UtcNow.AddDays(-90));

    if (completedJobs.Count == 0)
    {
        Console.WriteLine("No completed jobs found.");
        Console.WriteLine("Submit a job and wait for it to complete (see Examples 3-4).");
    }
    else
    {
        var job = completedJobs.First();
        Console.WriteLine($"Job ID: {job.Id}");
        Console.WriteLine($"Dataset: {job.Dataset}, Symbols: {string.Join(", ", job.Symbols)}");
        Console.WriteLine($"Completed: {job.TsProcessDone}");
        Console.WriteLine();

        var files = await client.BatchListFilesAsync(job.Id);

        Console.WriteLine($"Found {files.Count} file(s):");
        Console.WriteLine();

        foreach (var file in files)
        {
            Console.WriteLine($"  {file.Filename}");
            Console.WriteLine($"    Size: {FormatBytes(file.Size)}");
            Console.WriteLine($"    Hash: {file.Hash}");
            Console.WriteLine($"    HTTPS: {file.HttpsUrl}");
            Console.WriteLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error listing batch files: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 7: Download Files from a Batch Job
// ============================================================================

Console.WriteLine("Example 7: Download Files from a Batch Job");
Console.WriteLine("-------------------------------------------");
Console.WriteLine("Download completed batch job files to your local machine.");
Console.WriteLine();

try
{
    // Find a completed job
    var completedJobs = await client.BatchListJobsAsync(
        new[] { JobState.Done },
        DateTimeOffset.UtcNow.AddDays(-90));

    if (completedJobs.Count == 0)
    {
        Console.WriteLine("No completed jobs found to download.");
    }
    else
    {
        var job = completedJobs.First();
        Console.WriteLine($"Job ID: {job.Id}");
        Console.WriteLine($"Dataset: {job.Dataset}");
        Console.WriteLine();

        // Create output directory
        var outputDir = Path.Combine(Path.GetTempPath(), "databento_batch_downloads");
        Directory.CreateDirectory(outputDir);

        Console.WriteLine($"Downloading all files to: {outputDir}");
        Console.WriteLine();

        // Download all files for the job
        var downloadedPaths = await client.BatchDownloadAsync(outputDir, job.Id);

        Console.WriteLine($"✅ Downloaded {downloadedPaths.Count} file(s):");
        foreach (var path in downloadedPaths)
        {
            var fileInfo = new FileInfo(path);
            Console.WriteLine($"  {Path.GetFileName(path)} ({FormatBytes((ulong)fileInfo.Length)})");
        }

        Console.WriteLine();
        Console.WriteLine($"Files saved to: {outputDir}");
    }

    Console.WriteLine();
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error downloading batch files: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Example 8: Download Specific File from a Batch Job
// ============================================================================

Console.WriteLine("Example 8: Download Specific File from a Batch Job");
Console.WriteLine("---------------------------------------------------");
Console.WriteLine("Download just one file from a batch job.");
Console.WriteLine();

try
{
    // Find a completed job
    var completedJobs = await client.BatchListJobsAsync(
        new[] { JobState.Done },
        DateTimeOffset.UtcNow.AddDays(-90));

    if (completedJobs.Count == 0)
    {
        Console.WriteLine("No completed jobs found.");
    }
    else
    {
        var job = completedJobs.First();
        var files = await client.BatchListFilesAsync(job.Id);

        if (files.Count == 0)
        {
            Console.WriteLine($"No files available for job {job.Id}");
        }
        else
        {
            var firstFile = files.First();
            Console.WriteLine($"Job ID: {job.Id}");
            Console.WriteLine($"Downloading: {firstFile.Filename}");
            Console.WriteLine($"Size: {FormatBytes(firstFile.Size)}");
            Console.WriteLine();

            // Create output directory
            var outputDir = Path.Combine(Path.GetTempPath(), "databento_batch_downloads");
            Directory.CreateDirectory(outputDir);

            // Download specific file
            var downloadedPath = await client.BatchDownloadAsync(outputDir, job.Id, firstFile.Filename);

            var fileInfo = new FileInfo(downloadedPath);
            Console.WriteLine($"✅ Downloaded successfully!");
            Console.WriteLine($"  Path: {downloadedPath}");
            Console.WriteLine($"  Size: {FormatBytes((ulong)fileInfo.Length)}");
            Console.WriteLine();
        }
    }
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error downloading specific file: {ex.Message}");
    Console.WriteLine();
}

// ============================================================================
// Summary
// ============================================================================

Console.WriteLine("=== Batch Downloads Summary ===");
Console.WriteLine();
Console.WriteLine("Batch downloads provide:");
Console.WriteLine("  1. Efficient bulk downloads for large historical data requests");
Console.WriteLine("  2. Multiple download attempts without additional cost");
Console.WriteLine("  3. Flexible output formats (DBN, CSV, JSON) with compression");
Console.WriteLine("  4. File splitting by symbol, time, or size for easier processing");
Console.WriteLine("  5. Background processing - submit and check back later");
Console.WriteLine();
Console.WriteLine("Best practices:");
Console.WriteLine("  • Use batch downloads for large date ranges or many symbols");
Console.WriteLine("  • Monitor job status to know when files are ready");
Console.WriteLine("  • Download files before expiration date");
Console.WriteLine("  • Use appropriate compression (Zstd recommended) for faster transfer");
Console.WriteLine("  • Split large jobs by symbol or date for parallel processing");
Console.WriteLine();
Console.WriteLine("=== Batch Example Complete ===");

// Helper function to format byte sizes
static string FormatBytes(ulong bytes)
{
    string[] sizes = ["B", "KB", "MB", "GB", "TB"];
    double len = bytes;
    int order = 0;
    while (len >= 1024 && order < sizes.Length - 1)
    {
        order++;
        len /= 1024;
    }
    return $"{len:0.##} {sizes[order]}";
}
