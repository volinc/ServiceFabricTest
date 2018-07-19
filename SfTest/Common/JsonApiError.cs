using System.ComponentModel;

namespace Common
{
    /// <summary>
    /// Описывает <see cref="!:http://jsonapi.org/format/#error-objects">ошибку из JSON API</see>.
    /// </summary>    
    public class JsonApiError
    {
        /// <summary>
        /// Возвращает или задает уникальный идентификатор ошибочной ситуации.
        /// </summary>        
        public string Id { get; set; }

        /// <summary>
        /// Возвращает или задает HTTP-статус, представленный как строковое значение.
        /// </summary>        
        public string Status { get; set; }

        /// <summary>
        /// Возвращает или задает код ошибки, специфичный для приложения.
        /// </summary>        
        public string Code { get; set; }

        /// <summary>
        /// Возвращает или задает краткое описание ошибки.
        /// </summary>        
        [Localizable(true)]
        public string Title { get; set; }

        /// <summary>
        /// Возвращает или задает детальное описание ошибки.
        /// </summary>        
        [Localizable(true)]
        public string Detail { get; set; }
    }
}
