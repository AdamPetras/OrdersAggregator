using Microsoft.AspNetCore.Mvc;
using OrdersAggregator.Contracts.Dtos;
using OrdersAggregator.Server.Business.Services.Orders;

namespace OrdersAggregator.Server.Controllers
{
    /// <summary>
    /// Accepts product order batches for asynchronous aggregation and dispatch.
    /// </summary>
    [ApiController]
    [Route("api/orders")]
    public sealed class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly ILogger<OrdersController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="OrdersController"/> class.
        /// </summary>
        /// <param name="orderService">The business service that validates and stores orders.</param>
        /// <param name="logger">The logger used to record submission outcomes.</param>
        public OrdersController(IOrderService orderService, ILogger<OrdersController> logger)
        {
            ArgumentNullException.ThrowIfNull(orderService);
            ArgumentNullException.ThrowIfNull(logger);

            _orderService = orderService;
            _logger = logger;
        }

        /// <summary>
        /// Accepts a batch of one or more product orders for asynchronous aggregation.
        /// </summary>
        /// <param name="request">The submitted order lines.</param>
        /// <param name="cancellationToken">The token used to cancel the request.</param>
        /// <returns>An accepted response when the batch is stored, or validation details when the payload is invalid.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ProductOrderResponseDto), StatusCodes.Status202Accepted)]
        [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ProductOrderResponseDto>> SubmitOrdersAsync(
            [FromBody] ProductOrderRequestDto? request,
            CancellationToken cancellationToken)
        {
            if (request is null)
            {
                OrdersControllerLogs.MissingRequestBody(_logger);

                return BadRequest(
                    new ValidationProblemDetails(
                        new Dictionary<string, string[]>
                        {
                            ["request"] = ["The request body must contain at least one order."],
                        })
                    {
                        Title = "Invalid order payload.",
                    });
            }

            try
            {
                OrderSubmissionResult submissionResult =
                    await _orderService.AddOrdersAsync(request.ProductOrders, cancellationToken);

                OrdersControllerLogs.SubmissionAccepted(_logger, submissionResult.AcceptedOrderCount);

                return Accepted(new ProductOrderResponseDto(submissionResult.AcceptedOrderCount));
            }
            catch (OrderSubmissionValidationException ex)
            {
                Dictionary<string, string[]> errors =
                    ex.Errors.ToDictionary(static pair => pair.Key, static pair => pair.Value, StringComparer.Ordinal);

                OrdersControllerLogs.SubmissionRejected(_logger, errors.Count);

                return BadRequest(
                    new ValidationProblemDetails(errors)
                    {
                        Title = "Invalid order payload.",
                    });
            }
        }
    }
}
