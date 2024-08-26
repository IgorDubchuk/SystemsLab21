using Microsoft.AspNetCore.Mvc;
using SharedKernel;

namespace Api.Infrastructure
{
    internal static class ApiResponseExtensions
    {
        internal static ActionResult<TResponse> GetApiResponse<TResponse> (this Result<TResponse> result, ILogger logger)
        {
            if (result == null)
                throw new ArgumentNullException ("Result should not be null");
            if (result.IsFailure)
            {
                logger.LogError("Error while API request processing: " + result.Error.Description);
                return GetResult(result.Error);
            }
            else
            {
                return new ObjectResult(result.Value)
                {
                    StatusCode = 200,
                };
            }
        }


        private static ObjectResult GetResult(Error error)
        {
            var result = new ObjectResult("Error while request processing: " + error.Description);
            result.StatusCode = 500;
            foreach (var conf in ErrorResultConfigurations)
            {
                if (conf.ErrorCode == error.Code)
                    result.StatusCode = conf.HttpCode;
            }
            return result;
        }


        private static readonly List<ErrorResultConfiguration> ErrorResultConfigurations =
        [
            new(errorCode :  "Common.ValidationError",
                httpCode : 400),
            new(errorCode :  "GetSafeRoute.CreatingStationGraphNodesError",
                httpCode : 400),
            new(errorCode :  "GetSafeRoute.CreatingStationGraphEdgesError",
                httpCode : 400),
            new(errorCode :  "GetSafeRoute.CreatingRoutesError",
                httpCode : 400)
        ];

        public class ErrorResultConfiguration
        {
            public ErrorResultConfiguration(string errorCode, int httpCode)
            {
                ErrorCode = errorCode;
                HttpCode = httpCode;
            }

            public string ErrorCode { get; set; }
            public int HttpCode { get; set; }
        }

    }
}
