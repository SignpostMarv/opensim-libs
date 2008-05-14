using System;

namespace HttpServer.Controllers
{
    /// <summary>
    /// Methods marked with BeforeFilter will be invoked before each request.
    /// </summary>
    /// <remarks>
    /// BeforeFilters should take no arguments and return false
    /// if controller method should not be invoked.
    /// </remarks>
    public class BeforeFilterAttribute : Attribute
    {
        private readonly FilterPosition _position;

        public BeforeFilterAttribute()
        {
            _position = FilterPosition.Between;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="position">Specify if the filter should be invoked among the first filters, in between or among the last.</param>
        public BeforeFilterAttribute(FilterPosition position)
        {
            _position = position;
        }

        public FilterPosition Position
        {
            get { return _position; }
        }
    }

    public enum FilterPosition
    {
        /// <summary>
        /// Filter will be invoked first (unless another filter is added after this one with the First position)
        /// </summary>
        First,
        /// <summary>
        /// Invoke after all first filters, and before the last filters.
        /// </summary>
        Between,
        /// <summary>
        /// Filter will be invoked last (unless another filter is added after this one with the Last position)
        /// </summary>
        Last
    }
}