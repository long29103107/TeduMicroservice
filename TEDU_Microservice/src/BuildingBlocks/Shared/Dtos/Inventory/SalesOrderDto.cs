namespace Shared.Dtos.Inventory;
public class SalesOrderDto
{
    //Order's Document No
    public string OrderNo { get; set; }
    public List<SaleItemDto> SalesItems {get;set;}
}
