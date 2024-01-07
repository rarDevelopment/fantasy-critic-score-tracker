using System.Globalization;
using System.Net.Http.Json;
using CsvHelper;
using CsvHelper.Configuration;

namespace FantasyCriticScoreTracker;

internal class Program
{
    private const string LeagueIdKeyword = "{{LEAGUE_ID}}";
    private const string YearKeyword = "{{YEAR}}";
    
    //Step 1: Update your folder path here
    private const string CsvFileLocation = @"LETTER:\YOUR\FOLDER\PATH\HERE";

    private const string LeagueUrlTemplate = "https://www.fantasycritic.games/api/League/GetLeagueYear?leagueID=" + LeagueIdKeyword + "&year=" + YearKeyword;

    static async Task Main(string[] args)
    {
        // Step 2: Put all of your League IDs here, each must be in "quotes" and separated by a comma. You're allowed to only have 1 league also.
        var leagueIds = new List<string>
        {
            "A-LEAGUE-ID-HERE",
            "ANOTHER-LEAGUE-ID-HERE",
        };

        // Step 3: Make sure this year is the correct year for your league(s).
        const string year = "2024";

        foreach (var leagueId in leagueIds)
        {
            try
            {
                var leagueYearData = await GetLeagueYearData(leagueId, year);
                var publishers = leagueYearData.Publishers.OrderBy(p => p.DraftPosition).ToList();

                var scoreFileName = $"{leagueYearData.Year} - {leagueYearData.League.LeagueName} - Scores.csv";
                var budgetFileName = $"{leagueYearData.Year} - {leagueYearData.League.LeagueName} - Budgets.csv";

                var scorePath = $"{CsvFileLocation}\\{scoreFileName}";
                var budgetPath = $"{CsvFileLocation}\\{budgetFileName}";

                await WriteCsvFile(scorePath, publishers, TrackingType.TotalFantasyPoints);
                await WriteCsvFile(budgetPath, publishers, TrackingType.Budget);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    private static async Task WriteCsvFile(string filePath,
        List<Publisher> publishers, TrackingType trackingType)
    {
        var isNewFile = !File.Exists(filePath);

        var firstConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            Delimiter = ",",
        };

        await using var fileStream = isNewFile ? null : new FileStream(filePath, FileMode.Append);
        await using var streamWriter = isNewFile
            ? new StreamWriter(filePath) // creates new file at path
            : new StreamWriter(fileStream!); // appends existing file
        await using var csvWriter = isNewFile
            ? new CsvWriter(streamWriter,
                firstConfiguration)
            : new CsvWriter(streamWriter,
                CultureInfo.InvariantCulture);

        if (isNewFile)
        {
            csvWriter.WriteField("Date");
            foreach (var publisher in publishers)
            {
                csvWriter.WriteField(publisher.PlayerName);
            }

            await csvWriter.NextRecordAsync();
        }

        csvWriter.WriteField(DateTime.Now.ToString("yyyy-MM-d"));

        foreach (var publisher in publishers)
        {
            switch (trackingType)
            {
                case TrackingType.TotalFantasyPoints:
                    csvWriter.WriteField(publisher.TotalFantasyPoints);
                    break;
                case TrackingType.Budget:
                    csvWriter.WriteField($"${publisher.Budget}.00");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(trackingType), trackingType, null);
            }
        }

        await csvWriter.NextRecordAsync();
    }

    private static async Task<Root> GetLeagueYearData(string leagueId, string year)
    {
        var leagueUrl = LeagueUrlTemplate.Replace(LeagueIdKeyword, leagueId).Replace(YearKeyword, year);

        var client = new HttpClient(new HttpClientHandler());
        var response = await client.GetAsync(new Uri(leagueUrl));
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error retrieving League Year data: Status: {response.StatusCode}: {response.Content}");
        }

        var fcResponse = await response.Content.ReadFromJsonAsync<Root>();
        return fcResponse ?? throw new Exception($"Error retrieving League Year data: Empty response when reading JSON");
    }
}
