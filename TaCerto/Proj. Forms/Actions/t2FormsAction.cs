using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TaCerto.Models;

namespace TaCerto.Forms.Controllers {
    public partial class FormsController {
        public IActionResult t2(){
            return View(GetPath("t2"));
        }
    }
}