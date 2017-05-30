using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace PerSymplex.Models
{
    public class TwoDimensionalArrayBinder<T> : IModelBinder
	{
	    public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
	    {
	        string key = bindingContext.ModelName;
	        try
	        {
	            int totalRows = 0;
	            while (bindingContext.ValueProvider.GetValue(key + totalRows) != null)
	                totalRows++;
	            ValueProviderResult[] val = new ValueProviderResult[totalRows];
	            for (int i = 0; i<totalRows; i++)
	                val[i] = bindingContext.ValueProvider.GetValue(key + i);
	            if (val.Length > 0)
	            {
	                int totalColumn = ((string[])val[0].RawValue).Length;
	                T[,] twoDimensionalArray = new T[totalRows, totalColumn];
	                StringBuilder attemptedValue = new StringBuilder();
	                for (int i = 0; i<totalRows; i++)
	                {
	                    for (int j = 0; j<totalColumn; j++)
	                    {
	                        twoDimensionalArray[i, j] = (T)Convert.ChangeType(((string[])val[i].RawValue)[j], typeof(T));
	                        attemptedValue.Append(twoDimensionalArray[i, j]);
	                    }
	                }
	                bindingContext.ModelState.SetModelValue(key, new ValueProviderResult(twoDimensionalArray, attemptedValue.ToString(), CultureInfo.InvariantCulture));
	                return twoDimensionalArray;
	            }
	        }
	        catch
	        {
	            bindingContext.ModelState.AddModelError(key, "Data is not in correct Format");
	        }
                return null;
	    }
	}
}