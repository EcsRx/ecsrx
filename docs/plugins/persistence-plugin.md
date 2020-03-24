# Persistence Plugin

The persistence plugin provides some helpers and conventions around saving/loading entity databases.

As part of this it also adds support for building your own pipelines to send data to various endpoints, which is all built on top of Persistity/LazyData.

- [Persistity](https://github.com/grofit/persistity)
- [LazyData](https://github.com/grofit/LazyData)

## A bit of background

Historically this functionality all used to be within EcsRx when it was purely a Unity project, as this was an attempt to try and serialize entity/component data from the scene. However Unity is a complex beast and due to various other reasons the serialization stuff got sidelined.

Anyway fast-forward a bit and now EcsRx is cross platform and Unity is just one of the supported frameworks, so now seemed a good time to re-introduce the serialization aspect of it all.

Given the actual functionality developed was pretty self contained it made sense to split them out into their own libraries that could be used outside of EcsRx, which is why they are their own separate repos, but just wanted to let you know that these libraries were specifically developed originally as part of EcsRx to assist with data push/pulling to various places.

## `EcsRxPersistedApplication`

As part of this there is a helper application class, which extends the default `EcsRxApplication` and by default will look for an existing entity database file and load it when the app starts.

If you do not want it to load automatically and want to handle the load yourself you can override `LoadOnStart` or `SaveOnStop` if you dont want to override the entity database. Also as part of this there is some helper methods for saving and loading the entity database.

There is an example which shows how to use this, as well as how to override the default binary file and use a JSON file instead.

## Pipelines In General

For more info on pipelines look at [Persistity](https://github.com/grofit/persistity), but the gist of it is that they are basic ETL (Extract Transform Load) processes, where you specify how the pipeline should operate. Rather than simple serialization where you just take a model and turn it into a format, a pipeline is a level above that, so you express the entire process/pipeline for taking a raw object and converting it into a given format. Out the box you have support for Binary, Xml, Json, Bson, Yaml.

### Example pipeline 
For example if you wanted to have a pipeline to make an encrypted save game file it may look like:

```csharp
// Manually create an encryption/decryption processor
var encryptor = new AesEncryptor("some-pass-phrase");
var encryptionProcessor = new EncryptDataProcessor(encryptor);
var decryptionProcessor = new DecryptDataProcessor(encryptor);

// Tell it where to store our stuff
var fileEndpoint = new FileEndpoint("savegame.sav");

// Create the pipeline to save the data
container.BuildPipeline("SaveGame", x => x
        .StartWithInput()
        .SerializeWith<IBinarySerializer>()
        .ProcessWith(encryptionProcessor)
        .ThenSendTo(fileEndpoint));

// Create the pipeline to load the data
container.BuildPipeline("LoadGame", x => x
        .StartFrom(fileEndpoint)
        .ProcessWith(decryptionProcessor)
        .DeserializeWith<IBinaryDeerializer>());

// Then to use it you would do
var mySaveGameData = //...
var savePipeline = container.ResolveSendPipeline("SaveGame");
savePipeline.Execute(mySaveGameData); // Now encrypted and saved in savegame.sav

// Then to load it
var loadPipeline = container.ResolveReceivePipeline("LoadGame");
var mySaveGameData = loadPipeline.Execute();
```

As you can see you can setup complex pipelines which can process data/transform it and send it to files, http endpoints, databases or even store it in memory or raise an event etc.

The above example uses some of the helper functionality which act as extensions on the DI container, but you can also inject `EcsRxPipelineBuilder` manually and use that directly to make a pipeline without injecting it in.

## Some things to know

When using pipelines you need to have objects which have a parameterless constructor and have everything get/settable. If you are using 3rd party objects which have complex constructors you are advised to make a proxy type which you transform to and from in your pipeline. For an example of this approach look at how we have `EntityDatabaseData` which acts as a proxy type for `EntityDatabase` which has a complex constructor.

You may want to have 2 different types of the same pipeline, for example in development you may want to have an item database as a json file, but when you go to production you want it as binary. The only difference between the 2 pipelines would be the transport format, so you can re-use a lot of the same steps but just change the send/receive format, you can even use LazyData directly and just convert from Json -> Binary (As shown with SuperLazy helpers in the LazyData repo).