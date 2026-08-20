using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;

public class roundData
{
    public string roundNum;
    public string teamId;
    public string teamName;
}

public class GetScores
{
    public BowlingScoreCardData data;
}


[Serializable]
public class BowlingScoreCardData
{

    [JsonProperty("status")]
    public string status;
    [JsonProperty("score")]
    public List<PlayerScoreCardData> score;
    //public int columnsCount;
}

#region Exhange Cards
[Serializable]

public partial class ExchangedCards
{
    [JsonProperty("cards")]
    public List<int> Cards;
}
#endregion

[Serializable]
public class PlayerScoreCardData
{
    [JsonProperty("player_id")]
    public string PlayerId;

    [JsonProperty("username")]
    public string Username;

    [JsonProperty("image")]
    public string Image;

    [JsonProperty("rolls")]
    public List<int> Rolls = new List<int>();

    [JsonProperty("cell_scores")]
    public List<int> CellScores = new List<int>();

    [JsonProperty("cards")]
    public List<int> Cards = new List<int>();

    [JsonProperty("is_winner")]
    public bool IsWinner;

    [JsonProperty("exchange_cards")]
    public bool ExchangeCards;


    [JsonProperty("poker_hands")]

    public string PokerHands;
}

[Serializable]
public partial class Carde
{
    public string Id;
}

[Serializable]
public class WinnerData
{
    public PlayerScoreCardData winner;
}

[Serializable]
public partial class ResponseData
{
    public bool success;
    public JToken data; // This will hold either a single object or an array of objects.
    public string message;
}

#region Login
[Serializable]
public partial class PlayerData
{
    [JsonProperty("player_id")]
    public string PlayerId;

    [JsonProperty("username")]
    public string Username;

    [JsonProperty("email")]
    public string Email;

    [JsonProperty("phone")]
    public string PhoneNumber;

    [JsonProperty("image")]
    public string Image;

    [JsonProperty("user_type")]
    public string UserType;

    [JsonProperty("auth_provider")]
    public string AuthProvider;

    [JsonProperty("platform")]
    public string Platform;

    [JsonProperty("social_id")]
    public string SocialId;

    [JsonProperty("social_token")]
    public string SocialToken;

    [JsonProperty("device_id")]
    public string DeviceId;

    [JsonProperty("created_at")]
    public string CreatedAt;

    [JsonProperty("access_token")]
    public string AccessToken;
}
#endregion

#region Logout
public partial class Logout
{
    [JsonProperty("success")]
    public bool Success;

    [JsonProperty("message")]
    public string Message;
}

#endregion

#region CreateGame
public partial class CreateGame
{
    public string game_id;
}

#endregion

#region Create Update League
public partial class CreateLeagueData
{
    [JsonProperty("id")]
    public string LeagueId;
}


public partial class UpdateLeague
{
    [JsonProperty("success")]
    public string Success;

    [JsonProperty("message")]
    public string Message;
}
#endregion

#region LeagueData
public class LeagueData
{
    public string leagueName;
    public string leagueDescription;
    public string leagueImageUrl;
    public string leagueStartTime;
    public string leaugeParticipants;
    public string leaugeTotalParticipants;
}


#endregion

#region Get All Leagues By User
// Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);

[Serializable]
public partial class GetLeaguesByUser
{
    [JsonProperty("id")]
    public string leagueId;

    [JsonProperty("player_id")]
    public string moderator_Id;

    [JsonProperty("name")]
    public string leagueName;

    [JsonProperty("participants")]
    public string participants;

    [JsonProperty("prize_pool")]
    public string prize_pool;

    [JsonProperty("image")]
    public string image;

    [JsonProperty("start_time")]
    public string start_time;

    [JsonProperty("created_at")]
    public string created_at;

    [JsonProperty("rules")]
    public List<Rule> rules;

    [JsonProperty("games")]
    public List<Games> games;

    [JsonProperty("requests")]
    public List<Request> leagueRequests;

}
#endregion

