﻿using System.Net.Http.Json;
using System.Text.Json;
using DrinksApi.Categories;
using DrinksApi.Drinks;

namespace DrinksApi;

public class Api(Client httpClient)
{
    readonly Client HttpClient = httpClient;

    public async Task<Response<List<CategoryDto>>> FetchCategories()
    {
        try
        {
            var baseListEndpoint = Util.AssertNonNull(ConfigManager.ApiRoutes()["LIST"]);

            await using Stream stream = await HttpClient.client.GetStreamAsync($"{baseListEndpoint}?c=list");

            var categoriesList = await JsonSerializer
                .DeserializeAsync<GenericDrinksListDto<CategoryDto>>(stream);

            return new Response<List<CategoryDto>>(
                true,
                categoriesList?.Drinks ?? []
            );
        }
        catch (Exception ex)
        {
            return new Response<List<CategoryDto>>(
                false,
                [],
                message: ex.Message
            );
        }

    }

    public async Task<Response<List<DrinkFilterListItemDto>>> FetchDrinksInCategory(
        CategoryDto category
    )
    {
        try
        {
            var baseFilterEndpoint = Util.AssertNonNull(
                ConfigManager.ApiRoutes()["FILTER"]
            );

            await using Stream stream = await HttpClient.client.GetStreamAsync(
                $"{baseFilterEndpoint}?c={category.StrCategory}"
            );

            var drinksList = await JsonSerializer.DeserializeAsync<
                    GenericDrinksListDto<DrinkFilterListItemDto>
                >(stream);

            return new Response<List<DrinkFilterListItemDto>>(
                true,
                drinksList?.Drinks ?? []
            );
        }
        catch (Exception ex)
        {
            return new Response<List<DrinkFilterListItemDto>>(
                false,
                [],
                message: ex.Message
            );
        }

    }

    public async Task<Response<DrinkDto?>> FetchDrinkInfo(
        string drinkId
    )
    {
        try
        {
            var endpoint = Util.AssertNonNull(
                ConfigManager.ApiRoutes()["DRINK_BY_ID"]
            );

            var jsonResponse = await HttpClient.client.GetStringAsync(
                $"{endpoint}?i={drinkId}"
            );

            var parsed = JsonDocument.Parse(jsonResponse);

            var drinks = parsed.RootElement.GetProperty("drinks").EnumerateArray().ToList();

            var drinkInfo = drinks[0];

            var id = Util.TryParseJsonStringOrNull(drinkInfo, "idDrink");
            var name = Util.TryParseJsonStringOrNull(drinkInfo, "strDrink");
            var type = Util.TryParseJsonStringOrNull(drinkInfo, "strAlcoholic");
            var glass = Util.TryParseJsonStringOrNull(drinkInfo, "strGlass");

            List<string> ingredients = Util.TryParseMultipleJsonValues(drinkInfo, "strIngredient", 1, 15);
            List<string> measures = Util.TryParseMultipleJsonValues(drinkInfo, "strMeasure", 1, 15);

            DrinkDto drink = new DrinkDto(id, name, type, glass, ingredients, measures);

            return new Response<DrinkDto?>(true, drink);
        }
        catch (Exception ex)
        {
            return new Response<DrinkDto?>(
                false,
                null,
                message: ex.Message
            );
        }

    }
}
