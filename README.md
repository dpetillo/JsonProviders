# JsonProviders

Json formatted Provider Model for .NET 

Remember the Provider Model? 

https://en.wikipedia.org/wiki/Provider_model

https://msdn.microsoft.com/en-us/library/ms972319.aspx

I'm rewriting this using a json format & with the option to override the key/value parameters injected to the provider.  Here's a sample config file, where we set the values to initialize a BasicTestProvider object.  That object will receive specially injected values - if "Override1" or "Override2" are set. 

```

{
    "BasicTestProvider": {
        "type": "JsonProvidersUnitTests.BasicTestProvider, JsonProvidersUnitTests",
        "Test1Property": "foo",
        "Test2Property": 1.0,
        "Test3Property": 1,
        "Test4Property": false,
        "Test5Property": [ "val1", "baz" ],
        "Test6Property": "2012-04-23T18:25:43.511Z",

        "overrides": {
            "Override1": {
                "Test3Property": 2,
                "Test4Property": true
            },
            "Override2": {
                "Test2Property": 0.1,
                "Test4Property": true,
                "Test5Property": [ "test" ]
            }
        }
    }
}
```


