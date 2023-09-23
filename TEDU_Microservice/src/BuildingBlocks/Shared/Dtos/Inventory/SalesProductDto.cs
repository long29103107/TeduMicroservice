using Shared.Enums.Inventory;

namespace Shared.Dtos.Inventory;
public record SalesProductDto(string ExtenalDocumentNo, int Quantity)
{
    public EDocumentType DocumentType = EDocumentType.Sale;

    public string ItemNo { get; set; }

    public void SetItemNo(string itemNo)
    {
        ItemNo = itemNo;
    }
}
