using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Products.Queries.GetPagedProductsList;
using ProductService.Application.Features.Products.Queries.GetProduct;
using ProductService.Application.Models;
using System.Net;

namespace ProductService.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// get product
        /// </summary>
        /// <param name="id"></param>
        /// <returns>product</returns>
        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(typeof(ProductVm), (int)HttpStatusCode.OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ProductVm>> GetProduct([FromRoute] int id)
        {
            return Ok(await _mediator.Send(new GetProductQuery(id)));
        }

        /// <summary>
        /// get paged product list
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns>paged product list</returns>
        [HttpGet(Name = "GetPagedProductsList")]
        [ProducesResponseType(typeof(PagedResult<PagedProductsListVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<PagedProductsListVm>>> GetPagedProductsListQuery([FromQuery] int currentPage, [FromQuery] int pageSize)
        {
            return Ok(await _mediator.Send(new GetPagedProductsListQuery(currentPage, pageSize)));
        }

        /// <summary>
        /// create product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommandRequest request)
        {
            return Ok(await _mediator.Send(new CreateProductCommand(request)));
        }

        /// <summary>
        /// update product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut(Name = "UpdateProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductCommandRequest request)
        {
            await _mediator.Send(new UpdateProductCommand(request));

            return NoContent();
        }

        /// <summary>
        /// delete product
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}", Name = "DeleteProduct")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesDefaultResponseType]
        public async Task<ActionResult> DeleteProduct([FromRoute] int id)
        {
            await _mediator.Send(new DeleteProductCommand(id));

            return NoContent();
        }
    }
}
