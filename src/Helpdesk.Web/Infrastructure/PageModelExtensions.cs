using Helpdesk.Web.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Helpdesk.Web.Infrastructure;

public static class PageModelExtensions
{
    public static RedirectToPageResult RedirectToPageWithError(this PageModel page, string pageName, ServiceError error)
    {
        page.TempData[PageMessages.Error] = ServiceErrorMessages.For(error);
        return page.RedirectToPage(pageName);
    }
}
