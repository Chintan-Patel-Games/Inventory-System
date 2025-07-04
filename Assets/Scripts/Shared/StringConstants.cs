public static class StringConstants
{
    // Tooltip Labels
    public const string ITEM_LABEL = "Item : ";
    public const string TYPE_LABEL = "Type : ";
    public const string RARITY_LABEL = "Rarity : ";
    public const string DESCRIPTION_LABEL = "Description : ";
    public const string COUNT_LABEL = "{0}/{1}";
    public const string BUY_VALUE_LABEL = "Buy Value : ";
    public const string SELL_VALUE_LABEL = "Sell Value : ";
    public const string WEIGHT_LABEL = "Weight : ";

    // Inventory UI Labels
    public const string CURRENT_WEIGHT_LABEL = "Weight : ";
    public const string CURRENT_VALUE_LABEL = "Value : ";

    // Popup Messages
    public const string NOT_ENOUGH_SPACE_OR_COINS = "Not enough space in Inventory or not enough Gold coins!";
    public const string MAX_WEIGHT_LIMIT_REACHED_POPUP = "You cannot carry more weight!";
    public const string INVENTORY_FULL_POPUP = "Inventory is full!";
    public const string NOT_ENOUGH_COINS_POPUP = "Not enough coins!";
    public static string BUY_CONFIRMATION_POPUP = "Do you want to buy\n{0} x {1}\nfor {2}";
    public static string SELL_CONFIRMATION_POPUP = "Do you want to sell\n{0} x {1}\nfor {2}";
    public static string CLOSE_GAME_CONFIRMATION_POPUP = "Are you sure you want to exit the game?";

    // Toaster Messages
    public static string BUY_TOASTER = "You Bought {0} x {1}";
    public static string SELL_TOASTER = "You Sold {0} x {1}";

    public static string FormatItemCount(int itemCount, int maxStack) =>
        string.Format(COUNT_LABEL, itemCount, maxStack);

    public static string FormatBuyConfirmation(int quantity, string itemName, int totalPrice) =>
        string.Format(BUY_CONFIRMATION_POPUP, quantity, itemName, totalPrice);

    public static string FormatSellConfirmation(int quantity, string itemName, int totalPrice) =>
        string.Format(SELL_CONFIRMATION_POPUP, quantity, itemName, totalPrice);

    public static string FormatBuyToaster(int quantity, string itemName) =>
        string.Format(BUY_TOASTER, quantity, itemName);

    public static string FormatSellToaster(int quantity, string itemName) =>
        string.Format(SELL_TOASTER, quantity, itemName);
}