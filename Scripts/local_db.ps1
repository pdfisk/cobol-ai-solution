$env:PGPASSWORD="chichi"
psql -h localhost `
     -p 5432 `
     -U admin `
     -d cobol_studio
