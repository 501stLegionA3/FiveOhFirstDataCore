using FiveOhFirstDataCore.Data.Account;
using FiveOhFirstDataCore.Data.Structures;
using FiveOhFirstDataCore.Data.Structures.Transfer;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FiveOhFirstDataCore.Data.Services;

public interface ITransferService
{
    public Task<TransferResult> FileTransferRequestAsync(TransferRequest request, int transferFor, int filedBy, Slot filedAt);
    public Task<TransferResult> ApproveTransferRequestAsync(TransferRequest request, int approvedBy, List<Slot> approvedAt);
    public Task<TransferResult> DenyTransferRequestAsync(TransferRequest request, int deniedBy, Slot deniedAt, string deniedReason);
    public Task<List<Trooper>> GetTransferableTroopersAsync(int id);
    public Task<List<TransferRequest>> GetPendingTransfers(Slot slot);
}
