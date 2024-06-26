using System.Text.Json.Serialization;

namespace DrinksApi.Drinks;

public record class DrinkFilterListItemDto(
    [property: JsonPropertyName("idDrink")] string IdDrink,
    [property: JsonPropertyName("strDrink")] string Name
    )
{
    public override string ToString()
    {
        return $"{IdDrink}\t\t{Name}";
    }
}