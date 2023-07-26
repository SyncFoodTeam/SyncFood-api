using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.FoodContainers.DTO
{
    public class FoodContainerCreateDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public required int GroupId { get; set; }
    }

    public class FoodContainerEditDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

}
