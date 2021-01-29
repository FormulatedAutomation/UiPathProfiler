![FA Github Header](https://user-images.githubusercontent.com/2868/98735818-fabe8a80-2371-11eb-884a-e555e31aa348.png)

# UiPath System Profiler

You can find this in the [UiPath Marketplace](https://marketplace.uipath.com/listings/system-profiler)

## Testing

You can find the tests under the Tests directory and can run them using Visual Studio 2019's built-in test runner

## Build and Release

We use the 'nuget' cli to build releases of this package. Simply update the version, build and run:

`nuget pack .\FormulatedAutomation.UiPathProfiler.Activities.csproj -Symbols -Prop Configuration=Release -OutputDirectory ..\releases`

Zip for submission
`Compress-Archive -DestinationPath ..\releases\[version].src.zip -Path *`
