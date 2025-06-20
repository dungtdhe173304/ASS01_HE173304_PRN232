using BusinessObject.Models;
using DataAccessObject;

namespace Repositories;

public class MemberRepository : GenericRepository<Member>, IMemberRepository
{
    private readonly IMemberDAO _memberDAO;

    public MemberRepository(IMemberDAO memberDAO) : base(memberDAO)
    {
        _memberDAO = memberDAO;
    }

    public Member? GetByEmail(string email)
    {
        return _memberDAO.GetByEmail(email);
    }

    public IEnumerable<Member> GetMembersByCountry(string country)
    {
        return _memberDAO.GetMembersByCountry(country);
    }
}