#region Get All Leagues By Moderator
[Serializable]
public partial class GetLeaguesByModerator
{
    [JsonProperty("id")]
    public string leagueId;

    [JsonProperty("player_id")]
    public string playerId;

    [JsonProperty("name")]
    public string leagueName;

    [JsonProperty("participants")]
    public string participants;

    [JsonProperty("prize_pool")]
    public string PrizePool;

    [JsonProperty("start_time")]
    public string StartTime;

    [JsonProperty("image")]
    public string image;

    [JsonProperty("created_at")]
    public string CreatedAt;

    [JsonProperty("rules")]
    public List<Rule> rules;

    [JsonProperty("league_info")]
    public string leagueInfo;

    [JsonProperty("requests")]
    public List<LeagueRequest> leagueRequests;
}

[Serializable]
public partial class LeagueRequest
{
    [JsonProperty("id")]
    public string requestId;

    [JsonProperty("status")]
    public string Status;

    [JsonProperty("created_at")]
    public string CreatedAt;

    [JsonProperty("user")]
    public UserLeagueRequest userLeagueRequest;
}

[Serializable]
public partial class UserLeagueRequest
{
    [JsonProperty("player_id")]
    public string PlayerId;

    [JsonProperty("username")]
    public string Username;

    [JsonProperty("image")]
    public string Image;
}
#endregion

#region Get Games By Moderator
[Serializable]
public partial class GetgamesByModerator
{
    [JsonProperty("league_participants")]
    public string LeagueParticipants { get; set; }

    [JsonProperty("games")]
    public List<Games> Games { get; set; }
}
#endregion

#region SearchResult
public partial class SearchResult
{
    [JsonProperty("leagues")]
    public List<GetLeaguesByUser> Leagues;

    [JsonProperty("games")]
    public List<Games> Games;
}

#endregion

#region Get Notification

public partial class NotificationData
{
    [JsonProperty("id")]
    public long Id;

    [JsonProperty("player_id")]
    public long PlayerId;

    [JsonProperty("title")]
    public string Title;

    [JsonProperty("body")]
    public string Body;

    [JsonProperty("is_read")]
    public string IsRead;

    [JsonProperty("created_at")]
    public string CreatedAt;
}
#endregion

#region Notification Seen
[Serializable]
public partial class NotificationSeenData
{
    [JsonProperty("user_id")]
    public long UserId;

    [JsonProperty("notifiable_id")]
    public long NotifiableId;

    [JsonProperty("is_read")]
    public long IsRead;

    [JsonProperty("updated_at")]
    public DateTimeOffset UpdatedAt;

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt;

    [JsonProperty("id")]
    public long Id;
}
#endregion

#region Common
[Serializable]
public partial class Games
{
    [JsonProperty("id")]
    public string Id;

    [JsonProperty("player_id")]
    public string PlayerId;

    [JsonProperty("league_id")]
    public string LeagueId;

    [JsonProperty("name")]
    public string Name;

    [JsonProperty("lane")]
    public string Lane;

    [JsonProperty("start_time")]
    public string startTime;

    [JsonProperty("participants")]
    public string Participants;

    [JsonProperty("created_at")]
    public string CreatedAt;

    [JsonProperty("game_info")]
    public string gameInfo;

    [JsonProperty("requests")]
    public List<Request> GameRequests;
}


[Serializable]
public partial class Request
{
    [JsonProperty("status")]
    public string Status;

    [JsonProperty("created_at")]
    public string CreatedAt;

    [JsonProperty("assigned_lane")]
    public string AssignedLane;

    [JsonProperty("user")]
    public User User;
}

[Serializable]
public partial class User
{
    [JsonProperty("player_id")]
    public string PlayerId;

    [JsonProperty("username")]
    public string Username;

    [JsonProperty("image")]
    public string Image;
    /*
        [JsonProperty("Cards")]
        public List<int> Cards;

        [JsonProperty("is_winner")]
        public bool IsWinner;*/
}


[Serializable]
public partial class Rule
{
    [JsonProperty("id")]
    public string id;

