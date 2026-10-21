using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;


public class MembersController(AppDBContext context) : BaseApiController
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AppUser>>> GetMembers()
    {
        var members = await context.Users.ToListAsync();
        return members;
    }

    [Authorize]
    [HttpGet("{id}")] //localhost:5001/api/members/cosmin-id
    public async Task<ActionResult<AppUser>> GetMember(string id)
    {
        var member = await context.Users.FindAsync(id);
        if (member == null) return NotFound();
        return member;
    }

    [HttpDelete("{id}")] //localhost:5001/api/members/cosmin-id
    public async Task<ActionResult> DeleteMember(string id)
    {
        var member = await context.Users.FindAsync(id);
        if (member == null) return NotFound();
        context.Users.Remove(member);
        await context.SaveChangesAsync();
        return Ok($"{member.DisplayName} was deleted");
    }

    [HttpDelete] //localhost:5001/api/members
    public async Task<ActionResult> DeleteAllMembers()
    {
        var members = await context.Users.ToListAsync();
        if (members.Count == 0) return NotFound();
        context.Users.RemoveRange(members);
        await context.SaveChangesAsync();
        return Ok($"{members.Count} members were deleted");
    }
}