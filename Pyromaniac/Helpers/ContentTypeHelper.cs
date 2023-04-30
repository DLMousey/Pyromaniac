namespace Pyromaniac.Helpers
{
    public static class ContentTypeHelper
    {
        #region Public Methods & Functions

        private static readonly List<string> ValidContentTypes = new() { "application/java-archive", "application/EDI-X12", "application/EDIFACT", "application/javascript", "application/octet-stream", "application/ogg", "application/pdf", "application/xhtml+xml", "application/x-shockwave-flash", "application/json", "application/ld+json", "application/xml", "application/zip", "application/x-www-form-urlencoded" };

        /// <summary>
        /// Fetch A Random ContentTpye Based On Internal Valid ContentTypes
        /// </summary>
        /// <returns></returns>
        private static string FetchContentType()
        {
            return ValidContentTypes.OrderBy(x => Guid.NewGuid()).First();
        }

        /// <summary>
        ///  Fetch A Random Status Code Based On Defined Status Codes In appsetting.config
        /// </summary>
        /// <returns></returns>
        private static string? FetchContentTypeByConfiguration()
        {
            var configValidContentTypes = ConfigurationHelper.Configuration["Pyromaniac:ContentTypes"]?.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            if (configValidContentTypes is null || configValidContentTypes.Count == 0)
                return null;

            return configValidContentTypes.OrderBy(x => Guid.NewGuid()).FirstOrDefault();
        }

        #endregion

        /// <summary>
        /// Fetch HTTP ContentType Based On Configuration (If Configuration Is Missing Use Internal Valid ContentTypes)
        /// </summary>
        /// <returns></returns>
        public static string Fetch()
        {
            return FetchContentTypeByConfiguration() ?? FetchContentType();
        }
    }
}
