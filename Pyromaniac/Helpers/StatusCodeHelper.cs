namespace Pyromaniac.Helpers
{
    public static class StatusCodeHelper
    {
        #region Public Methods & Functions

        private static readonly List<int> ValidResponseCodes = new() { 400, 401, 403, 404, 405, 406, 408, 415, 500, 501, 502, 503, 504 };

        /// <summary>
        /// Fetch A Random Status Code Based On Internal Valid Status Codes
        /// </summary>
        /// <returns></returns>
        private static int FetchStatusCode()
        {
            return ValidResponseCodes.OrderBy(x => Guid.NewGuid()).First();
        }

        /// <summary>
        ///  Fetch A Random Status Code Based On Defined Status Codes In appsetting.config
        /// </summary>
        /// <returns></returns>
        private static int? FetchStatusCodeByConfiguration()
        {
            var configValidStatusCodes = ConfigurationHelper.Configuration["Pyromaniac:StatusCodes"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();

            if (configValidStatusCodes is null || configValidStatusCodes.Count == 0)
                return null;

            return configValidStatusCodes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// Fetch HTTP StatusCode Based On Configuration (If Configuration Is Missing Use Internal Valid HTTP Status Codes)
        /// </summary>
        /// <returns></returns>
        public static int Fetch()
        {
            return FetchStatusCodeByConfiguration() ?? FetchStatusCode();
        }
    }
}
