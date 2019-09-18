# DiscordWowTracker

[![Build Status](https://dev.azure.com/nikolaitopping/nikolaitopping/_apis/build/status/Topping.DiscordWowTracker?branchName=master)](https://dev.azure.com/nikolaitopping/nikolaitopping/_build/latest?definitionId=1&branchName=master)

This bot is just a small Discord bot written in C#, targeting .NET Core 2.2

## Building the code

To build the code you need the following installed:

### Mandatory
- [.NET Core SDK](https://dotnet.microsoft.com/download)

### Optional
- [Docker](https://www.docker.com/)

## Running the bot

### Setting up the database

The bot runs against a PostgreSQL database. For development purposes it's recommended to simply run this database in a docker container.
The Docker image can be found [Here](https://hub.docker.com/_/postgres)

**Volatile image**

Running with this command will not persist database information if you remove the container

`docker run --name postgres -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=defaultpassword -d postgres:12`

**Non-volatile Image**

Running with this command will mount a local folder to the container, where data will be persisted if you remove the container

`docker run --name postgres -e POSTGRES_PASSWORD=defaultpassword -v <YOUR LOCAL PATH>:/var/lib/postgresql/data -d postgres:12`

### Joining the bot to your guild

- Make an application on the [Discord Developer Portal](https://discordapp.com/developers/applications)
- Create a bot user and get the token
- Add the token to an environment variable called DiscordToken, or add it to appsettings.{environment}.json. **Remember that your token has to be kept secret**
- Run the code from your IDE or terminal
- Create an invite link for your bot [here](https://discordapi.com/permissions.html)


