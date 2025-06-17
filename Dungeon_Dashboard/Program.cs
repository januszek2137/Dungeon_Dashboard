using Dungeon_Dashboard.Areas.Identity.Pages.Account.EmailSender;
using Dungeon_Dashboard.CharactersEncounterGeneration;
using Dungeon_Dashboard.Group;
using Dungeon_Dashboard.Group.Notes;
using Dungeon_Dashboard.Home;
using Dungeon_Dashboard.Invitations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();

builder.Services.AddSingleton<IDataService, DataService>();

builder.Services.AddSingleton<ICharacterGeneratorService, ContentGenerationService>();

builder.Services.AddSingleton<IUserIdProvider, NameUserIdProvider>();

builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
);

builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<AppDBContext>();

builder.Services.ConfigureApplicationCookie(options => {
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
});

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.AddConsole();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(!app.Environment.IsDevelopment()) {
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

using(var scope = app.Services.CreateScope()) {
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDBContext>();
    dbContext.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<NotificationHub>("/notificationhub");
app.MapHub<ParticipantsHub>("/participantsHub");
app.MapHub<NotesHub>("/notesHub");

app.Run();
