
public enum PacketType
{
    Unknown,

    WelcomeMessage,
    RegisterServer,
    Ping,

    LoginRequest,
    RegisterRequest,
    GetCharactersRequest,
    CharacterCreationRequest,
    CharacterLoginRequest,
    GetWorldRequest,
    PlayerMovementRequest,
    SelectTargetRequest,
    InteractionRequest,

    LoginAnswer,
    RegisterAnswer,
    GetCharactersAnswer,
    CharacterCreationAnswer,
    CharacterLoginAnswer,
    GetWorldAnswer,
    EntitySync,
    SelectTargetAnswer,
    InteractionAnswer,
    InventorySync,

    PlayerDisconnected,
}

public enum NetworkState
{
    NotConnected,
    Connecting,
    Connected,
    NotLoggedIn,
    StartingUp,
    Handshake,
    ProcessingLogin,
    CharacterScreen,
    LoggedIn,
}

public enum ConsoleLevel
{
    Minimal,
    Default,
    Verbose,
    Debug,
}