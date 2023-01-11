var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var products = new Dictionary<int, Dictionary<ProductType, decimal>>
                   {
                       {
                           1,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 5_000},
                                   {ProductType.Laptop, 10_000},
                                   {ProductType.Desktop, 15_000}
                               }
                       },
                       {
                           2,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 6_000},
                                   {ProductType.Laptop, 11_000},
                                   {ProductType.Desktop, 16_000}
                               }
                       },
                       {
                           3,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 7_000},
                                   {ProductType.Laptop, 12_000},
                                   {ProductType.Desktop, 17_000}
                               }
                       },
                       {
                           4,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 8_000},
                                   {ProductType.Laptop, 13_000},
                                   {ProductType.Desktop, 18_000}
                               }
                       },
                       {
                           5,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 9_000},
                                   {ProductType.Laptop, 14_000},
                                   {ProductType.Desktop, 19_000}
                               }
                       },
                       {
                           6,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 10_000},
                                   {ProductType.Laptop, 15_000},
                                   {ProductType.Desktop, 20_000}
                               }
                       },
                       {
                           7,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 11_000},
                                   {ProductType.Laptop, 16_000},
                                   {ProductType.Desktop, 21_000}
                               }
                       },
                       {
                           8,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 12_000},
                                   {ProductType.Laptop, 17_000},
                                   {ProductType.Desktop, 22_000}
                               }
                       },
                       {
                           9,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 13_000},
                                   {ProductType.Laptop, 18_000},
                                   {ProductType.Desktop, 23_000}
                               }
                       },
                       {
                           10,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 14_000},
                                   {ProductType.Laptop, 19_000},
                                   {ProductType.Desktop, 24_000}
                               }
                       },
                       {
                           11,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 15_000},
                                   {ProductType.Laptop, 20_000},
                                   {ProductType.Desktop, 25_000}
                               }
                       },
                       {
                           12,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 16_000},
                                   {ProductType.Laptop, 21_000},
                                   {ProductType.Desktop, 26_000}
                               }
                       },
                       {
                           13,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 17_000},
                                   {ProductType.Laptop, 22_000},
                                   {ProductType.Desktop, 27_000}
                               }
                       },
                       {
                           14,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 18_000},
                                   {ProductType.Laptop, 23_000},
                                   {ProductType.Desktop, 28_000}
                               }
                       },
                       {
                           15,
                           new Dictionary<ProductType, decimal>
                               {
                                   {ProductType.CellPhone, 19_000},
                                   {ProductType.Laptop, 24_000},
                                   {ProductType.Desktop, 29_000}
                               }
                       }
                   };

// Configure the HTTP request pipeline.
app.MapGet("/search-product/{id}", (ProductType id, IConfiguration configuration) =>
    {
        Thread.Sleep(configuration.GetValue("Duration", 2_000));
        
        var storeId = configuration.GetValue("StoreId", 1);
        return products[storeId][id];
    });

app.Run();

internal enum ProductType
{
    CellPhone,
    Laptop,
    Desktop
}