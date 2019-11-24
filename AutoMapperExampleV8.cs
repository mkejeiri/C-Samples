using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AutoMapperExample
{
    class Program
    {
        static void Main(string[] args)
        {
            Init();
        }

        //Old way before automapper 8.0
        private static void Init()
        {

           // Automapper 8 and beyond
            var configuration = new MapperConfiguration(cfg =>
            {
                //cfg.CreateMap<SourceOrder, DestinationOrder>();
                //cfg.AddProfile<MyProfile>();
                //var profiles = typeof(MyProfile).Assembly.GetTypes().Where(x => typeof(Profile).IsAssignableFrom(x)).ToList();
                //cfg.AddProfiles();
                //cfg.AddMaps(Assembly.GetExecutingAssembly());
                cfg.AddMaps(typeof(MyProfile).Assembly);
            });

            IMapper iMapper = configuration.CreateMapper();

            var src = SourceOrder.SeedData();
            Console.WriteLine(JValue.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(src)).ToString(Formatting.Indented));

            var dest = iMapper.Map<DestinationOrder>(src, opt => opt.Items.Add("IsPayByCard", false));
            //var dest = iMapper.Map<DestinationOrder>(src, opt=>opt.Items.Add("IsPayByCard",true));


            Console.WriteLine(JValue.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(dest)).ToString(Formatting.Indented));
           
            dest = iMapper.Map(new SourceDelivery()
            {
                Company = "company",
                Cost = 2.0m
            }, dest);

            var srcDelivery = new SourceDelivery()
            {
                Company = "company",
                Cost = 2.0m
            };
            var another = iMapper.Map<DestinationDelivery>(srcDelivery);

            List<FlatDestination> destinations = iMapper.Map<List<FlatDestination>>(src);

            Console.WriteLine(JValue.Parse(Newtonsoft.Json.JsonConvert.SerializeObject(destinations)).ToString(Formatting.Indented));
        } 

        public class FromSourceToFlatDestinationConverter : ITypeConverter<SourceOrder, List<FlatDestination>>
        {
            public List<FlatDestination> Convert(SourceOrder source, List<FlatDestination> destination, ResolutionContext context)
            {
               //var result = new List<FlatDestination>(source.Items.Count);
               var result = new List<FlatDestination>();

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
                //ClearPrefixes();
                RecognizeDestinationPrefixes("Delivery");
                CreateMap<SourceDelivery, DestinationDelivery>().ReverseMap();
                CreateMap<SourceDelivery, DestinationOrder>();
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
                    //.ForMember(dest => dest.Addresses, opt => opt.ResolveUsing((src, dest, member, context) => //old
                    .ForMember(dest => dest.Addresses, opt => opt.MapFrom((src, dest, member, context) =>
                    {
                        return  new DestinationAddress()
                        {
                            BillingAddress = context.Mapper.Map<string>(src.BillingAddress), 
                            ShippingAddress= context.Mapper.Map<string>(src.ShippingAddress)
                        };
                    }))
                    .AfterMap((src, dest) => dest.Created = DateTime.Now.ToString())
                    .ForMember(dest => dest.PaymentType, opt => opt.MapFrom((src, dest, member, context) =>
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

        private DestinationDelivery Delivery { get; set; }

        public SourceDelivery SourceDeliveryCompany { get; set; }

    }

        public class DestinationDelivery

        {
            public string DeliveryCompany { get; set; }
            public decimal DeliveryCost { get; set; }
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
        public decimal Cost { get; set; }
        
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
        public List<SourceItem> Items { get; set; } = new List<SourceItem>();

        public Customer Customer { get; set; }

        public static SourceOrder SeedData()
        {
            return new SourceOrder()
            {
                Created = DateTime.Now.ToString(),
                OrderId = "123459-xcx",
                Customer = new Customer()
                {
                    LastName = "me",
                    FirstName = "you"
                },
                Items =  new List<SourceItem>()
                {
                    new SourceItem()
                    {
                        Name = "item1",
                        Cost = 5.06m
                    }, new SourceItem()
                    {
                        Name = "item2",
                        Cost = 6.5m
                    }, new SourceItem()
                    {
                        Name = "item3",
                        Cost = 8.5m
                    },
                },
                BillingAddress = new SourceAddress()
                {
                    ZipCode = "1120",
                    City = "Brussels",
                    Street = "BillingAddress Street"
                },

                ShippingAddress = new SourceAddress()
                {
                    ZipCode = "085661",
                    City = "Barcelona",
                    Street = "ShippingAddress Street"
                }

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
