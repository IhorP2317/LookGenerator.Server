namespace LookGenerator.Application.Common.Helpers;

public static class CategoryHelper
{
    public static Dictionary<Guid, List<string>> BuildCategoryPaths(IEnumerable<(Guid Id, string Name, Guid? ParentCategoryId)> categoriesEnumerable)
    {
        var categories = categoriesEnumerable.ToList();
        var categoryById = categories.ToDictionary(c => c.Id);
        var result = new Dictionary<Guid, List<string>>();

        foreach (var category in categories)
        {
            var path = new List<string> { category.Name };
            var currentId = category.ParentCategoryId;

            while (currentId.HasValue)
            {
                if (categoryById.TryGetValue(currentId.Value, out var parentCategory))
                {
                    path.Add(parentCategory.Name);
                    currentId = parentCategory.ParentCategoryId;
                }
                else
                {
                    break;
                }
            }

            result[category.Id] = path;
        }

        return result;
    }
}

