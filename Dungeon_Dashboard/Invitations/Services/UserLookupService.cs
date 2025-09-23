using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Dungeon_Dashboard.Invitations.Services;

public interface IUserLookupService {
    Task<IReadOnlyList<UserSuggestionDto>> SearchByEmailPrefixAsync(
        string term,
        string? excludeUserId = null,
        int take = 10,
        CancellationToken ct = default);
}
public sealed record UserSuggestionDto(string Id, string Label, string Value);

public sealed class UserLookupService<TUser> : IUserLookupService
    where TUser : IdentityUser
{
    private readonly UserManager<TUser> _userManager;

    public UserLookupService(UserManager<TUser> userManager)
        => _userManager = userManager;

    public async Task<IReadOnlyList<UserSuggestionDto>> SearchByEmailPrefixAsync(
        string term, string? excludeUserId = null, int take = 10, CancellationToken ct = default) {
        term ??= string.Empty;
        var nEmail = _userManager.NormalizeEmail(term) ?? string.Empty;

        var query = _userManager.Users.AsQueryable();

        if (!string.IsNullOrEmpty(excludeUserId))
            query = query.Where(u => u.Id != excludeUserId);

        var items = await query
            .Where(u => EF.Functions.Like(u.NormalizedEmail!, nEmail + "%"))
            .OrderBy(u => u.Email)
            .Select(u => new UserSuggestionDto(
                u.Id,
                u.Email!,
                u.Email!
            ))
            .Take(take)
            .ToListAsync(ct);

        return items;
    }

}