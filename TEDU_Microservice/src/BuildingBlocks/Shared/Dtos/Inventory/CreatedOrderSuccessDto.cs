namespace Shared.Dtos.Inventory;
public class CreatedOrderSuccessDto
{
    public string DocumentNo { get; }
    public CreatedOrderSuccessDto(string documentNo)
    {
        DocumentNo = documentNo;
    }
}
