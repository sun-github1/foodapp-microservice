using Food.Web.Services.Interfaces;
using Food.Web.Services;
using Food.Web;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();


builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme= "Cookies";
    options.DefaultChallengeScheme = "oidc";

})
    .AddCookie("Cookies",c=>c.ExpireTimeSpan=TimeSpan.FromMinutes(5))
    .AddOpenIdConnect("oidc", options => {
        options.Authority = builder.Configuration["ServiceUrls:IdentityServerAPI"];
        options.GetClaimsFromUserInfoEndpoint = true;
        options.ClientId = "food";
        options.ClientSecret = "secret";
        options.ResponseType = "code";
        options.TokenValidationParameters.NameClaimType = "name";
        options.TokenValidationParameters.RoleClaimType = "role";
        options.Scope.Add("food");
        options.SaveTokens = true;
    });


builder.Services.AddHttpClient();
builder.Services.AddHttpClient<IProductService, ProductService>();
StartingDetails.ProductAPIbase = builder.Configuration["ServiceUrls:ProductAPI"];
StartingDetails.ShoppingCartAPIAPIbase = builder.Configuration["ServiceUrls:ShoppingCartAPI"];
builder.Services.AddScoped<IBaseService, BaseService>();
builder.Services.AddScoped<IProductService, ProductService>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
