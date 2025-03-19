﻿using BusinessObject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repo.IRepository
{
    public interface IBatchRepository
    {
        Task<Batch?> GetLatestBatchByProductIdAsync(long productId);
    }

}
