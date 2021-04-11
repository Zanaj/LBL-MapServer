public enum InventoryErrorCode
{
    Unknown,

    AlmostEnoughSpace,
    NotEnoughSpace,

    DoesNotOwnItem,
    DoesNotHaveEnoughItem,
    HaveSomeOfItems,
    ItemIsSpecialItem,

    Success = 99,
}