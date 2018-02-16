use corporatepool
EXEC sp_MSforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
delete from EfTripRequests
delete from EfTrips
delete from EfCommuters
delete from EfVehicles
exec sp_MSforeachtable @command1="print '?'", @command2="ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"

use corporatepoollog
delete from LogObjects