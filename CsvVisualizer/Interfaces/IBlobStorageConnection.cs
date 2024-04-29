﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CsvVisualizer.Interfaces
{
    public interface IBlobStorageConnection
    {
        Task<bool> SaveFileAsync(string fullPath, Stream file);
        Task<MemoryStream?> DownloadFileAsync(string fullPath);

        Task<DateTimeOffset?> GetFileModificationAsync(string fullPath);
    }
}
