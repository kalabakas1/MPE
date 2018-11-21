# Pinger
So this project contains of one application that can be installed with two different modes. A client mode that reads different matrics, and a server mode that is a selfhosted web service receiving all the metrics a client-mode application have gathered. All this data is then persisted into a storage, in this case I choose ElasticSearch. To make all this visual, I choose to use Grafana. The reason for this was that it contained user management and alerting, without having to do scripts (the end user is not tended to be a programmer).

If you watch the PDF version, i apologies about the indentations. You can find the configuration files and examples on my git hub account:  
[https://github.com/kalabakas1/MPE](https://github.com/kalabakas1/MPE)

## Motivation
Basically I needed some pretty simple metric about how the server acted, and I needed it without paying a high price in the sense of money. A lot of the good tools offer a lot of functionality that I don't need or use, so instead of buying a took to get my metric, I build a lightweight client. This project developed over time, so it not only gathers interesting metrics that the programmer or hosting can use, but can also gather data that the customer can use to identify the health of their business by quiring the MSSQL database.

## Storage
So the server stores the collected metrics in the database. This is done by using a specific kind of object called a MetricResult. It have the following parameters:

```csharp
private class Metric
{
    [JsonProperty("Timestamp")]
    public DateTime Timestamp { get; set; }
    [JsonProperty("Path")]
    public string Path { get; set; }
    [JsonProperty("Alias")]
    public string Alias { get; set; }
    [JsonProperty("Value")]
    public float Value { get; set; }
    [JsonProperty("Message")]
    public string Message { get; set; }
}
```

When it comes to visualizing the metrics, Grafana are dependent on the structure that is defined in the Path property. The string defined in the "Host" parameter in the connections.json configuration file is defining the base of Path. Meaning, that if "Host" have the value "FooBar", then a path could have the following format if the metric alias are "CpuPct":

    FooBar.CpuPct

That way you can have different servers for your customers piping their metric data to the same central metric endpoint. That way you can have a customer called FooBar having 3 webservers that have different host values in each of their local configuration files (field in connections.json called "Host"), e.g. FooBar.Web1, FooBar.Web2, and FooBar.Web3, and visualizing Cpu percentage graph spanning all three servers.

## Configuration
The big part of this is the client. The client reads two configuration files:
1. General configuration file defining central aspects of the execution of the pinger program
2. Connection configuration file that contains the different metrics and tests the program have to execute on the server it is installed on

### General configuration file
```json
{
    "Logging.MinimumLevel": "Debug",
    "Logging.File.MinimumLevel" :"Warning",
    "Logging.Console.MinimumLevel": "Verbose",
	
    "Logging.Elastic.MinimumLevel": "Debug",
    "Logging.Elastic.Url": "http://localhost:9200",
    "Logging.Elastic.IndexFormat": "customername_log_{0:yyyy.MM}",
	
    "Logging.Slack.MinimumLevel": "Error",
    "Logging.Slack.Url": "",
	
    "Logging.Sentry.Dsn": "",
	
    "Logging.CoolSms.Key": "",
    "Logging.CoolSms.FromName": "",
    "Logging.CoolSms.Phonenumbers": "",
	
    "MPE.Pinger.Configuration.Path": "./Configuration/connections.json",
    "MPE.Pinger.Fail1.Pause.Secs": "5",
    "MPE.Pinger.Fail2.Pause.Secs": "60",
    "MPE.Pinger.Fail3.Pause.Secs": "180",
    "MPE.Pinger.WaitBetweenTest.Secs": "30",
    "MPE.Pinger.Report.Inteval.Sec": "60",
    "MPE.Pinger.Metric.Inteval.Sec": "5",
    "MPE.Pinger.TimeSpan.From": "00:01",
    "MPE.Pinger.TimeSpan.To": "23:59",

    "MPE.Pinger.ApiKeys.Path": "keys.txt",
    "MPE.Pinger.Server.Host": "localhost",
    "MPE.Pinger.Server.Port": "8080",
    "MPE.Pinger.RetentionInDays": "5"
}
```
### General configurations
__Logging.*__  
Configuration to define the different way of logging. Normally used to define how it reports the failed alerts.

__MPE.Pinger.Configuration.Path__  
Path to the connections configuration file containing the tests and what metrics it needs to extract

### Client specific configurations
__MPE.Pinger.Fail?.Pause.Secs__ 
How may seconds it needs to pause after the first, second or third failed test

__MPE.Pinger.WaitBetweenTest.Secs__  
How may seconds between the different test executions

__MPE.Pinger.Report.Inteval.Sec__  
How often it reports its metrics to the report server

__MPE.Pinger.Metric.Inteval.Sec__  
How often it gathers metrics

__MPE.Pinger.TimeSpan.From__  
Start time for alerting period in case of failed test

__MPE.Pinger.TimeSpan.To__  
End time for alerting period in case of failed test

### Server specific configurations
__MPE.Pinger.ApiKeys.Path__  
Path to newline separated file containing valid API keys - used if the execution mode is "Server"

__MPE.Pinger.Server.Host__  
Host for the reporting server

__MPE.Pinger.RetentionInDays__  
How long it persist data in ElasticSearch

## Connections configuration file
This configuration file contains how it will gather the data and what tests it should execute while on the server. It also defines the naming for the different objects when persisted to storage, along with the report endpoint and api key.

```json
{
	"Host": "MpeLocal",
	"RestEndpoint": "http://localhost:8080",
	"ApiKey": "7dcb7c7a-8d9f-4b56-9ce0-52fa40085b35",
	"Metrics": [
		{
			"Alias": "CpuPct",
			"Category": "Processor",
			"Name": "% Processor Time",
			"Instance": "_Total"
		}
	],
	"EventLogging": {
		"MinimumLevel": "Information",
		"Categories": [
			"Application",
			"System"
		]
	},
	"Connections": [
		{
			"Alias": "Redis-Connect",
			"Target": "127.0.0.1",
			"Port": 6379,
			"Type": "Tcp"
		}
	],
	"Redis": {
		"Host": "127.0.0.1",
		"Port": 6379,
		"Metrics": [
			"total_connections_received",
			"total_commands_processed",
			"expired_keys",
			"used_memory",
			"used_memory_peak",
			"connected_clients",
			"used_cpu_sys",
			"uptime_in_seconds",
			"uptime_in_days",
			"maxmemory"
		]
	},
	"RabbitMQ": {
		"Host": "localhost",
		"Port": 15672,
		"Username": "guest",
		"Password": "guest",
		"Fields": [
			"messages_ready$",
			"messages_unacknowledged$",
			"messages$",
			"deliver$",
			"ack$",
			"publish$"
		]
	},
	"ElasticSearch": {
		"Host": "localhost",
		"Port": 9200,
		"Fields": [
			"indices.search",
			"jvm.mem"
		]
	},
	"HaProxy": {
		"Endpoint": "https://localhost:9000/haproxy_stats",
		"Username": "admin",
		"Password": "admin",
		"Fields": [
			"*.*.(?:hrsp_1xx|hrsp_2xx|hrsp_3xx|hrsp_4xx|hrsp_5xx)"
		]
	},
	"Sql": {
		"ConnectionStrings": {
			"Con1": "qweasdqweasd",
			"Con2": "lkajsdpodsfo"
		},
		"Queries": [{
				"Interval": 60,
				"Alias": "YYY",
				"ConnectionString": "Con1",
				"FilePath": "./Queries/XXX.sql"
			}
		]
	}
}

```

__Host__  
Defines the base of the Path the metric is specified under. e.g. Foobar.CpuPct. That way it is possible to target in Grafana.

__RestEndpoint__  
The HTTP endpoint the client should pipe its data to

__ApiKey__  
The clients api key that should be defined in the servers keys.txt, else the server will not accept the request.

__Metrics__  
So metrics are defined by using the following setup:

```json
{
    "Alias": "CpuPct",
    "Category": "Processor",
    "Name": "% Processor Time",
    "Instance": "_Total"
}
```

The Alias are used as a part of the Path. It will be appended to the Host property e.g. FooBar.CpuPct. The category are the metric property, name the metric name, and the Instance defines witch process it should target.

If you need a more detailed understanding of it (and I recommend it), then try do a google search on the keywords "performance counters"

__EventLogging__  
To make use of annotations in Grafana you can enable that the client gathers informations about from the EventViewer. Using the following format, you can define which level of information to gather and which event categories to listen to.

```json
{
    "MinimumLevel": "Information",
    "Categories": [
        "Application",
        "System"
    ]
}
```

__Connections__  
One of the more central aspects, and actually the reason I started this project, is the connections section of the configuration. This defines the different tests the client should perform, if it fails on one, it should do some re-tries before alerting (SeriLog)

```json
TCP-connection
{
    "Alias": "Redis-Connect",
    "Target": "127.0.0.1",
    "Port": 6379,
    "Type": "Tcp"
}

Web-connection
{
	"Alias": "some.website.dk",
	"Target": "some.website.dk",
	"Type": "Web",
	"Response": [200],
    "Contains": "Foobar 1234"
}

WindowsService
{
	"Alias": "MongoDB-Service",
	"Target": "MongoDB",
	"Type": "Service"
}

SSL-certificate
{
    "Alias": "Ssl.XXX",
    "Target":  "XXX.dk", 
    "DaysLeft": 2,
    "Type": "Ssl"
}

Powershell
{
    "Alias": "ScheduledTask",
    "Type": "Powershell",
    "Script":  ".\\Configuration\\Scripts\\LastExecutedScheduledTask.ps1 -jobName \"XXX\" -secDiffMax 86400" 
}
```

But basically it can test if a WindowsService (Service) are running, if a TCP connection (Tcp) are open on a specific port, or you get a specific response on a http address (Web).

The web based have a field containing the Response that is expected, this i required. Likewise you can define that it have to check for a specific string is a part of the returned response.

The Powershell tester just requires a Script that it needs to execute. In this case it will execute a ps1 file with some parameters. This enables easy reuse. Just rementer that it only returns $true or $false. That is the only thing it expects, all other data will mark the test as failed. Please observe the impact the script have on you machine like RAM and CPU.

There is also added the possibility of self-healing the service. That way it can fire a script when the alerting is about to fire. The script will get fired and wait for 30 sec, then run a single test to see if the service is restored. If not it will alert through the hub:

```json
{
	"Alias": "RabbitMQ-Service",
	"Target": "RabbitMQ",
	"Type": "Service",
	"Healing": {
		"Script": "Start-Service RabbitMQ"
	}
}
```

If the service is unstable and fakes that it works for 30 sec and then crashes, the alert will not get fired. This might be fixed in a later commit.

__Redis__  
So basically you want to keep track of you redis instance. Here I have implemented a way to get some basic information from the server. You just define the host and port, along with the given metrics you want to track. 

```json
{
    "Host": "127.0.0.1",
    "Port": 6379,
    "Metrics": [
        "total_connections_received",
        "total_commands_processed",
        "expired_keys",
        "used_memory",
        "used_memory_peak",
        "connected_clients",
        "used_cpu_sys",
        "uptime_in_seconds",
        "uptime_in_days",
        "maxmemory"
    ]
}
```

__RabbitMQ__    
When your infrastructure on a project you really want to know when there are queues that starts to pile up because a consumer perhaps fails, or isnt started. Again, you can define the host, port, and credentials. The differene are that RabbitMQ exposes a complex JSON structure containing all the metric information. Imagine you flatten this JSON structure to a key value collection. That way you can target the different metrics by using regex patterns to collect all the informations regarding the metric "messages_ready" by postfixing it with $

```json
{
    "Host": "localhost",
    "Port": 15672,
    "Username": "guest",
    "Password": "guest",
    "Fields": [
        "messages_ready$",
        "messages_unacknowledged$",
        "messages$",
        "deliver$",
        "ack$",
        "publish$"
    ]
}
```

__ElasticSearch__  
One of the central things I needed to keep track on was the usage of the heep. Here you can configure it the same ways as the other service providers, and define what fields you want. It uses the same "flatten-JSON-structure" approach, so it is possible to target it by regex.

```json
{
    "Host": "localhost",
    "Port": 9200,
    "Fields": [
        "indices.search",
        "jvm.mem"
    ]
}
```

__HaProxy__  
I think this was the most irritating gathing of data. HaProxy exposes data by exporting a CSV file with a lot of rows and cols. So to be able to get the data I needed i did a cross product of the cols and rows, and that way it was possible to target the correct fields. In the example you define the endpoint, credentials and the fields you want. Here I get the count on different response codes
```json
{
    "Endpoint": "https://localhost:9000/haproxy_stats",
    "Username": "admin",
    "Password": "admin",
    "Fields": [
        "*.*.(?:hrsp_1xx|hrsp_2xx|hrsp_3xx|hrsp_4xx|hrsp_5xx)"
    ]
}
```

__SQL__  
So this is the more business aspect of the project. This is now possible to define a SQL query that gets executed against a specific connection every defined seconds. As long as the result have the same fields as the MetricResult class (Alias, Message, Value)

```json
{
    "ConnectionStrings": {
	    "Con1": "qweasdqweasd",
	    "Con2": "lkajsdpodsfo"
    },
    "Queries": [{
		    "Interval": 60,
		    "Alias": "YYY",
		    "ConnectionString": "Con1",
		    "FilePath": "./Queries/XXX.sql"
	    }
    ]
}
```

## Client implementation
The one thing I want to know something about are the different Web API's that I exposes in a project. That way I needed a class that I could copy paste into any given project. In the ideal world you would create a nuget package that you could install and update. But it wasnt really my target for this project.

It basically just exposes a singleton that contains of one public method with the following signature:
```csharp
void Enqueue(long elapsed, string prefix, string endpoint, string responseCode)
```
It basically collects all of this information in a queue and flushes it to the metric server every 15 seconds. 

### Complete implementation of Reporter class

```csharp
public class Reporter
{
    private static Reporter _instance;
    private static object _lock = new object();
    private static Timer _timer;
    private static FixedConcurrentQueue<Metric> _store;
    private static MetricRestService _metricRestService;
    private static List<string> _ignoreEnvironments;

    private const string TimerIntevalSecAppSettingsName = "Pinger.Client.TimerIntevalSec";
    private const string MaxLocalItemsAppSettingsName = "Pinger.Client.MaxLocalItems";
    private const string IgnoreEnvironmentAppSettingsName = "Pinger.Client.IgnoreEnvironmentsCsv";
    private const string HostAppSettingsName = "Pinger.Host";
    private const string ApiKeyAppSettingsName = "Pinger.Key";

    private const int DefaultTimerIntevalSec = 15;
    private const int DefaultMaxLocalItems = 1000000;
    private const int BulkSize = 256;

    public static Reporter Instance
    {
        get
        {
            lock (_lock)
            {
                if (_instance == null)
                {
                    _instance = new Reporter();
                }

                return _instance;
            }
        }
    }

    private Reporter()
    {
        _store = new FixedConcurrentQueue<Metric>(GetAppSettingOrDefault(MaxLocalItemsAppSettingsName, DefaultMaxLocalItems));

        _timer = new Timer(GetAppSettingOrDefault(TimerIntevalSecAppSettingsName, DefaultTimerIntevalSec) * 1000);
        _timer.Elapsed += (sender, args) => FlushStore();
        _timer.AutoReset = false;
        _timer.Start();

        _metricRestService = new MetricRestService(
            ConfigurationManager.AppSettings[ApiKeyAppSettingsName],
            ConfigurationManager.AppSettings[HostAppSettingsName]);

        _ignoreEnvironments = GetAppSettingOrDefault(IgnoreEnvironmentAppSettingsName, string.Empty).Split(',').ToList();
    }

    public void Enqueue(long elapsed, string prefix, string endpoint, string responseCode)
    {
        var environment = CalculateEnvironment();

        if (_ignoreEnvironments.Contains(environment))
        {
            return;
        }

        var itemName = endpoint;
        if (!string.IsNullOrEmpty(prefix))
        {
            itemName = $"{prefix}.{endpoint}";
        }

        var metric = new Metric
        {
            Timestamp = DateTime.Now,
            Value = elapsed,
            Path = $"FooBar.{RemoveSpecialCharacters(environment)}.Response.{itemName}",
            Alias = itemName,
            Message = responseCode
        };

        _store.Enqueue(metric);
    }

    private void FlushStore()
    {
        var run = true;
        var metrics = new List<Metric>();

        var count = 0;
        while (run)
        {
            while (run && count < BulkSize)
            {
                try
                {
                    Metric item;
                    if (_store.TryDequeue(out item))
                    {
                        metrics.Add(item);
                    }
                }
                catch
                {
                    run = false;
                }
                count++;
            }

            try
            {
                _metricRestService.Persist(metrics);
            }
            catch
            {
                metrics.ForEach(x => _store.Enqueue(x));
                run = false;
            }

            metrics = new List<Metric>();

            count = 0;
        }

        _timer.Start();
    }

    private static string CalculateEnvironment()
    {
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

        if (string.IsNullOrEmpty(environment))
        {
            environment = ConfigurationManager.AppSettings["Pinger.Client.Environment"];
        }

        if (string.IsNullOrEmpty(environment))
        {
            environment = "localhost";
        }

        return environment;
    }

    private static T GetAppSettingOrDefault<T>(string appSettingsName, T defaultValue)
    {
        T result = default(T);
        try
        {
            result = (T)Convert.ChangeType(ConfigurationManager.AppSettings[appSettingsName], typeof(T));
        }
        catch
        {
            result = defaultValue;
        }

        return result;
    }

    private static string RemoveSpecialCharacters(string str)
    {
        return Regex.Replace(str, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);
    }

    private class MetricRestService
    {
        private string _apiKey;
        private string _host;

        private RestClient _client;
        public MetricRestService(
            string apiKey,
            string host)
        {
            _apiKey = apiKey;
            _host = host;

            _client = new RestClient($"{_host}");
        }

        public void Persist(List<Metric> metrics)
        {
            var request = new RestRequest("/api/MetricResult");
            request.Method = Method.POST;
            request.AddHeader("Authorization", _apiKey);
            request.Timeout = 2000;
            request.AddParameter("application/json", JsonConvert.SerializeObject(metrics), ParameterType.RequestBody);

            var response = _client.Execute(request);
        }
    }

    private class FixedConcurrentQueue<T> : ConcurrentQueue<T>
    {
        private static object Lock = new object();

        private int _maxItemx;
        public FixedConcurrentQueue(int maxItems)
        {
            _maxItemx = maxItems;
        }

        public new void Enqueue(T obj)
        {
            base.Enqueue(obj);
            lock (Lock)
            {
                T item;
                while (Count > _maxItemx && TryDequeue(out item)) ;
            }
        }
    }

    private class Metric
    {
        [JsonProperty("Timestamp")]
        public DateTime Timestamp { get; set; }
        [JsonProperty("Path")]
        public string Path { get; set; }
        [JsonProperty("Alias")]
        public string Alias { get; set; }
        [JsonProperty("Value")]
        public float Value { get; set; }
        [JsonProperty("Message")]
        public string Message { get; set; }
    }
}
```

### Web API attribute
Well the implementation above can be used to track all kind of different things. The real usage for me was to track the execution time for my API methods. By implementing this ActionFilterAttribute (i used some inspiration from a StackOverflow post that showed how to track the execution by a stopwatch) I ended up by being able to get the execution time for my API methods, by controller or on specific action methods.

```csharp
public class StopwatchAttribute : ActionFilterAttribute
{
    private const string StopwatchKey = "StopwatchFilter.Value";

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
        base.OnActionExecuting(actionContext);

        actionContext.Request.Properties[StopwatchKey] = Stopwatch.StartNew();
    }

    public override void OnActionExecuted(HttpActionExecutedContext actionExecutedContext)
    {
        base.OnActionExecuted(actionExecutedContext);
        try
        {
            var stopwatch = (Stopwatch)actionExecutedContext.Request.Properties[StopwatchKey];
            stopwatch.Stop();

            Reporter.Instance.Enqueue(stopwatch.ElapsedMilliseconds,
                actionExecutedContext.ActionContext.ActionDescriptor.ControllerDescriptor.ControllerName,
                actionExecutedContext.ActionContext.ActionDescriptor.ActionName,
                actionExecutedContext.Request.RequestUri.LocalPath);
        }
        catch { }
    }
}
```

## Visualization
One of the central things was to be able to visualize the data. This aspect is not something I would try to program myself, so I choose to use Grafana for this task. The reason I used Grafana and not Kibana was because of the good user interface and the possibility to configure users and permissions easily. No offense to Kibana, I still use its "Discover" functionality.

### Flow of data
I drew this small drawing to visualize what the setup actually does. It might need some work, but it took no more than five minutes
![Grafana visualization of the data](./dumps/2018-06-21-1555.png)

### Example dashboard
This dashboard shows a big part of the data that I have explained above, and this have been the entry point to identify items in error message queues, windows services not started, and website endpoints not available. It have been tested on roughly ten different servers for at least six different production infrastructures.

![Grafana visualization of the data](./dumps/2018-05-20_2347.png)
