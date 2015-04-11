using FluentNHibernate.Mapping;

namespace NRules.Samples.ClaimsExpert.Domain.Mappings
{
    public class NameMap : ComponentMap<Name>
    {
        public NameMap()
        {
            Map(x => x.FirstName);
            Map(x => x.LastName);
            Map(x => x.MiddleName);
        }
    }
}