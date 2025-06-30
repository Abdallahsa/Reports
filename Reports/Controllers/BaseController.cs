using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reports.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseController : ControllerBase
    {
        private IMediator? mediator;

        protected IMediator _mediator => mediator ??= HttpContext.RequestServices.GetService<IMediator>()!;

    }
}
