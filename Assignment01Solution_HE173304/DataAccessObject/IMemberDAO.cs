using BusinessObject.Models;

namespace DataAccessObject;

public interface IMemberDAO : IGenericDAO<Member>
{
    Member? GetByEmail(string email);
    IEnumerable<Member> GetMembersByCountry(string country);
}