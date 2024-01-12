using System;
using System.IO;
using System.Net;
using Godot;

public partial class HttpListenerNode : Node
{

    public int Port = 8080;
    private HttpListener _listener;

    [Signal]
    public delegate void CostumeChangeEventHandler(string newCostume);

    [Signal]
    public delegate void WeatherChangeEventHandler(string newWeather);

    [Signal]
    public delegate void LightingEffectEventHandler(string type);

    [Signal]
    public delegate void AttachmentEventHandler(string type);

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        _listener = new HttpListener();
        _listener.Prefixes.Add("http://*:" + Port.ToString() + "/");
        _listener.Start();
        Receive();

    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void Stop()
    {
        _listener.Stop();
    }

    private void Receive()
    {
        _listener.BeginGetContext(new AsyncCallback(ListenerCallback), _listener);
    }

    private void ProcessUrl(string path)
    {

        string lc = path.ToLower();

        string[] split = lc.Split('/', StringSplitOptions.RemoveEmptyEntries);

        // In try block incase the split is the wrong length
        // We don't care that much, fuck it.
        try
        {
            if (split[0] == "costume")
            {
                string costume = split[1];
                CallDeferred(MethodName.EmitSignal, SignalName.CostumeChange, costume);
            }
            else if (split[0] == "weather")
            {
                string weatherType = split[1];
                CallDeferred(MethodName.EmitSignal, SignalName.WeatherChange, weatherType);
            }
            else if (split[0] == "lighting")
            {
                string type = split[1];
                CallDeferred(MethodName.EmitSignal, SignalName.LightingEffect, type);
            }
            else if (split[0] == "attachment")
            {
                string type = split[1];
                CallDeferred(MethodName.EmitSignal, SignalName.Attachment, type);
            }

        }
        catch (Exception ex)
        {
            GD.Print(ex.Message);
        }

    }

    private void ListenerCallback(IAsyncResult result)
    {
        if (_listener.IsListening)
        {
            var context = _listener.EndGetContext(result);
            var request = context.Request;

            // do something with the request
            GD.Print($"{request.Url}");

            Console.WriteLine($"{request.HttpMethod} {request.Url}");

            ProcessUrl(request.Url.AbsolutePath);

            if (request.HasEntityBody)
            {
                var body = request.InputStream;
                var encoding = request.ContentEncoding;
                var reader = new StreamReader(body, encoding);
                if (request.ContentType != null)
                {
                    Console.WriteLine("Client data content type {0}", request.ContentType);
                }
                Console.WriteLine("Client data content length {0}", request.ContentLength64);

                Console.WriteLine("Start of data:");
                string s = reader.ReadToEnd();
                Console.WriteLine(s);
                Console.WriteLine("End of data:");
                reader.Close();
                body.Close();
            }

            var response = context.Response;
            response.StatusCode = (int)HttpStatusCode.OK;
            response.ContentType = "text/plain";
            response.OutputStream.Write(Array.Empty<byte>(), 0, 0);
            response.OutputStream.Close();

            Receive();
        }
    }
}
