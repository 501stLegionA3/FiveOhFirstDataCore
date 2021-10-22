using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Transfer;
using FiveOhFirstDataCore.Data.Structuresbase;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Services;

public class TransferService : ITransferService
{
    private readonly IDbContextFactory<ApplicationDbContext> _dbContextFactory;
    private readonly IWebsiteSettingsService _websiteSettingsService;

    public TransferService(IDbContextFactory<ApplicationDbContext> dbContextFactory, IWebsiteSettingsService websiteSettingsService)
        => (_dbContextFactory, _websiteSettingsService) = (dbContextFactory, websiteSettingsService);

    #region Private Methods
    /// <summary>
    /// Check to see if the transfer request is complete.
    /// </summary>
    /// <param name="request">The request to check.</param>
    /// <returns>True if the request is completed, false if not.</returns>
    private bool IsTransferRequestComplete(TransferRequest request)
    {
        foreach(var s in request.FiledTo)
            if (!request.ApprovedAt.Contains(s))
                return false;

        return true;
    }

    private void FinalizeTransferAsync(TransferRequest request)
    {
        request.TransferFor.Slot = request.TransferTo;
        request.TransferFor.Role = request.TransferToRole;

        request.TransferFor.LastBilletChange = DateTime.UtcNow;
        request.TransferFor.LastUpdate = DateTime.UtcNow;
    }
    #endregion

    public async Task<TransferResult> ApproveTransferRequestAsync(TransferRequest request, int approvedBy, List<Slot> approvedAt)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();

        var actual = await _dbContext.FindAsync<TransferRequest>(request.Key);

        if (actual is null)
            return new(false, null, new List<string> { "No transfer was found to approve." });

        await _dbContext.Entry(actual).Collection(e => e.Signees).LoadAsync();

        var signee = await _dbContext.FindAsync<Trooper>(approvedBy);

        if (signee is null)
            return new(false, null, new List<string> { "No approver was found." });

        // Allow someone to approve a promotion at multiple levels if they have a capacity to do so.
        actual.ApprovedAt.AddRange(approvedAt);

        if(!actual.Signees.Any(x => x.Id == approvedBy))
            actual.Signees.Add(signee);

        if (IsTransferRequestComplete(actual))
        {
            actual.Completed = true;
            actual.Approved = true;

            await _dbContext.Entry(actual).Reference(e => e.TransferFor).LoadAsync();

            FinalizeTransferAsync(actual);
        }

        await _dbContext.SaveChangesAsync();
        return new(true, actual, null);
    }

    public async Task<TransferResult> DenyTransferRequestAsync(TransferRequest request, int deniedBy, Slot deniedAt, string deniedReason)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();

        var actual = await _dbContext.FindAsync<TransferRequest>(request.Key);

        if (actual is null)
            return new(false, null, new List<string> { "No transfer was found to approve." });

        actual.Completed = true;
        actual.Approved = false;
        actual.DeniedAt = deniedAt;
        actual.DeniedById = deniedBy;
        actual.Reason = deniedReason;

        await _dbContext.SaveChangesAsync();
        return new(true, actual, null);
    }

    public async Task<TransferResult> FileTransferRequestAsync(TransferRequest request, int transferFor, int filedBy, Slot filedAt)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();

        request.FiledById = filedBy;
        request.TransferForId = transferFor;

        var signee = await _dbContext.FindAsync<Trooper>(filedBy);

        if (signee is null)
            return new(false, null, new List<string> { "No approver was found." });

        request.FiledTo.Add(filedAt);
        request.Signees.Add(signee);
        request.ApprovedAt.Add(filedAt);

        // TODO: Connect to website settings service to get the needed approval locations.
        throw new NotImplementedException();

        await _dbContext.AddAsync(request);
        await _dbContext.SaveChangesAsync();
        return new(true, request, null);
    }

    public Task<List<TransferRequest>> GetPendingTransfers(Slot slot)
    {
        throw new NotImplementedException();
    }

    public Task<List<Trooper>> GetTransferableTroopersAsync(int id)
    {
        throw new NotImplementedException();
    }
}
