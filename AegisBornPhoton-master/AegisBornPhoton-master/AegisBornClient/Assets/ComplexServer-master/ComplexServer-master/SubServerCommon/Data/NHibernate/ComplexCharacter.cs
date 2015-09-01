using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using ComplexServerCommon.MessageObjects;

namespace SubServerCommon.Data.NHibernate
{
    public class ComplexCharacter
    {
        public virtual int Id { get; set; }
        public virtual User UserId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Class { get; set; }
        public virtual string Sex { get; set; }
        public virtual int Level { get; set; }

        public virtual CharacterListItem BuilderCharacterListItem()
        {
            CharacterListItem item = new CharacterListItem()
            {
                Id = Id,
                Class = Class,
                Name = Name,
                Level = Level,
                Sex = Sex
            };
            return item;
        }

    }
}
