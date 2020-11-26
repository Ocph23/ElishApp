﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    public interface IBaseController<T>
    {
        Task<IActionResult> Get();
        Task<IActionResult> Get(int id);
        Task<IActionResult> Post([FromBody] T value);
        Task<IActionResult> Put(int id, [FromBody] T value);
        Task<IActionResult> Delete(int id);
    }
}