    [JsonProperty("player_id")]
    public string player_id;

    [JsonProperty("type")]
    public string type;

    [JsonProperty("description")]
    public string description;

    [JsonProperty("created_at")]
    public string created_at;
}
#endregion

#region Dispute
public partial class CreateDisputeResponse
{
    [JsonProperty("moderator_id")]
    public string ModeratorId { get; set; }

    [JsonProperty("game_id")]
    public string GameId { get; set; }

    [JsonProperty("disputer_id")]
    public string DisputerId { get; set; }

    [JsonProperty("disputed_against_id")]
    public string DisputedAgainstId { get; set; }

    [JsonProperty("cell_index")]
    public string CellIndex { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
}

public partial class GetDisputesResponse
{
    [JsonProperty("moderator_id")]
    public string ModeratorId { get; set; }

    [JsonProperty("game_id")]
    public string GameId { get; set; }

    [JsonProperty("disputer_id")]
    public string DisputerId { get; set; }

    [JsonProperty("league_name")]
    public string LeagueName { get; set; }
    [JsonProperty("game_name")]
    public string GameName { get; set; }

    [JsonProperty("disputer_name")]
    public string DisputerName { get; set; }

    [JsonProperty("disputed_against_id")]
    public string DisputedAgainstId { get; set; }

    [JsonProperty("disputed_against_name")]
    public string DisputedAgainstName { get; set; }

    [JsonProperty("cell_index")]
    public string CellIndex { get; set; }

    [JsonProperty("group_id")]
    public string groupID { get; set; }

    [JsonProperty("status")]
    public string Status { get; set; }

    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
}

public partial class ImagePathResponse
{
    [JsonProperty("image")]
    public string Image { get; set; }
}
#endregion

#region Get Leagues And Games Count
public partial class GetLeaguesAndGamesRequestCount
{
    [JsonProperty("data")]
    public GetLeaguesAndGamesRequestCountData Data;
}

public partial class GetLeaguesAndGamesRequestCountData
{
    [JsonProperty("total_count")]
    public long TotalCount;
}

#endregion

#region SerializeDeserialize
[Serializable]
public partial class Deserialzer
{
    public static T FromJson<T>(string json) => JsonConvert.DeserializeObject<T>(json, Converter.Settings);
}

[Serializable]
public partial class Serializer
{
    public static string ToJson<T>(T self) => JsonConvert.SerializeObject(self, Converter.Settings);
}

internal static class Converter
{
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
        MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
        DateParseHandling = DateParseHandling.None,
        Converters =
                {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
    };
}
#endregion

#region Chat
public partial class SocketResponse
{
    [JsonProperty("object_type")]
    public string ObjectType { get; set; }
}

public partial class GetMessages
{
    [JsonProperty("moderator_avatar")]
    public string ModeratorAvatar { get; set; }

    [JsonProperty("disputer_avatar")]
    public string DisputerAvatar { get; set; }

    [JsonProperty("disputer_against_avatar")]
    public string RespondedAvatar { get; set; }

    [JsonProperty("data")]
    public List<MessageData> Data { get; set; }
}

public partial class GetMessage
{
    [JsonProperty("data")]
    public MessageData Data { get; set; }
}

public partial class MessageData
{
    [JsonProperty("sended_by")]
    public string SendedBy { get; set; }

    [JsonProperty("username")]
    public string Username { get; set; }

    [JsonProperty("disputer_id")]
    public string DisputerId { get; set; }

    [JsonProperty("disputed_against_id")]
    public string DisputedAgainstId { get; set; }

    [JsonProperty("moderator_id")]
    public string ModeratorId { get; set; }

    [JsonProperty("group_id")]
    public string GroupId { get; set; }

    [JsonProperty("message")]
    public string Message { get; set; }

    [JsonProperty("type")]
    public string Type { get; set; }

    [JsonProperty("seen")]
    public string Seen { get; set; }

    [JsonProperty("created_at")]
    public DateTimeOffset CreatedAt { get; set; }
}

#endregion