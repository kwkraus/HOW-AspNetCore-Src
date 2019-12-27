﻿using HOW.AspNetCore.Services.Lifetime;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace HOW.AspNetCore.Razor.WebApp.Pages.Lifetime
{
    public class IndexModel : PageModel
    {
        public IndexModel(
           OperationService operationService,
           IOperationTransient transientOperation,
           IOperationScoped scopedOperation,
           IOperationSingleton singletonOperation,
           IOperationSingletonInstance singletonInstanceOperation)
        {
            OperationService = operationService;
            TransientOperation = transientOperation;
            ScopedOperation = scopedOperation;
            SingletonOperation = singletonOperation;
            SingletonInstanceOperation = singletonInstanceOperation;
        }

        public OperationService OperationService { get; }
        public IOperationTransient TransientOperation { get; }
        public IOperationScoped ScopedOperation { get; }
        public IOperationSingleton SingletonOperation { get; }
        public IOperationSingletonInstance SingletonInstanceOperation { get; }

        public void OnGet()
        {

        }
    }
}