using SyncFoodApi.Models;

namespace SyncFoodApi.Controllers.Groups.DTO
{
    public class GroupCreateDTO
    {
        public required string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; } = 0f;


        // Permet de caster un Group en GroupCreate

/*        public static explicit operator GroupCreateDTO(Group group)
        {
            return new GroupCreateDTO { Name = group.Name, Description = group.Description, Budget = group.Budget };
        }*/
    }

    public class GroupEditDTO
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public float Budget { get; set; } = 0f;
        public required int groupID { get; set; }


        // Permet de caster un Group en GroupCreate

/*        public static explicit operator GroupEditDTO(Group group)
        {
            return new GroupEditDTO { Name = group.Name, Description = group.Description, Budget = group.Budget };
        }*/
    }


}
