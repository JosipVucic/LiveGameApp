using LiveGameApp.ViewModels.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiveGameApp.ViewModels
{
    public class LoadParams
    {

        [FromQuery(Name = "page")]
        [Required]
        [Range(0, int.MaxValue)]
        public int Page { get; set; }

        [FromQuery(Name = "perPage")]
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "PageSize must be positive number")]
        public int PerPage { get; set; }

        [FromQuery(Name = "field")]
        public string Field { get; set; }

        [FromQuery(Name = "order")]
        public string Order { get; set; }
        [BindNever]
        public string SimpleFilter
        {
            get
            {
                try
                {
                    if (Filter == null)
                        return "";
                    else
                    { SimpleFilter filter = JsonSerializer.Deserialize<SimpleFilter>(Filter);
                        return filter.q == null ? "": filter.q;
                    }
                }
                catch (Exception)
                {
                    return "";
                }
            }
        }

        [FromQuery(Name = "filter")]
        public string Filter { get; set; }

        [FromQuery(Name = "target")]
        public string Target { get; set; }

        [FromQuery(Name = "ids")]
        public string Ids { get; set; }

        [BindNever]
        public int Start => (Page - 1) * PerPage;

        [BindNever]
        public int[] IdList
        {
            get
            {
                try { return JsonSerializer.Deserialize<int[]>(Ids == null ? "[]" : Ids); }
                catch (Exception)
                {
                    return new int[0];
                }
            }
        }
        [BindNever]
        public string[] StringIdList
        {
            get
            {
                try { return JsonSerializer.Deserialize<string[]>(Ids == null ? "[]" : Ids); }
                catch (Exception)
                {
                    return new string[0];
                }
            }
        }

        [BindNever]
        public bool Descending => !string.IsNullOrWhiteSpace(Order) && Order.Equals("DESC", StringComparison.OrdinalIgnoreCase);

    }
}
