using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApiGateWay.Entidades.Context;
using Microsoft.Extensions.Configuration;
namespace WebApiGateWay.Api.Controllers;

public class RequestToApplogController: ControllerBase
{
    private readonly HttpContext _httpContext;
    private readonly string _action;
    public RequestToApplogController(HttpContext httpContext, string action)
    {
        _httpContext = httpContext;
        _action = action;
    }

    public async Task<ObjectResult> sendRequestToApplog(HttpContext httpContext,string action )
    {
        try
        {
            var body = httpContext.Response.Body;
            return new ObjectResult(1);
        }
        catch (HttpRequestException e)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, "Se ha producido un error en el servidor.");
        }
    }
    public class ApplogResponse
    {
        public int StatusCode { get; set; }
        public object Body { get; set; }
    }
}