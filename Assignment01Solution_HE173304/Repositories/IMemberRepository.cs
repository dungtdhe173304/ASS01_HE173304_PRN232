using BusinessObject.Models;

namespace Repositories;

public interface IMemberRepository : IGenericRepository<Member>
{
    Member? GetByEmail(string email);
    IEnumerable<Member> GetMembersByCountry(string country);
}