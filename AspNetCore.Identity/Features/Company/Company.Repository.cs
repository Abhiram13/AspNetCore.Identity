using AspNetCore.Identity.Features.Company.Entities;
using Microsoft.EntityFrameworkCore;

namespace AspNetCore.Identity.Features.Company.Repository;

using Company = AspNetCore.Identity.Features.Company.Entities.Company;

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
}
