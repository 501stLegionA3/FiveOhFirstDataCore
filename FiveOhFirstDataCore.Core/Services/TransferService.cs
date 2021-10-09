using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Transfer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Services;

public class TransferService : ITransferService
{
    public Task<TransferResult> ApproveTransferRequestAsync(TransferRequest request, int approvedBy, Slot approvedAt)
    {
        throw new NotImplementedException();
    }

    public Task<TransferResult> DenyTransferRequestAsync(TransferRequest request, int deniedBy, Slot deniedAt)
    {
        throw new NotImplementedException();
    }

    public Task<TransferResult> FileTransferRequestAsync(TransferRequest request, int filedBy, Slot filedAt)
    {
        throw new NotImplementedException();
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
