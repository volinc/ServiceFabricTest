using System.Collections.Generic;

namespace Common
{
    /// <summary>
    /// Описывает объект, который возвращается в теле (content) ответа HTTP при возникновении ошибок.
    /// </summary>
    /// <remarks>Подробности в <see cref="!:http://jsonapi.org/format/#document-top-level">JSON API</see>.</remarks>    
    public class JsonApiErrors
    {
        /// <summary>
        /// Возвращает список ошибок.
        /// </summary>        
        public ICollection<JsonApiError> Errors { get; set; }
    }
}
