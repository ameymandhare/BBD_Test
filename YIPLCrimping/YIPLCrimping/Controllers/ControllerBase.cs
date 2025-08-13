using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json.Linq;

public abstract class ControllerBase : Controller
{
    internal string RequestorIP = "";
    public JObject ResponseData { get; set; }

    public ControllerBase() : base() => this.ResponseData = new JObject();

    public override void OnActionExecuting(ActionExecutingContext context) => this.InitializeController();

    public abstract void InitializeController();

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
    }

    [NonAction]
    public async Task<JsonResult> FinalizeEmpty()
    {
        this.ResponseData = JObject.FromObject(new JObject());
        return await Task<JsonResult>.Run(new Func<JsonResult>(() => this.PrepareResponseMessage(this.ResponseData)));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeSingle<T>(T data)
    {
        this.ResponseData = JObject.FromObject(data);
        ResponseData.Remove("Id");
        ResponseData.Remove("Exception");
        ResponseData.Remove("Status");
        ResponseData.Remove("IsCanceled");
        ResponseData.Remove("IsCompleted");
        ResponseData.Remove("CreationOptions");
        ResponseData.Remove("AsyncState");
        ResponseData.Remove("IsFaulted");
        int statusCode = 200; // Default to 200
        bool HasError = false;
        bool Success = true;

        // Ensure "Result" is a JObject before accessing nested properties
        var resultObject = ResponseData;

        if (resultObject?["StatusCode"] != null)
        {
            statusCode = (int)resultObject["StatusCode"];
            resultObject.Remove("StatusCode"); // Remove from the nested "Result" JObject
        }

        if (resultObject?["HasError"] != null)
        {
            HasError = (bool)resultObject["HasError"];
            resultObject.Remove("HasError");
        }

        if (resultObject?["Success"] != null)
        {
            Success = (bool)resultObject["Success"];
            resultObject.Remove("Success");
        }

        // Construct the final response
        this.ResponseData = new JObject
        {
            {"Result", resultObject}, // Pass the modified resultObject
            {"HasError", HasError},
            {"StatusCode", statusCode},
            {"Success", Success}
        };
        return await Task.Run(() => this.PrepareResponseMessage(this.ResponseData));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeMultiple<T>(T data)
    {
        if (this.ResponseData == null)
        {
            this.ResponseData = new JObject();
        }
        this.ResponseData.Add("Rows", JArray.FromObject(data));
        return await Task<JsonResult>.Run(new Func<JsonResult>(() => this.PrepareResponseMessage(this.ResponseData)));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeMessage(string message)
    {
        this.ResponseData = new JObject
            {
                { "Message", message }
            };
        return await Task<JsonResult>.Run(new Func<JsonResult>(() => this.PrepareResponseMessage(this.ResponseData)));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeJObject(JObject data)
    {
        this.ResponseData = data;
        if (this.ResponseData == null)
        {
            this.ResponseData = new JObject();
        }
        return await Task<JsonResult>.Run(new Func<JsonResult>(() => this.PrepareResponseMessage(this.ResponseData)));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeJArray(JArray data)
    {
        if (this.ResponseData == null)
        {
            this.ResponseData = new JObject();
        }
        this.ResponseData.Add("Rows", data);
        return await Task<JsonResult>.Run(new Func<JsonResult>(() => this.PrepareResponseMessage(this.ResponseData)));
    }

    [NonAction]
    public async Task<JsonResult> FinalizeException(Exception ex)
    {
        this.ResponseData = new JObject
        {
            { "HasError", true },
            { "Message", ex.Message },
            { "Details", ex.StackTrace }, // Optional: remove or obfuscate in production
            { "StatusCode", 500 },
            { "Success", false }
        };
        return await Task.Run(() => this.PrepareResponseMessage(this.ResponseData));
    }

    [NonAction]
    public JsonResult PrepareResponseMessage(JObject data) => this.Json(data);
}