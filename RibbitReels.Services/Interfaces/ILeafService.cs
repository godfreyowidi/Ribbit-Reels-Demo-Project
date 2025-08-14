using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using RibbitReels.Data.Models;
using RibbitReels.Services.Shared;

namespace RibbitReels.Services.Interfaces;
public interface ILeafService
{
    Task<OperationResult<Leaf>> CreateLeafAsync(Guid branchId, Leaf leaf, IFormFile file);
    Task<OperationResult<Leaf>> GetLeafByIdAsync(Guid id);
    Task<OperationResult<List<Leaf>>> GetLeafsAsync();

    Task<OperationResult<List<Leaf>>> GetLeafsByBranchIdAsync(Guid branchId);
    Task<OperationResult<Leaf>> UpdateLeafAsync(Guid id, Leaf updatedLeaf);
    Task<OperationResult<bool>> DeleteLeafAsync(Guid id);
}
