﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KafkaProducer.Api.Data.Interfaces
{
    public interface IUnitOfWork
    {
        Task<bool> CommitAsync();
    }
}
