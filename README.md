1. Change both appsettings in matelso.api and matelso.dbmutator.
2. Make matelso.dbmutator as start up project.
3. Run these two command from package manager console.
	Add-Migration initial
	Update-Database
4. After running these command database and table should be created.
5. Now make the matelso.api as startup project.
6. Run the project matelso.api.
Swagger has been used and also postman collection is under the folder as matelso_assignment_postman collection.

