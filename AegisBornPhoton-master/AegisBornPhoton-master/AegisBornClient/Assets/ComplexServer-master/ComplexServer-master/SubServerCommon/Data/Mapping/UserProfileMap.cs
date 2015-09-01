using FluentNHibernate.Mapping;
using FluentNHibernate.MappingModel.Output.Sorting;
using SubServerCommon.Data.NHibernate;

namespace SubServerCommon.Data.Mapping
{
    public class UserProfileMap : ClassMap<UserProfile>
    {
        public UserProfileMap()
        {
            Id(x => x.Id).Column("id");
            Map(x => x.CharacterSlots).Column("character_slots");
            References(x => x.UserId).Column("user_id");
            Table("cs_user_profile");
        }
    }
   
}
