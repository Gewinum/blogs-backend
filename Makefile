addmigration:
	dotnet ef migrations add $(name) --project Blogs.Infrastructure --startup-project Blogs.API

removemigration:
	dotnet ef migrations remove --project Blogs.Infrastructure --startup-project Blogs.API

migratedb:
	dotnet ef database update --project Blogs.Infrastructure --startup-project Blogs.API

dropdb:
	dotnet ef database drop --project Blogs.Infrastructure --startup-project Blogs.API

.PHONY: addmigration, removemigration, migratedb, dropdb
