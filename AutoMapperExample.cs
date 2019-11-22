using AutoMapper;
using System;
using System.Collections.Generic;

namespace AutoMapperExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Mapper.Initialize(cfg =>
            {
                //cfg.AddProfile<MyProfile>();

                //Scan all program profiles
                cfg.AddProfiles(typeof(Program));
            });

            //Automapper 8 and beyond
           //var configuration = new MapperConfiguration(cfg => {
           //     //cfg.CreateMap<SourceOrder, DestinationOrder>();
           //     cfg.AddProfile<MyProfile>();
           // });
           //IMapper iMapper = configuration.CreateMapper();

           var src = new SourceOrder() {Created = DateTime.Now.ToLongDateString(), OrderId =  Guid.NewGuid().ToString()};
           //var dest = iMapper.Map<DestinationOrder>(src);
           var dest = Mapper.Map<DestinationOrder>(src, opt=>opt.Items.Add("IsPayByCard",false));
           //var dest = Mapper.Map<DestinationOrder>(src, opt=>opt.Items.Add("IsPayByCard",true));
           var another = Mapper.Map(new SourceDelivery()
           {
               Company = "company", Cost = 2.0m
           }, dest);

           List<FlatDestination> destinations = Mapper.Map<List<FlatDestination>>(src);



        }


        public class FromSourceToFlatDestinationConverter : ITypeConverter<SourceOrder, List<FlatDestination>>
        {
            public List<FlatDestination> Convert(SourceOrder source, List<FlatDestination> destination, ResolutionContext context)
            {
               var result = new List<FlatDestination>(source.Items.Count);

               source.Items.ForEach(i =>
               {
                   var item = context.Mapper.Map<FlatDestination>(i);
                   item.OrderId = source.OrderId;
                   result.Add(item);
               });
               return result;
            }
        }


        private class SourceFlatProfile : Profile
        {
            public SourceFlatProfile()
            {
                CreateMap<SourceOrder, List<FlatDestination>>().ConvertUsing<FromSourceToFlatDestinationConverter>();
                CreateMap<SourceItem, FlatDestination>();


            }
        }
 private class SourceDeliveryProfile : Profile
        {
            public SourceDeliveryProfile()
            {
                CreateMap<SourceDelivery, DestinationOrder>();
                RecognizeDestinationPrefixes("Delivery");

            }
        }

        private class MyProfile : Profile
        {
            public MyProfile()
            {
                CreateMap<SourceOrder, DestinationOrder>()
                    //.ForMember(dest => dest.Created, opt => opt.UseValue(DateTime.Now()));
                    //.ForMember(dest => dest.Created, opt => opt.UseDestinationValue());
                    .ForMember(dest => dest.Created, opt => opt.Ignore())
                    
                    .ForMember(dest => dest.Created, opt => opt.MapFrom(src=> $"{src.Customer.FirstName} {src.Customer.LastName}"))
                    .ForMember(dest => dest.Addresses, opt => opt.ResolveUsing((src, dest, member, context) =>
                    {
                        return  new DestinationAddress()
                        {
                            BillingAddress = context.Mapper.Map<string>(src.BillingAddress), 
                            ShippingAddress= context.Mapper.Map<string>(src.ShippingAddress)
                        };
                    }))
                    .AfterMap((src, dest) => dest.Created = DateTime.Now.ToString("0"))
                    .ForMember(dest => dest.PaymentType, opt => opt.ResolveUsing((src, dest, member, context) =>
                        {
                            return context.Items.ContainsKey("IsPayByCard") && context.Items["IsPayByCard"].Equals(true) //use equals to unbox true
                                ? PaymentType.Card
                                : PaymentType.Cash;
                        }))
                    ;

                CreateMap<SourceItem, DestinationItem>();

                CreateMap<SourceAddress, string>().ConstructUsing(src=> $"{src.Street}, {src.ZipCode}, {src.City}");
            }
        }
    }

  
    public class SourceDelivery
    {
        public string Company { get; set; }
        public decimal Cost { get; set; }

    }

        public class DestinationOrder
    {
        public string OrderId { get; set; }
        public string Created { get; set; }
        //AM will understand it as FirstName
        public string CustomerName { get; set; }

        public DestinationAddress Addresses { get; set; }
        public List<DestinationItem> Items { get; set; }
        
        public PaymentType PaymentType { get; set; }

        public string DeliveryCompany { get; set; }
        public decimal DeliveryCost { get; set; }

        public SourceDelivery SourceDeliveryCompany { get; set; }

    }

    public enum PaymentType
    {
        Cash, Card
    }

    public class DestinationAddress
    {
        public string ShippingAddress { get; set; }
        public string BillingAddress { get; set; }

    }

    public class SourceItem
    {
        public string Name { get; set; }
        public string Cost { get; set; }
        
    }

  public class FlatDestination
    {

        public string OrderId { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        
    }


    public class DestinationItem
    {
        public string Name { get; set; }
        public string Cost { get; set; }
    }

    public class SourceOrder
    {
        public string OrderId { get; set; }
        public string Created { get; set; }
        public SourceAddress ShippingAddress { get; set; }
        public SourceAddress BillingAddress { get; set; }
        public List<SourceItem> Items { get; set; }

        public Customer Customer { get; set; }

        public static SourceOrder SeedData()
        {
            return new SourceOrder()
            {
                Created = DateTime.Now.ToString(),
                OrderId = "123459-xcx"
            };
        }

    }

    public class SourceAddress
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
    }

    public class Customer
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
