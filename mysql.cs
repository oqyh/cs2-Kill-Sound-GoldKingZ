using MySqlConnector;

namespace Kill_Sound_GoldKingZ;


public class MySqlDataManager
{
    public class MySqlConnectionSettings
    {
        public string? MySqlHost { get; set; }
        public string? MySqlDatabase { get; set; }
        public string? MySqlUsername { get; set; }
        public string? MySqlPassword { get; set; }
        public int MySqlPort { get; set; }
    }

    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public bool freezemenu { get; set; }
        public bool headshotkill { get; set; }
        public bool headshothit { get; set; }
        public bool bodyshotkill { get; set; }
        public bool bodyshothit { get; set; }
        public bool quakesounds { get; set; }
        public bool quakehmessages { get; set; }
        public bool quakecmessages { get; set; }
        public DateTime DateAndTime { get; set; }
    }

    public static async Task CreatePersonDataTableIfNotExistsAsync(MySqlConnection connection)
    {
        string query = @"CREATE TABLE IF NOT EXISTS PersonData (
                        PlayerSteamID BIGINT UNSIGNED PRIMARY KEY,
                        freezemenu BOOL,
                        headshotkill BOOL,
                        headshothit BOOL,
                        bodyshotkill BOOL,
                        bodyshothit BOOL,
                        quakesounds BOOL,
                        quakehmessages BOOL,
                        quakecmessages BOOL,
                        DateAndTime DATETIME
                    );";

        try
        {
            using (var command = new MySqlCommand(query, connection))
            {
                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"======================== ERROR =============================");
            Console.WriteLine($"Error creating PersonData table: {ex.Message}");
            Console.WriteLine($"======================== ERROR =============================");
            throw;
        }
    }

    public static async Task SaveToMySqlAsync(ulong PlayerSteamID, bool freezemenu, bool headshotkill, bool headshothit, bool bodyshotkill, bool bodyshothit, bool quakesounds, bool quakehmessages, bool quakecmessages, DateTime DateAndTime, MySqlConnection connection, MySqlConnectionSettings connectionSettings)
    {
        string query = @"INSERT INTO PersonData (PlayerSteamID, freezemenu, headshotkill, headshothit, bodyshotkill, bodyshothit, quakesounds, quakehmessages, quakecmessages, DateAndTime)
                        VALUES (@PlayerSteamID, @freezemenu, @headshotkill, @headshothit, @bodyshotkill, @bodyshothit, @quakesounds, @quakehmessages, @quakecmessages, @DateAndTime)
                        ON DUPLICATE KEY UPDATE freezemenu = VALUES(freezemenu), headshotkill = VALUES(headshotkill), headshothit = VALUES(headshothit),
                        bodyshotkill = VALUES(bodyshotkill), bodyshothit = VALUES(bodyshothit), quakesounds = VALUES(quakesounds), quakehmessages = VALUES(quakehmessages),
                        quakecmessages = VALUES(quakecmessages), DateAndTime = VALUES(DateAndTime)";

        try
        {
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@PlayerSteamID", PlayerSteamID);
                command.Parameters.AddWithValue("@freezemenu", freezemenu);
                command.Parameters.AddWithValue("@headshotkill", headshotkill);
                command.Parameters.AddWithValue("@headshothit", headshothit);
                command.Parameters.AddWithValue("@bodyshotkill", bodyshotkill);
                command.Parameters.AddWithValue("@bodyshothit", bodyshothit);
                command.Parameters.AddWithValue("@quakesounds", quakesounds);
                command.Parameters.AddWithValue("@quakehmessages", quakehmessages);
                command.Parameters.AddWithValue("@quakecmessages", quakecmessages);
                command.Parameters.AddWithValue("@DateAndTime", DateAndTime);

                await command.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"======================== ERROR =============================");
            Console.WriteLine($"Error saving data to MySQL: {ex.Message}");
            Console.WriteLine($"======================== ERROR =============================");
            throw;
        }
    }

    public static async Task<PersonData> RetrievePersonDataByIdAsync(ulong targetId, MySqlConnection connection)
    {
        string query = "SELECT * FROM PersonData WHERE PlayerSteamID = @PlayerSteamID";
        var personData = new PersonData();

        using (var command = new MySqlCommand(query, connection))
        {
            command.Parameters.AddWithValue("@PlayerSteamID", targetId);

            using (var reader = await command.ExecuteReaderAsync())
            {
                if (await reader.ReadAsync())
                {
                    personData = new PersonData
                    {
                        PlayerSteamID = Convert.ToUInt64(reader["PlayerSteamID"]),
                        freezemenu = Convert.ToBoolean(reader["freezemenu"]),
                        headshotkill = Convert.ToBoolean(reader["headshotkill"]),
                        headshothit = Convert.ToBoolean(reader["headshothit"]),
                        bodyshotkill = Convert.ToBoolean(reader["bodyshotkill"]),
                        bodyshothit = Convert.ToBoolean(reader["bodyshothit"]),
                        quakesounds = Convert.ToBoolean(reader["quakesounds"]),
                        quakehmessages = Convert.ToBoolean(reader["quakehmessages"]),
                        quakecmessages = Convert.ToBoolean(reader["quakecmessages"]),
                        DateAndTime = Convert.ToDateTime(reader["DateAndTime"])
                    };
                }
            }
        }
        return personData;
    }
}
