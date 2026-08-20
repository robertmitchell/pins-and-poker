using Unity.VisualScripting;

public class Global
{
   /// <summary>
    public static bool isleagueDataFromPrefab=true;
    public static bool ShowTutorial=false;
    /// </summary>
    // Scene Names
    public const string MainMenuScene = "MainMenu";
    public const string GameScene = "Gameplay";

    // Tags
    public const string PlayerTag = "Player";
    public const string EnemyTag = "Enemy";

    // Default Values
    public const float DefaultVolume = 0.7f;
    public const int DefaultMaxScore = 1000;
    public static int myCurrentCellIndex =0;

    // Error Messages
    public const string ErrorNetwork = "Network Error: Unable to connect.";
    public const string ErrorDataLoad = "Failed to load data.";


    public const string splashScreen    = "splash";
    public const string dashboardScreen = "dashboard";
    public const string profileScreen = "profile";
    public const string editProfileScreen = "editProfile";
     
    public static UserType currentUserType;
    public enum UserType
    {
        user,
        moderator
    }
    public static RuleType currentRuleType;
    public enum RuleType
    {
        general,
        special
    }

    public static LoginType currentLoginType;
    public enum LoginType
    {
        guest,
        social,
        connect
    }

    public static AuthProvider currentAuthProvider;
    public enum AuthProvider
    {
        guest, google, apple, normal
    }

    public enum Status
    {
        accepted,
        declined,
        rejected,
        pending,
        started,
        resolved,
        ended
    }
  
}
public class Db_Keys : Global
{
    public const string isFirstTime = "isFirstTime";
    public const string isFirstTimeInfo = "isFirstTimeInfo";
    public const string islogedIn = "IslogedIn";
    public const string playerID = "player_id";
    public const string moderatorId = "moderator_id";
    public const string socialId = "social_id";
    public const string userImage = "image";
    public const string userName = "username";
    public const string userEmail = "email";
    public const string userPassword = "password";
    public const string userPhoneNumber = "phone";
    public const string userType = "user_type";  //user, moderator
    public const string loginType = "login_type";  //guest, social, connect
    public const string Moderator = "Moderator";
    public const string platform = "platform";      //android, ios
    public const string socialToken = "social_token";
    public const string authProvider = "auth_provider";//guest, google, apple
    public const string deviceToken = "device_token";
    public const string fcmToken = "fcm_token";
    public const string token = "token";
    public const string removefrom = "remove_from";
    public const string notifyID = "notify_id";
    public const string isRead = "is_read";

    //Edit League
    public const string leagueName = "name";
    public const string leagueId = "league_id";
    public const string prizePool = "prize_pool";
    public const string special_rules = "special_rules";
    public const string start_time = "start_time";
    public const string start_date = "start_date";
    public const string frequency = "frequency";
    public const string image = "image";
    public const string generalRules = "general_rules";
    public const string specialRules = "special_rules";
    //Create Game

    public const string gameName = "name"; 
    public const string laneName = "lane";
    public const string assignedLane = "assigned_lane";
    public const string gameId = "game_id";



    public const string status = "status";
    public const string searchTerm = "search_term";

    // Cards
    public const string cardIdSaved = "cardIdSaved";
    public const string cardId = "id";
    public const string cardIndex = "card_index";
    public const string cardName = "name";
    public const string cardsuit = "suit";
    public const string cardrank = "rank";

    // Profile Stats
    public const string gamesPlayed = "games_played";
    public const string gamesWon = "games_won";
    public const string gamesLost = "games_lost";
    public const string pointsAccumulated = "points_accumulated";
    public const string moneyEarned = "money_earned";

    // Edit/Dispute Score
    public const string rolls = "rolls";
    public const string disputerId = "disputer_id";
    public const string disputedgainstId = "disputed_against_id";
    public const string cell_index = "cell_index";
    public const string disputeGroupID = "dispute_group_id";
}


