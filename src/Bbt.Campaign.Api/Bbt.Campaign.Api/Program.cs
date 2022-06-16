using Bbt.Campaign.Api.Filter;
using Bbt.Campaign.EntityFrameworkCore.Redis;
using Bbt.Campaign.Public.BaseResultModels;
using Bbt.Campaign.Services;
using Bbt.Campaign.Shared.ServiceDependencies;
using Bbt.Campaign.Shared.Static;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

ConfigurationManager configuration = builder.Configuration;
IWebHostEnvironment environment = builder.Environment;

StaticValues.Campaign_Redis_ConStr = configuration["ConnectionStrings:RedisConnection"];
StaticValues.Campaign_Redis_Ttl = configuration["ConnectionStrings:RedisTtl"];
StaticValues.Campaign_MsSql_ConStr = configuration["ConnectionStrings:DefaultConnection"];
StaticValues.CampaignListImageUrlDefault = configuration["CampaignDefaultImageUrl:List"];
StaticValues.CampaignDetailImageUrlDefault = configuration["CampaignDefaultImageUrl:Detail"];
StaticValues.IsDevelopment = configuration["IsDevelopment"] == null ? false : Convert.ToBoolean(configuration["IsDevelopment"]);
StaticValues.BranchServiceUrl = configuration["ServiceUrl:Branch"];
StaticValues.ChannelCodeServiceUrl = configuration["ServiceUrl:ChannelCode"];
StaticValues.ContractServiceUrl = configuration["ServiceUrl:Contract"];
StaticValues.SessionTimeout = configuration["SessionTimeout"] == null ? 20 : Convert.ToInt32(configuration["SessionTimeout"]);
StaticValues.Audience = configuration["Token:Audience"];
StaticValues.Issuer = configuration["Token:Issuer"];
StaticValues.SecurityKey = configuration["Token:SecurityKey"];

if (StaticValues.IsDevelopment)
    Bbt.Campaign.Shared.Redis.RedisServer.StartRedis();

//builder.Services.AddSwaggerGen(options =>
//{
//    options.SwaggerDoc("v1", new OpenApiInfo
//    {
//        Version = "v1",
//        Title = "Bbt.Campaign.Api",
//        Description = "An ASP.NET Core Web API for managing Bbt.Campaign.Api items"
//    });

//    // using System.Reflection;
//    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
//    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
//});

IocLoader.UseIocLoader(builder.Services);
ServiceModule.Configure(configuration, builder.Services);

builder.Services.AddSingleton<IRedisDatabaseProvider>(c => new RedisDatabaseProvider(StaticValues.Campaign_Redis_ConStr));
builder.Services.AddMemoryCache();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//              .AddJwtBearer(options =>
//              {
//                  options.Authority = Configuration["Authentication:Authority"];
//                  options.Audience = "api";
//              });


builder.Services.AddCors(o => o.AddPolicy("CampaignApiCors", builder =>
{
    builder.AllowAnyOrigin()
           .AllowAnyMethod()
           .AllowAnyHeader();
})); ;

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1.5",
        Title = "Contract Approval API",
        Description = "Provides validation infrastructure for contracts that customers need to approve."
    });

    // To Enable authorization using Swagger (JWT)  
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                          new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "Bearer"
                                }
                            },
                            new string[] {}

                    }
                });

    // options.SchemaFilter<EnumSchemaFilter>();
    options.UseInlineDefinitionsForEnums();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
    options.IncludeXmlComments(xmlPath);
    options.CustomSchemaIds(x => x.FullName);
    //options.EnableAnnotations(enableAnnotationsForInheritance: true, enableAnnotationsForPolymorphism: true);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateAudience = false, // Oluþturulacak token deðerini kimlerin/hangi originlerin/sitelerin kullanacaðýný belirlediðimiz alandýr.
        ValidateIssuer = false, // Oluþturulacak token deðerini kimin daðýttýðýný ifade edeceðimiz alandýr.
        ValidateLifetime = true, // Oluþturulan token deðerinin süresini kontrol edecek olan doðrulamadýr.
        ValidateIssuerSigningKey = true, // Üretilecek token deðerinin uygulamamýza ait bir deðer olduðunu ifade eden security key verisinin doðrulamasýdýr.
        ValidIssuer = "Issuer",
        ValidAudience = "Audience",
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VS7djKZZ79RvVXJgH7RsefzvqtbqsFqLxbCzjwLvtqj4Jq2fJzZ7UMrERz8XtEAE")),
        ClockSkew = TimeSpan.Zero // Üretilecek token deðerinin expire süresinin belirtildiði deðer kadar uzatýlmasýný saðlayan özelliktir. 
    };
});




var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseHttpsRedirection();

//app.Use(async (context, next) =>
//{
//    context.Features.Get<IHttpMaxRequestBodySizeFeature>().MaxRequestBodySize = int.MaxValue;
//    await next.Invoke();
//});

//app.UseMiddleware<LoggerMiddleware>();

app.UseExceptionHandler(c => c.Run(async context =>
{
    var exception = context.Features
        .Get<IExceptionHandlerPathFeature>()
        .Error;
    var response = new BaseResponse<object> { Data = null, HasError = true, ErrorMessage = exception.Message };
    await context.Response.WriteAsJsonAsync(response);
}));

app.UseCors("CampaignApiCors");


app.UseAuthentication();
app.UseAuthorization();


app.UseCors(x => x
               .AllowAnyMethod()
               .AllowAnyHeader()
               .SetIsOriginAllowed(origin => true) // allow any origin
               .AllowCredentials());

app.MapControllers();

app.Run();
