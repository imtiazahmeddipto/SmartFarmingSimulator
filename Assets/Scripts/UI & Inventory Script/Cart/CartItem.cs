public class CartItem
{
    public string itemName;
    public int price;
    public int quantity;

    public CartItem(string itemName, int price, int quantity)
    {
        this.itemName = itemName;
        this.price = price;
        this.quantity = quantity;
    }
}
