using BusinessObject.Models;
using Microsoft.EntityFrameworkCore;

namespace DataAccessObject;

public class MemberDAO : GenericDAO<Member>, IMemberDAO
{
    public Member? GetByEmail(string email)
    {
        return _context.Members.FirstOrDefault(m => m.Email == email);
    }

    public IEnumerable<Member> GetMembersByCountry(string country)
    {
        return _context.Members.Where(m => m.Country == country).ToList();
    }
    
    // Override GetById for Member entity
    public new Member? GetById(object id)
    {
        if (id is int memberId)
        {
            return _context.Members
                .Include(m => m.Orders)
                .FirstOrDefault(m => m.MemberId == memberId);
        }
        return null;
    }
}