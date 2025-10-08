using Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Console;

internal class MainDialog(IProductService productService)
{

    private readonly IProductService _productService = productService;

    public void Show()
    {

    }
}
