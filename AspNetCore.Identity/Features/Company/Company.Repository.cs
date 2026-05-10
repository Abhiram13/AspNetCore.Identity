using AspNetCore.Identity.Features.Companies.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Features.Companies.Repository;

public sealed class CompanyRepository
{
    private readonly BusinessDbContext _businessDbContext;

    public CompanyRepository(BusinessDbContext businessDbContext)
    {
        _businessDbContext = businessDbContext;
    }

    public async Task CreateOneCompanyAsync(string companyName)
    {
        Company company = Company.Create(companyName);
        await _businessDbContext.AddAsync(company);
        await _businessDbContext.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<Company>> GetAllCompaniesAsync()
    {
        List<Company> allCompanies = await _businessDbContext.Companies.ToListAsync();
        return allCompanies;
    }

    public async Task CreateOneCompanyUserAsync(int companyId, int userId, int roleId)
    {
        CompanyUser companyUser = CompanyUser.Create(companyId, userId, roleId);
        await _businessDbContext.AddAsync(companyUser);
        await _businessDbContext.SaveChangesAsync();
    }

    public async Task<bool> IsAdminRoleExistsInCompanyAsync(int companyId, int roleId)
    {
        return await _businessDbContext.CompanyUsers.AnyAsync(x => x.CompanyId == companyId && x.RoleId == roleId);
    }
}
