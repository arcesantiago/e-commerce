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
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
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
        public async Task<ActionResult<ProductVm>> GetProduct([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new GetProductQuery(id), cancellationToken));
        }

        /// <summary>
        /// get paged product list
        /// </summary>
        /// <param name="currentPage"></param>
        /// <param name="pageSize"></param>
        /// <returns>paged product list</returns>
        [HttpGet(Name = "GetPagedProductsList")]
        [ProducesResponseType(typeof(PagedResult<PagedProductsListVm>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<PagedResult<PagedProductsListVm>>> GetPagedProductsListQuery([FromQuery] int currentPage, [FromQuery] int pageSize, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new GetPagedProductsListQuery(currentPage, pageSize), cancellationToken));
        }

        /// <summary>
        /// create product
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost(Name = "CreateProduct")]
        [ProducesResponseType(typeof(int), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<int>> CreateProduct([FromBody] CreateProductCommandRequest request, CancellationToken cancellationToken = default)
        {
            return Ok(await _mediator.Send(new CreateProductCommand(request), cancellationToken));
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
        public async Task<ActionResult> UpdateProduct([FromBody] UpdateProductCommandRequest request, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new UpdateProductCommand(request), cancellationToken);

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
        public async Task<ActionResult> DeleteProduct([FromRoute] int id, CancellationToken cancellationToken = default)
        {
            await _mediator.Send(new DeleteProductCommand(id), cancellationToken);

            return NoContent();
        }
    }
}
