#r "nuget: Fun.Build"

open Fun.Build

let options =
    {| NugetAPIKey = EnvArg.Create("NUGET_API_KEY", description = "Nuget api key") |}

pipeline "packages" {
    description "Build and deploy to nuget"

    stage "Build packages" { run "dotnet pack -c src/Compiler/FSharp.Compiler.Service.fsproj -o ." }

    stage "Publish packages to nuget" {
        whenBranch "main"
        whenEnvVar options.NugetAPIKey
        whenEnvVar "GITHUB_ENV" "" "Only push packages in github action"

        run (fun ctx ->
            let key = ctx.GetEnvVar options.NugetAPIKey.Name

            ctx.RunSensitiveCommand
                $"dotnet nuget push FSharpTour.FCS.nupkg -s https://api.nuget.org/v3/index.json --skip-duplicate -k {key}")
    }

    runIfOnlySpecified
}
