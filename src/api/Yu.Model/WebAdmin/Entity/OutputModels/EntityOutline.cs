using System;

namespace Yu.Model.WebAdmin.Entity.OutputModels
{
    public class EntityOutline
    {
        public Guid Id { get; set; }

        public string DbContext { get; set; }

        public string Table { get; set; }

        public string Field { get; set; }
    }
}
