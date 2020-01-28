# Database docker image

## Setup
To build the image you need `init.sql` file

This file is created by running:

```sh
dotnet ef migrations script -p src/App -o database/init.sql
```
