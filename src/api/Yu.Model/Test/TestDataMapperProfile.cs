using AutoMapper;
using Yu.Model.Test.InputOuputModels;

namespace Yu.Model.Test
{
    public class TestDataMapperProfile : Profile
    {
        public TestDataMapperProfile()
        {
            CreateMap<TestDataDetail, Data.Entities.Test.TestData>();
            CreateMap<Data.Entities.Test.TestData, TestDataDetail>();
        }

    }
}
