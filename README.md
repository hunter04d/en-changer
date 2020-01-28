# ENcrypted exCHANGER

## Docker images

The main app image is fine as is.

However to build the database image you will need to run the following commands:

1. `dotnet tool restore`

2. `dotnet ef migrations script -p src/App -o database/init.sql`

After this sequence of commands `docker-compose up --build` will work as intended.

In the future this should probably be handled by the build system.
